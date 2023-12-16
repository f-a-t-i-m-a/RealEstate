using System;
using System.Collections.Generic;
using JahanJooy.Common.Util.DomainModel;

namespace JahanJooy.RealEstate.Domain.Directory
{
	public class Agency : ICreationTime, ILastModificationTime, IIndexedEntity, IEntityContentContainer<AgencyContent>
	{
		public long ID { get; set; }

		public DateTime CreationTime { get; set; }
		public DateTime LastModificationTime { get; set; }
		public DateTime? DeleteTime { get; set; }
		public DateTime? IndexedTime { get; set; }
        public bool? Approved { get; set; }

		public ICollection<AgencyBranch> AgencyBranches { get; set; }
		public ICollection<User> MemberUsers { get; set; }

		//
		// Content

		public string ContentString { get; set; }
		public AgencyContent Content { get; set; }
	}
}
