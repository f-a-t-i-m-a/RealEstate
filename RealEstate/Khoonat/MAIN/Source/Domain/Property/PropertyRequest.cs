using System;
using JahanJooy.Common.Util.DomainModel;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Domain.Property
{
	public class PropertyRequest : ICreationTime, ILastModificationTime, IIndexedEntity, IEntityContentContainer<PropertyRequestContent>
	{
		public long ID { get; set; }
		public long Code { get; set; }
		public DateTime? DeleteDate { get; set; }
		public bool? Approved { get; set; }

		public DateTime CreationTime { get; set; }
		public DateTime LastModificationTime { get; set; }
		public DateTime? IndexedTime { get; set; }

		//
		// Owner

		public string EditPassword { get; set; }
		public long? CreatorSessionID { get; set; }
		public HttpSession CreatorSession { get; set; }
		public long? CreatorUserID { get; set; }
		public User CreatorUser { get; set; }
		public long? OwnerUserID { get; set; }
		public User OwnerUser { get; set; }

		//
		// Statistics

		public long NumberOfVisits { get; set; }
		public long NumberOfContactInfoRetrievals { get; set; }
		public long NumberOfSearches { get; set; }

		//
		// Content

		public string ContentString { get; set; }
		public PropertyRequestContent Content { get; set; }
	}
}