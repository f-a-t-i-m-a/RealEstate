using System.Linq;
using JahanJooy.Common.Util.General;
using JahanJooy.RealEstate.Core.Search;

namespace JahanJooy.RealEstate.Web.Models.Properties
{
	public class PropertiesSelectLocationModel
	{
		public string Query { get; set; }
		public PropertySearch Search { get; set; }

        public string VicinityIDsCsv
        {
            get { return CsvUtils.ToCsvString(VicinityIDs); }
            set { VicinityIDs = CsvUtils.ParseInt64Enumerable(value).ToArray(); }
        }
        public long[] VicinityIDs { get; set; }
        
		public string SelectedSubmit { get; set; }
	}
}