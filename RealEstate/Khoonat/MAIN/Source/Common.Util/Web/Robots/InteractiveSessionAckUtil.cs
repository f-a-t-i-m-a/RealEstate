using System;
using System.Web.Security;

namespace JahanJooy.Common.Util.Web.Robots
{
	public static class InteractiveSessionAckUtil
	{
		private const string InteractiveSessionAckMachineKeyPurpose = "InteractiveSessionAck";
		public const int MinValidateDelayMilliSeconds = 2000;
		public const int MaxValidateDelayMilliSeconds = 8000;
		
		public static string GenerateToken()
		{
			var nowBytes = BitConverter.GetBytes(DateTime.Now.ToBinary());
			var protectedNowBytes = MachineKey.Protect(nowBytes, InteractiveSessionAckMachineKeyPurpose);
			return Convert.ToBase64String(protectedNowBytes);
		}

		public static bool ValidateToken(string token)
		{
			var protectedThenBytes = Convert.FromBase64String(token);
			var thenBytes = MachineKey.Unprotect(protectedThenBytes, InteractiveSessionAckMachineKeyPurpose);
			if (thenBytes == null)
				return false;

			var then = DateTime.FromBinary(BitConverter.ToInt64(thenBytes, 0));
			var now = DateTime.Now;

			if (then > now)
				return false;

			var diff = now - then;
			return diff.TotalMilliseconds > MinValidateDelayMilliSeconds && diff.TotalMilliseconds < MaxValidateDelayMilliSeconds;
		}
	}
}