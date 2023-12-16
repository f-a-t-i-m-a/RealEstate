namespace JahanJooy.RealEstate.Domain.Audit
{
	public enum AbuseFlagEntityType : byte
	{
		User = 1,
		PropertyListing = 2,
        Agency = 3
	}

	public enum AbuseFlagReason : byte
	{
		NeedsModeratorAttention = 1,
		HasIncorrectOrMisleadingInformation = 2,
		IsOffensive = 3,
		IsAbusiveOrHasSecurityRisk = 4,
		IsSpam = 5
	}
}