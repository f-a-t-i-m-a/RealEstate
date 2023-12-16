using System.Collections.Generic;

namespace JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch
{
	public class GlobalSearchIndexItem : GlobalSearchResultItem
	{
		public string Text { get; set; }
		public List<string> Tags { get; set; }

		public List<string> PrincipalsAllowedToView { get; set; }
		public bool Archived { get; set; }
		public bool Deleted { get; set; }
	}
}