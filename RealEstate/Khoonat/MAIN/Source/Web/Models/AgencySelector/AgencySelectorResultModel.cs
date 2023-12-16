using System.Collections.Generic;

namespace JahanJooy.RealEstate.Web.Models.AgencySelector
{
    public class AgencySelectorResultModel
    {
        public bool More { get; set; }
        public List<AgencySelectorResultItemModel> Items { get; set; }
    }
}