using System;
using System.Collections.Generic;
using System.Linq;
using JahanJooy.Common.Util;

namespace JahanJooy.RealEstate.Domain.Audit
{
	public class UniqueVisitor
	{
		public long ID { get; set; }
		public string UniqueIdentifier { get; set; }

		public DateTime CreationDate { get; set; }
		public DateTime LastVisitDate { get; set; }

		public ICollection<HttpSession> HttpSessions { get; set; } 

		#region Initialization

		public UniqueVisitor()
		{
		}

		public UniqueVisitor(UniqueVisitor source)
		{
			ID = source.ID;
			UniqueIdentifier = source.UniqueIdentifier;

			CreationDate = source.CreationDate;
			LastVisitDate = source.LastVisitDate;
		}

		public static UniqueVisitor Copy(UniqueVisitor source)
		{
			return source.IfNotNull(s => new UniqueVisitor(s));
		}

		public static ICollection<UniqueVisitor> Copy(ICollection<UniqueVisitor> source)
		{
			return source?.Select(Copy).ToList();
		}

		#endregion
	}
}