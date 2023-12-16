using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
    [Component]
    public class VicinityService : IVicinityService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IActivityLogService ActivityLogService { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        #region Implementation of IVicinityService

        public void CreateVicinity(Vicinity vicinity)
        {
            DbManager.Db.VicinityDbSet.Add(vicinity);
            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.Vicinity, vicinity.ID, ActivityAction.Create);
            VicinityCache.InvalidateAll();
        }

        public void UpdateVicinity(Vicinity model)
        {
            var vicinity = DbManager.Db.VicinityDbSet.SingleOrDefault(v => v.ID == model.ID);

            if (vicinity == null)
                throw new ArgumentException("Vicinity does not exist");

            vicinity.Name = model.Name;
            vicinity.AlternativeNames = model.AlternativeNames;
            vicinity.AdditionalSearchText = model.AdditionalSearchText;
            vicinity.Description = model.Description;
            vicinity.AdministrativeNotes = model.AdministrativeNotes;
            vicinity.Enabled = model.Enabled;
            vicinity.Order = model.Order;
            vicinity.Type = model.Type;
            vicinity.WellKnownScope = model.WellKnownScope;
            vicinity.ShowTypeInTitle = model.ShowTypeInTitle;
            vicinity.ShowInSummary = model.ShowInSummary;
            vicinity.CanContainPropertyRecords = model.CanContainPropertyRecords;

            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.Vicinity, vicinity.ID, ActivityAction.Edit);
            VicinityCache.InvalidateAll();
        }

        public void UpdateVicinityGeography(Vicinity model)
        {
            var vicinity = DbManager.Db.VicinityDbSet.SingleOrDefault(v => v.ID == model.ID);
            if (vicinity == null)
                throw new AggregateException("Vicinity does not exist");

            vicinity.CenterPoint = model.CenterPoint;
            vicinity.Boundary = model.Boundary;

            var centerPoint = vicinity.CenterPoint ?? vicinity.Boundary.FindBoundingBox().IfNotNull(bb => bb.GetCenter().ToDbGeography());
            if (centerPoint != null)
            {
                var propertyListings = DbManager.Db.PropertyListingsDbSet
                    .Include(pl => pl.Estate)
                    .Where(pl =>
                        pl.VicinityID == vicinity.ID &&
                        (pl.GeographicLocationType == GeographicLocationSpecificationType.InferredFromVicinity ||
                         pl.GeographicLocationType == GeographicLocationSpecificationType.InferredFromVicinityAndAddress ||
                         !pl.GeographicLocationType.HasValue));

                foreach (var listing in propertyListings)
                {
                    if (listing.Estate != null)
                    {
                        listing.Estate.GeographicLocation = centerPoint;
                        listing.Estate.GeographicLocationType = GeographicLocationSpecificationType.InferredFromVicinity;
                    }
                    listing.GeographicLocation = centerPoint;
                    listing.GeographicLocationType = GeographicLocationSpecificationType.InferredFromVicinity;
                }
            }

            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.Vicinity, vicinity.ID, ActivityAction.Edit);
            VicinityCache.InvalidateAll();
        }

        public void SetEnabled(IEnumerable<long> vicinityIDs, bool enabled)
        {
            foreach (var vicinity in DbManager.Db.VicinityDbSet.Where(v => vicinityIDs.Contains(v.ID)))
            {
                vicinity.Enabled = enabled;
            }
            VicinityCache.InvalidateAll();
        }

        public void SetCanContainPropertyRecords(IEnumerable<long> vicinityIDs, bool canContainPropertyRecords)
        {
            foreach (var vicinity in DbManager.Db.VicinityDbSet.Where(v => vicinityIDs.Contains(v.ID)))
            {
                vicinity.CanContainPropertyRecords = canContainPropertyRecords;
            }
            VicinityCache.InvalidateAll();
        }

        public void RemoveVicinities(IEnumerable<long> vicinityIDs)
        {
            var vicinities = DbManager.Db.VicinityDbSet.Where(v => vicinityIDs.Contains(v.ID));
            foreach (var vicinity in vicinities)
            {
                DbManager.Db.VicinityDbSet.Remove(vicinity);
            }

            VicinityCache.InvalidateAll();
        }

        public void MoveVicinities(long[] vicinityIDs, long? targetVicinityId)
        {
            if (vicinityIDs == null)
                throw new ArgumentNullException("vicinityIDs");

            var parentId = targetVicinityId;
            while (parentId.HasValue)
            {
                var parentVicinity = DbManager.Db.VicinityDbSet.FirstOrDefault(v => v.ID == parentId.Value);
                if (parentVicinity == null)
                    throw new ArgumentException("Target Vicinity does not exist");

                if (vicinityIDs.Any(vid => vid == parentId.Value))
                    throw new ArgumentException("One of the selected vicinities is the parent of target destination");

                parentId = parentVicinity.ParentID;
            }

            foreach (var vicinity in DbManager.Db.VicinityDbSet.Where(v => vicinityIDs.Any(vid => v.ID == vid)))
            {
                vicinity.ParentID = targetVicinityId;
            }
            VicinityCache.InvalidateAll();
        }

        #endregion
    }
}