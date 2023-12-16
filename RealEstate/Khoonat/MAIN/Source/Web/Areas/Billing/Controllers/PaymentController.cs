using System;
using System.Globalization;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Util.Log4Net;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Billing.Models.Payment;
using JahanJooy.RealEstate.Web.Areas.Billing.Payment;
using ServiceStack;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Controllers
{
    [Authorize]
    public class PaymentController : CustomControllerBase
    {
        private const int PageSize = 20;

        #region Injected dependencies

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

        [ComponentPlug]
        public IUserPaymentService UserPaymentService { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public PasargadPaymentGateway PasargadPaymentGateway { get; set; }

        #endregion

        [HttpGet]
        public ActionResult ViewAllPayments(PaymentViewAllPaymentsModel model)
        {
            if (model == null)
                model = new PaymentViewAllPaymentsModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var wireQuery = DbManager.Db.UserWireTransferPayments
                .Where(p => p.TargetUserID == User.CoreIdentity.UserId.Value)
                .Select(p => new PaymentViewAllPaymentsItemModel
                {
                    Type = UserBillingSourceType.UserWireTransferPayment,
                    ID = p.ID,
                    Amount = p.Amount,
                    CreationTime = p.CreationTime,
                    CompletionTime = p.CompletionTime,
                    SourceBank = p.SourceBank,
                    BillingState = p.BillingState,
                    ForwardTransactionID = p.ForwardTransactionID,
                    ReverseTransactionID = p.ReverseTransactionID
                });

            var elecQuery = DbManager.Db.UserElectronicPayments
                .Where(p => p.TargetUserID == User.CoreIdentity.UserId.Value)
                .Select(p => new PaymentViewAllPaymentsItemModel
                {
                    Type = UserBillingSourceType.UserWireTransferPayment,
                    ID = p.ID,
                    Amount = p.Amount,
                    CreationTime = p.CreationTime,
                    CompletionTime = p.CompletionTime,
//                    SourceBank = p.PaymentGatewayProvider,
                    BillingState = p.BillingState,
                    ForwardTransactionID = p.ForwardTransactionID,
                    ReverseTransactionID = p.ReverseTransactionID
                });

            var query = wireQuery.Union(elecQuery).OrderByDescending(p => p.CreationTime);

            model.Payments = PagedList<PaymentViewAllPaymentsItemModel>.BuildUsingPageNumber(query.Count(), PageSize,
                pageNum);
            model.Payments.FillFrom(query);

            return View(model);
        }

        [HttpGet]
        public ActionResult ViewWireTransferPayments(PaymentViewWireTransferPaymentsModel model)
        {
            if (model == null)
                model = new PaymentViewWireTransferPaymentsModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var query =
                DbManager.Db.UserWireTransferPayments.Where(p => p.TargetUserID == User.CoreIdentity.UserId.Value)
                    .OrderByDescending(p => p.CreationTime);

            model.Payments = PagedList<UserWireTransferPayment>.BuildUsingPageNumber(query.Count(), PageSize, pageNum);
            model.Payments.FillFrom(query);

            return View(model);
        }

        [HttpGet]
        public ActionResult ViewElectronicPayments(PaymentViewElectronicPaymentsModel model)
        {
            if (model == null)
                model = new PaymentViewElectronicPaymentsModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var query =
                DbManager.Db.UserElectronicPayments.Where(p => p.TargetUserID == User.CoreIdentity.UserId.Value)
                    .OrderByDescending(p => p.CreationTime);

            model.Payments = PagedList<UserElectronicPayment>.BuildUsingPageNumber(query.Count(), PageSize, pageNum);
            model.Payments.FillFrom(query);

            return View(model);
        }

        [HttpGet]
        public ActionResult NewPayment()
        {
            return View();
        }

        [HttpGet]
        public ActionResult NewWireTransferPayment()
        {
            var model = new PaymentNewWireTransferPaymentModel();
            model.UserEnteredDate = DateTime.Now.Date;
            model.UserEnteredTimeOfDay = (((int) DateTime.Now.TimeOfDay.TotalMinutes)/15)*15;

            return View(model);
        }

        [HttpPost]
        [ActionName("NewWireTransferPayment")]
        public ActionResult NewWireTransferPaymentPostback(PaymentNewWireTransferPaymentModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var domain = model.ToDomain();
            domain.TargetBank = IranianBank.Saman;
            UserPaymentService.CreateWireTransferPayment(domain);
            return RedirectToAction("ViewWireTransferPayments");
        }

        [HttpGet]
        public ActionResult NewElectronicPayment()
        {
            var model = new NewElectronicPaymentModel();
            model.Amount = 10000;
            model.PaymentGatewayProvider = PaymentGatewayProvider.Pasargad;

            return View("NewElectronicPayment", model);
        }

        [HttpPost]
        public ActionResult NewElectronicPaymentConfirmation(NewElectronicPaymentConfirmationModel confirmationModel)
        {
            if (!ModelState.IsValid)
                return View("NewElectronicPayment", new NewElectronicPaymentModel());

            var domain = new UserElectronicPayment();
            domain.Amount = confirmationModel.Amount;
            domain.PaymentGatewayProvider = confirmationModel.PaymentGatewayProvider;

            var userElectronicPayment = UserPaymentService.CreateElectronicPayment(domain);

            Uri redirectUrl;
            Uri.TryCreate(Request.Url, Url.Action(nameof(RedirectedFromGatewayProviderBank)), out redirectUrl);
            confirmationModel.RedirectAddress = redirectUrl.ToString();

            if (confirmationModel.PaymentGatewayProvider == PaymentGatewayProvider.Pasargad)
                confirmationModel = PasargadPaymentGateway.FillModel(confirmationModel, userElectronicPayment);

            return View("NewElectronicPaymentConfirmation", confirmationModel);
        }

        [HttpGet]
        public ActionResult RedirectedFromGatewayProviderBank(long tref, long iN, string iD)
        {
            var model = new RedirectedFromGatewayProviderBankModel();
            RealEstateStaticLogs.IpgLogger.InfoFormat(
                "Redirected From Gateway Provider Bank with Transaction Reference ID: {0}, Invoice Number: {1}, Invoice Date: {2}, ",
                tref, iN, iD);

            var userElectronicPayment = DbManager.Db.UserElectronicPayments.SingleOrDefault(uep => uep.ID == iN);
            if (userElectronicPayment == null)
            {
                model.InvoiceNumberNotFound = true;
                RealEstateStaticLogs.IpgLogger.ErrorFormat("Invoice Number {0} Not Found", iN);
                return View("RedirectedFromGatewayProviderBankErrorPage", model);
            }

            if (userElectronicPayment.BillingState != BillingSourceEntityState.Pending)
            {
                if (userElectronicPayment.BillingState == BillingSourceEntityState.Applied)
                {
                    model.BillingStateIsAppliedError = true;
                    RealEstateStaticLogs.IpgLogger.InfoFormat("Credit increase has already been applied. Billing State: {0}, is valid", userElectronicPayment.BillingState);
                    return View("RedirectedFromGatewayProviderBankNotificationPage", model);
                }

                model.BillingStateError = true;
                RealEstateStaticLogs.IpgLogger.ErrorFormat("Billing State: {0}, not valid ",
                    userElectronicPayment.BillingState);
                return View("RedirectedFromGatewayProviderBankErrorPage", model);
            }

            if ((DateTime.Now.Subtract(userElectronicPayment.CreationTime).TotalMinutes) > 30)
            {
                model.PaymentRequestTooOld = true;
                RealEstateStaticLogs.IpgLogger.ErrorFormat("Transaction timed out for Invoice {0}, TREF {1}", iN, tref);
                return View("RedirectedFromGatewayProviderBankErrorPage", model);
            }

            var newCreationTime = DateTime.ParseExact(
                userElectronicPayment.CreationTime.ToString(CultureInfo.InvariantCulture), "MM/dd/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture).ToString("yyyy/MM/dd HH:mm:ss");

            if (!newCreationTime.Equals(DateTime.Parse(iD).ToString("yyyy/MM/dd HH:mm:ss")))
            {
                model.InvoiceDateError = true;
                RealEstateStaticLogs.IpgLogger.ErrorFormat(
                    "Creation Times do not match. From bank: {0}, From DB: {1} ", iD, userElectronicPayment.CreationTime);
                return View("RedirectedFromGatewayProviderBankErrorPage", model);
            }

            if (userElectronicPayment.CompletionTime.HasValue)
            {
                RealEstateStaticLogs.IpgLogger.ErrorFormat("Internal state error: Invoide number {0} has not been applied but has completion time set to {1}", iN, userElectronicPayment.CompletionTime.Value);
            }

            userElectronicPayment.BankTransactionReferenceID = tref;
            model.Amount = userElectronicPayment.Amount;

            RealEstateStaticLogs.IpgLogger.Debug("Checking Transaction Result just started ... ");
            var checkTransactionResponse = PasargadPaymentGateway.CheckTransactionResult(tref);
            RealEstateStaticLogs.IpgLogger.InfoFormat(
                "checking Transaction Response has been received. The result is {0}, ",
                checkTransactionResponse.ResultBool);

            if (!userElectronicPayment.ID.ToString().EqualsIgnoreCase(checkTransactionResponse.InvoiceNumber.Trim()))
            {
                model.InvoiceNumberConflict = true;
                RealEstateStaticLogs.IpgLogger.ErrorFormat("Invoice Number {0} not found on bank servers when checking for transaction result",
                    checkTransactionResponse.InvoiceNumber);
                return View("RedirectedFromGatewayProviderBankErrorPage", model);
            }

            if (Convert.ToInt64(userElectronicPayment.Amount) != checkTransactionResponse.Amount)
            {
                model.AmountConflict = true;
                RealEstateStaticLogs.IpgLogger.ErrorFormat("For invoice number {0}, the amount reported by the bank ({1}) does not match the original amount ({2})",
                    checkTransactionResponse.InvoiceNumber, checkTransactionResponse.Amount, userElectronicPayment.Amount);
                return View("RedirectedFromGatewayProviderBankErrorPage", model);
            }

            if (!ApplicationSettings["PaymentGateway.Pasargad.MerchantCode"].EqualsIgnoreCase(
                    checkTransactionResponse.MerchantCode.Trim()))
            {
                model.MerchantCodeConflict = true;
                RealEstateStaticLogs.IpgLogger.ErrorFormat("For invoice number {0} returned merchant code ({1}) does not match out merchant code",
                    checkTransactionResponse.InvoiceNumber, checkTransactionResponse.MerchantCode);
                return View("RedirectedFromGatewayProviderBankErrorPage", model);
            }

            if (!ApplicationSettings["PaymentGateway.Pasargad.TerminalCode"].EqualsIgnoreCase(
                    checkTransactionResponse.TerminalCode.Trim()))
            {
                model.TerminalCodeConflict = true;
                RealEstateStaticLogs.IpgLogger.ErrorFormat("For invoice number {0} returned terminal code ({1}) does not match our terminal code",
                    checkTransactionResponse.InvoiceNumber, checkTransactionResponse.TerminalCode);
                return View("RedirectedFromGatewayProviderBankErrorPage", model);
            }


            if (!checkTransactionResponse.ResultBool)
            {
                model.CheckTransactionResultResposeError = false;
                RealEstateStaticLogs.IpgLogger.ErrorFormat(
                    "Transaction did not get approved by bank, Invoice Number: {0}", checkTransactionResponse.InvoiceNumber);
                return View("RedirectedFromGatewayProviderBankErrorPage", model);
            }

            RealEstateStaticLogs.IpgLogger.InfoFormat(
                "Checking Transaction Result with invoice number {0} successfully finished.",
                checkTransactionResponse.InvoiceNumber);
            UserPaymentService.UpdateElectronicPayment(iN,
                payment => PasargadPaymentGateway.UpdateUserElectronicPayment(payment, checkTransactionResponse));

            RealEstateStaticLogs.IpgLogger.Info("Verifing payment action just started...");
            var verifyPaymentResponse = PasargadPaymentGateway.VerifyPayment(checkTransactionResponse);
            if (!verifyPaymentResponse.ResultBool)
            {
                model.VerifyPaymentResponseError = false;
                RealEstateStaticLogs.IpgLogger.ErrorFormat("Payment did not get verified by bank with message: {0}",
                    verifyPaymentResponse.ResultMessage);
                return View("RedirectedFromGatewayProviderBankErrorPage", model);
            }

            RealEstateStaticLogs.IpgLogger.Info("Verifying payment action successfully finished.");
            UserPaymentService.UpdateElectronicPayment(iN,
                payment => PasargadPaymentGateway.UpdateUserElectronicPayment(payment, verifyPaymentResponse));

            UserPaymentService.ApplyElectronicPayment(iN);
            RealEstateStaticLogs.IpgLogger.InfoFormat(
                "Credit increasing successfully finished with invoice number: {0}", iN);
            model.Message = "حساب شما با موفقیت شارژ شد";

            return View("RedirectedFromGatewayProviderBank", model);
        }


        public static bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            return false;
        }
    }
}