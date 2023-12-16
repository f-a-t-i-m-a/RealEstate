using System.Web.Http;
using JahanJooy.RealEstateAgency.ShishDong.Server.Formatters;

namespace JahanJooy.RealEstateAgency.ShishDong.Server.Config
{
	public static class FormatterConfig
	{
		public static void Setup(HttpConfiguration configuration)
		{
			configuration.Formatters.Insert(0, new ServiceStackTextFormatter());
		}
	}
}