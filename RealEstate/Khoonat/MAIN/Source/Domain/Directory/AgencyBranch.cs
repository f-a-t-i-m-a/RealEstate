using System;
using JahanJooy.Common.Util.DomainModel;

namespace JahanJooy.RealEstate.Domain.Directory
{
	public class AgencyBranch : ICreationTime, ILastModificationTime, IIndexedEntity, IEntityContentContainer<AgencyBranchContent>
    {
        public long ID { get; set; }

        public Agency Agency { get; set; }
        public long AgencyID { get; set; }
		public bool IsMainBranch { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }
        public DateTime? DeleteTime { get; set; }
		public DateTime? IndexedTime { get; set; }

		//
		// Content

		public string ContentString { get; set; }
		public AgencyBranchContent Content { get; set; }
    }
}
