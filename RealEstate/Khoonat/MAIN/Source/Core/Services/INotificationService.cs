using Compositional.Composer;

namespace JahanJooy.RealEstate.Core.Services
{
   [Contract]
    public interface INotificationService
   {
       bool SendEmailAndSms();
   }
}
