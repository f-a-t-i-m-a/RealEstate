using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Billing.Models.BillingHome;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Controllers
{
    [Authorize]
    public class BillingHomeController : CustomControllerBase
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IUserBillingBalanceCache BalanceCache { get; set; }

        [ChildActionOnly]
        public ActionResult RenderTopBarBalance()
        {
            var model = BalanceCache[User.CoreIdentity.UserId.Value];
            return PartialView(model);
        }

        [HttpGet]
        public ActionResult Index()
        {
            long userId = User.CoreIdentity.UserId.Value;

            var model = new BillingHomeIndexModel();

            model.Balance = BalanceCache[userId];
            model.LastTransaction = model.Balance.LastTransaction;
            model.NumberOfTransactions = DbManager.Db.UserBillingTransactions.Count(t => t.UserID == userId);

            model.NumberOfPendingWireTransferPayments = DbManager.Db.UserWireTransferPayments
                .Count(p => p.TargetUserID == userId && p.BillingState == BillingSourceEntityState.Pending);
            model.NumberOfCompletedWireTransferPayments = DbManager.Db.UserWireTransferPayments
                .Count(p => p.TargetUserID == userId && p.BillingState == BillingSourceEntityState.Applied);
            model.NumberOfCancelledWireTransferPayments = DbManager.Db.UserWireTransferPayments
                .Count(p => p.TargetUserID == userId && (p.BillingState == BillingSourceEntityState.Cancelled || p.BillingState == BillingSourceEntityState.Reversed));
            model.TotalCompletedWireTransferPaymentsCashAmount = DbManager.Db.UserWireTransferPayments
                .Where(p => p.TargetUserID == userId && p.BillingState == BillingSourceEntityState.Applied)
                .Sum(p => (decimal?)p.ForwardTransaction.CashDelta).GetValueOrDefault();
            model.TotalCompletedWireTransferPaymentsBonusAmount = DbManager.Db.UserWireTransferPayments
                .Where(p => p.TargetUserID == userId && p.BillingState == BillingSourceEntityState.Applied)
                .Sum(p => (decimal?)p.ForwardTransaction.BonusDelta).GetValueOrDefault();

            model.NumberOfPendingElectronicPayments = DbManager.Db.UserElectronicPayments
                .Count(p => p.TargetUserID == userId && p.BillingState == BillingSourceEntityState.Pending);
            model.NumberOfCompletedElectronicPayments = DbManager.Db.UserElectronicPayments
                .Count(p => p.TargetUserID == userId && p.BillingState == BillingSourceEntityState.Applied);
            model.NumberOfCancelledElectronicPayments = DbManager.Db.UserElectronicPayments
                .Count(p => p.TargetUserID == userId && (p.BillingState == BillingSourceEntityState.Cancelled || p.BillingState == BillingSourceEntityState.Reversed));
            model.TotalCompletedElectronicPaymentsCashAmount = DbManager.Db.UserElectronicPayments
                .Where(p => p.TargetUserID == userId && p.BillingState == BillingSourceEntityState.Applied)
                .Sum(p => (decimal?)p.ForwardTransaction.CashDelta).GetValueOrDefault();
            model.TotalCompletedElectronicPaymentsBonusAmount = DbManager.Db.UserElectronicPayments
                .Where(p => p.TargetUserID == userId && p.BillingState == BillingSourceEntityState.Applied)
                .Sum(p => (decimal?)p.ForwardTransaction.BonusDelta).GetValueOrDefault();

            model.NumberOfPendingRefundRequests = DbManager.Db.UserRefundRequests
                .Count(r => r.TargetUserID == userId && r.BillingState == BillingSourceEntityState.Applied  && !r.ClearedByUserID.HasValue );
            model.NumberOfCompletedRefundRequests = DbManager.Db.UserRefundRequests
                .Count(r => r.TargetUserID == userId && r.BillingState == BillingSourceEntityState.Applied && r.ReviewedByUserID.HasValue && r.ClearedByUserID.HasValue);
            model.NumberOfCancelledRefundRequests = DbManager.Db.UserRefundRequests
                .Count(r => r.TargetUserID == userId &&  r.BillingState == BillingSourceEntityState.Reversed);
            model.TotalNonCancelledRefundRequestsAmount = DbManager.Db.UserRefundRequests
                .Where(r => r.TargetUserID == userId && r.BillingState != BillingSourceEntityState.Cancelled && r.BillingState != BillingSourceEntityState.Reversed)
                .Sum(r => (decimal?)r.RequestedAmount).GetValueOrDefault();
            model.TotalClearedRefundRequestsAmount = DbManager.Db.UserRefundRequests
                .Where(r => r.TargetUserID == userId && r.BillingState == BillingSourceEntityState.Applied && r.ReviewedByUserID.HasValue && r.ClearedByUserID.HasValue)
                .Sum(r => r.PayableAmount).GetValueOrDefault();

            model.NumberOfRewardedPromotionalBonuses = DbManager.Db.PromotionalBonuses
                .Count(b => b.TargetUserID == userId && b.BillingState == BillingSourceEntityState.Applied);
            model.NumberOfClaimedPromotionalCoupons = DbManager.Db.PromotionalBonusCoupons
                .Count(b => b.TargetUserID == userId && b.BillingState == BillingSourceEntityState.Applied);
            model.TotalRewardedPromotionalBonusAmount = DbManager.Db.PromotionalBonuses
                .Where(b => b.TargetUserID == userId && b.BillingState == BillingSourceEntityState.Applied)
                .Sum(b => (decimal?)b.BonusAmount).GetValueOrDefault();
            model.TotalClaimedPromotionalCouponAmount = DbManager.Db.PromotionalBonusCoupons
                .Where(b => b.TargetUserID == userId && b.BillingState == BillingSourceEntityState.Applied)
                .Sum(b => (decimal?)b.CouponValue).GetValueOrDefault();

            return View(model);
        }

        public ActionResult IncreaseCredit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RedirectedFromPasargadBank()
        {
            //TODO : should save the result in DB. 
            //TODO: should analyze the returned result from the bank and return a result accordingly.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://pep.shaparak.ir /CheckTransactionResult.aspx");
            string text = "invoiceUID=" + Request.QueryString["tref"];
            byte[] textArray = Encoding.UTF8.GetBytes(text);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = textArray.Length;
            ServicePointManager.ServerCertificateValidationCallback =new RemoteCertificateValidationCallback(RemoteCertificateValidation); 
            request.GetRequestStream().Write(textArray, 0, textArray.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string result = reader.ReadToEnd();  //ﻣﯿﺒﺎﺷﺪ XML ﺷﺎﻣﻞ ﻧﺘﯿﺠﻪ ﺑﻪ ﺻﻮرت Result   در اﯾﻦ ﻣﺮﺣﻠﻪ }   
            //TODO: 
            //a class should be defined, filled with result fields like merchant code etc. and passed to VerifiedPayment()
            // if result khoobe! VerifyPayment
            
            VerifyPayment();
            return null;

        }

        private static bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)             
                return true; 
            return false;
        }

        private static bool VerifyPayment()
        {
//            merchantCode = 724477; //   ﮐﺪ ﭘﺬﯾﺮﻧﺪه       
//            terminalCode = 725197; //  ﮐﺪ ﺗﺮﻣﯿﻨﺎل       
//            amount = 2000000;  //  ﻣﺒﻠﻎ ﻓﺎﮐﺘﻮر       
//            invoiceNumber = 1949945; //  ﺷﻤﺎره ﻓﺎﮐﺘﻮر       
//            invoiceDate = 1387/10/12 12:45:32; //  ﺗﺎرﯾﺦ ﻓﺎﮐﺘﻮر 
//            timeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"); 
//            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); 
//            rsa.FromXmlString(“<RSAKeyValue><Modulus>oQRshGhLf2Fh...”);  
//            string data = "#" + merchantCode + "#" + terminalCode + "#" +      invoiceNumber + "#" + invoiceDate + "#" + amount + "#" + timeStamp +"#"; 
//            byte[] signMain = rsa.SignData(Encoding.UTF8.GetBytes(data), new  SHA1CryptoServiceProvider()); 
//            sign = Convert.ToBase64String(signMain);  
//            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://pep.co.ir/ipgtest/"); string text = " InvoiceNumber =" + invoiceNumber +"& InvoiceDate=" + invoiceDate +"&MerchantCode=" + merchantCode +"&TerminalCode=" + terminalCode +"& Amount=" + amount +"& TimeStamp=" + timeStamp + "&Sign=" + sign;  
//            byte[] textArray = Encoding.UTF8.GetBytes(text);         request.Method = "POST";         request.ContentType = "application/x-www-form-urlencoded";         request.ContentLength = textArray.Length;         request.GetRequestStream().Write(textArray, 0, textArray.Length);         HttpWebResponse response = (HttpWebResponse)request.GetResponse();         StreamReader reader = new StreamReader(response.GetResponseStream());         string result = reader.ReadToEnd(); 
            return true;
        }
    }
}