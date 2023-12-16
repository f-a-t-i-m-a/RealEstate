using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Web.Models.Shared.Profile.Tabs
{
	public class ProfileTabGeneralModel
	{
		public User User { get; set; }
        public UserBillingBalance Balance { get; set; }

		public bool EnableEdit { get; set; }
		public bool EnableAdmin { get; set; }

        
	}
}