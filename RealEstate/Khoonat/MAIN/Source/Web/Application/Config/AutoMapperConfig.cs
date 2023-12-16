using AutoMapper;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Search;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared.Spatial;
using JahanJooy.RealEstate.Web.Models.AgencySelector;
using JahanJooy.RealEstate.Web.Models.PropertyRequest;

namespace JahanJooy.RealEstate.Web.Application.Config
{
	public static class AutoMapperConfig
	{
		public static void ConfigureAllMappers()
		{
			ApiGeoPoint.ConfigureMapper();
			ApiGeoBox.ConfigureMapper();
			ApiOutputUserModel.ConfigureMapper();
			ApiOutputValidationErrorsModel.ConfigureMapper();
			ApiOutputPaginationStats.ConfigureMapper();

			ApiPropertyDetailsModel.ConfigureMapper();
			ApiPropertyGeneralModel.ConfigureMapper();
			ApiPropertyEstateModel.ConfigureMapper();
			ApiPropertyBuildingModel.ConfigureMapper();
			ApiPropertyUnitModel.ConfigureMapper();
			ApiPropertySaleConditionsModel.ConfigureMapper();
			ApiPropertyRentConditionsModel.ConfigureMapper();
			ApiPropertyContactInfoModel.ConfigureMapper();
			ApiPropertyDeliveryModel.ConfigureMapper();
			ApiPropertyVisitModel.ConfigureMapper();
			ApiPropertyPhotoInputModel.ConfigureMapper();

			ApiPropertyGetDetailsOutputModel.ConfigureMapper();
			ApiPropertyCreateNewOutputModel.ConfigureMapper();

			ApiSearchRunInputCriteriaModel.ConfigureMapper();
			ApiSearchRunOutputModel.ConfigureMapper();

			PropertyRequestEditModel.ConfigureMapper();
		    AgencySelectorResultItemModel.ConfigureMapper();

			Mapper.AssertConfigurationIsValid();
		}
	}
}