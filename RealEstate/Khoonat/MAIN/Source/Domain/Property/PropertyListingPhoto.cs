using System;
using JahanJooy.Common.Util.DomainModel;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Domain.Property
{
	public class PropertyListingPhoto : ICreationTime
	{
		public long ID { get; set; }
		public Guid StoreItemID { get; set; }
		public DateTime? DeleteTime { get; set; }
		public bool? Approved { get; set; }

		public long PropertyListingID { get; set; }
		public PropertyListing PropertyListing { get; set; }

		public long UntouchedLength { get; set; }
		public long ThumbnailLength { get; set; }
		public long MediumSizeLength { get; set; }
		public long FullSizeLength { get; set; }

		public PropertyListingPhotoSubject? Subject { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public long Order { get; set; }

		public DateTime CreationTime { get; set; }
		public long? CreatorSessionID { get; set; }
		public HttpSession CreatorSession { get; set; }
		public long? CreatorUserID { get; set; }
		public User CreatorUser { get; set; }
    }
}