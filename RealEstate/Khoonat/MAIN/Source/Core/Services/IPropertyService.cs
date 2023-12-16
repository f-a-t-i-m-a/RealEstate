using System;
using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
	public interface IPropertyService
	{
		ValidationResult Save(PropertyListing listing);
		ValidationResult Update(long listingID, Func<PropertyListing, EntityUpdateResult> updateFunction);
		bool ValidateEditPassword(long propertyListingId, string editPassword);
		void SetOwner(IEnumerable<long> propertyIds, long userId);
		void SetApproved(long listingId, bool? approved);

        ValidationResult Publish(long listingID, int days);
		void UnPublish(long listingID);
		ToggleFavoriteResult ToggleFavorite(long listingID);

		ValidationResult ValidateForSave(PropertyListing listing);
		ValidationResult ValidateForPublish(PropertyListingDetails listing);

		void Delete(long listingID);

        PropertyListingDetails Load(long listingID, bool includeEstate = false, bool includeBuilding = false, bool includeUnit = false, bool includeSaleAndRentConditions = false, bool includeContactInfo = false, bool includeDeleted = false);
		PropertyListingDetails LoadWithAllDetails(long listingID, bool includeDeleted = false);
		PropertyListingDetails LoadForVisit(long listingID);
		PropertyListingDetails LoadForContactInfo(long listingID);
		PropertyListingSummary LoadSummary(long listingID);
		List<PropertyListingSummary> LoadSummaries(IEnumerable<long> listingIDs);

		SearchResult RunSearch(PropertySearch search, int startIndex, int pageSize, bool includeSponsored, bool incrementNumberOfSearches);
		long? FindSearchIndex(PropertySearch search, int index);

		PagedList<PropertyListingSummary> ListingsOfUser(long? userId, bool publicOnly, bool publishedOnly, int page, int pageSize);
	}
}