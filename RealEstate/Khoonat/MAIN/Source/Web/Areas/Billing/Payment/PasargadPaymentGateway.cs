using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Compositional.Composer;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Util.Log4Net;
using JahanJooy.RealEstate.Web.Areas.Billing.Models.Payment;
using ServiceStack.Text;
using XmlSerializer = System.Xml.Serialization.XmlSerializer;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Payment
{
    [Contract]
    [Component]
    public class PasargadPaymentGateway : IEagerComponent
    {
//        private static PasargadCheckTransactionResultResponse _pasargadCheckTransactionResultResponse ;
//        private static PasargadCheckTransactionResultResponse actionResultVerifyPayment;
        #region Injected dependencies

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

        #endregion

        #region StoreIdConstants

        public const string MerchantCode = "PaymentGateway.Pasargad.MerchantCode";
        public const string TerminalCode = "PaymentGateway.Pasargad.TerminalCode";
        public const string ActionPurchase = "PaymentGateway.Pasargad.ActionPurchase";
        public const string ActionPurchaseReturn = "PaymentGateway.Pasargad.ActionPurchaseReturn";

        #endregion

        #region Initialization

        static PasargadPaymentGateway()
        {
            ApplicationSettingKeys.RegisterKey(MerchantCode);
            ApplicationSettingKeys.RegisterKey(TerminalCode);
            ApplicationSettingKeys.RegisterKey(ActionPurchase);
            ApplicationSettingKeys.RegisterKey(ActionPurchaseReturn);
        }

        #endregion


        public NewElectronicPaymentConfirmationModel FillModel(NewElectronicPaymentConfirmationModel confirmationModel, UserElectronicPayment userElectronicPayment)
        {
            RealEstateStaticLogs.PasargadPaymentGatewayLogger.Info("Filling Electronic Payment Confirmation Model just started with user electronic payment Info ");

            confirmationModel.InvoiceNumber = userElectronicPayment.ID;
            confirmationModel.InvoiceDate = userElectronicPayment.CreationTime.ToString("yyyy/MM/dd HH:mm:ss");
            confirmationModel.MerchantCode = ApplicationSettings[MerchantCode];
            confirmationModel.TerminalCode = ApplicationSettings[TerminalCode];
            confirmationModel.TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            confirmationModel.Action = "1003";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString("<RSAKeyValue><Modulus>wNEX1L6FF4bn/1O4BicxiSx0/kuU4QDN41e9jO9R+UI2S4XoHk38FfHD1u3GfbTao5YJrfBr2b3wG9qLTaAINyrEKHDPgs75xke/ntpxMLGJsNEcUk9wS1rc8dz1vsoxaYN8PEmFaWeElT2b6RgB7eT68Z7egLGGMgCP/L4V/xs=</Modulus><Exponent>AQAB</Exponent><P>6FkLZkGtcdYftVCSe2mvsQtHC4iZrnrOhG8gDDXxhfSofrySuTrORcdmN2VKhnb1p9OGRQAzElKWwk+WU6+WKQ==</P><Q>1HHfwm247QpCQDKfJ8nBvHdrOPsFD5qWVdcWEvVU8ukywsM59bkBp6mtC8CdXp3G2Xe5RYqLc2pprUN5s5yrow==</Q><DP>AojuKehvhv1qDSVa48PMaecQmFyeKJwoYqN/uwJfpzF7IR7XjvPISlSZleMiBAOKPJF/NoOBCyMhh+8sWa9huQ==</DP><DQ>xuXuZqwvjM4kBNLgK3I7jmYH3ws1S8yhn7CGHC3Q4LPwMGRzaVlS8VRsjSpCrAjv2T68GR2DPgWM9wjLcpvmHQ==</DQ><InverseQ>LdfIhGoAKBVyVKr6pZj0pyv9MknwPEdKdmJ1Yu8pXe+rttaeGr3SFREhDrddIqsHUhV/RvAX0NeBsnPH5xN67g==</InverseQ><D>Ec3c4PicqoD7ACEXX34T/WIdBXQuMZ6U5FawOojIrXb3M38QYWf7DH2wAzMefnIKNEvA5g5KrybyOmgyL79EssZOWuHDkUyqk5BiYwVUTsp1yOBroz0bQyiBgqFyRizgRcUDDb0cj6rtjTV22wJ8U3CsgwmMeGP5pnmIWUVBmJk=</D></RSAKeyValue>");

            string data = "#" + confirmationModel.MerchantCode + "#" + confirmationModel.TerminalCode + "#"+ confirmationModel.InvoiceNumber + "#" + confirmationModel.InvoiceDate + "#" + 
                                confirmationModel.Amount + "#" + confirmationModel.RedirectAddress + "#" + confirmationModel.Action + "#" + confirmationModel.TimeStamp + "#";

            byte[] signMain = rsa.SignData(Encoding.UTF8.GetBytes(data), new SHA1CryptoServiceProvider());
            confirmationModel.Sign = Convert.ToBase64String(signMain);

            RealEstateStaticLogs.PasargadPaymentGatewayLogger.InfoFormat("Electronic Payment Confirmation Model  has been Filled successfully with : {0} ", confirmationModel.Dump());
            return confirmationModel;

        }
        
        // Here we send the received TransactionReferenceID to the back and check the result. If the bank says "true" it means the TransactionReferenceID we have is from the bank (not manipulated by user)
        // and then we Post a verification message to the bank. This is when the bank will finalize the transaction and transfer the money to our account.
        //???  check
        public UserElectronicPayment ReadPaymentResultReceivedFromBandAndSendVerificationToBak(
            UserElectronicPayment userElectronicPayments, string text)
        {
            return null;
        }

        //Sends the TransactionReferenceId received from user to the bank to check if it's correct.
        public PasargadCheckTransactionResultResponseXmlObject CheckTransactionResult(long transactionReferenceId)
        {

            RealEstateStaticLogs.PasargadPaymentGatewayLogger.InfoFormat("Checking transaction result is strarted with transaction Reference Id : {0},", transactionReferenceId);
            var request = (HttpWebRequest)WebRequest.Create("https://pep.shaparak.ir/CheckTransactionResult.aspx");
            byte[] textArray = Encoding.UTF8.GetBytes("invoiceUID=" + transactionReferenceId);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = textArray.Length;

            request.GetRequestStream().Write(textArray, 0, textArray.Length);
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());

            XmlSerializer serializer = new XmlSerializer(typeof(PasargadCheckTransactionResultResponseXmlObject));

            var result = (PasargadCheckTransactionResultResponseXmlObject)serializer.Deserialize(reader);

            RealEstateStaticLogs.PasargadPaymentGatewayLogger.InfoFormat("Transaction result has been received from bank with : {0}", result.Dump());

            return result;
        }

        public void UpdateUserElectronicPayment(UserElectronicPayment payment,
            PasargadCheckTransactionResultResponseXmlObject response)
        {
            RealEstateStaticLogs.PasargadPaymentGatewayLogger.InfoFormat("Updating User Electronic Payment( ID: {0}) just started after checking transaction result with bank." , payment.ID);
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            if (response == null)
                throw new ArgumentNullException(nameof(response));

            payment.BankPaymentResult = response.ResultBool;
            payment.BankInvoiceNumber = response.InvoiceNumber;
            payment.BankInvoiceDate = response.InvoiceDate;
            payment.BankTransactionReferenceID = response.TransactionReferenceID;
            payment.BankTraceNumber = response.TraceNumber;
            payment.BankReferenceNumber = response.ReferenceNumber;
            payment.BankTransactionDate = response.TransactionDateTime;
            payment.BankTerminalCode = response.TerminalCode;
            payment.BankMerchantCode = response.MerchantCode;
            payment.BankAmount = response.Amount;

            RealEstateStaticLogs.PasargadPaymentGatewayLogger.Info("User electronic paymen has been updated.");
        }

        public void UpdateUserElectronicPayment(UserElectronicPayment payment,
            PasargadVerifyPaymentResponseXmlObject response)
        {
            RealEstateStaticLogs.PasargadPaymentGatewayLogger.InfoFormat("Updating User Electronic Payment( ID: {0}) just started after verifying payment with bank.", payment.ID);
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            if (response == null)
                throw new ArgumentNullException(nameof(response));

            payment.BankVerifyPaymentResult = response.ResultBool;
            payment.BankVerifyPaymentResultMessage = response.ResultMessage;
            RealEstateStaticLogs.PasargadPaymentGatewayLogger.Info("User electronic paymen has been  updated.");
        }

        public PasargadVerifyPaymentResponseXmlObject VerifyPayment(PasargadCheckTransactionResultResponseXmlObject pasargadCheckTransactionResultResponse)
        {
            RealEstateStaticLogs.PasargadPaymentGatewayLogger.InfoFormat("Verifying payment just started with pasargad Response  with invoice number :{0}", pasargadCheckTransactionResultResponse.InvoiceNumber);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString("<RSAKeyValue><Modulus>wNEX1L6FF4bn/1O4BicxiSx0/kuU4QDN41e9jO9R+UI2S4XoHk38FfHD1u3GfbTao5YJrfBr2b3wG9qLTaAINyrEKHDPgs75xke/ntpxMLGJsNEcUk9wS1rc8dz1vsoxaYN8PEmFaWeElT2b6RgB7eT68Z7egLGGMgCP/L4V/xs=</Modulus><Exponent>AQAB</Exponent><P>6FkLZkGtcdYftVCSe2mvsQtHC4iZrnrOhG8gDDXxhfSofrySuTrORcdmN2VKhnb1p9OGRQAzElKWwk+WU6+WKQ==</P><Q>1HHfwm247QpCQDKfJ8nBvHdrOPsFD5qWVdcWEvVU8ukywsM59bkBp6mtC8CdXp3G2Xe5RYqLc2pprUN5s5yrow==</Q><DP>AojuKehvhv1qDSVa48PMaecQmFyeKJwoYqN/uwJfpzF7IR7XjvPISlSZleMiBAOKPJF/NoOBCyMhh+8sWa9huQ==</DP><DQ>xuXuZqwvjM4kBNLgK3I7jmYH3ws1S8yhn7CGHC3Q4LPwMGRzaVlS8VRsjSpCrAjv2T68GR2DPgWM9wjLcpvmHQ==</DQ><InverseQ>LdfIhGoAKBVyVKr6pZj0pyv9MknwPEdKdmJ1Yu8pXe+rttaeGr3SFREhDrddIqsHUhV/RvAX0NeBsnPH5xN67g==</InverseQ><D>Ec3c4PicqoD7ACEXX34T/WIdBXQuMZ6U5FawOojIrXb3M38QYWf7DH2wAzMefnIKNEvA5g5KrybyOmgyL79EssZOWuHDkUyqk5BiYwVUTsp1yOBroz0bQyiBgqFyRizgRcUDDb0cj6rtjTV22wJ8U3CsgwmMeGP5pnmIWUVBmJk=</D></RSAKeyValue>");

            var timeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string dataToSign = "#" + ApplicationSettings[MerchantCode] + "#" + ApplicationSettings[TerminalCode] + "#" + pasargadCheckTransactionResultResponse.InvoiceNumber + "#" + pasargadCheckTransactionResultResponse.InvoiceDate + "#" + pasargadCheckTransactionResultResponse.Amount + "#" + timeStamp + "#";
            byte[] signatureBytes = rsa.SignData(Encoding.UTF8.GetBytes(dataToSign), new SHA1CryptoServiceProvider());
            var signature = Convert.ToBase64String(signatureBytes);

            HttpWebRequest verifyPaymentRequest =(HttpWebRequest)WebRequest.Create("https://pep.shaparak.ir/VerifyPayment.aspx");
            string textVerifyPayment = "InvoiceNumber=" + pasargadCheckTransactionResultResponse.InvoiceNumber + "&InvoiceDate=" + pasargadCheckTransactionResultResponse.InvoiceDate + "&MerchantCode=" + ApplicationSettings[MerchantCode] +
                                       "&TerminalCode=" + ApplicationSettings[TerminalCode] + "&Amount=" + pasargadCheckTransactionResultResponse.Amount.ToString("#") + "&TimeStamp=" + timeStamp + "&Sign=" + signature;

            byte[] textArrayVerifyPayment = Encoding.UTF8.GetBytes(textVerifyPayment);
            verifyPaymentRequest.Method = "POST";
            verifyPaymentRequest.ContentType = "application/x-www-form-urlencoded";
            verifyPaymentRequest.ContentLength = textArrayVerifyPayment.Length;
            verifyPaymentRequest.GetRequestStream().Write(textArrayVerifyPayment, 0, textArrayVerifyPayment.Length);
            HttpWebResponse responseVerifyPayment = (HttpWebResponse)verifyPaymentRequest.GetResponse();
            StreamReader readerVerifyPayment = new StreamReader(responseVerifyPayment.GetResponseStream());

            XmlSerializer XmlSerializerVerifyPayment = new XmlSerializer(typeof(PasargadVerifyPaymentResponseXmlObject));
            var result = (PasargadVerifyPaymentResponseXmlObject)XmlSerializerVerifyPayment.Deserialize(readerVerifyPayment);
            RealEstateStaticLogs.PasargadPaymentGatewayLogger.InfoFormat("Verifiying  Payment has been received successfully with : {0} ", result.Dump());
            return result;
        }
    }
}