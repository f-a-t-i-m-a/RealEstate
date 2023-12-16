using System;
using System.Xml.Serialization;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.Payment
{
    [XmlRoot("actionResult")]
    public class PasargadVerifyPaymentResponseXmlObject
    {
        [XmlElement("result")]
        public string Result { get; set; }

        [XmlIgnore]
        public bool ResultBool
        {
            get { return Result.Equals("true", StringComparison.InvariantCultureIgnoreCase); }
        }

        [XmlElement("resultMessge")]
        public string ResultMessage { get; set; }


    }}