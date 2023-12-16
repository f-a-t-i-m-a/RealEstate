using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
	public class ApiPropertyRentConditionsModel
	{
		//
		// Rent price

		public decimal? Mortgage { get; set; }
		public decimal? Rent { get; set; }
		public bool MortgageAndRentConvertible { get; set; }
		public decimal? MinimumMortgage { get; set; }
		public decimal? MinimumRent { get; set; }

		//
		// Options

		public bool? IsDischargeGuaranteeChequeRequired { get; set; }
		public bool? DiscountOnBulkPayment { get; set; }

		//
		// Contract duration

		public ContractDuration? MinimumContractDuration { get; set; }
		public ContractDuration? MaximumContractDuration { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<RentConditions, ApiPropertyRentConditionsModel>();

			Mapper.CreateMap<ApiPropertyRentConditionsModel, PropertyListing>()
				.IgnoreAll()
				.ForMember(l => l.RentConditions, opt => opt.MapFrom(m => m));

			Mapper.CreateMap<ApiPropertyRentConditionsModel, RentConditions>()
				.Ignore(rc => rc.ID);
		}
	}
}