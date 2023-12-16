namespace JahanJooy.RealEstateAgency.Domain.Base
{
    public class PhoneInfo : ContactInfo
    {
        public string CountryCode { get; set; }
        public string AreaCode { get; set; }
        public bool CanReceiveSms { get; set; }
    }
}