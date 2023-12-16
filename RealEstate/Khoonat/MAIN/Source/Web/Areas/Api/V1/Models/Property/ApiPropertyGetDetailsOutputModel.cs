using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Core.DomainExtensions;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property
{
	public class ApiPropertyGetDetailsOutputModel : ApiOutputModelBase
	{
		public long ID { get; set; }
		public long Code { get; set; }
		public bool IsArchived { get; set; }

		public ApiPropertyDetailsModel Details { get; set; }

		public DateTime CreationTime { get; set; }
		public DateTime ModificationTime { get; set; }

        public DateTime? PublishDate { get; set; }
        public DateTime? PublishEndDate { get; set; }

        public ApiOutputUserModel CreatorUser { get; set; }
		public ApiOutputUserModel OwnerUser { get; set; }

		public long NumberOfVisits { get; set; }
		public long NumberOfContactInfoRetrievals { get; set; }
		public long NumberOfSearches { get; set; }
		public int NumberOfPhotos { get; set; }
		public long? CoverPhotoId { get; set; }
		public bool IsFavoritedByUser { get; set; }
		public int TimesFavorited { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<PropertyListingDetails, ApiPropertyGetDetailsOutputModel>()
				.Ignore(m => m.Context)
				.ForMember(m => m.IsArchived, opt => opt.ResolveUsing(detail => !detail.IsPublished()))
				.ForMember(m => m.CreationTime, opt => opt.MapFrom(detail => detail.CreationDate))
				.ForMember(m => m.ModificationTime, opt => opt.MapFrom(detail => detail.ModificationDate))
				.ForMember(m => m.PublishDate, opt => opt.MapFrom(detail => detail.PublishDate))
				.ForMember(m => m.PublishEndDate, opt => opt.MapFrom(detail => detail.PublishEndDate))
				.ForMember(m => m.Details, opt => opt.MapFrom(detail => detail))
				.Ignore(m => m.CreatorUser)
				.Ignore(m => m.OwnerUser);
		}
	}
}