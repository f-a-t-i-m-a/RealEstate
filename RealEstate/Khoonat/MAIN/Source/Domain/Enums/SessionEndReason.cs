namespace JahanJooy.RealEstate.Domain.Enums
{
	public enum SessionEndReason : byte
	{
		Timeout = 1,
		Logout = 2,
		ServerShutdown = 3,
		ServerCrash = 4,
		UserDisabled = 5,
		UserAuthenticated = 6,
		UserRegistered = 7
	}
}