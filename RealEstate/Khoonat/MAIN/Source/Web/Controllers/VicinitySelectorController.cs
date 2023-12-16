using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Log4Net;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Util.DomainUtil;
using JahanJooy.RealEstate.Util.Log4Net;
using JahanJooy.RealEstate.Util.Resources;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Models.VicinitySelector;

namespace JahanJooy.RealEstate.Web.Controllers
{
    public class VicinitySelectorController : CustomControllerBase
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [HttpGet]
        public ActionResult Search(string query, int? page, int? pageLength, long? searchScope, bool? includeDisabled)
        {
			// Calculate pagination

            if (!pageLength.HasValue || pageLength.Value < 5)
                pageLength = 20;
            if (pageLength.Value > 100)
                pageLength = 100;

            var skipCount = 0;
            var takeCount = pageLength.Value;

            if (page.HasValue && page.Value > 1)
                skipCount = (page.Value - 1)*pageLength.Value;

			var searchResult = VicinityCache.Search(query, false, skipCount, takeCount + 1, searchScope, includeDisabled.GetValueOrDefault());

            var resultItems = new List<VicinitySelectorResultItemModel>();
            resultItems.AddRange(VicinitySelectorResultItemModel.Map(searchResult));

            var result = new VicinitySelectorResultModel {Items = resultItems, More = false};
            if (resultItems.Count > pageLength.Value)
            {
                // If there's more, remove the last (additionally queried) item

                result.More = true;
                result.Items.RemoveAt(result.Items.Count - 1);
            }

            RealEstateStaticLogs.VicinitySearch.InfoFormat("{0}Q: '{1}', {2} results, SID {3}", 
				resultItems.Any() ? "" : "NOTFOUND ", 
				LogUtils.SanitizeUserInput(query),
				resultItems.Count,
				SessionInfo.IfNotNull(si => si.Record.IfNotNull(sr => sr.ID)));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Get(string id)
        {
            var vicinity = VicinityCache[long.Parse(id)];

            var resultItems = new VicinitySelectorResultItemModel
            {
                ID = vicinity.ID,
                Name = vicinity.Name,
                AlternativeNames = vicinity.AlternativeNames,
                Enabled = vicinity.Enabled,
                HierarchyString = string.Join(GeneralResources.Comma, vicinity.GetParents()
                    .Select(h => h.GetDisplayName())
                    .Reverse()),
                Type = vicinity.Type,
                TypeLabel = vicinity.Type.Label(DomainEnumResources.ResourceManager),
                FullName = vicinity.GetDisplayFullName(),
                ParentID = vicinity.ParentID,
                HasChildren = (vicinity.Children != null),
                Hierarchy = VicinitySelectorHierarchyItemModel.Map(vicinity.GetParents()),
                CanContainPropertyRecords = vicinity.CanContainPropertyRecords,
                ShowTypeInTitle = vicinity.ShowTypeInTitle
            };

            return Json(resultItems, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMany(string ids)
        {
            var resultItems = new List<VicinitySelectorResultItemModel>();
            var vicinityIDs = ids.Split(Convert.ToChar(","));
            foreach (var id in vicinityIDs)
            {
                var vicinity = VicinityCache[long.Parse(id)];

                var resultItem = new VicinitySelectorResultItemModel
                {
                    ID = vicinity.ID,
                    Name = vicinity.Name,
                    AlternativeNames = vicinity.AlternativeNames,
                    Enabled = vicinity.Enabled,
                    HierarchyString = string.Join(GeneralResources.Comma, vicinity.GetParents()
                        .Select(h => h.GetDisplayName())
                        .Reverse()),
                    Type = vicinity.Type,
                    TypeLabel = vicinity.Type.Label(DomainEnumResources.ResourceManager),
                    FullName = vicinity.GetDisplayFullName(),
                    ParentID = vicinity.ParentID,
                    HasChildren = (vicinity.Children != null),
                    Hierarchy = VicinitySelectorHierarchyItemModel.Map(vicinity.GetParents()),
                    CanContainPropertyRecords = vicinity.CanContainPropertyRecords,
                    ShowTypeInTitle = vicinity.ShowTypeInTitle
                };
                resultItems.Add(resultItem);
            }

            return Json(resultItems, JsonRequestBehavior.AllowGet);
        }
    }
}