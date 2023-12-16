using System;
using System.Collections.Generic;
using System.Linq;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Domain.Audit
{
	public class HttpSession
	{
		public long ID { get; set; }

		public SessionType Type { get; set; }
		public DateTime Start { get; set; }
		public DateTime? End { get; set; }
		public SessionEndReason? EndReason { get; set; }

		public string UserAgent { get; set; }
		public string StartupUri { get; set; }
		public string ReferrerUri { get; set; }
		public string ClientAddress { get; set; }
		public string HttpSessionID { get; set; }
		public string PrevHttpSessionID { get; set; }
		public bool GotInteractiveAck { get; set; }

		public long? UserID { get; set; }
		public User User { get; set; }

		public long UniqueVisitorID { get; set; }
		public UniqueVisitor UniqueVisitor { get; set; }

		public ICollection<ActivityLog> ActivityLogs { get; set; }

	    public HttpSession()
	    {
	    }

	    public HttpSession(HttpSession source)
	    {
	        ID = source.ID;

	        Type = source.Type;
	        Start = source.Start;
	        End = source.End;
	        EndReason = source.EndReason;

	        UserAgent = source.UserAgent;
	        StartupUri = source.StartupUri;
	        ReferrerUri = source.ReferrerUri;
	        ClientAddress = source.ClientAddress;
	        HttpSessionID = source.HttpSessionID;
	        PrevHttpSessionID = source.PrevHttpSessionID;
	        GotInteractiveAck = source.GotInteractiveAck;

	        UserID = source.UserID;
	        User = null;

	        UniqueVisitorID = source.UniqueVisitorID;
	        UniqueVisitor = null;

	        ActivityLogs = null;
	    }

	    public static HttpSession Copy(HttpSession source)
	    {
	        return source == null ? null : new HttpSession(source);
	    }

	    public static List<HttpSession> Copy(IEnumerable<HttpSession> sources)
	    {
	        return sources?.Select(Copy).ToList();
	    }
	}
}