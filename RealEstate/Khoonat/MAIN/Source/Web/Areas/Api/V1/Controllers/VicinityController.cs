using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Log4Net;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Util.Log4Net;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Application;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Vicinity;
using log4net;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Controllers
{
	public class VicinityController : ApiControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(VicinityController));

        [ComponentPlug]
		public IVicinityCache VicinityCache { get; set; }

		[HttpPost]
		public ActionResult Search(ApiVicinitySearchInputModel input)
		{
            Log.DebugFormat("Searching  in {0} Scope for {1}", input.Scope.IfHasValue(s => s.ToString(CultureInfo.InvariantCulture), "<null>"), LogUtils.SanitizeUserInput(input.Query));

			if (!ModelState.IsValid)
				return ValidationErrorFromModelState();

			var searchResult = VicinityCache.Search(input.Query, input.CanContainPropertyRecordsOnly, input.Pagination.StartIndex, input.Pagination.PageSize + 1, input.Scope);

			var result = new ApiVicinitySearchOutputModel
			             {
							 Vicinities = searchResult.Select(v => ApiOutputVicinityModel.FromVicinity(v, true)).Take(input.Pagination.PageSize).ToArray(),
							 ThereIsMore = searchResult.Count > input.Pagination.PageSize
			             };

			RealEstateStaticLogs.VicinitySearch.InfoFormat("{0}Q: '{1}', {2} results, API",
				searchResult.Any() ? "" : "NOTFOUND ",
				LogUtils.SanitizeUserInput(input.Query),
				searchResult.Count);

			return Json(result);
		}

		public ActionResult Browse(ApiVicinityBrowseInputModel input)
		{
			if (!ModelState.IsValid)
				return ValidationErrorFromModelState();

			var parentVicinity = input.ParentID.HasValue
				? VicinityCache[input.ParentID.Value]
				: VicinityCache.Root;

			var vicinities = parentVicinity.Children.IfNotNull(ch => ch.Where(v => v.Enabled));
			var result = new ApiVicinityBrowseOutputModel
			             {
				             Vicinities = vicinities.IfNotNull(vs => vs.Select(v => ApiOutputVicinityDetailsModel.FromVicinity(v, input.IncludeGeoBox, input.IncludeGeoPath)).ToArray())
			             };

			return Json(result);
		}

		public ActionResult Get(ApiVicinityGetInputModel input)
		{
			if (!ModelState.IsValid)
				return ValidationErrorFromModelState();

			if (input.VicinityIDs == null)
				return Error(ApiErrorCode.InputQueryIsEmpty);

			var vicinities = input.VicinityIDs.Select(vid => VicinityCache[vid]).Where(v => v != null && v.Enabled);
			var result = new ApiVicinityGetOutputModel
			             {
							 Vicinities = vicinities.Select(v => ApiOutputVicinityDetailsModel.FromVicinity(v, input.IncludeGeoBox, input.IncludeGeoPath)).ToArray()
			             };

			return Json(result);
		}
	}
}