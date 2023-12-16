using System;
using JahanJooy.Common.Util.DomainModel;

namespace JahanJooy.RealEstate.Domain
{
	public class ApiUser : ICreationTime, ILastModificationTime
	{
		public long ID { get; set; }
		public string Key { get; set; }

		public DateTime CreationTime { get; set; }
		public DateTime LastModificationTime { get; set; }
		public DateTime? ExpirationTime { get; set; }

		public long RequestedByUserID { get; set; }
		public long? ReviewedByUserID { get; set; }

		public long DailyCallQuota { get; set; }
		public long DailyCallQuotaPerUser { get; set; }

		public bool RequireSessions { get; set; }
		public bool RequireSignature { get; set; }
		public bool RequireAuthentication { get; set; }
	}
}