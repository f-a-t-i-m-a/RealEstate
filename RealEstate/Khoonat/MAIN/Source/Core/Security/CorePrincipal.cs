using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Permissions;

namespace JahanJooy.RealEstate.Core.Security
{
	public class CorePrincipal : IPrincipal
	{
		public static readonly CorePrincipal Anonymous = new CorePrincipal(CoreIdentity.Anonymous, false, true, false, null);

		#region Private fields

		private readonly CoreIdentity _identity;
		private readonly bool _isOperator;
		private readonly bool _isEnabled;
		private readonly bool _isVerified;
		private readonly HashSet<GeneralPermission> _generalPermissions;

		#endregion

		#region Construction and Cloning

		public CorePrincipal(CoreIdentity identity, bool isOperator, bool isEnabled, bool isVerified, IEnumerable<GeneralPermission> generalPermissions)
		{
			_identity = identity;
			_isOperator = isOperator;
			_isEnabled = isEnabled;
			_isVerified = isVerified;
			_generalPermissions = new HashSet<GeneralPermission>(generalPermissions ?? Enumerable.Empty<GeneralPermission>());
		}

		public CorePrincipal(User user)
		{
			_identity = new CoreIdentity(user);
			_isOperator = user.IsOperator;
			_isEnabled = user.IsEnabled;
			_isVerified = user.IsVerified;
			_generalPermissions = new HashSet<GeneralPermission>(user.GeneralPermissions);
		}

		public CorePrincipal(CorePrincipal source)
			: this(CoreIdentity.Copy(source._identity), source._isOperator, source._isEnabled, source._isVerified, source._generalPermissions)
		{
		}

		public static CorePrincipal Copy(CorePrincipal source)
		{
			return source == null ? null : new CorePrincipal(source);
		}

		public static ICollection<CorePrincipal> Copy(ICollection<CorePrincipal> sources)
		{
			return sources?.Select(Copy).ToList();
		}

		#endregion

		#region Properties and Permissions

		public CoreIdentity CoreIdentity
		{
			get { return _identity ?? CoreIdentity.Anonymous; }
		}

		public bool IsAuthenticated
		{
			get { return CoreIdentity.IsAuthenticated; }
		}

		public bool IsOperator
		{
			get { return _isOperator; }
		}

		public bool IsEnabled
		{
			get { return _isEnabled; }
		}

		public bool IsVerified
		{
			get { return _isVerified; }
		}

		public bool HasPermission(GeneralPermission generalPermission)
		{
			return _generalPermissions.Contains(generalPermission);
		}

		#endregion

		#region Implementation of IPrincipal

		public bool IsInRole(string role)
		{
			role = role.ToLower();

			if (role == "operator")
				return IsOperator;

			GeneralPermission generalPermission;
			if (Enum.TryParse(role, true, out generalPermission))
				return HasPermission(generalPermission);

			return false;
		}

		public IIdentity Identity
		{
			get { return _identity ?? CoreIdentity.Anonymous; }
		}

		#endregion
	}
}