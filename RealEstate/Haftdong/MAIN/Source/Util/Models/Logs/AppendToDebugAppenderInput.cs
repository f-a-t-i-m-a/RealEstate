using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Logs
{
    [TsClass]
    public class AppendToDebugAppenderInput
    {
        public string Logger { get; set; }
        public bool Append { get; set; }
    }
}