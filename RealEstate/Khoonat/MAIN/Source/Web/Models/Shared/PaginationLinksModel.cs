using System;
using JahanJooy.Common.Util.Collections;

namespace JahanJooy.RealEstate.Web.Models.Shared
{
	public class PaginationLinksModel
	{
		public Func<int, string> UrlFunction { get; set; }
		public int CurrentPageNumber { get; set; }
		public int TotalNumberOfPages { get; set; }
		public int? IndexOfFirstRecord { get; set; }
		public int? IndexOfLastRecord { get; set; }
		public int? TotalNumberOfRecords { get; set; }

		public static PaginationLinksModel BuildFromPagedList<T>(PagedList<T> pagedList, string urlTemplate)
		{
			return BuildFromPagedList(pagedList, i => string.Format(urlTemplate, i));
		}

		public static PaginationLinksModel BuildFromPagedList<T>(PagedList<T> pagedList, Func<int, string> urlFunction)
		{
			return new PaginationLinksModel
				       {
						   UrlFunction = urlFunction,
					       CurrentPageNumber = pagedList.PageNumber,
					       TotalNumberOfPages = pagedList.TotalNumberOfPages,
					       IndexOfFirstRecord = pagedList.FirstItemIndex,
					       IndexOfLastRecord = pagedList.LastItemIndex,
					       TotalNumberOfRecords = pagedList.TotalNumberOfItems
				       };
		}
    }
}