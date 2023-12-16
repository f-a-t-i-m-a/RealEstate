using System.Globalization;
using System.Threading;

namespace JahanJooy.RealEstateAgency.ShishDong.Server.Globalization
{
	public static class GlobalizationUtil
	{
		private static readonly CultureInfo FaIr;

		static GlobalizationUtil()
		{
		    FaIr = new CultureInfo("fa-IR")
		    {
		        NumberFormat =
		        {
		            CurrencyDecimalSeparator = ".",
		            NumberDecimalSeparator = ".",
		            PercentDecimalSeparator = "."
		        }
		    };

		}

		public static void SetCurrentCulture()
		{
			Thread.CurrentThread.CurrentCulture = FaIr;
			Thread.CurrentThread.CurrentUICulture = FaIr;
		}
	}
}