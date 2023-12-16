using Compositional.Composer;
using JahanJooy.Common.Util.Validation;

namespace JahanJooy.RealEstateAgency.Util.Notification
{
    [Contract]
    public interface IEmailTransmitter
    {
        ValidationResult Send(string subject, string body, string[] toAddresses, string[] ccAddresses,
            string[] bccAddresses);
    }
}