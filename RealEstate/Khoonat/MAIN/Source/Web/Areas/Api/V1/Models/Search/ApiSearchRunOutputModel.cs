using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared.Spatial;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Search
{
	public class ApiSearchRunOutputModel : ApiOutputModelBase
	{
		// TODO: Adapt these for appropriate JSON output

		public ApiGeoBox Bounds { get; set; }
		public ApiOutputPaginationStats Stats { get; set; }
		public List<ItemModel> Items { get; set; }
		public List<ClusterModel> Clusters { get; set; }

		public class ItemModel
		{
			public GlobalSearchRecordType Type { get; set; }
			public string SubType { get; set; }
			public long ID { get; set; }
			public string Title { get; set; }

			public ApiOutputVicinitySummaryModel Vicinity { get; set; }
			public ApiGeoPoint GeographicLocation { get; set; }
			public GeographicLocationSpecificationType? GeographicLocationType { get; set; }
		}

		public class ClusterModel
		{
			public ApiOutputVicinitySummaryModel Vicinity { get; set; }
			public LatLng GeographicLocation { get; set; }
			public bool Decomposable { get; set; }
			public int TotalNumberOfRecords { get; set; }

			public Dictionary<GlobalSearchRecordType, int> NumberOfRecords { get; set; }
		}

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<GlobalSearchResult, ApiSearchRunOutputModel>()
				.ForMember(m => m.Clusters, opt => opt.MapFrom(r => r.Clusters.Values))
				.Ignore(m => m.Context);

			Mapper.CreateMap<GlobalSearchResultItem, ItemModel>()
				.ForMember(m => m.Vicinity, opt => opt.ResolveUsing(ApiOutputVicinitySummaryModel.NullableVicinityIDValueResolver).FromMember(i => i.VicinityID));

			Mapper.CreateMap<GlobalSearchResultCluster, ClusterModel>()
				.ForMember(m => m.Vicinity, opt => opt.ResolveUsing(ApiOutputVicinitySummaryModel.VicinityIDValueResolver).FromMember(c => c.VicinityID));
		}
	}
}