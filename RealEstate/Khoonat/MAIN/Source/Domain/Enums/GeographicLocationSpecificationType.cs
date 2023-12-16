namespace JahanJooy.RealEstate.Domain.Enums
{
	public enum GeographicLocationSpecificationType : byte
	{
		UserSpecifiedExact = 1,
		UserSpecifiedApproximate = 2,
		InferredFromVicinity = 101,
		InferredFromVicinityAndAddress = 102
	}
}