using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
	public class ApiPropertyDetailsModel
	{
		public ApiPropertyGeneralModel General { get; set; }
		public ApiPropertyEstateModel Estate { get; set; }
		public ApiPropertyBuildingModel Building { get; set; }
		public ApiPropertyUnitModel Unit { get; set; }
		public ApiPropertySaleConditionsModel SaleConditions { get; set; }
		public ApiPropertyRentConditionsModel RentConditions { get; set; }
		public ApiPropertyContactInfoModel ContactInfo { get; set; }
		public ApiPropertyVisitModel Visit { get; set; }
		public ApiPropertyDeliveryModel Delivery { get; set; }

		public ApiPropertyUserAccessModel UserAccess { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<PropertyListingDetails, ApiPropertyDetailsModel>()
				.ForMember(m => m.General, opt => opt.MapFrom(details => details))
				.ForMember(m => m.Visit, opt => opt.MapFrom(details => details))
 				.ForMember(m => m.Delivery, opt => opt.MapFrom(details => details))
				.Ignore(m => m.UserAccess);

			Mapper.CreateMap<ApiPropertyDetailsModel, PropertyListing>()
				.IgnoreAll()
				.AfterMap((m, l) => m.General.IfNotNull(inner => Mapper.Map(inner, l)))
				.AfterMap((m, l) => m.Estate.IfNotNull(inner => Mapper.Map(inner, l)))
				.AfterMap((m, l) =>
				          {
					          m.Unit.IfNotNull(inner => Mapper.Map(inner, l));
					          if (l.Unit != null)
					          {
						          if (l.Building == null)
							          l.Building = new Building();
						          if (l.Building.Units == null)
							          l.Building.Units = new List<Unit>();

						          l.Building.Units.Add(l.Unit);
					          }
				          })
				.AfterMap((m, l) =>
				          {
					          m.Building.IfNotNull(inner => Mapper.Map(inner, l));
					          if (l.Building != null)
					          {
						          if (l.Estate == null)
							          l.Estate = new Estate();
						          if (l.Estate.Buildings == null)
							          l.Estate.Buildings = new List<Building>();

						          l.Estate.Buildings.Add(l.Building);
					          }
				          })
				.AfterMap((m, l) => m.SaleConditions.IfNotNull(inner => Mapper.Map(inner, l)))
				.AfterMap((m, l) => m.RentConditions.IfNotNull(inner => Mapper.Map(inner, l)))
				.AfterMap((m, l) => m.ContactInfo.IfNotNull(inner => Mapper.Map(inner, l)))
				.AfterMap((m, l) => m.Visit.IfNotNull(inner => Mapper.Map(inner, l)))
				.AfterMap((m, l) => m.Delivery.IfNotNull(inner => Mapper.Map(inner, l)));

			Mapper.CreateMap<ApiPropertyDetailsModel, Estate>()
				.IgnoreAll()
				.AfterMap((m, e) => Mapper.Map(m.General, e))
				.AfterMap((m, e) => Mapper.Map(m.Estate, e));
		}
	}
}