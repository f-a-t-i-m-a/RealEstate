using System.Collections.Generic;
using JahanJooy.Common.Util.Spatial;

namespace JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch
{
	public class GlobalSearchCriteria
	{
		public LatLngBounds Bounds { get; set; }
		public List<long> VicinityIDs { get; set; } 
		public bool ClusterGeographically { get; set; }

		public string SearchText { get; set; }

		public List<string> IncludedTags { get; set; }
		public List<string> ExcludedTags { get; set; }

		public List<GlobalSearchRecordType> RecordTypes { get; set; }

		public bool IncludeCurrentRecords { get; set; }
		public bool IncludeArchivedRecords { get; set; }
		public bool IncludeDeletedRecords { get; set; }
	}
}