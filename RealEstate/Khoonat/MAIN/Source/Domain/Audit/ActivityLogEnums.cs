namespace JahanJooy.RealEstate.Domain.Audit
{
	public enum ActivityAction : byte
	{
		Create = 11,
		Edit = 12,
		Delete = 13,
		Enable = 21,
		Disable = 22,
		Publish = 23,
		Unpublish = 24,
		Approve = 31,
		Reject = 32,
        Reverse = 33,
		Other = 99,
		CreateDetail = 111,
		EditDetail = 112,
		DeleteDetail = 113,
		EnableDetail = 121,
		DisableDetail = 122,
		PublishDetail = 123,
		UnpublishDetail = 124,
		ApproveDetail = 131,
		RejectDetail = 132,
		OtherDetail = 199,
		Authenticate = 200,
	}

	public enum TargetEntityType : byte
	{
		Province = 1,
		City = 2,
		CityRegion = 3,
		Neighborhood = 4,
		Vicinity = 5,
		User = 10,
		PropertyListing = 11,
		PropertyListingPhoto = 12,
		SavedPropertySearch = 31,
        Agency = 41,
        AgencyBranch = 42,
        PromotionalBonus = 51,
        PromotionalBonusCoupon = 52,
        UserBalanceAdministrativeChange = 53,
        UserElectronicPayment = 54,
        UserWireTransferPayment = 55,
        UserRefundRequest = 56,
        ConfigurationDataItem = 80,
		AbuseFlag = 91,
	}

	public enum DetailEntityType : byte
	{
        UserBillingTransaction = 50,
		UserContactMethod = 101,
	}

	public enum AuditEntityType : byte
	{
		LoginNameQuery = 1,
		PasswordResetRequest = 2,
		UserContactMethodVerification = 3,
		PropertyListingPublishHistory = 11,
		PropertyListingUpdateHistory = 12
	}


}