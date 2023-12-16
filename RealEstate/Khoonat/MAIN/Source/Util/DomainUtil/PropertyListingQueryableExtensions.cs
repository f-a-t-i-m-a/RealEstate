using System.Data.Entity;
using System.Linq;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Util.DomainUtil
{
	public static class PropertyListingQueryableExtensions
	{
		public static IQueryable<PropertyListing> IncludeInfoProperties(this IQueryable<PropertyListing> listings)
		{
			return listings.Include(l => l.Estate)
				.Include(l => l.Building)
				.Include(l => l.Unit)
				.Include(l => l.SaleConditions)
				.Include(l => l.RentConditions)
				.Include(l => l.ContactInfo);
		}

		public static IQueryable<PropertyListing> IncludeAllDetailsProperties(this IQueryable<PropertyListing> listings)
		{
			return listings.IncludeInfoProperties()
				.Include(l => l.Photos)
				.Include(l => l.OwnerUser)
				.Include(l => l.FavoritedBy);
		}
	}
}