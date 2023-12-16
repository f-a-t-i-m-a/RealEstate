namespace JahanJooy.RealEstate.Domain.Property
{
	public class PropertyListingContactInfo
	{
		public long ID { get; set; }

		public string AgencyName { get; set; }
		public string AgencyAddress { get; set; }
		public string ContactName { get; set; }
		public string ContactPhone1 { get; set; }
		public string ContactPhone2 { get; set; }
		public string ContactEmail { get; set; }

        public bool ContactPhone1Verified { get; set; }
        public bool ContactPhone2Verified { get; set; }
        public bool ContactEmailVerified { get; set; }

		public bool OwnerCanBeContacted { get; set; }
		public string OwnerName { get; set; }
		public string OwnerPhone1 { get; set; }
		public string OwnerPhone2 { get; set; }
		public string OwnerEmail { get; set; }
	}
}