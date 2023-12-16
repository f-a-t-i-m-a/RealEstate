using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application
{
	public class ApiCallContext
	{
		#region Lifecycle management

		private static readonly string HttpContextKey = typeof(ApiCallContext).FullName;

		private ApiCallContext()
		{
		}

		public static bool Exists => (HttpContext.Current.Items[HttpContextKey] as ApiCallContext) != null;

	    public static ApiCallContext Current
		{
			get
			{
				var currentApiCallContext = HttpContext.Current.Items[HttpContextKey] as ApiCallContext;
				if (currentApiCallContext == null)
				{
					throw new InvalidOperationException("No ApiCallContext exists in current context. " +
														"This may not be an API call. " +
														"Use ApiCallContext.Exists before ApiCallContext.Current to ensure an API call is in context.");
				}

				return currentApiCallContext;
			}
		}

		public static ApiCallContext Create(ApiInputContextModel inputContext)
		{
			if (Exists)
			{
				throw new InvalidOperationException("An ApiCallContext already exists in current context. " +
													"Cannot create another ApiCallContext. " +
													"Use ApiCallContext.Exists before ApiCallContext.Create() to determine if an ongoing context exists, " +
													"or use ApiCallContext.Destroy() to terminate the ongoing context.");
			}

			if (inputContext == null)
			{
				throw new ArgumentNullException(nameof(inputContext));
			}

			var result = new ApiCallContext
			             {
				             InputContext = inputContext,
							 Stopwatch = Stopwatch.StartNew()
			             };

			HttpContext.Current.Items[HttpContextKey] = result;

			return result;
		}

		public static void Destroy()
		{
			if (!Exists)
			{
				throw new InvalidOperationException("No ApiCallContext exists in current context to destroy. " +
													"Use ApiCallContext.Exists before ApiCallContext.Destroy() to determine if an ongoing context exists.");
			}

			HttpContext.Current.Items[HttpContextKey] = null;
		}


		#endregion

		#region Input context

		private static readonly CultureInfo DefaultCulture = CultureInfo.InvariantCulture;
		private const string DefaultUserAgent = "unknown-user-agent";
		private const string DefaultDeviceId = "unknown-device-id";

		public ApiInputContextModel InputContext { get; private set; }

		public bool MessageSignatureVerified { get; set; }
		public SessionInfo Session { get; set; }
		public ApiUser ApiUser { get; set; }
		public CorePrincipal EndUser { get; set; }
		public Stopwatch Stopwatch { get; private set; }

		private CultureInfo _userCulture;

		public CultureInfo UserCulture
		{
			get { return _userCulture ?? DefaultCulture; }
			set { _userCulture = value; }
		}

		public string UserAgent => Session?.Record?.UserAgent ?? DefaultUserAgent;

	    public string DeviceId => Session?.Record?.StartupUri ?? DefaultDeviceId;

	    #endregion

		#region Output context

		private List<string> _serverMessages;
		private List<string> _customMessages; 

		public bool MarkedAsObsolete { get; private set; }

		public void AddServerMessage(string message)
		{
			if (_serverMessages == null)
				_serverMessages = new List<string>();

			_serverMessages.Add(message);
		}

		public void AddCustomMessage(string message)
		{
			if (_customMessages == null)
				_customMessages = new List<string>();

			_customMessages.Add(message);
		}

		public void MarkAsObsolete()
		{
			MarkedAsObsolete = true;
		}

		public string[] ServerMessages => (_serverMessages ?? Enumerable.Empty<string>()).ToArray();

	    public string[] CustomMessages => (_customMessages ?? Enumerable.Empty<string>()).ToArray();

	    #endregion
	}
}