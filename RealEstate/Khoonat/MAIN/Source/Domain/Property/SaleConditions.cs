using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Domain.Property
{
	public class SaleConditions
	{
		public long ID { get; set; }

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

		#region Price calculations

		public void RecalculatePrices(decimal? estateArea, decimal? unitArea)
		{
			if (!PriceSpecificationType.HasValue)
				return;

			switch (PriceSpecificationType.Value)
			{
				case SalePriceSpecificationType.PerEstateArea:
					Price = (PricePerEstateArea.HasValue && estateArea.HasValue) ? estateArea.Value*PricePerEstateArea : null;
					break;

				case SalePriceSpecificationType.PerUnitArea:
					Price = (PricePerUnitArea.HasValue && unitArea.HasValue) ? unitArea.Value*PricePerUnitArea : null;
					break;
			}

			if (PriceSpecificationType.Value != SalePriceSpecificationType.PerEstateArea)
				PricePerEstateArea = (Price.HasValue && estateArea.HasValue && estateArea.Value != decimal.Zero) ? Price/estateArea.Value : null;
			if (PriceSpecificationType.Value != SalePriceSpecificationType.PerUnitArea)
				PricePerUnitArea = (Price.HasValue && unitArea.HasValue && unitArea.Value != decimal.Zero) ? Price/unitArea.Value : null;

			if (Price.HasValue)
				Price = decimal.Round(Price.Value, 0);
			if (PricePerEstateArea.HasValue)
				PricePerEstateArea = decimal.Round(PricePerEstateArea.Value, 0);
			if (PricePerUnitArea.HasValue)
				PricePerUnitArea = decimal.Round(PricePerUnitArea.Value, 0);
		}

		#endregion
	}
}