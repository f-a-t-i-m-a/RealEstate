using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Domain.Property
{
	public class RentConditions
	{
		public long ID { get; set; }

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
	}
}