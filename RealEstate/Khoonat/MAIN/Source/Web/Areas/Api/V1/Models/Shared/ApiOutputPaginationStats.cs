using AutoMapper;
using JahanJooy.Common.Util.Collections;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared
{
	public class ApiOutputPaginationStats
	{
		public int PageSize { get; set; }
		public int PageNumber { get; set; }
		public int TotalNumberOfPages { get; set; }
		public int TotalNumberOfItems { get; set; }
		public int FirstItemIndex { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<PaginationStats, ApiOutputPaginationStats>();
		}
	}
}