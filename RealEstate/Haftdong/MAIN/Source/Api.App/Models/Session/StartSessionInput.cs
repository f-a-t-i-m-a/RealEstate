namespace JahanJooy.RealEstateAgency.Api.App.Models.Session
{
    public class StartSessionInput
    {
        public string PhoneOperator { get; set; }
        public string PhoneSerialNumber { get; set; }
        public string PhoneSubscriberId { get; set; }
        public string SessionID { get; set; }
    }
}