using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Models.Property
{
	public class PropertyLocationModel : PropertySectionModelBase
	{
		public PropertyLocationModel(long propertyListingID, long? propertyListingCode = null, PropertyType? propertyType = null, IntentionOfOwner? intentionOfOwner = null)
			: base(propertyListingID, propertyListingCode, propertyType, intentionOfOwner)
		{
		}

		public bool ShowApartmentProperties { get; set; }

		#region Address

		public string Address { get; set; }
		public string AdditionalAddressInfo { get; set; }

		#endregion

		#region Convert to/from domain

		public static PropertyLocationModel CreateFromDto(PropertyListingDetails dto, bool showAllAttributes = false)
		{
			var result = new PropertyLocationModel(dto.ID, dto.Code, dto.PropertyType, dto.IntentionOfOwner) {ShowAllAttributes = showAllAttributes};
			result.ShowApartmentProperties = dto.PropertyType.HasUnit() && !dto.PropertyType.IsSingleUnitBuilding();

			if (dto.Estate != null)
			{
				result.FillFrom(dto.Estate);
			}

			return result;
		}

		public void UpdateDomain(PropertyListing domain)
		{
			if (domain.Estate == null)
				domain.Estate = new Estate();

			
			domain.Estate.Address = Address;
			domain.Estate.AdditionalAddressInfo = AdditionalAddressInfo;
		}

		private void FillFrom(Estate estate)
		{
			
			Address = estate.Address;
			if (!string.IsNullOrWhiteSpace(estate.AdditionalAddressInfo))
				AdditionalAddressInfo = estate.AdditionalAddressInfo;
		}

		#endregion
	}
}