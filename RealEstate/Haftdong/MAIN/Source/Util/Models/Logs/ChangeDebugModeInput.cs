using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Logs
{
    [TsClass]
    public class ChangeDebugModeInput
    {
        public string Logger { get; set; }
        public bool DebugEnabled { get; set; }
    }
}