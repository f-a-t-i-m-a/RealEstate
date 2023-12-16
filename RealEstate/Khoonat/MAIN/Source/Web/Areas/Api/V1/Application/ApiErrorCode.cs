namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application
{
	public enum ApiErrorCode
	{
		Success = 0,

		MethodIsOnlyAllowedOnNewerVersion = 302,
		MethodInputIsInvalid = 400,
		MethodInvocationNotAuthorized = 401,
		MethodInvocationIsForbidden = 403,
		MethodNotFound = 404,
		MethodNoLongerSupported = 410,
		InternalServerError = 500,
		MethodNotImplementedYet = 501,
		ServiceCurrentlyUnavailable = 503,

		InputContextIsMissing = 1000,
		ApiKeyIsMissing = 1001,
		ApiKeyIsNotInCorrectFormat = 1002,
		ApiKeyIsInvalid = 1003,
		ApiKeyIsExpired = 1009,
		ApiKeyQuotaReached = 1010,
		ApiKeyDailyQuotaReached = 1011,
		ApiKeyDaylyPerUserQuotaReached = 1012,
		ApiKeyIsNotGrantedToRequestedMethod = 1020,

		SessionIdIsRequiredForThisApiKey = 1030,
		SessionIdIsNotInCorrectFormat = 1031,
		SessionIdIsInvalid = 1032,
		SessionIsExpired = 1033,
		SessionIsAuthenticatedAndNeedsCredentials = 1034,
        SessionIsRequiredForThisApiCall = 1035,

		AuthTokenIsRequiredForThisApiKey = 1040,
		AuthTokenIsRequiredForThisMethod = 1041,
		AuthTokenIsNotInCorrectFormat = 1042,
		AuthTokenSignatureDoesNotMatch = 1043,
		AuthTokenIsExpired = 1044,

		MessageSignatureIsRequiredForThisApiKey = 1050,
		MessageSignatureTimestampIsMissing = 1051,
		MessageSignatureTimestampIsNotInAllowedWindow = 1052,
		MessageSignatureIsNotInCorrectFormat = 1053,
		MessageSignatureValidationFailed = 1054,

		UserCultureIsRequired = 1060,
		UserCultureIsNotInCorrectFormat = 1061,
		UserCultureIsNotIdentified = 1062,
		UserCultureIsNotSupported = 1063,

		InputIsEmpty = 2005,
		EntityNotFound = 2010,
		InputQueryIsEmpty = 2011,
		InputQueryTooLarge = 2012,
		InputFileIsEmpty = 2021,
		InputFileTooLarge = 2022,

		ValidationError = 2100,

		PermissionDeniedForService = 2902,
		PermissionDeniedForEntity = 2905,

		UnhandledException = 9901,
		UnknownError = 9909
	}
}