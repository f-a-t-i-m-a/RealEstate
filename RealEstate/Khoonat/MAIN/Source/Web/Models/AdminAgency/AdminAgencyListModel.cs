using JahanJooy.Common.Util.Collections;

namespace JahanJooy.RealEstate.Web.Models.AdminAgency    
{
    public class AdminAgencyListModel
	{
        public PagedList<Domain.Directory.Agency> Agencies { get; set; }

        public string Page { get; set; }

        public bool IsIncludeDeletedAgencies { get; set; }

	}
}

