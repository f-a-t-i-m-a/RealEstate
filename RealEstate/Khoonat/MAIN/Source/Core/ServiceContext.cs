using System;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Core.Services.Dto;

namespace JahanJooy.RealEstate.Core
{
	public static class ServiceContext
	{
		[ThreadStatic]
		private static SessionInfo _currentSession;

		[ThreadStatic]
		private static CorePrincipal _principal;

		[ThreadStatic]
		private static int _txRecursionLevel;

		public static SessionInfo CurrentSession => _currentSession ?? SessionInfo.Default;

	    public static CorePrincipal Principal => _principal ?? CorePrincipal.Anonymous;

	    public static int TxRecursionLevel
		{
			get { return _txRecursionLevel; }
			set { _txRecursionLevel = value; }
		}

		public static void Set(SessionInfo currentSession, CorePrincipal principal)
		{
			_currentSession = currentSession;
			_principal = principal;
		}

		public static void Reset()
		{
			_currentSession = null;
			_principal = CorePrincipal.Anonymous;
			_txRecursionLevel = 0;
		}
	}
}