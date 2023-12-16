using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Identity.MongoDB;
using JahanJooy.Common.Util.General;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Domain.Users
{
    public class ApplicationUser : IdentityUser
    {
        public long Code { get; set; }
        public bool IsOperator { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsVerified { get; set; }

        public DateTime CreationTime { get; set; }
        public ObjectId? CreatedByID { get; set; }
        public DateTime? ModificationTime { get; set; }
        public DateTime? DeletionTime { get; set; }
        public DateTime? LastIndexingTime { get; set; }

        public LicenseType? LicenseType { get; set; }
        public DateTime? LicenseActivationTime { get; set; }

        public DateTime? LastLogin { get; set; }
        public DateTime? LastLoginAttempt { get; set; }
        public int FailedLoginAttempts { get; set; }

        public string DisplayName { get; set; }

        public string About { get; set; }
        public string WebSiteUrl { get; set; }
        public UserType Type { get; set; }
        public long? AgencyID { get; set; }
        public PhotoInfo ProfilePicture { get; set; }

        public ContactMethodCollection Contact { get; set; }

        public ICollection<UserGeneralPermission> GeneralPermissions { get; set; }

        public string GeneralPermissionsValue
        {
            get { return CsvUtils.ToCsvString(GeneralPermissions, p => ((int)p).ToString(CultureInfo.InvariantCulture)); }
            set { GeneralPermissions = new HashSet<UserGeneralPermission>(CsvUtils.ParseEnumArray<UserGeneralPermission>(value)); }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}