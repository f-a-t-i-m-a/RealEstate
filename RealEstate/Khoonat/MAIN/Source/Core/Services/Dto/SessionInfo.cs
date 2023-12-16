using System.Collections.Generic;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Services.Dto
{
	public class SessionInfo
	{
		public static readonly SessionInfo Default = new SessionInfo
		                                             {
			                                             Record = null,
			                                             OwnedProperties = null,
			                                             IsCrawler = false
		                                             };

	    public HttpSession Record { get; set; }
	    public HashSet<long> OwnedProperties { get; set; }

	    public bool IsCrawler { get; set; }

		public bool IsValidSession => Record != null;

	    public bool CheckIfOwnsProperty(long id)
		{
			return OwnedProperties != null && OwnedProperties.Contains(id);
		}
	}
}
