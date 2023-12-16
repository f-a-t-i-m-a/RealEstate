using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Localization;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Messages;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminOutgoingSms;
using ServiceStack;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
    public class AdminOutgoingSmsController : AdminControllerBase
    {
        private static readonly Regex IgnoredCharsInPhoneNumber = new Regex("[\\s]+");

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ISmsMessageService SmsMessageService { get; set; }

        [HttpGet]
        public ActionResult List(AdminOutgoingSmsListModel model)
        {
            if (model == null)
                model = new AdminOutgoingSmsListModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            IQueryable<OutgoingSmsMessage> query = DbManager.Db.OutgoingSmsMessagesDbSet;
            query = ApplyFilterQuery(model, query);

			model.Messages = PagedList<OutgoingSmsMessage>.BuildUsingPageNumber(query.Count(), 20, pageNum);
            model.Messages.FillFrom(query.OrderByDescending(l => l.CreationDate));

            return View(model);
        }

        [HttpGet]
        public ActionResult NewAdvertisement()
        {
            var model = new AdminOutgoingSmsNewAdvertisementModel();
            return View(model);
        }

        [HttpPost]
        [ActionName("NewAdvertisement")]
        public ActionResult NewAdvertisementPostback(AdminOutgoingSmsNewAdvertisementModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var numbers = ValidateAndFormatNumbers(model.TargetNumbers);

            if (!ModelState.IsValid)
                return View(model);

            model.TargetNumbers = string.Join(Environment.NewLine, numbers);
            return View("NewAdvertisementConfirmation", model);
        }

        [HttpPost]
        [SubmitButton("btnReturn")]
        [ActionName("NewAdvertisementConfirmed")]
        public ActionResult NewAdvertisementConfirmReturn(AdminOutgoingSmsNewAdvertisementModel model)
        {
            return View("NewAdvertisement", model);
        }

        [HttpPost]
        [SubmitButton("btnSend")]
        public ActionResult NewAdvertisementConfirmed(AdminOutgoingSmsNewAdvertisementModel model)
        {
            if (!ModelState.IsValid)
                return View("NewAdvertisement", model);

            var numbers = ValidateAndFormatNumbers(model.TargetNumbers);

            if (!ModelState.IsValid)
                return View("NewAdvertisement", model);

            foreach (var number in numbers)
            {
                SmsMessageService.EnqueueOutgoingMessage(
                    model.MessageText, number, NotificationReason.Advertisement, NotificationSourceEntityType.None, 0);
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult NewSingle(string targetNumber)
        {
            var model = new AdminOutgoingSmsNewSingleModel();
            if (!targetNumber.IsEmpty())
                model.TargetNumber = targetNumber;
            return View(model);
        }

        [HttpPost]
        [ActionName("NewSingle")]
        public ActionResult NewSinglePostback(AdminOutgoingSmsNewSingleModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var number = ValidateAndFormatNumber(model.TargetNumber);

            if (!ModelState.IsValid)
                return View(model);

            SmsMessageService.EnqueueOutgoingMessage(
                model.MessageText, number, NotificationReason.OperatorRequest, NotificationSourceEntityType.None, 0,
                allowTransmissionOnAnyTimeOfDay: model.AllowTransmissionOnAnyTimeOfDay, isFlash: model.IsFlash);

            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult ResetMessage(long id)
        {
            SmsMessageService.ResetOutgoingMessage(id);
            return Json(true);
        }

        [HttpPost]
        public ActionResult CancelMessage(long id)
        {
            SmsMessageService.CancelOutgoingMessage(id);
            return Json(true);
        }

        #region Public helper methods

        public static string GetRowBackgroundColor(OutgoingSmsMessage message)
        {
            if (message.State == OutgoingSmsMessageState.Delivered)
            {
                return "success";
            }

            if (message.State == OutgoingSmsMessageState.DeliveryUnknown ||
                message.State == OutgoingSmsMessageState.ErrorInTransmission ||
                message.State == OutgoingSmsMessageState.NotDelivered)
            {
                return "danger";
            }

            if (message.ExpirationDate.HasValue && message.ExpirationDate.Value < DateTime.Now)
            {
                return "info";
            }

            if (message.State == OutgoingSmsMessageState.AwaitingDelivery)
            {
                return "warning";
            }
            
            return "active";
        }

        #endregion

        #region Private helper methods

        private static IQueryable<OutgoingSmsMessage> ApplyFilterQuery(AdminOutgoingSmsListModel model, IQueryable<OutgoingSmsMessage> query)
        {
            if (model.OutgoingSmsIdFilter.HasValue)
                query = query.Where(m => m.ID == model.OutgoingSmsIdFilter.Value);

            if (model.ApplyTargetUserIdFilter)
            {
                query = model.TargetUserIdFilter.HasValue
                    ? query.Where(m => m.TargetUserID == model.TargetUserIdFilter.Value)
                    : query.Where(m => m.TargetUserID == null);
            }

            if (model.ReasonFilter.HasValue)
                query = query.Where(m => m.Reason == model.ReasonFilter.Value);

            if (model.StateFilter.HasValue)
                query = query.Where(m => m.State == model.StateFilter.Value);

            if (model.SourceEntityTypeFilter.HasValue)
                query = query.Where(m => m.SourceEntityType == model.SourceEntityTypeFilter.Value);

            if (model.SourceEntityIdFilter.HasValue)
                query = query.Where(m => m.SourceEntityID == model.SourceEntityIdFilter.Value);

            if (model.RetryIndexFilter.HasValue)
                query = query.Where(m => m.RetryIndex == model.RetryIndexFilter.Value);


            if (!string.IsNullOrWhiteSpace(model.TargetNumberFilter))
                query = query.Where(m => m.TargetNumber.Contains(model.TargetNumberFilter));

            if (!string.IsNullOrWhiteSpace(model.MessageTextFilter))
                query = query.Where(m => m.MessageText.Contains(model.MessageTextFilter));

            if (!string.IsNullOrWhiteSpace(model.SenderNumberFilter))
                query = query.Where(m => m.SenderNumber.Contains(model.SenderNumberFilter));


            if (model.ErrorCodeFilter.HasValue)
                query = query.Where(m => m.ErrorCode == model.ErrorCodeFilter.Value);

            if (model.LastDeliveryCodeFilter.HasValue)
                query = query.Where(m => m.LastDeliveryCode == model.LastDeliveryCodeFilter.Value);

            if (!string.IsNullOrWhiteSpace(model.OperatorAssignedIdFilter))
                query = query.Where(m => m.OperatorAssignedID.Contains(model.OperatorAssignedIdFilter));

            return query;
        }

        private string[] ValidateAndFormatNumbers(string numbersString)
        {
            var numbers = numbersString
                .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => IgnoredCharsInPhoneNumber.Replace(s, ""))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToArray();

            for (int i = 0; i < numbers.Length; i++)
            {
                var validatedResult = LocalPhoneNumberUtils.ValidateAndFormat(numbers[i], true, true);
                if (validatedResult.IsValid)
                    numbers[i] = validatedResult.Result;
                else
                    ModelState.AddModelError("",
                        "Error in phone number '" + numbers[i] + "': " + string.Join("; ", validatedResult.Errors.Select(e => e.ErrorKey)));
            }

            return numbers;
        }

        private string ValidateAndFormatNumber(string numberString)
        {
            var number = IgnoredCharsInPhoneNumber.Replace(numberString, "");
            var validatedResult = LocalPhoneNumberUtils.ValidateAndFormat(number, true, true);

            if (validatedResult.IsValid)
                return validatedResult.Result;

            ModelState.AddModelError("",
                "Error in phone number '" + number + "': " + string.Join("; ", validatedResult.Errors.Select(e => e.ErrorKey)));

            return number;
        }

        #endregion
    }
}