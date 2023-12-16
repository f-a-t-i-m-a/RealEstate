using System.Reflection;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;

namespace JahanJooy.RealEstateAgency.Naroon.Server.Config
{
	public static class AutoMapperConfig
	{
		public static void ConfigureAllMappers()
		{
			AutoMapperConfigurator.Scan(Assembly.GetExecutingAssembly());
			Mapper.AssertConfigurationIsValid();
		}
	}
}
