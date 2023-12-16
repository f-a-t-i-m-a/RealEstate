using JahanJooy.Common.Util.Collections;

namespace JahanJooy.RealEstate.Web.Models.Agency    
{
    public class AgencyListModel
	{
        public PagedList<Domain.Directory.Agency> Agencies { get; set; }

        public string Page { get; set; }

        public bool IsIncludeDeletedAgencies { get; set; }

	}
}

