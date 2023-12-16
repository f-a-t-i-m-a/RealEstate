using System.Web.Mvc;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Application;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Search;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Controllers
{
	public class SearchController : ApiControllerBase
	{
		[ComponentPlug]
		public IGlobalSearchService SearchService { get; set; }

		public ActionResult Run(ApiSearchRunInputModel input)
		{
			if (!ModelState.IsValid)
				return ValidationErrorFromModelState();

			var criteria = Mapper.Map<GlobalSearchCriteria>(input.Criteria);
			var searchResult = SearchService.RunSearch(criteria, input.Pagination.StartIndex, input.Pagination.PageSize);

			var result = Mapper.Map<ApiSearchRunOutputModel>(searchResult);
			return Json(result);
		}
	}
}