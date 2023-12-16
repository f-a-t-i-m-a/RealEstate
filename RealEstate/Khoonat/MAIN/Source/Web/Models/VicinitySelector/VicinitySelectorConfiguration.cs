namespace JahanJooy.RealEstate.Web.Models.VicinitySelector
{
    public class VicinitySelectorConfiguration
    {
        public string Name { get; set; }
        public int MaxNumberOfSelections { get; set; }
        public long? SearchScope { get; set; }
        public string InitialValue { get; set; }
		public bool IncludeDisabled { get; set; }
		public bool FromHomePage { get; set; }
    }
}