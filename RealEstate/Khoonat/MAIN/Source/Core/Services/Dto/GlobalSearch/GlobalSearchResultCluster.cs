using System.Collections.Generic;
using JahanJooy.Common.Util.Spatial;

namespace JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch
{
	public class GlobalSearchResultCluster
	{
		public long VicinityID { get; set; }
		public LatLng GeographicLocation { get; set; }
		public bool Decomposable { get; set; }
		public int TotalNumberOfRecords { get; set; }

		public Dictionary<GlobalSearchRecordType, int> NumberOfRecords { get; set; }
	}
}