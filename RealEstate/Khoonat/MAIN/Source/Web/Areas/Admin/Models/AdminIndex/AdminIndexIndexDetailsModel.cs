using System;
using System.Collections.Generic;
using JahanJooy.Common.Util.Search;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminIndex
{
    public class AdminIndexIndexDetailsModel
    {
        public LuceneIndexStatistics LuceneIndexStatistics { get; set; }

        public List<Exception> Errors { get; set; }
    }
}