using System;
using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
    [Component]
    public class UserService : IUserService
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        #endregion

        #region Implementation of IUserService

        public void SetUserTypeAndAgencyMembership(long userID, UserType userType, long? agencyID)
        {
            var user = DbManager.Db.UsersDbSet.SingleOrDefault(u => u.ID == userID);
            if (user == null)
                throw new ArgumentException("User not found.");

	        if (userType == UserType.IndependentAgencyMember || userType == UserType.AgencyAgent)
	        {
				if (agencyID == null)
					userType = UserType.IndependentAgent;
	        }
	        else
	        {
		        agencyID = null;
	        }

            user.Type = userType;
            user.AgencyID = agencyID;
        }

        #endregion
    }
}