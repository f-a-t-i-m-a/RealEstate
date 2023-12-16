namespace JahanJooy.RealEstate.Domain.SavedSearch
{
	public class SavedPropertySearchGeographicRegion
	{
		public long ID { get; set; }

		public SavedPropertySearch PropertySearch { get; set; } 
		public long PropertySearchID { get; set; }

		public Vicinity Vicinity { get; set; }
		public long VicinityID { get; set; }
	}
}