using System.Collections.Generic;

namespace JahanJooy.RealEstate.Web.Models.VicinitySelector
{
    public class VicinitySelectorResultModel
    {
        public bool More { get; set; }
        public List<VicinitySelectorResultItemModel> Items { get; set; }
    }
}