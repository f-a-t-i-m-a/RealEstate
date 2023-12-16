using Compositional.Composer;
using JahanJooy.Common.Util.Validation;

namespace JahanJooy.RealEstateAgency.Util.Notification
{
    [Contract]

    public interface ISmsTransmitter
    {
        ValidationResult Send(string contactMethodText, string text);
    }
}