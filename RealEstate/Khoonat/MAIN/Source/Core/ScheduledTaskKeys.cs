namespace JahanJooy.RealEstate.Core
{
	public static class ScheduledTaskKeys
	{
		public const string SendSavedPropertySearchEmailsTask = "SendSavedPropertySearchEmailsTask";
		public const string SendSavedPropertySearchSmsTask = "SendSavedPropertySearchSmsTask";
		public const string BillSavedSearchSmsNotificationsTask = "BillSavedSearchSmsNotificationsTask";
		public const string FinalizePartialSavedSearchSmsNotificationBillingsTask = "FinalizePartialSavedSearchSmsNotificationBillingsTask";
		public const string TransmitSmsMessagesTask = "TransmitSmsMessagesTask";
		public const string CheckSmsMessageDeliveryTask = "CheckSmsMessageDeliveryTask";
		public const string SendPublishTimeWarningEmailTask = "SendPublishTimeWarningEmailTask";
		public const string SendEmailToInactiveUsersTask = "SendEmailToInactiveUsersTask";
        public const string RecalculateSponsoredEntitiesTask = "RecalculateSponsoredEntitiesTask";
        public const string BillSponsoredEntityImpressionsTask = "BillSponsoredEntityImpressionsTask";
        public const string FinalizePartialSponsoredEntityImpressionBillingsTask = "FinalizePartialSponsoredEntityImpressionBillingsTask";
        public const string BillSponsoredEntityClicksTask = "BillSponsoredEntityClicksTask";
        public const string FinalizePartialSponsoredEntityClickBillingsTask = "FinalizePartialSponsoredEntityClickBillingsTask";
        public const string SendNotificationMessageEmailAndSms = "SendNotificationMessageEmailAndSms";

		public const string OptimizeAllIndexes = "OptimizeAllIndexes";
		public const string IndexAgencies = "IndexAgencies";
		public const string IndexAgencyBranches = "IndexAgencyBranches";
		public const string IndexPropertyListings = "IndexPropertyListings";
		public const string IndexPropertyRequests = "IndexPropertyRequests";
	}
}