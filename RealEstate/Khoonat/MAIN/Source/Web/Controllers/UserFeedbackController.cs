using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Web.Captcha;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Models.UserFeedback;

namespace JahanJooy.RealEstate.Web.Controllers
{
	public class UserFeedbackController : CustomControllerBase
	{
		#region Injected dependencies

		[ComponentPlug]
		public IUserFeedbackService UserFeedbackService { get; set; }

		#endregion

		#region Action methods

		[HttpPost]
		public ActionResult ReportAbuse(AbuseFlagEntityType entityType, long entityId)
		{
			var model = new UserFeedbackReportAbuseModel {EntityType = entityType, EntityID = entityId};
			return PartialView(model);
		}

		[HttpPost]
		[ValidateCaptcha(validateScriptCaptcha: true)]
		public ActionResult ReportAbusePostback(UserFeedbackReportAbuseModel model)
		{
			if (!User.IsVerified)
				ValidateCaptchaAttribute.AuthorizeImageCaptcha(HttpContext, ModelState);

			if (!ModelState.IsValid)
				return PartialView("ReportAbuse", model);

			var result = UserFeedbackService.ReportAbuse(model.EntityType.Value, model.EntityID.Value, model.Reason.Value, model.Comments);

			return PartialView(result);
		}

		[HttpPost]
		public ActionResult RequestNeighborhood()
		{
			var model = new UserFeedbackRequestNeighborhoodModel();
			return PartialView("Forms/RequestNeighborhoodForm", model);
		}

		[HttpPost]
		[ValidateCaptcha(validateScriptCaptcha: true)]
		public ActionResult RequestNeighborhoodPostback(UserFeedbackRequestNeighborhoodModel model)
		{
			return ProcessPostback("Neighborhood Request", model, "Forms/RequestNeighborhoodForm", true);
		}

		[HttpGet]
		public ActionResult ContactUs()
		{
			return View();
		}

		[HttpPost]
		public ActionResult GeneralContactForm()
		{
			return View("Forms/GeneralContactForm", new UserFeedbackGeneralContactFormModel());
		}

		[HttpPost]
		[ValidateCaptcha(validateScriptCaptcha: true)]
		public ActionResult GeneralContactFormPostback(UserFeedbackGeneralContactFormModel model)
		{
			return ProcessPostback("General contact form", model, "ContactUs", false);
		}

		[HttpGet]
		public ActionResult ReportIssue()
		{
			return View();
		}

		[HttpPost]
		[ValidateCaptcha(validateScriptCaptcha: true)]
		public ActionResult ReportIssuePostback(UserFeedbackReportIssueModel model)
		{
			return ProcessPostback("Report issue", model, "ReportIssue", false);
		}

		[HttpGet]
		public ActionResult ReportSuggestion()
		{
			return View();
		}

		[HttpPost]
		[ValidateCaptcha(validateScriptCaptcha: true)]
		public ActionResult ReportSuggestionPostback(UserFeedbackReportSuggestionModel model)
		{
			return ProcessPostback("Report suggestion", model, "ReportSuggestion", false);
		}

		#endregion

		#region Private helper methods

		private ActionResult ProcessPostback(string subject, object model, string originalViewName, bool popup)
		{
			if (!User.IsVerified)
				ValidateCaptchaAttribute.AuthorizeImageCaptcha(HttpContext, ModelState);

			if (!ModelState.IsValid)
				return popup ? (ActionResult) PartialView(originalViewName, model) : View(originalViewName, model);

			var feedbackContent = ModelToDictionary(model);
			UserFeedbackService.SubmitFeedback(subject, feedbackContent);

			return popup ? (ActionResult) PartialView("FeedbackSubmittedPopup") : View("FeedbackSubmitted");
		}

		private static Dictionary<string, string> ModelToDictionary(object model)
		{
			var result = new Dictionary<string, string>();
			foreach (PropertyDescriptor propDesc in TypeDescriptor.GetProperties(model))
			{
				string name = propDesc.Name;
				string value = (propDesc.GetValue(model) ?? "").ToString();

				var displayAttribute = propDesc.Attributes.OfType<DisplayAttribute>().FirstOrDefault();
				if (displayAttribute != null)
					name = displayAttribute.GetName();

				result.Add(name, value);
			}

			return result;
		}

		#endregion
	}
}