using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
	public class ApiPropertySaleConditionsModel
	{
		//
		// Price

		public SalePriceSpecificationType? PriceSpecificationType { get; set; }
		public decimal? Price { get; set; }
		public decimal? PricePerEstateArea { get; set; }
		public decimal? PricePerUnitArea { get; set; }

		//
		// Payment conditions

		public int? PaymentPercentForContract { get; set; }
		public int? PaymentPercentForDelivery { get; set; }
		public bool CanHaveDebt { get; set; }
		public int? PaymentPercentForDebt { get; set; }
		public decimal? MinimumMonthlyPaymentForDebt { get; set; }
		public DebtGuaranteeType? DebtGuaranteeType { get; set; }

		//
		// Loan

		public bool HasTransferableLoan { get; set; }
		public decimal? TransferableLoanAmount { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<SaleConditions, ApiPropertySaleConditionsModel>();

			Mapper.CreateMap<ApiPropertySaleConditionsModel, PropertyListing>()
				.IgnoreAll()
				.ForMember(l => l.SaleConditions, opt => opt.MapFrom(m => m));

			Mapper.CreateMap<ApiPropertySaleConditionsModel, SaleConditions>()
				.Ignore(sc => sc.ID);
		}
	}
}