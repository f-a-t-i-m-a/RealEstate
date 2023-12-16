using System.Data.Entity;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Directory;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
	[Component]
	public class OperationsService : IOperationsService
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public IVicinityCache VicinityCache { get; set; }

		[ComponentPlug]
		public IEntityContentSerializer<AgencyContent> AgencyContentSerializer { get; set; }

		[ComponentPlug]
		public IEntityContentSerializer<AgencyBranchContent> AgencyBranchContentSerializer { get; set; }

		[ComponentPlug]
		public IUserBillingComponent UserBillingComponent { get; set; }

		[ComponentPlug]
		public IPropertyPhotoService PropertyPhotoService { get; set; }

		public void RebuildAgencyIndex()
		{
			DbManager.Db.Database.ExecuteSqlCommand("UPDATE [Agency] SET [IndexedTime] = NULL");
			DbManager.Db.Database.ExecuteSqlCommand("UPDATE [AgencyBranch] SET [IndexedTime] = NULL");
		}

		public void RebuildPropertyListingIndex()
		{
			DbManager.Db.Database.ExecuteSqlCommand("UPDATE [PropertyListing] SET [IndexedTime] = NULL");
		}

		public void RebuildPropertyRequestIndex()
		{
			DbManager.Db.Database.ExecuteSqlCommand("UPDATE [PropertyRequest] SET [IndexedTime] = NULL");
		}

		public void RebuildPropertyListingsVicinityHierarchyString()
		{
			var propertyListing = DbManager.Db.PropertyListingsDbSet.ToList();

			foreach (var listing in propertyListing)
			{
				if (listing.VicinityID != null)
				{
					var vicinity = VicinityCache[listing.VicinityID.Value];
					if (vicinity != null)
					{
						var hierarchyString = string.Join("/", vicinity.GetParentIDsInclusive());
						listing.VicinityHierarchyString = "/" + hierarchyString + "/";
					}
				}
			}

			DbManager.SaveDefaultDbChanges();
		}

		public void UpdatePropertyListingsGeoLocationFromVicinities()
		{
			var propertyListing = DbManager.Db.PropertyListingsDbSet
				.Where(pl => pl.GeographicLocation == null || pl.GeographicLocationType.Value == GeographicLocationSpecificationType.InferredFromVicinity)
				.Include(pl => pl.Estate)
				.ToList();

			foreach (var listing in propertyListing)
			{
				if (listing.VicinityID != null)
				{
					var vicinity = VicinityCache[listing.VicinityID.Value];
					if (vicinity != null && vicinity.CenterPoint != null)
					{
						listing.GeographicLocationType = GeographicLocationSpecificationType.InferredFromVicinity;
						listing.GeographicLocation = vicinity.CenterPoint;

						if (listing.Estate != null)
						{
							listing.Estate.GeographicLocationType = GeographicLocationSpecificationType.InferredFromVicinity;
							listing.Estate.GeographicLocation = vicinity.CenterPoint;
						}
					}
				}
			}

			DbManager.SaveDefaultDbChanges();
		}

		public void RecalculateUserTransactions(long userId)
		{
			UserBillingComponent.RecalculateUserBalanceHistory(userId);
		}

		public void RebuildPropertyListingPhotos(long fromId, long toId)
		{
			if (fromId > toId || fromId < 1)
				return;

			for (long id = fromId; id <= toId; id++)
				PropertyPhotoService.RebuildPhoto(id);
		}
	}
}