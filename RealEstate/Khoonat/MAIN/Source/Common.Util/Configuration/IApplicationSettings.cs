using Compositional.Composer;

namespace JahanJooy.Common.Util.Configuration
{
    [Contract]
    public interface IApplicationSettings
    {
        string this[string key] { get; }
        void Reload();
    }
}