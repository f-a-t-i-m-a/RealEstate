using System.Data.Entity.Spatial;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Core.Services.Dto;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
	public interface IGeographyService
	{
		GeoSearchResult RunSearch(DbGeography bounds, PropertySearch propertyListingFilter = null);
	}
}