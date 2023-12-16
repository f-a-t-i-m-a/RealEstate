using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Application;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Geo;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Controllers
{
	public class GeoController : ApiControllerBase
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public IVicinityCache VicinityCache { get; set; }


		public ActionResult Code(ApiGeoCodeInputModel input)
		{
			if (input.Point == null)
				return Error(ApiErrorCode.InputIsEmpty);

			var result = new ApiGeoCodeOutputModel
			             {
				             Alternatives = new List<ApiGeoCodeOutputModel.Alternative>()
			             };

			var pointDbGeography = input.Point.ToDbGeography();
			var vicinityIds = DbManager.Db.Vicinities
				.Where(v => v.Boundary.Intersects(pointDbGeography))
				.OrderBy(v => v.Boundary.Area)
				.Select(v => v.ID)
				.Take(20)
				.ToList();

			if (vicinityIds.IsNullOrEmptyEnumerable())
				return Json(result);

			var resultVicinityIds = new HashSet<long>();
			var vicinityIdsToIgnore = new HashSet<long>();
			foreach (var vicinityId in vicinityIds)
				ProcessVicinityForGeoCoding(vicinityId, pointDbGeography, resultVicinityIds, vicinityIdsToIgnore);

			result.Alternatives.AddRange(resultVicinityIds.Select(vid => new ApiGeoCodeOutputModel.Alternative
			                                                             {
																			 Vicinity = ApiOutputVicinityDetailsModel.FromVicinityID(vid, VicinityCache, input.IncludeGeoBox, input.IncludeGeoPath)
			                                                             }));

			return Json(result);
		}

		#region Private helper methods

		private void ProcessVicinityForGeoCoding(long vicinityId, DbGeography point, HashSet<long> resultVicinityIds, HashSet<long> vicinityIdsToIgnore)
		{
			if (resultVicinityIds.Contains(vicinityId) || vicinityIdsToIgnore.Contains(vicinityId))
				return;

			var vicinity = VicinityCache[vicinityId];
			if (vicinity == null)
				return;

			var maxDistance = vicinity.Boundary.IfNotNull(b => b.Area.IfHasValue(d => Math.Sqrt(d) / 8, Double.MaxValue), Double.MaxValue);
			var childDistances =
				vicinity.GetBfsChildrenTree()
					.Where(v => v.Boundary == null && v.CenterPoint != null)
					.Take(400) // Just as a safe guard, Probably never reached
					.Select(v => new Tuple<Vicinity, double>(v, point.Distance(v.Boundary ?? v.CenterPoint).GetValueOrDefault(-1)))
					.Where(t => t.Item2 >= 0 && t.Item2 < maxDistance)
					.ToList();

			if (childDistances.Any())
			{
				var closestDistance = childDistances.Select(t => t.Item2).Min();
				var farthestDistance = closestDistance*1.25;

				// Include vicinities that are 25% farther from the closest one too.

				foreach (var childVicinity in childDistances.Where(t => t.Item2 <= farthestDistance).Select(t => t.Item1))
				{
					resultVicinityIds.Add(childVicinity.ID);
					vicinityIdsToIgnore.AddAll(childVicinity.GetParentIDs().Skip(1));
				}
			}
			else
			{
				resultVicinityIds.Add(vicinityId);
				vicinityIdsToIgnore.AddAll(vicinity.GetParentIDs().Skip(1));
			}
		}

		#endregion

	}
}