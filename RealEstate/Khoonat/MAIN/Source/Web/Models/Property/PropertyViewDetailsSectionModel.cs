namespace JahanJooy.RealEstate.Web.Models.Property
{
	public class PropertyViewDetailsSectionModel
	{
		public string Title { get; set; }
		public string Identifier { get; set; }

		public string EditAction { get; set; }
		public string DetailsViewName { get; set; }

		public object SectionModel { get; set; }
		public long PropertyListingID { get; set; }
		public bool IsOwner { get; set; }
	}
}