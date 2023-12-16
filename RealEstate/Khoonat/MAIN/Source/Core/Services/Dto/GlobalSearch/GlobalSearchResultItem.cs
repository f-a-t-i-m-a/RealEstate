using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch
{
	public class GlobalSearchResultItem
	{
		public GlobalSearchRecordType Type { get; set; }
		public string SubType { get; set; }
		public long ID { get; set; }
		public string Title { get; set; }

		public long? VicinityID { get; set; }
		public LatLng GeographicLocation { get; set; }
		public GeographicLocationSpecificationType? GeographicLocationType { get; set; }
	}
}