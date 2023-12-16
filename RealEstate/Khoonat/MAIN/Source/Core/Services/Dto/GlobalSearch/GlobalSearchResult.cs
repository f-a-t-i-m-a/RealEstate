using System.Collections.Generic;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Spatial;

namespace JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch
{
	public class GlobalSearchResult
	{
		public LatLngBounds Bounds { get; set; }
		public PaginationStats Stats { get; set; }
		public List<GlobalSearchResultItem> Items { get; set; }
		public Dictionary<long, GlobalSearchResultCluster> Clusters { get; set; }
	}
}