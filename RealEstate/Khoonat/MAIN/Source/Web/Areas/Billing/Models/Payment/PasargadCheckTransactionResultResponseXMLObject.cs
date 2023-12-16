using System;
using System.Xml.Serialization;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.Payment
{
    [XmlRoot("resultObj")]
    public class PasargadCheckTransactionResultResponseXmlObject
    {
        [XmlElement("result")]
        public string Result { get; set; }

        [XmlIgnore]
        public bool ResultBool
        {
            get { return Result.Equals("true", StringComparison.InvariantCultureIgnoreCase); }
        }

        [XmlElement("action")]
        public int Action { get; set; }

        [XmlElement("invoiceNumber")]
        public string InvoiceNumber { get; set; }

        [XmlElement("invoiceDate")]
        public string InvoiceDate { get; set; }

        [XmlElement("transactionReferenceID")]
        public long? TransactionReferenceID { get; set; }

        [XmlElement("traceNumber")]
        public long TraceNumber { get; set; }

        [XmlElement("referenceNumber")]
        public long ReferenceNumber { get; set; }

        [XmlElement("trancsactionDate")]
        public string TransactionDateTime { get; set; }

        [XmlElement("terminalCode")]
        public string TerminalCode { get; set; }

        [XmlElement("merchantCode")]
        public string MerchantCode { get; set; }

        [XmlElement("amount")]
        public decimal Amount { get; set; }

    }
}