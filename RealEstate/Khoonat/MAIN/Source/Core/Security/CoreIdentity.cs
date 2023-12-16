using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Security
{
	public class CoreIdentity : IIdentity
	{
		public static readonly CoreIdentity Anonymous = new CoreIdentity(null, null, "Anonymous", "Anonymous",null,Domain.UserType.NormalUser);

		#region Private fields

		private readonly long? _userId;
		private readonly string _loginName;
		private readonly string _displayName;
		private readonly string _fullName;
	    private readonly long? _agencyId;
	    private readonly UserType _userType;

		#endregion

		#region Construction and Cloning

		public CoreIdentity(long? userId, string loginName, string displayName, string fullName, long? agencyId, UserType userType)
		{
			_userId = userId;
			_loginName = loginName;
			_displayName = displayName;
			_fullName = fullName;
		    _agencyId = agencyId;
		    _userType = userType;
		}

		public CoreIdentity(User user)
		{
		    _userId = user.ID;
			_loginName = user.LoginName;
			_displayName = user.DisplayName;
			_fullName = user.FullName;
            _agencyId = user.AgencyID;
            _userType = user.Type;
        }

		public CoreIdentity(CoreIdentity source)
			: this(source._userId, source._loginName, source._displayName, source._fullName, source._agencyId, source._userType)
		{
		}

	    public static CoreIdentity Copy(CoreIdentity source)
		{
			return source == null ? null : new CoreIdentity(source);
		}

		public static ICollection<CoreIdentity> Copy(ICollection<CoreIdentity> sources)
		{
			return sources == null ? null : sources.Select(Copy).ToList();
		}

		#endregion

		#region Properties

		public long? UserId
		{
			get { return _userId; }
		}

		public string LoginName
		{
			get { return _loginName; }
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		public string FullName
		{
			get { return _fullName; }
		}

		#endregion

		#region Implementation of IIdentity

		public string Name
		{
			get { return _loginName; }
		}

		public string AuthenticationType
		{
			get { return "Custom"; }
		}

		public bool IsAuthenticated
		{
			get { return _userId.HasValue; }
		}

        public long? AgencyId
        {
            get { return _agencyId; }
        }

        public UserType UserType
        {
            get { return _userType; }
        }
		#endregion
	}
}