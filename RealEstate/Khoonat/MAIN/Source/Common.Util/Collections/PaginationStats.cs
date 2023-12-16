using System;

namespace JahanJooy.Common.Util.Collections
{
	public class PaginationStats
	{
		private int _pageSize;
		private int _pageNumber;
		private int _totalNumberOfItems;
		private int _totalNumberOfPages;
		private int _firstItemIndex;

		public int PageSize
		{
			get { return _pageSize; }
		}

		public int PageNumber
		{
			get { return _pageNumber; }
		}

		public int TotalNumberOfPages
		{
			get { return _totalNumberOfPages; }
		}

		public int TotalNumberOfItems
		{
			get { return _totalNumberOfItems; }
		}

		public int FirstItemIndex
		{
			get { return _firstItemIndex; }
		}

		#region Initialization

		private PaginationStats()
		{
		}

		public static PaginationStats SinglePage(int count)
		{
			var result = new PaginationStats();

			result._totalNumberOfItems = result._pageSize = count;
			result._pageNumber = result._totalNumberOfPages = result._firstItemIndex = 1;

			return result;
		}

		public static PaginationStats FromPageNumber(int totalNumberOfItems, int pageSize, int pageNumber)
		{
			var result = new PaginationStats();

			result._totalNumberOfItems = totalNumberOfItems;
			result._pageSize = pageSize;
			result._totalNumberOfPages = (totalNumberOfItems + pageSize - 1) / pageSize;

			result._pageNumber = Math.Max(Math.Min(pageNumber, result._totalNumberOfPages), 1);
			result._firstItemIndex = (result._pageNumber - 1) * result._pageSize + 1;

			return result;
		}

		public static PaginationStats FromStartIndex(int totalNumberOfItems, int pageSize, int startIndex)
		{
			var result = new PaginationStats();

			result._totalNumberOfItems = totalNumberOfItems;
			result._pageSize = pageSize;
			result._totalNumberOfPages = (totalNumberOfItems + pageSize - 1) / pageSize;

			result._firstItemIndex = Math.Max(Math.Min(startIndex, totalNumberOfItems), 1);
			result._pageNumber = (startIndex + pageSize - 2)/pageSize + 1;

			return result;
		}

		public static PaginationStats FromPaginationStats(PaginationStats source)
	    {
            var result = new PaginationStats();

            result._totalNumberOfItems = source._totalNumberOfItems;
            result._pageSize = source._pageSize;
	        result._totalNumberOfPages = source._totalNumberOfPages;

	        result._firstItemIndex = source._firstItemIndex;
	        result._pageNumber = source._pageNumber;

	        return result;
	    }

		#endregion
	}
}