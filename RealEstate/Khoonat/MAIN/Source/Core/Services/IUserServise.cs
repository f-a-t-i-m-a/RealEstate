using Compositional.Composer;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Services
{
    [Contract]
    public interface IUserService
    {
        void SetUserTypeAndAgencyMembership(long userID, UserType userType, long? agencyID);
    }
}
