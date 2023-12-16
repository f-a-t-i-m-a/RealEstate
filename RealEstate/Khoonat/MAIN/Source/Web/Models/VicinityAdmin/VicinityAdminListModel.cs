using System.Collections.Generic;
using System.Web.Mvc;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Web.Models.VicinityAdmin
{
    public class VicinityAdminListModel
	{
		public List<Vicinity> Vicinities { get; set; }
        public Vicinity CurrentVicinity { get; set; }
		public Vicinity CurrentVicinityFromCache { get; set; }
		public bool AllParentsEnabled { get; set; }
		public List<Vicinity> Hierarchy { get; set; }

        public List<SelectListItem> SelectScope { get; set; }
        public long? SelectedScope { get; set; }

	}
}