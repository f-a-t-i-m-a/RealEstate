using JahanJooy.Common.Util.DomainModel;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Domain.Property
{
	public class PropertyRequestContent : IEntityContent
	{
		public PropertyType PropertyType { get; set; }
		public IntentionOfOwner Intention { get; set; }
		public string Description { get; set; }
		public long[] VicinityIDs { get; set; }
	}
}