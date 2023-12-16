namespace JahanJooy.RealEstate.Web.Areas.Ad.Models.AdAdminHome
{
    public class AdAdminHomeStatisticsModel
    {
        public long SponsoredPropertiesQueueLength { get; set; }
        
        public long TotalQueueLength
        {
            get { return SponsoredPropertiesQueueLength; }
        }
    }
}