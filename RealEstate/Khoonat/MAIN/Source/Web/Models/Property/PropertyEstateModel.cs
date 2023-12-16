using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Resources.Models.Property;

namespace JahanJooy.RealEstate.Web.Models.Property
{
	public class PropertyEstateModel : PropertySectionModelBase
	{
		public PropertyEstateModel(long propertyListingID, long? propertyListingCode = null, PropertyType? propertyType = null, IntentionOfOwner? intentionOfOwner = null)
			: base(propertyListingID, propertyListingCode, propertyType, intentionOfOwner)
		{
		}

		#region Main properties

		[Numeric(ErrorMessageResourceType = typeof (PropertyEstateResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertyEstateResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[Range(1, 10000000, ErrorMessageResourceType = typeof (PropertyEstateResources), ErrorMessageResourceName = "Validation_Area_Range")]
		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_Area")]
		public decimal? Area { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_Direction")]
		public EstateDirection? Direction { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_VoucherType")]
		public EstateVoucherType? VoucherType { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_Sides")]
		public int? Sides { get; set; }

		[Numeric(ErrorMessageResourceType = typeof (PropertyEstateResources), ErrorMessageResourceName = "Validation_PassageEdgeLength_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertyEstateResources), ErrorMessageResourceName = "Validation_PassageEdgeLength_Numeric")]
		[Range(1, 1000, ErrorMessageResourceType = typeof (PropertyEstateResources), ErrorMessageResourceName = "Validation_PassageEdgeLength_Range")]
		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_PassageEdgeLength")]
		public decimal? PassageEdgeLength { get; set; }

		[Numeric(ErrorMessageResourceType = typeof (PropertyEstateResources), ErrorMessageResourceName = "Validation_PassageWidth_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertyEstateResources), ErrorMessageResourceName = "Validation_PassageWidth_Numeric")]
		[Range(1, 100, ErrorMessageResourceType = typeof (PropertyEstateResources), ErrorMessageResourceName = "Validation_PassageWidth_Range")]
		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_PassageWidth")]
		public decimal? PassageWidth { get; set; }

		#endregion

		#region Situation

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_IsInDeadEnd")]
		public bool? IsInDeadEnd { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_IsCloseToHighway")]
		public bool? IsCloseToHighway { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_IsOnMainPassage")]
		public bool? IsOnMainPassage { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_IsOnGreenPassage")]
		public bool? IsOnGreenPassage { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_IsCloseToPark")]
		public bool? IsCloseToPark { get; set; }

		#endregion

		#region Other properties

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_SlopeAmount")]
		public EstateSlopeAmount? SlopeAmount { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_SurfaceType")]
		public EstateSurfaceType? SurfaceType { get; set; }

		#endregion

		#region Utilities

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_HasElectricity")]
		public bool? HasElectricity { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_HasThreePhaseElectricity")]
		public bool? HasThreePhaseElectricity { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_HasIndustrialElectricity")]
		public bool? HasIndustrialElectricity { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_HasDrinkingWater")]
		public bool? HasDrinkingWater { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_HasCultivationWater")]
		public bool? HasCultivationWater { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_HasGasPiping")]
		public bool? HasGasPiping { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_HasSewerExtension")]
		public bool? HasSewerExtension { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_HasWaterWells")]
		public bool? HasWaterWells { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_HasWaterWellsPrivilege")]
		public bool? HasWaterWellsPrivilege { get; set; }

		[Display(ResourceType = typeof (PropertyEstateResources), Name = "Label_HasNiches")]
		public bool? HasNiches { get; set; }

		#endregion

		#region Convert to/from domain

		public static PropertyEstateModel CreateFromDto(PropertyListingDetails domain, bool showAllAttributes = false)
		{
			var result = new PropertyEstateModel(domain.ID, domain.Code, domain.PropertyType, domain.IntentionOfOwner) {ShowAllAttributes = showAllAttributes};

			if (domain.Estate != null)
			{
				result.Area = domain.Estate.Area;
				result.Direction = domain.Estate.Direction;
				result.VoucherType = domain.Estate.VoucherType;

				result.Sides = domain.Estate.Sides;
				result.PassageEdgeLength = domain.Estate.PassageEdgeLength;
				result.PassageWidth = domain.Estate.PassageWidth;

				result.IsInDeadEnd = domain.Estate.IsInDeadEnd;
				result.IsCloseToHighway = domain.Estate.IsCloseToHighway;
				result.IsOnMainPassage = domain.Estate.IsOnMainPassage;
				result.IsOnGreenPassage = domain.Estate.IsOnGreenPassage;
				result.IsCloseToPark = domain.Estate.IsCloseToPark;

				result.SlopeAmount = domain.Estate.SlopeAmount;
				result.SurfaceType = domain.Estate.SurfaceType;

				result.HasElectricity = domain.Estate.HasElectricity;
				result.HasThreePhaseElectricity = domain.Estate.HasThreePhaseElectricity;
				result.HasIndustrialElectricity = domain.Estate.HasIndustrialElectricity;
				result.HasDrinkingWater = domain.Estate.HasDrinkingWater;
				result.HasCultivationWater = domain.Estate.HasCultivationWater;
				result.HasGasPiping = domain.Estate.HasGasPiping;
				result.HasSewerExtension = domain.Estate.HasSewerExtension;
				result.HasWaterWells = domain.Estate.HasWaterWells;
				result.HasWaterWellsPrivilege = domain.Estate.HasWaterWellsPrivilege;
				result.HasNiches = domain.Estate.HasNiches;
			}

			return result;
		}

		public void UpdateDomain(PropertyListing domain)
		{
			if (domain.Estate == null)
				domain.Estate = new Estate();

			domain.Estate.Area = Area;
			domain.Estate.Direction = Direction;
			domain.Estate.VoucherType = VoucherType;

			domain.Estate.Sides = Sides;
			domain.Estate.PassageEdgeLength = PassageEdgeLength;
			domain.Estate.PassageWidth = PassageWidth;

			domain.Estate.IsInDeadEnd = IsInDeadEnd;
			domain.Estate.IsCloseToHighway = IsCloseToHighway;
			domain.Estate.IsOnMainPassage = IsOnMainPassage;
			domain.Estate.IsOnGreenPassage = IsOnGreenPassage;
			domain.Estate.IsCloseToPark = IsCloseToPark;

			domain.Estate.SlopeAmount = SlopeAmount;
			domain.Estate.SurfaceType = SurfaceType;

			domain.Estate.HasElectricity = HasElectricity;
			domain.Estate.HasThreePhaseElectricity = HasThreePhaseElectricity;
			domain.Estate.HasIndustrialElectricity = HasIndustrialElectricity;
			domain.Estate.HasDrinkingWater = HasDrinkingWater;
			domain.Estate.HasCultivationWater = HasCultivationWater;
			domain.Estate.HasGasPiping = HasGasPiping;
			domain.Estate.HasSewerExtension = HasSewerExtension;
			domain.Estate.HasWaterWells = HasWaterWells;
			domain.Estate.HasWaterWellsPrivilege = HasWaterWellsPrivilege;
			domain.Estate.HasNiches = HasNiches;
		}

		#endregion
	}
}