using System.Collections.Generic;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Models.Shared.Profile
{
	public class ProfileModel
	{
		public User User { get; set; }
        public UserBillingBalance Balance { get; set; }
		public ProfileActiveTab ActiveTab { get; set; }

		public PagedList<PropertyListingSummary> PropertyListings { get; set; }
		public PagedList<PropertyListingPublishHistory> PropertyPublishes { get; set; }
		public PagedList<PropertySearchHistory> Searches { get; set; } 

		public PagedList<HttpSession> Sessions { get; set; }
		public IEnumerable<PasswordResetRequest> LatestPasswordResets { get; set; }
		public IEnumerable<LoginNameQuery> LatestLoginNameQueries { get; set; }

		public string PaginationUrlTemplate { get; set; }

		public bool EnableAdmin { get; set; }
		public bool EnableEdit { get; set; }

		public enum ProfileActiveTab
		{
			General,
			AllPropertyListings,
			PublishedPropertyListings,
			PublishHistory,
			Searches,
			SecurityInfo
		}
	}
}