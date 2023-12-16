using System.Collections.Generic;

namespace JahanJooy.RealEstate.Web.Models.Properties
{
	public class PropertiesSelectOptionModel
	{
		public string Query { get; set; }
		public IEnumerable<PropertiesSelectOptionItemModel> UnitOptions { get; set; } 
		public IEnumerable<PropertiesSelectOptionItemModel> BuildingOptions { get; set; } 
		public IEnumerable<PropertiesSelectOptionItemModel> EstateOptions { get; set; } 
		public IEnumerable<PropertiesSelectOptionItemModel> OtherOptions { get; set; } 
	}

	public class PropertiesSelectOptionItemModel
	{
		public string Label { get; set; }
		public string Query { get; set; }
	}
}