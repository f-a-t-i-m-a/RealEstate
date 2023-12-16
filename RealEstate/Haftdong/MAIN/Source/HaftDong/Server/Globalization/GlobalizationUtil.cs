using System.Globalization;
using System.Threading;

namespace JahanJooy.RealEstateAgency.HaftDong.Server.Globalization
{
	public static class GlobalizationUtil
	{
		private static readonly CultureInfo FaIr;

		static GlobalizationUtil()
		{
			FaIr = new CultureInfo("fa-IR");

			FaIr.NumberFormat.CurrencyDecimalSeparator = ".";
			FaIr.NumberFormat.NumberDecimalSeparator = ".";
			FaIr.NumberFormat.PercentDecimalSeparator = ".";
		}

		public static void SetCurrentCulture()
		{
			Thread.CurrentThread.CurrentCulture = FaIr;
			Thread.CurrentThread.CurrentUICulture = FaIr;
		}
	}
}