using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Util.Resources;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Models.VicinityAdmin;

namespace JahanJooy.RealEstate.Web.Controllers
{
    public class VicinityAdminController : AdminControllerBase
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IVicinityService VicinityService { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        #endregion

        #region Vicinity action methods

        [HttpGet]
        public ActionResult List(long? parentID)
        {
            Vicinity currentVicinity = null;
            if (parentID.HasValue)
                currentVicinity = DbManager.Db.Vicinities.SingleOrDefault(v => v.ID == parentID.Value);

            if (parentID.HasValue && currentVicinity == null)
                return Error(ErrorResult.EntityNotFound);

			var hierarchy = new List<Vicinity>();
            var currentHierarchyItem = currentVicinity;
            while (currentHierarchyItem != null)
            {
	            hierarchy.Add(currentHierarchyItem);
	            currentHierarchyItem = currentHierarchyItem.ParentID.HasValue
		            ? DbManager.Db.Vicinities.SingleOrDefault(v => v.ID == currentHierarchyItem.ParentID.Value)
		            : null;
            }

	        hierarchy.Reverse();

            var listItems = new List<SelectListItem>();
            if (currentVicinity != null)
            {
                foreach (var h in hierarchy)
                {
                    listItems.Add(new SelectListItem
                    {
                        Text = h.ShowTypeInTitle ? h.Type.Label(DomainEnumResources.ResourceManager) + " " + h.Name : h.Name,
                        Value = h.ID.ToString(CultureInfo.InvariantCulture),
                    });
                }
            }

            var childVicinities = DbManager.Db.Vicinities
                    .Where(v => v.ParentID == parentID)
                    .OrderBy(p => p.Order)
                    .ThenBy(p => p.Name)
                    .ToList();

            if (currentVicinity != null)
                return View(new VicinityAdminListModel
                {
                    Vicinities = childVicinities,
                    CurrentVicinity = currentVicinity,
					CurrentVicinityFromCache = VicinityCache[currentVicinity.ID],
                    Hierarchy = hierarchy,
                    SelectScope=listItems,
                    SelectedScope = currentVicinity.ID,
					AllParentsEnabled = hierarchy.All(v => v.Enabled)
                });

            return View(new VicinityAdminListModel
            {
                Vicinities = childVicinities,
                CurrentVicinity = null,
				CurrentVicinityFromCache = null,
                Hierarchy = hierarchy,
                SelectScope = listItems,
				AllParentsEnabled = hierarchy.All(v => v.Enabled)
            });
        }

        [HttpGet]
        public PartialViewResult NewVicinity(long? currentVicinityID)
        {
            if (currentVicinityID == 0)
                currentVicinityID = null;

	        var currentVicinity = currentVicinityID.IfHasValue(cvid => VicinityCache[cvid]);

            var model = new VicinityAdminNewVicinityModel
            {
                CurrentVicinityID = currentVicinityID,
				Type = GetDefaultChildType(currentVicinity.IfNotNull(v => (VicinityType?)v.Type)),
				WellKnownScope = currentVicinity.IfNotNull(v => v.Type, VicinityType.HierarchyNode),
				ShowInSummary = true,
				CanContainPropertyRecords = true,
				AllowedVicinityTypes = GetAllowedChildTypes(currentVicinity.IfNotNull(v => (VicinityType?)v.Type))
            };

            return PartialView(model);
        }

        [HttpPost]
        [ActionName("NewVicinity")]
        public ActionResult NewVicinityPostback(VicinityAdminNewVicinityModel model)
        {
            if (!ModelState.IsValid)
                return NewVicinity(null);

            if (model.CurrentVicinityID == 0)
                model.CurrentVicinityID = null;

            var vicinity = new Vicinity
            {
                Name = model.Name,
                Type = model.Type.GetValueOrDefault(),
				ShowTypeInTitle = model.ShowTypeInTitle,
				AlternativeNames = model.AlternativeNames,
				AdditionalSearchText = model.AdditionalSearchText,
				WellKnownScope = model.WellKnownScope ?? VicinityType.HierarchyNode,
				ShowInSummary = model.ShowInSummary,
				CanContainPropertyRecords = model.CanContainPropertyRecords,
                ParentID = model.CurrentVicinityID
            };

            VicinityService.CreateVicinity(vicinity);
            return RedirectToAction("List", new { parentID = model.CurrentVicinityID });
        }

        [HttpGet]
        public ActionResult EditProperties(long vicinityID)
        {
            var vicinity = DbManager.Db.Vicinities.Single(v => v.ID == vicinityID);
            if (vicinity == null)
                return Error(ErrorResult.EntityNotFound);

            return View(vicinity);
        }

        [HttpPost]
        [ActionName("EditProperties")]
        public ActionResult EditPropertiesPostback(Vicinity model)
        {
            ModelState.Clear();
            ValidateForSave(model);
            if (!ModelState.IsValid)
                return EditProperties(model.ID);

            VicinityService.UpdateVicinity(model);
            return RedirectToAction("List", "VicinityAdmin", new { parentID =model.ParentID});
        }

        [HttpGet]
        public ActionResult EditGeography(long vicinityID)
        {
            var vicinity = DbManager.Db.Vicinities.Include(v => v.Parent).Single(p => p.ID == vicinityID);
            return View(VicinityAdminEditGeographyModel.FromDomain(vicinity, vicinity.Parent));
        }

	    [HttpPost]
		public ActionResult GetSiblingBoundaries(long vicinityId)
	    {
			var result = new List<LatLngPath>();

		    var vicinity = VicinityCache[vicinityId];
		    if (vicinity == null || vicinity.Parent == null || vicinity.Parent.Children == null)
			    return Json(result);

		    result.AddRange(
			    vicinity.Parent.Children
				    .Where(c => c.ID != vicinityId && c.Boundary != null)
				    .Select(cr => cr.Boundary.ToLatLngPath().SetTitle(cr.Name)));

			return Json(result);
		}

        [HttpPost]
        public ActionResult GetVicinityBoundary(long vicinityId)
        {

            var result = new LatLngPath();
            var vicinity = VicinityCache[vicinityId];
            if (vicinity == null || vicinity.Parent == null || vicinity.Parent.Children == null)
                return Json(result);

            if (vicinity.Boundary != null)
            {
                result = vicinity.Boundary.ToLatLngPath().SetTitle(vicinity.Name);
            }
            else
            {
                result = null;
            }
            return Json(result);
        }

        [HttpPost]
        [ActionName("EditGeography")]
        public ActionResult EditGeographyPostback(VicinityAdminEditGeographyModel model)
        {
            if (!model.ID.HasValue)
                return RedirectToAction("List");

            if (!ModelState.IsValid)
                return EditGeography(model.ID.Value);

            var vicinity = DbManager.Db.Vicinities.Single(v => v.ID == model.ID.Value);
            model.UpdateDomain(vicinity);

            VicinityService.UpdateVicinityGeography(vicinity);
            return RedirectToAction("List", new{model.ParentID});
        }

        [HttpPost]
        [SubmitButton("btnEnableVicinities")]
        [ActionName("BatchChangeVicinities")]
        public ActionResult EnabledVicinities(long[] vicinityIds, long? parentID)
        {
            if (vicinityIds == null)
            {
                ModelState.AddModelError("", "You haven't selected any vicinities");
                return List(parentID);
            }
          
            VicinityService.SetEnabled(vicinityIds, true);
            return RedirectToAction("List", "VicinityAdmin", new {parentID});
        }

        [HttpPost]
        [SubmitButton("btnDisableVicinities")]
        [ActionName("BatchChangeVicinities")]
        public ActionResult DisableVicinities(long[] vicinityIds, long? parentID)
        {
            if (vicinityIds == null)
            {
				ModelState.AddModelError("", "You haven't selected any vicinities");
                return List(parentID);
            }
           
            VicinityService.SetEnabled(vicinityIds, false);
            return RedirectToAction("List", "VicinityAdmin", new {parentID});
        }

        [HttpPost]
        [SubmitButton("btnRemoveVicinities")]
        [ActionName("BatchChangeVicinities")]
        public ActionResult RemoveVicinities(long[] vicinityIds, long? parentID)
        {
            if (vicinityIds == null)
            {
				ModelState.AddModelError("", "You haven't selected any vicinities");
                return List(parentID);
            }
            
            VicinityService.RemoveVicinities(vicinityIds);
            return RedirectToAction("List", "VicinityAdmin", new {parentID});
        }

        [HttpPost]
        [SubmitButton("btnSelectMoveTarget")]
        [ActionName("BatchChangeVicinities")]
        public ActionResult SelectMoveTarget(long[] vicinityIds, long? parentID)
        {
            if (vicinityIds == null)
            {
				ModelState.AddModelError("", "You haven't selected any vicinities");
                return List(parentID);
            }

            var vicinities = new List<Vicinity>();
            foreach (var id in vicinityIds)
            {
                var vicinity= DbManager.Db.Vicinities.Single(v => v.ID == id);
                vicinities.Add(vicinity);
            }


            var vicinityAdminMoveVicinitiesModel = new VicinityAdminMoveVicinitiesModel
            {
                Vicinities = vicinities,
                SelectScope = null,
                ParetntID = parentID,
            };

            return View("MoveVicinities", vicinityAdminMoveVicinitiesModel);
        }

		[HttpPost]
		[SubmitButton("btnSetCanContainPropertyToTrue")]
		[ActionName("BatchChangeVicinities")]
		public ActionResult SetCanContainPropertyToTrue(long[] vicinityIds, long? parentID)
		{
			if (vicinityIds == null)
			{
				ModelState.AddModelError("", "You haven't selected any vicinities");
				return List(parentID);
			}

			VicinityService.SetCanContainPropertyRecords(vicinityIds, true);
			return RedirectToAction("List", "VicinityAdmin", new { parentID });
		}

		[HttpPost]
		[SubmitButton("btnSetCanContainPropertyToFalse")]
		[ActionName("BatchChangeVicinities")]
		public ActionResult SetCanContainPropertyToFalse(long[] vicinityIds, long? parentID)
		{
			if (vicinityIds == null)
			{
				ModelState.AddModelError("", "You haven't selected any vicinities");
				return List(parentID);
			}

			VicinityService.SetCanContainPropertyRecords(vicinityIds, false);
			return RedirectToAction("List", "VicinityAdmin", new { parentID });
		}

        [HttpPost]
        [ActionName("SearchVicinity")]
        public ActionResult SearchVicinity(long vicinityID)
        {
          return RedirectToAction("List", "VicinityAdmin", new { parentID = vicinityID }) ;
        }

        [ActionName("MoveVicinities")]
        public ActionResult MoveVicinities(long[] vicinityIds, long? vicinityID)
        {
            VicinityService.MoveVicinities(vicinityIds, vicinityID);
            return RedirectToAction("List", "VicinityAdmin", new{ parentId=vicinityID});
        }

        #endregion

        #region Helpper methods

        public static string GetVicinityBkgColor(Vicinity vicinity)
        {
            if (!vicinity.Enabled)
                return "#C0C0C0";
            return null;
        }

        #endregion

        #region Private methods

        private void ValidateForSave(Vicinity model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError("", VicinityAdminModelResources.Validation_Name_Required);
            }
            if (model.Type == 0)
            {
                ModelState.AddModelError("", VicinityAdminModelResources.Validation_Type_Required);
            }
        }

	    private VicinityType? GetDefaultChildType(VicinityType? value)
	    {
			if (!value.HasValue)
				return VicinityType.Country;

		    switch (value.Value)
		    {
				case VicinityType.Country:
				case VicinityType.CountryPartition:
				    return VicinityType.Province;

				case VicinityType.State:
				case VicinityType.StatePartition:
				case VicinityType.Province:
				case VicinityType.ProvincePartition:
				    return VicinityType.County;

				case VicinityType.County:
				case VicinityType.District:
				case VicinityType.SubDistrict:
					return VicinityType.City;

				case VicinityType.Metropolis:
				case VicinityType.MetropolisPartition:
				case VicinityType.City:
				case VicinityType.CityPartition:
				    return VicinityType.CityRegion;

				case VicinityType.Suburb:
				case VicinityType.Village:
				case VicinityType.Phase:
				case VicinityType.Town:
				case VicinityType.CityRegion:
					return VicinityType.Neighborhood;

				case VicinityType.Campus:
				case VicinityType.Premises:
					return VicinityType.Building;

				case VicinityType.Complex:
					return VicinityType.Block;

				default:
				    return null;
		    }
	    }

	    private IEnumerable<VicinityType> GetAllowedChildTypes(VicinityType? type)
	    {
		    if (!type.HasValue)
			    return GetRootAllowedChildTypes().Concat(GetGeneralAllowedChildTypes());

		    return GetSpecificAllowedChildTypes(type.Value).Concat(GetGeneralAllowedChildTypes());
	    }

	    private IEnumerable<VicinityType> GetSpecificAllowedChildTypes(VicinityType value)
	    {
		    if ((int) value <= 2999)
			    return GetVicinityTypeRange(3000, 9999);

		    if (value <= VicinityType.ProvincePartition)
			    return GetVicinityTypeRange((int)value, 4999);

			if ((int)value <= 4999)
				return GetVicinityTypeRange((int)value, 8499);

			return GetVicinityTypeRange(5000, 9999).Concat(VicinityType.Phase.Yield());
		}

	    private IEnumerable<VicinityType> GetGeneralAllowedChildTypes()
	    {
		    yield return VicinityType.HierarchyNode;
	    }

	    private IEnumerable<VicinityType> GetRootAllowedChildTypes()
	    {
		    return GetVicinityTypeRange(3000, 3999);
	    }

	    private IEnumerable<VicinityType> GetVicinityTypeRange(int from, int to)
	    {
			foreach (VicinityType value in Enum.GetValues(typeof(VicinityType)))
		    {
			    if ((int) value <= to && (int) value >= from)
				    yield return value;
		    }
	    }

        #endregion
    }
}