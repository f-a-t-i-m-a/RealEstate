namespace JahanJooy.RealEstate.Core.Services.Enums
{
	public static class PropertyValidationErrors
	{
		public const string EstateIsNull = "EstateIsNull";
		public const string BuildingIsNull = "BuildingIsNull";
		public const string UnitIsNull = "UnitIsNull";
		public const string SaleConditionsIsNull = "SaleConditionsIsNull";
		public const string RentConditionsIsNull = "RentConditionsIsNull";
		public const string ContactInfoIsNull = "ContactInfoIsNull";
        public const string SelectedVicinityCannotContainPropertyRecords = "SelectedVicinityCannotContainPropertyRecords";
	}
}