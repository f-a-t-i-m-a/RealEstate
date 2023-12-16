using System;
using System.Globalization;
using System.Collections.Generic;
using JahanJooy.Common.Util.General;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Directory;
using JahanJooy.RealEstate.Domain.Permissions;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Domain
{
	public class User
	{
		#region Constructors

		public User()
		{
			GeneralPermissions = new HashSet<GeneralPermission>();
		}

		#endregion

		public long ID { get; set; }
		public long Code { get; set; }
		public bool IsOperator { get; set; }

		public string LoginName { get; set; }
		public string PasswordHash { get; set; }
		public string PasswordSalt { get; set; }
		public bool IsEnabled { get; set; }
        public bool? Approved { get; set; }

        
		public DateTime CreationDate { get; set; }
		public DateTime ModificationDate { get; set; }
        public DateTime? DeleteTime { get; set; }
		public long CreatorSessionID { get; set; }
		public HttpSession CreatorSession { get; set; }

		public DateTime? LastLogin { get; set; }
		public DateTime? LastLoginAttempt { get; set; }
		public int FailedLoginAttempts { get; set; }

		public string DisplayName { get; set; }
		public string FullName { get; set; }

        public bool ShowInUsersList { get; set; }
        public string Activity { get; set; }
        public string About { get; set; }
        public string Services { get; set; }
        public string WorkBackground { get; set; }
        public string Address { get; set; }
        public string WebSiteUrl { get; set; }
	    public UserType Type { get; set; }
        public Agency Agency { get; set; }
        public long? AgencyID { get; set; }
        public Guid? ProfilePictureStoreItemID { get; set; }
       
	    //
        // Redundant re-calculatable properties

        public bool IsVerified { get; set; }
        public int ReputationScore { get; set; }

        //
        // Relation properties

		public ICollection<UserContactMethod> ContactMethods { get; set; } 
		public ICollection<UserFavoritePropertyListing> FavoritePropertyListings { get; set; }

		public ICollection<GeneralPermission> GeneralPermissions { get; set; }
        public ICollection<UserBillingTransaction> BillingTransactions { get; set; } 

		#region Field wrappers

		public string GeneralPermissionsValue
		{
		    get { return CsvUtils.ToCsvString(GeneralPermissions, p => ((int) p).ToString(CultureInfo.InvariantCulture)); }
		    set { GeneralPermissions = new HashSet<GeneralPermission>(CsvUtils.ParseEnumArray<GeneralPermission>(value)); }
		}

		#endregion
	}

}