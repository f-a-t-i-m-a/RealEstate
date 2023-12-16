using System.ComponentModel.DataAnnotations;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
	public class ApiPropertyPhotoInputModel
	{
		public PropertyListingPhotoSubject? Subject { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		[Required]
		public ApiFileContentModel File { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<ApiPropertyPhotoInputModel, PropertyListingPhoto>()
				.IgnoreSource(m => m.File)
				.IgnoreUnmappedProperties();
		}
	}
}