using System;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
    public class ApiPropertyPhotoOutputModel
    {
        public long ID { get; set; }

        public PropertyListingPhotoSubject? Subject { get; set; }
        public string SubjectStr { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long Order { get; set; }

        public long ThumbnailLength { get; set; }
        public long MediumSizeLength { get; set; }
        public long FullSizeLength { get; set; }
        
		[Obsolete("Renamed to CreationTime")]
		public DateTime CreationDate { get; set; }
        public DateTime CreationTime { get; set; }

		public ApiFileContentModel ThumbnailFile { get; set; }
		public ApiFileContentModel MediumSizeFile { get; set; }
		public ApiFileContentModel FillSizeFile { get; set; }
    }
}