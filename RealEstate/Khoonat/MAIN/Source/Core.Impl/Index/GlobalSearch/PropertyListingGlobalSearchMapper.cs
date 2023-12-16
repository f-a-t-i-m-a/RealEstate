using System;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.Common.Util.Text;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Index;
using JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Util.DomainUtil;
using JahanJooy.RealEstate.Util.Presentation.Property;
using JahanJooy.RealEstate.Util.Resources;
using ServiceStack;

namespace JahanJooy.RealEstate.Core.Impl.Index.GlobalSearch
{
	[Component]
	public class PropertyListingGlobalSearchMapper : IGlobalSearchEntityMapper<PropertyListing>
	{
		[ComponentPlug]
		public PropertyPresentationHelper PresentationHelper { get; set; }

		[ComponentPlug]
		public DbManager DbManager { get; set; }

		public GlobalSearchIndexItem Map(PropertyListing listing)
		{
			if (listing == null)
				return null;

			var summary = PropertyListingSummary.Summarize(PropertyListingDetails.MakeDetails(listing));
			var title = new[]
			            {
				            PresentationHelper.BuildShortTitle(summary),
				            PresentationHelper.BuildAreaString(summary)
			            }.WhereNotNullOrWhitespace().Join(GeneralResources.Comma);

			return new GlobalSearchIndexItem
			       {
				       Type = GlobalSearchRecordType.PropertyListing,
				       SubType = listing.PropertyType.ToString() + listing.IntentionOfOwner,
				       ID = listing.ID,
				       Title = title,
				       VicinityID = listing.Estate.IfNotNull(e => e.VicinityID),
					   GeographicLocation = listing.Estate.IfNotNull(e => e.GeographicLocation.ToLatLng()),
					   GeographicLocationType = listing.Estate.IfNotNull(e => e.GeographicLocationType),
					   Text = string.Empty, // TODO
					   Tags = new List<string>(), // TODO
					   PrincipalsAllowedToView = new List<string>(), // TODO
					   Archived = listing.PublishEndDate.IfHasValue(d => d < DateTime.Now),
					   Deleted = listing.DeleteDate.HasValue
			       };
		}

		public PropertyListing Retrieve(GlobalSearchResultItem item)
		{
			if (item == null || item.Type != GlobalSearchRecordType.PropertyListing || item.ID <= 0)
				return null;

			return DbManager.Db.PropertyListings.IncludeInfoProperties().SingleOrDefault(l => l.ID == item.ID);
		}
	}
}