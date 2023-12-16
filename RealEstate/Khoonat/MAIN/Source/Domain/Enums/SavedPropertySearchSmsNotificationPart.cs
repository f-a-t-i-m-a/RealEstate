using System;

namespace JahanJooy.RealEstate.Domain.Enums
{
	[Flags]
	public enum SavedPropertySearchSmsNotificationPart : long
	{
		None = 0,
		SiteName = 1L << 0,
		ListingUrl = 1L << 1,
		ListingCode = 1L << 2,
		PropertyType = 1L << 4,
		NumberOfRooms = 1L << 5,
		ShortGeographicRegion = 1L << 8,
		LongGeographicRegion = 1L << 9,
		UserEnteredAddress = 1L << 10,
		Area = 1L << 16,
		PriceOrRent = 1L << 24,
		PerAreaPrice = 1L << 25,
		ContactName = 1L << 32,
		ContactPhone = 1L << 33,
		ContactSecondPhone = 1L << 34,
	}
}