using System.Reflection;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Api.Web.Properties;
using MongoDB.Driver.GeoJsonObjectModel;

namespace JahanJooy.RealEstateAgency.HaftDong.Server.Config
{
    public static class AutoMapperConfig
    {
        public static void ConfigureAllMappers()
        {
            ConfigureGeneralMappings();

            AutoMapperConfigurator.Scan(Assembly.GetExecutingAssembly());
            AutoMapperConfigurator.Scan(typeof (AssemblyPointer).Assembly);
            AutoMapperConfigurator.Scan(typeof (Api.App.Properties.AssemblyPointer).Assembly);
            AutoMapperConfigurator.Scan(typeof (Api.External.Properties.AssemblyPointer).Assembly);
            AutoMapperConfigurator.Scan(typeof (Domain.Properties.AssemblyPointer).Assembly);
            AutoMapperConfigurator.Scan(typeof (Util.Properties.AssemblyPointer).Assembly);
            Mapper.AssertConfigurationIsValid();
        }

        private static void ConfigureGeneralMappings()
        {
            Mapper.CreateMap<LatLng, GeoJson2DCoordinates>()
                .ConvertUsing(ll => ll != null ? new GeoJson2DCoordinates(ll.Lng, ll.Lat) : null);

            Mapper.CreateMap<GeoJson2DCoordinates, LatLng>()
                .ConvertUsing(c => c != null ? new LatLng {Lat = c.Y, Lng = c.X} : null);
        }
    }
}