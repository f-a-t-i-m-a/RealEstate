using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property
{
	public class ApiPropertyCreateNewOutputModel : ApiOutputModelBase
	{
		public long ID { get; set; }
		public long Code { get; set; }
		public string EditPassword { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<PropertyListing, ApiPropertyCreateNewOutputModel>()
				.Ignore(m => m.Context);
		}
	}
}