using System;
using System.Linq;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Domain.Enums.Vicinity;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Models.Vicinities;
using JahanJooy.RealEstateAgency.Util.Resources;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace JahanJooy.RealEstateAgency.Util.Utils
{
    [Contract]
    [Component]
    public class VicinityUtil
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        private readonly int MaxPageSize = 10000;

        #endregion

        #region Action methods

        public VicinityReference FindByPoint(SearchVicinityByPointInput input)
        {
            var userPointLocation = Mapper.Map<GeoJson2DCoordinates>(input.UserPoint);
            var filter = Builders<Vicinity>.Filter.GeoIntersects("Boundary",
                GeoJson.Point(userPointLocation));

            var vicinity = DbManager.Vicinity.Find(filter)
                .SortBy(v => v.BoundaryArea)
                .Project(v => Mapper.Map<VicinityReference>(v))
                .FirstOrDefaultAsync()
                .Result;

            return vicinity;
        }

        public SearchVicinityOutput Search(SearchVicinityInput searchInput)
        {
            var response = VicinityCache.Search(searchInput.SearchText, searchInput.CanContainPropertyRecordsOnly,
                searchInput.StartIndex, searchInput.PageSize, searchInput.ParentId);
            var allResponses = VicinityCache.Search(searchInput.SearchText, searchInput.CanContainPropertyRecordsOnly,
                0, MaxPageSize, searchInput.ParentId);
            var vicinitySummaries = response.Select(Mapper.Map<VicinitySummary>).ToList();

            vicinitySummaries.ForEach(v =>
            {
                var parentId = v.ParentID;
                while (parentId.HasValue)
                {
                    var parent = VicinityCache[parentId.Value];
                    v.CompleteName += ", " +
                                      StaticEnumResources.ResourceManager.GetString(nameof(VicinityType) + "_" +
                                                                                    parent.Type) + " " + parent.Name;
                    parentId = parent.ParentID;
                }
                if (v.CompleteName != null && v.CompleteName.Length >= 2)
                    v.CompleteName = v.CompleteName.Substring(2);
            });

            var result = new PagedListOutput<VicinitySummary>
            {
                PageItems = vicinitySummaries,
                PageNumber = (searchInput.StartIndex/searchInput.PageSize) + 1,
                TotalNumberOfItems = allResponses.Count,
                TotalNumberOfPages = (int) Math.Ceiling((decimal) allResponses.Count/searchInput.PageSize)
            };

            return new SearchVicinityOutput
            {
                Vicinities = result
            };
        }

        public string GetFullName(ObjectId vicinityId)
        {
            string vicinityCompleteName = "";
            var vicinity = VicinityCache[vicinityId];
            if (vicinity != null)
            {
                vicinityCompleteName =
                    StaticEnumResources.ResourceManager.GetString(nameof(VicinityType) + "_" + vicinity.Type) + " " +
                    vicinity.Name;
                var parentId = vicinity.ParentID;
                while (parentId.HasValue)
                {
                    var parent = VicinityCache[parentId.Value];
                    vicinityCompleteName += ", " + StaticEnumResources.ResourceManager.GetString(
                        nameof(VicinityType) + "_" +
                        parent.Type) + " " + parent.Name;
                    parentId = parent.ParentID;
                }
            }
            return vicinityCompleteName;
        }

        public string GetFullIds(ObjectId vicinityId)
        {
            string vicinityIds = "";
            var vicinity = VicinityCache[vicinityId];
            if (vicinity != null)
            {
                vicinityIds += vicinity.ID + " ";
                var parentId = vicinity.ParentID;
                while (parentId.HasValue)
                {
                    var parent = VicinityCache[parentId.Value];
                    vicinityIds += parentId.Value + " ";
                    parentId = parent.ParentID;
                }
            }
            return vicinityIds;
        }

        public LatLng GetParentCenterPoint(ObjectId parentId)
        {
            var vicinity = VicinityCache[parentId];
            while (vicinity?.CenterPoint == null)
            {
                vicinity = VicinityCache[parentId];
            }
            return new LatLng
            {
                Lat = vicinity.CenterPoint.Y,
                Lng = vicinity.CenterPoint.X
            };
        }

        #endregion

        #region Private helper methods 

        #endregion
    }
}