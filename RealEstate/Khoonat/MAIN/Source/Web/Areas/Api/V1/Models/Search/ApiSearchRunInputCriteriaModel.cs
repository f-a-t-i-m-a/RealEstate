using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared.Spatial;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Search
{
	public class ApiSearchRunInputCriteriaModel
	{
		public ApiSearchRunInputCriteriaModel()
		{
			ClusterGeographically = true;
			IncludeCurrentRecords = true;
		}

		public ApiGeoBox Bounds { get; set; }
		public List<long> VicinityIDs { get; set; }
		public bool ClusterGeographically { get; set; }

		public string SearchText { get; set; }

		public List<string> IncludedTags { get; set; }
		public List<string> ExcludedTags { get; set; }

		public List<GlobalSearchRecordType> RecordTypes { get; set; }

		public bool IncludeCurrentRecords { get; set; }
		public bool IncludeArchivedRecords { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<ApiSearchRunInputCriteriaModel, GlobalSearchCriteria>()
				.Ignore(c => c.IncludeDeletedRecords);
		}
	}
}