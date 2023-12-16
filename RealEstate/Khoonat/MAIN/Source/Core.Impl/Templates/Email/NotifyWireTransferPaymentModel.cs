using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Templates.Email
{
    public class NotifyWireTransferPaymentModel
    {
        public User User { get; set; }
        public UserWireTransferPayment Payment { get; set; }
    }
}
