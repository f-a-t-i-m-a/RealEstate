using System.Collections.Generic;

namespace JahanJooy.RealEstateAgency.Util.Models.Vicinities
{
    public class VicinityListOutput
    {
        public List<VicinitySummary> Vicinities { get; set; }
        public VicinitySummary CurrentVicinity { get; set; }
        public VicinitySummary CurrentVicinityFromCache { get; set; }
        public bool AllParentsEnabled { get; set; }
        public List<VicinitySummary> Hierarchy { get; set; }
    }
}