namespace JahanJooy.RealEstate.Domain.Ad
{
    public interface ISponsoredEntityRelated
    {
        long SponsoredEntityID { get; set; }
        SponsoredEntity SponsoredEntity { get; set; }
    }
}