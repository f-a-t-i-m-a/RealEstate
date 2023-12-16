using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JahanJooy.Common.Util.General;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminOperations
{
    public class AdminOperationsStartRecalculateUserTransactionModel
    {

        public string UserIDsCsv
        {
            get { return CsvUtils.ToCsvString(UserIDs); }
            set { UserIDs = CsvUtils.ParseInt64Enumerable(value).ToArray(); }
        }

        public long[] UserIDs { get; set; }
    }
}