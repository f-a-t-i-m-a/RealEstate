using System.Collections.Generic;
using JahanJooy.RealEstateAgency.Domain.Base;

namespace JahanJooy.RealEstateAgency.Domain.Users
{
    public class ContactMethodCollection
    {
        public string OrganizationName { get; set; }
        public string ContactName { get; set; }
        public List<PhoneInfo> Phones { get; set; }
        public List<EmailInfo> Emails { get; set; }
        public List<AddressInfo> Addresses { get; set; }
    }
}