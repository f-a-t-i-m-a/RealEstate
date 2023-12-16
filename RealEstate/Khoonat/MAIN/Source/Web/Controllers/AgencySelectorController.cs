using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Index;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Models.AgencySelector;

namespace JahanJooy.RealEstate.Web.Controllers
{
    public class AgencySelectorController : CustomControllerBase
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IAgencyIndex AgencyIndex { get; set; }

        public ActionResult Search(string query, int? page, int? pageLength)
        {
            // Calculate pagination

            if (!pageLength.HasValue || pageLength.Value < 5)
                pageLength = 20;
            if (pageLength.Value > 100)
                pageLength = 100;

            var skipCount = 0;
            var takeCount = pageLength.Value;

            if (page.HasValue && page.Value > 1)
                skipCount = (page.Value - 1) * pageLength.Value;

            var agencyIds = AgencyIndex.SearchByName(query, skipCount, takeCount + 1);
            var agencies = DbManager.Db.Agencies.Where(a => agencyIds.Contains(a.ID)).Include(a => a.AgencyBranches).ToList();

            var resultItems = agencies.Select(Mapper.Map<AgencySelectorResultItemModel>).ToList();


//            var resultItems = new List<AgencySelectorResultItemModel>();
//            resultItems.AddRange(AgencySelectorResultItemModel.Map(searchResult));

            var result = new AgencySelectorResultModel { Items = resultItems, More = false };

            if (resultItems.Count > pageLength.Value)
            {
                // If there's more, remove the last (additionally queried) item
                result.More = true;
                result.Items.RemoveAt(result.Items.Count - 1);
            }

            return Json(result);
        }

    }
}