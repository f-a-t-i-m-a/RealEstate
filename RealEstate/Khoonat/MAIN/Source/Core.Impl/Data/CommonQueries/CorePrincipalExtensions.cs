using System.Collections.Generic;
using System.Linq;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Core.Impl.Data.CommonQueries
{
	public static class CorePrincipalExtensions
	{
		public static List<UserContactMethod> LoadVerifiedContactMethods(this CorePrincipal principal, DbManager dbManager, ContactMethodType? contactMethodType = null)
		{
			var query = dbManager.Db.UserContactMethods.Where(cm => cm.UserID == principal.CoreIdentity.UserId && !cm.IsDeleted && cm.IsVerified);
			if (contactMethodType.HasValue)
				query = query.Where(cm => cm.ContactMethodType == contactMethodType.Value);

			return query.ToList();
		}
	}
}