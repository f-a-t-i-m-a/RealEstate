using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Index;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Models.Map;
using JahanJooy.RealEstate.Util.Presentation.Property;

namespace JahanJooy.RealEstate.Web.Controllers
{
    public class MapController : CustomControllerBase
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [ComponentPlug]
        public IGeographyService GeographyService { get; set; }

        [ComponentPlug]
        public IAgencyBranchIndex AgencyBranchIndex { get; set; }

        [ComponentPlug]
        public PropertyPresentationHelper PropertyPresentationHelper { get; set; }

        #endregion

        #region Map action methods

        [HttpGet]
        public ActionResult ShowMap(string q)
        {
            var model = new MapShowMapModel();
            model.SearchQuery = q != "" ? PropertySearchQueryUtil.ParseQuery(q) : new PropertySearch();
            FillOtherFields(model,q);          
            FillPropertyTypeMenuItems(model);
            return View(model);
        }

        private void FillOtherFields(MapShowMapModel model, string q)
        {
            if (q != null)
            {
                if (q.Contains("uo.prk"))
                    model.ShouldHaveParking = true;
                if (q.Contains("bo.ele"))
                    model.ShouldHaveElevator = true;
                if (q.Contains("uo.strg"))
                    model.ShouldHaveStorageRoom = true;
                if (q.Contains("o.photo"))
                    model.ShouldHasPhotos = true;
            }
        }

        [HttpPost]
        public ActionResult Search(MapSearchModel model)
        {
            if (model.Bounds == null)
                return Error(ErrorResult.AccessDenied);

            var geographyBounds = model.Bounds.ToDbGeography();
            var propertySearch = PropertySearchQueryUtil.ParseQuery(model.q);
            if (propertySearch == null)
                return Error(ErrorResult.EntityNotFound);
            if (model.PropertyTypes != null && model.PropertyTypes.Count > 0)
                propertySearch.PropertyType = model.PropertyTypes[0]; 
            
            if (model.IntentionOfOwners != null && model.IntentionOfOwners.Count > 0)
                propertySearch.IntentionOfOwner = model.IntentionOfOwners[0];

            if (model.NumberOfRoomsMinimum != null)
                propertySearch.NumberOfRoomsMinimum = model.NumberOfRoomsMinimum;

            if (model.NumberOfRoomsMaximum != null)
                propertySearch.NumberOfRoomsMaximum = model.NumberOfRoomsMaximum;

            if (model.SalePriceMinimum != null)
                propertySearch.SalePriceMinimum = model.SalePriceMinimum;

            if (model.SalePriceMaximum != null)
                propertySearch.SalePriceMaximum = model.SalePriceMaximum;

            if (model.SalePricePerUnitAreaMinimum != null)
                propertySearch.SalePricePerUnitAreaMinimum = model.SalePricePerUnitAreaMinimum;

            if (model.SalePricePerUnitAreaMaximum != null)
                propertySearch.SalePricePerUnitAreaMaximum = model.SalePricePerUnitAreaMaximum;

            if (model.SalePricePerEstateAreaMinimum != null)
                propertySearch.SalePricePerEstateAreaMinimum = model.SalePricePerEstateAreaMinimum;

            if (model.SalePricePerEstateAreaMaximum != null)
                propertySearch.SalePricePerEstateAreaMaximum = model.SalePricePerEstateAreaMaximum;

            if (model.RentMinimum != null)
                propertySearch.RentMinimum = model.RentMinimum;

            if (model.RentMaximum != null)
                propertySearch.RentMaximum = model.RentMaximum;

            if (model.RentMortgageMinimum != null)
                propertySearch.RentMortgageMinimum = model.RentMortgageMinimum;

            if (model.RentMortgageMaximum != null)
                propertySearch.RentMortgageMaximum = model.RentMortgageMaximum;

            if (model.UnitAreaMinimum != null)
                propertySearch.UnitAreaMinimum = model.UnitAreaMinimum;

            if (model.UnitAreaMaximum != null)
                propertySearch.UnitAreaMaximum = model.UnitAreaMaximum;

            if (model.EstateAreaMinimum != null)
                propertySearch.EstateAreaMinimum = model.EstateAreaMinimum;

            if (model.EstateAreaMaximum != null)
                propertySearch.EstateAreaMaximum = model.EstateAreaMaximum;


            if (model.Tags != null && model.Tags.Count > 0)
            {
                propertySearch.Options = new List<string>(model.Tags);
            }
          
            var searchResult = GeographyService.RunSearch(geographyBounds, propertySearch);

            if (model.ShowAgencyBranches.HasValue && model.ShowAgencyBranches.Value)
            {
                var searchAgencyBranchIds = AgencyBranchIndex.SearchAgencyBranches(model.Bounds);
                if (searchAgencyBranchIds != null)
                {
                    searchResult.Agencies =
                        DbManager.Db.AgencyBranches.Include(ab => ab.Agency)
                            .Where(ab => searchAgencyBranchIds.Contains(ab.ID))
                            .ToList();
                }
            }
            return Json(MapSearchResultModel.FromDto(searchResult, PropertyPresentationHelper, VicinityCache));
        }

        [HttpPost]
        public ActionResult GetVicinityBounds(long vicinityID)
        {
            var vicinity = VicinityCache[vicinityID];
            if (vicinity != null)
            {
		        DbGeography boundary = vicinity.GetParentsInclusive().Select(v => v.Boundary).FirstOrDefault(v => v != null);
                LatLngBounds bounds = boundary.FindBoundingBox();

                if (bounds != null)
                {
                    return Json(bounds);
                }
            }

            return Json(null);
        }

        [HttpPost]
        public ActionResult FindUserPointVicinity(LatLng userPoint, long? currentVicinityId)
        {
            var pointDbGeography = userPoint.ToDbGeography();
            var newVicinityId = DbManager.Db.Vicinities
                .Where(v => v.Boundary.Intersects(pointDbGeography))
                .OrderBy(v => v.Boundary.Area)
                .Select(v => (long?)v.ID)
                .FirstOrDefault();

			if (!newVicinityId.HasValue)
		        return Json(null);

			var newVicinity = VicinityCache[newVicinityId.Value];
	        if (newVicinity != null)
	        {
		        // Try to find the child with closest center point to the user point
		        var closestVicinity = FindClosestVicinity(
					newVicinity.GetBfsChildrenTree().Where(v => v.Boundary == null && v.CenterPoint != null).Take(100),
					pointDbGeography);

		        if (closestVicinity != null)
		        {
					newVicinity = closestVicinity;
			        newVicinityId = newVicinity.ID;
		        }
	        }

			if (newVicinityId == currentVicinityId)
				return Json(newVicinityId);

			// Get currently selected vicinity from cache to check for special conditions...

	        var currentVicinity = currentVicinityId.IfHasValue(cvid => VicinityCache[cvid]);
	        if (currentVicinity == null || newVicinity == null)
	        {
		        // There's a problem with the cache, return the results from DB
				return Json(newVicinityId);
			}

			// If the current vicinity is already more specific than the vicinity we found
			// (if the vicinity we found is in the parents of the current vicinity)
			// we won't be changing the currently selected vicinity

			if (currentVicinity.GetParents().Any(v => v.ID == newVicinityId.Value))
				return Json(currentVicinityId);

			// If the currently selected vicinity doesn't have a boundary but the new vicinity has,
			// current vicinity couldn't have been found in the search. So, if the currently selected vicinity
			// has the same parent with the vicinity found in search, we will ignore the search
			// and return the currently selected vicinity.

	        if (currentVicinity.Boundary == null && newVicinity.Boundary != null)
            {
		        if (currentVicinity.ParentID == newVicinity.ParentID)
					return Json(currentVicinityId);
            }
   
			return Json(newVicinityId);
        }

        #endregion

        #region Private helper methods

        private void FillPropertyTypeMenuItems(MapShowMapModel model)
        {
            var propertyTypeMenuItems = new List<PropertiesMapSearchMenuModel>();

            foreach (var propertyType in Enum.GetValues(typeof(PropertyType)))
            {
                propertyTypeMenuItems.Add(new PropertiesMapSearchMenuModel
                {
                    Label = propertyType.ToString(),
                    ID = (byte)propertyType,
                });  
            }
            model.PropertyTypeMenuItems = propertyTypeMenuItems;

        }

        private Vicinity FindClosestVicinity(IEnumerable<Vicinity> vicinities, DbGeography target)
		{
			if (vicinities == null)
				return null;

			if (target == null)
				throw new ArgumentNullException("target");

			Vicinity result = null;
			double? closestDistance = null;

			foreach (var vicinity in vicinities)
			{
				DbGeography vicinityGeo = vicinity.Boundary ?? vicinity.CenterPoint;
				if (vicinityGeo == null)
					continue;

				var vicinityDistance = target.Distance(vicinityGeo);
				if (vicinityDistance.HasValue)
				{
					if (!closestDistance.HasValue || closestDistance.Value > vicinityDistance.Value)
					{
						closestDistance = vicinityDistance;
						result = vicinity;
					}
				}
			}

			return result;
		}

        #endregion
    }
}