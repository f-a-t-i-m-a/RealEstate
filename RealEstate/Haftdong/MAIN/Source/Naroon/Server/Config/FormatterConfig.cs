using System.Web.Http;
using JahanJooy.RealEstateAgency.HaftDong.Server.Formatters;

namespace JahanJooy.RealEstateAgency.Naroon.Server.Config
{
	public static class FormatterConfig
	{
		public static void Setup(HttpConfiguration configuration)
		{
			configuration.Formatters.Insert(0, new ServiceStackTextFormatter());
		}
	}
}