using System;
using System.Linq.Expressions;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.DomainExtensions
{
	public static class PropertyListingExtensions
	{
		public static bool IsPublishedAsOf(this PropertyListing listing, DateTime dateTime)
		{
			return listing != null && IsPublishedAsOf(dateTime, listing.DeleteDate, listing.PublishDate, listing.PublishEndDate);
		}

		public static bool IsPublished(this PropertyListing listing)
		{
			return listing.IsPublishedAsOf(DateTime.Now);
		}

		public static bool IsPublic(this PropertyListing listing)
		{
			return listing != null && listing.IsPublished() && listing.Approved.HasValue && listing.Approved.Value;
		}

		public static bool IsPublishedAsOf(this PropertyListingDetails listing, DateTime dateTime)
		{
			return listing != null && IsPublishedAsOf(dateTime, listing.DeleteDate, listing.PublishDate, listing.PublishEndDate);
		}

		public static bool IsPublished(this PropertyListingDetails listing)
		{
			return listing.IsPublishedAsOf(DateTime.Now);
		}

		public static bool IsPublic(this PropertyListingDetails listing)
		{
			return listing != null && listing.IsPublished() && listing.Approved.HasValue && listing.Approved.Value;
		}

		public static bool IsPublishedAsOf(this PropertyListingSummary listing, DateTime dateTime)
		{
			return listing != null && IsPublishedAsOf(dateTime, listing.DeleteDate, listing.PublishDate, listing.PublishEndDate);
		}

		public static bool IsPublished(this PropertyListingSummary listing)
		{
			return listing.IsPublishedAsOf(DateTime.Now);
		}

		public static bool IsPublic(this PropertyListingSummary listing)
		{
			return listing != null && listing.IsPublished() && listing.Approved.HasValue && listing.Approved.Value;
		}

		public static Expression<Func<PropertyListing, bool>> GetPublishedListingExpression()
		{
			var now = DateTime.Now;
			return l => !l.DeleteDate.HasValue &&
						l.PublishEndDate.HasValue &&
						l.PublishEndDate > now;
		}

		public static Expression<Func<PropertyListing, bool>> GetPublicListingExpression()
		{
			var now = DateTime.Now;
			return l => !l.DeleteDate.HasValue &&
						l.PublishEndDate.HasValue &&
						l.PublishEndDate > now &&
						l.Approved.HasValue &&
						l.Approved.Value;
		}

		#region Private helper methods

		private static bool IsPublishedAsOf(DateTime dateTime, DateTime? deleteDate, DateTime? publishDate, DateTime? publishEndDate)
		{
			return !deleteDate.HasValue &&
				   publishDate.HasValue &&
				   publishEndDate.HasValue &&
				   publishDate.Value <= dateTime &&
				   publishEndDate.Value >= dateTime;
		}

		#endregion
	}
}