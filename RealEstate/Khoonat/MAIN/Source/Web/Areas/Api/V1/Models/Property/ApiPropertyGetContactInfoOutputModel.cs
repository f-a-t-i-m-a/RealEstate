using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property
{
    public class ApiPropertyGetContactInfoOutputModel : ApiOutputModelBase
    {
        public long ID { get; set; }

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone1 { get; set; }
        public string ContactPhone2 { get; set; }
        
        public string AgencyName { get; set; }
        public string AgencyAddress { get; set; }

        public bool ContactPhone1Verified { get; set; }
        public bool ContactPhone2Verified { get; set; }
        public bool ContactEmailVerified { get; set; }

        public bool OwnerCanBeContacted { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhone1 { get; set; }
        public string OwnerPhone2 { get; set; }
        public string OwnerEmail { get; set; }

        public bool IsAgencyListing { get; set; }
        public bool IsAgencyActivityAllowed { get; set; }


        public static ApiPropertyGetContactInfoOutputModel FromDomain(PropertyListingDetails domain)
        {
            var output = new ApiPropertyGetContactInfoOutputModel
            {
                ID = domain.ContactInfo.ID,
                ContactName = domain.ContactInfo.ContactName,
                ContactEmail = domain.ContactInfo.ContactEmail,
                ContactPhone1 = domain.ContactInfo.ContactPhone1,
                ContactPhone2 = domain.ContactInfo.ContactPhone2,
                AgencyName = domain.ContactInfo.AgencyName,
                AgencyAddress = domain.ContactInfo.AgencyAddress,
                ContactPhone1Verified = domain.ContactInfo.ContactPhone1Verified,
                ContactPhone2Verified = domain.ContactInfo.ContactPhone2Verified,
                ContactEmailVerified = domain.ContactInfo.ContactEmailVerified,
                OwnerCanBeContacted = domain.ContactInfo.OwnerCanBeContacted,
                OwnerName = domain.ContactInfo.OwnerName,
                OwnerPhone1 = domain.ContactInfo.OwnerPhone1,
                OwnerPhone2 = domain.ContactInfo.OwnerPhone2,
                OwnerEmail = domain.ContactInfo.OwnerEmail,
                IsAgencyListing = domain.IsAgencyListing,
                IsAgencyActivityAllowed = domain.IsAgencyActivityAllowed
            };

            return output;

        }
    }
}