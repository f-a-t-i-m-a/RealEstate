using System.Collections.Generic;
using System.Linq;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Util.Presentation;
using JahanJooy.RealEstate.Util.Resources;

namespace JahanJooy.RealEstate.Web.Models.Map
{
    public class MapSearchVicinityItemModel
    {
        public long VicinityID { get; set; }
        public LatLng Point { get; set; }
        public bool Decomposable { get; set; }
        public string Name { get; set; }
        public string Hierarchy { get; set; }

        public int NumberOfPropertyListings { get; set; }
        public int NumberOfAgencies { get; set; }

        public static MapSearchVicinityItemModel FromDto(GeoSearchVicinityResult dto, IVicinityCache vicinityCache)
        {
            return new MapSearchVicinityItemModel
            {
                VicinityID = dto.VicinityID,
                Point = dto.GeographicLocation,
                Decomposable = dto.Decomposable,
                Name =
                    vicinityCache[dto.VicinityID].IfNotNull(v => VicinityPresentationHelper.BuildTitle(v), string.Empty),
                Hierarchy =
                    string.Join(GeneralResources.Comma,
                        VicinityPresentationHelper.BuildHierarchyString(vicinityCache, dto.VicinityID)),
                NumberOfPropertyListings = dto.NumberOfPropertyListings,
                NumberOfAgencies = dto.NumberOfAgencies
            };
        }

        public static IEnumerable<MapSearchVicinityItemModel> FromDto(IEnumerable<GeoSearchVicinityResult> results,
            IVicinityCache vicinityCache)
        {
            return (results ?? Enumerable.Empty<GeoSearchVicinityResult>())
                .Select(r => FromDto(r, vicinityCache));
        }
    }
}