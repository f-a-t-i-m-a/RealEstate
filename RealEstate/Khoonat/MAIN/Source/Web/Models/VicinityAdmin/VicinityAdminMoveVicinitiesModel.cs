using System.Collections.Generic;
using System.Web.Mvc;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Web.Models.VicinityAdmin
{
    public class VicinityAdminMoveVicinitiesModel
    {
        public List<SelectListItem> SelectScope { get; set; }
        public long? SelectedScope { get; set; }
        public Vicinity CurrentVicinity { get; set; }
        public List<Vicinity>  Vicinities { get; set; }
        public long? ParetntID { get; set; }

    }
}