using System.Globalization;
using System.Threading;

namespace JahanJooy.RealEstate.Web.Application.Globalization
{
	public static class GlobalizationUtil
	{
		private static readonly CultureInfo _faIr;

		static GlobalizationUtil()
		{
			_faIr = new CultureInfo("fa-IR");
			
			_faIr.NumberFormat.CurrencyDecimalSeparator = ".";
			_faIr.NumberFormat.NumberDecimalSeparator = ".";
			_faIr.NumberFormat.PercentDecimalSeparator = ".";
		}

		public static void SetCurrentCulture()
		{
			Thread.CurrentThread.CurrentCulture = _faIr;
			Thread.CurrentThread.CurrentUICulture = _faIr;
		}
	}
}