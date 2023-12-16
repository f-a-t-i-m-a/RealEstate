





/// <reference path="Enums.ts" />

declare module JahanJooy.RealEstateAgency.Util.Models.Vicinities {
    interface ListOfVicinityInput {
        Ids: string[];
        Value: boolean;
    }
    interface MoveVicinitiesInput {
        Ids: string[];
        ParentId: string;
    }
    interface NewVicinityInput {
        Name: string;
        Type: JahanJooy.RealEstateAgency.Domain.Enums.Vicinity.VicinityType;
        ShowTypeInTitle: boolean;
        AlternativeNames: string;
        AdditionalSearchText: string;
        WellKnownScope: JahanJooy.RealEstateAgency.Domain.Enums.Vicinity.VicinityType;
        ShowInSummary: boolean;
        CanContainPropertyRecords: boolean;
        CurrentVicinityID: MongoDB.Bson.ObjectId;
        AllowedVicinityTypes: JahanJooy.RealEstateAgency.Domain.Enums.Vicinity.VicinityType[];
    }
    interface SearchVicinityByPointInput {
        UserPoint: JahanJooy.Common.Util.Spatial.LatLng;
    }
    interface SearchVicinityInput {
        SearchText: string;
        CanContainPropertyRecordsOnly: boolean;
        ParentId: MongoDB.Bson.ObjectId;
        StartIndex: number;
        PageSize: number;
        SortColumn: JahanJooy.RealEstateAgency.Util.Models.Properties.PropertySortColumn;
        SortDirection: JahanJooy.RealEstateAgency.Util.Models.Base.SortDirectionType;
    }
    interface UpdateVicinityInput {
        ID: MongoDB.Bson.ObjectId;
        Name: string;
        AlternativeNames: string;
        AdditionalSearchText: string;
        Description: string;
        OfficialLinkUrl: string;
        WikiLinkUrl: string;
        AdministrativeNotes: string;
        Enabled: boolean;
        Order: number;
        Type: JahanJooy.RealEstateAgency.Domain.Enums.Vicinity.VicinityType;
        WellKnownScope: JahanJooy.RealEstateAgency.Domain.Enums.Vicinity.VicinityType;
        ShowTypeInTitle: boolean;
        ShowInHierarchy: boolean;
        ShowInSummary: boolean;
        CanContainPropertyRecords: boolean;
    }
    interface VicinitySummary {
        ID: MongoDB.Bson.ObjectId;
        Name: string;
        AlternativeNames: string;
        AdditionalSearchText: string;
        Description: string;
        OfficialLinkUrl: string;
        WikiLinkUrl: string;
        AdministrativeNotes: string;
        Enabled: boolean;
        Order: number;
        CompleteName: string;
        Type: JahanJooy.RealEstateAgency.Domain.Enums.Vicinity.VicinityType;
        WellKnownScope: JahanJooy.RealEstateAgency.Domain.Enums.Vicinity.VicinityType;
        ShowTypeInTitle: boolean;
        ShowInHierarchy: boolean;
        ShowInSummary: boolean;
        CanContainPropertyRecords: boolean;
        CenterPoint: MongoDB.Driver.GeoJsonObjectModel.GeoJson2DCoordinates;
        Boundary: MongoDB.Driver.GeoJsonObjectModel.GeoJsonPolygon<MongoDB.Driver.GeoJsonObjectModel.GeoJson2DCoordinates>;
        Parent: JahanJooy.RealEstateAgency.Util.Models.Vicinities.VicinitySummary;
        ParentID: MongoDB.Bson.ObjectId;
        Children: JahanJooy.RealEstateAgency.Util.Models.Vicinities.VicinitySummary[];
        Properties: JahanJooy.RealEstateAgency.Domain.Property.PropertyReference[];
    }
}
declare module MongoDB.Bson {
    interface ObjectId {
        Empty: MongoDB.Bson.ObjectId;
        Timestamp: number;
        Machine: number;
        Pid: number;
        Increment: number;
        CreationTime: Date;
    }
    interface BsonElement {
        Name: string;
        Value: MongoDB.Bson.BsonValue;
    }
    interface BsonValue {
        AsBoolean: boolean;
        AsBsonArray: MongoDB.Bson.BsonValue[];
        AsBsonBinaryData: MongoDB.Bson.BsonBinaryData;
        AsBsonDateTime: MongoDB.Bson.BsonDateTime;
        AsBsonDocument: MongoDB.Bson.BsonElement[];
        AsBsonJavaScript: MongoDB.Bson.BsonJavaScript;
        AsBsonJavaScriptWithScope: MongoDB.Bson.BsonJavaScriptWithScope;
        AsBsonMaxKey: MongoDB.Bson.BsonMaxKey;
        AsBsonMinKey: MongoDB.Bson.BsonMinKey;
        AsBsonNull: MongoDB.Bson.BsonNull;
        AsBsonRegularExpression: MongoDB.Bson.BsonRegularExpression;
        AsBsonSymbol: MongoDB.Bson.BsonSymbol;
        AsBsonTimestamp: MongoDB.Bson.BsonTimestamp;
        AsBsonUndefined: MongoDB.Bson.BsonUndefined;
        AsBsonValue: MongoDB.Bson.BsonValue;
        AsByteArray: number[];
        AsDateTime: Date;
        AsDouble: number;
        AsGuid: System.Guid;
        AsInt32: number;
        AsLocalTime: Date;
        AsInt64: number;
        AsNullableBoolean: boolean;
        AsNullableDateTime: Date;
        AsNullableDouble: number;
        AsNullableGuid: System.Guid;
        AsNullableInt32: number;
        AsNullableInt64: number;
        AsNullableObjectId: MongoDB.Bson.ObjectId;
        AsObjectId: MongoDB.Bson.ObjectId;
        AsRegex: System.Text.RegularExpressions.Regex;
        AsString: string;
        AsUniversalTime: Date;
        BsonType: MongoDB.Bson.BsonType;
        IsBoolean: boolean;
        IsBsonArray: boolean;
        IsBsonBinaryData: boolean;
        IsBsonDateTime: boolean;
        IsBsonDocument: boolean;
        IsBsonJavaScript: boolean;
        IsBsonJavaScriptWithScope: boolean;
        IsBsonMaxKey: boolean;
        IsBsonMinKey: boolean;
        IsBsonNull: boolean;
        IsBsonRegularExpression: boolean;
        IsBsonSymbol: boolean;
        IsBsonTimestamp: boolean;
        IsBsonUndefined: boolean;
        IsDateTime: boolean;
        IsDouble: boolean;
        IsGuid: boolean;
        IsInt32: boolean;
        IsInt64: boolean;
        IsNumeric: boolean;
        IsObjectId: boolean;
        IsString: boolean;
        IsValidDateTime: boolean;
        RawValue: any;
        Item: MongoDB.Bson.BsonValue;
    }
    interface BsonBinaryData extends MongoDB.Bson.BsonValue {
        BsonType: MongoDB.Bson.BsonType;
        Bytes: number[];
        GuidRepresentation: MongoDB.Bson.GuidRepresentation;
        RawValue: any;
        SubType: MongoDB.Bson.BsonBinarySubType;
    }
    interface BsonDateTime extends MongoDB.Bson.BsonValue {
        BsonType: MongoDB.Bson.BsonType;
        IsValidDateTime: boolean;
        MillisecondsSinceEpoch: number;
        RawValue: any;
        Value: Date;
    }
    interface BsonJavaScript extends MongoDB.Bson.BsonValue {
        BsonType: MongoDB.Bson.BsonType;
        Code: string;
    }
    interface BsonJavaScriptWithScope extends MongoDB.Bson.BsonJavaScript {
        BsonType: MongoDB.Bson.BsonType;
        Scope: MongoDB.Bson.BsonElement[];
    }
    interface BsonMaxKey extends MongoDB.Bson.BsonValue {
        Value: MongoDB.Bson.BsonMaxKey;
        BsonType: MongoDB.Bson.BsonType;
    }
    interface BsonMinKey extends MongoDB.Bson.BsonValue {
        Value: MongoDB.Bson.BsonMinKey;
        BsonType: MongoDB.Bson.BsonType;
    }
    interface BsonNull extends MongoDB.Bson.BsonValue {
        Value: MongoDB.Bson.BsonNull;
        BsonType: MongoDB.Bson.BsonType;
    }
    interface BsonRegularExpression extends MongoDB.Bson.BsonValue {
        BsonType: MongoDB.Bson.BsonType;
        Pattern: string;
        Options: string;
    }
    interface BsonSymbol extends MongoDB.Bson.BsonValue {
        BsonType: MongoDB.Bson.BsonType;
        Name: string;
    }
    interface BsonTimestamp extends MongoDB.Bson.BsonValue {
        BsonType: MongoDB.Bson.BsonType;
        Value: number;
        Increment: number;
        Timestamp: number;
    }
    interface BsonUndefined extends MongoDB.Bson.BsonValue {
        Value: MongoDB.Bson.BsonUndefined;
        BsonType: MongoDB.Bson.BsonType;
    }
}
declare module JahanJooy.Common.Util.Spatial {
    interface LatLng {
        Lat: number;
        Lng: number;
    }
    interface LatLngBounds {
        NorthEast: JahanJooy.Common.Util.Spatial.LatLng;
        SouthWest: JahanJooy.Common.Util.Spatial.LatLng;
        NorthLat: number;
        SouthLat: number;
        EastLng: number;
        WestLng: number;
    }
}
declare module MongoDB.Driver.GeoJsonObjectModel {
    interface GeoJson2DCoordinates extends MongoDB.Driver.GeoJsonObjectModel.GeoJsonCoordinates {
        Values: number[];
        X: number;
        Y: number;
    }
    interface GeoJsonCoordinates {
        Values: number[];
    }
    interface GeoJsonPolygon<TCoordinates> extends MongoDB.Driver.GeoJsonObjectModel.GeoJsonGeometry<TCoordinates> {
        Coordinates: MongoDB.Driver.GeoJsonObjectModel.GeoJsonPolygonCoordinates<TCoordinates>;
        Type: MongoDB.Driver.GeoJsonObjectModel.GeoJsonObjectType;
    }
    interface GeoJsonPolygonCoordinates<TCoordinates> {
        Exterior: MongoDB.Driver.GeoJsonObjectModel.GeoJsonLinearRingCoordinates<TCoordinates>;
        Holes: MongoDB.Driver.GeoJsonObjectModel.GeoJsonLinearRingCoordinates<TCoordinates>[];
    }
    interface GeoJsonLinearRingCoordinates<TCoordinates> extends MongoDB.Driver.GeoJsonObjectModel.GeoJsonLineStringCoordinates<TCoordinates> {
    }
    interface GeoJsonLineStringCoordinates<TCoordinates> {
        Positions: TCoordinates[];
    }
    interface GeoJsonGeometry<TCoordinates> extends MongoDB.Driver.GeoJsonObjectModel.GeoJsonObject<TCoordinates> {
    }
    interface GeoJsonObject<TCoordinates> {
        BoundingBox: MongoDB.Driver.GeoJsonObjectModel.GeoJsonBoundingBox<TCoordinates>;
        CoordinateReferenceSystem: MongoDB.Driver.GeoJsonObjectModel.GeoJsonCoordinateReferenceSystem;
        ExtraMembers: MongoDB.Bson.BsonElement[];
        Type: MongoDB.Driver.GeoJsonObjectModel.GeoJsonObjectType;
    }
    interface GeoJsonBoundingBox<TCoordinates> {
        Max: TCoordinates;
        Min: TCoordinates;
    }
    interface GeoJsonCoordinateReferenceSystem {
        Type: string;
    }
}
declare module System {
    interface Guid {
    }
    interface TimeSpan {
        Ticks: number;
        Days: number;
        Hours: number;
        Milliseconds: number;
        Minutes: number;
        Seconds: number;
        TotalDays: number;
        TotalHours: number;
        TotalMilliseconds: number;
        TotalMinutes: number;
        TotalSeconds: number;
    }
}
declare module System.Text.RegularExpressions {
    interface Regex {
        CacheSize: number;
        Options: System.Text.RegularExpressions.RegexOptions;
        MatchTimeout: System.TimeSpan;
        RightToLeft: boolean;
    }
}
declare module JahanJooy.RealEstateAgency.Domain.Property {
    interface PropertyReference {
        ID: MongoDB.Bson.ObjectId;
        CreationTime: Date;
        PropertyType: JahanJooy.RealEstateAgency.Domain.Enums.Property.PropertyType;
        Owner: JahanJooy.RealEstateAgency.Domain.Customers.CustomerReference;
        State: JahanJooy.RealEstateAgency.Domain.Workflows.PropertyState;
        LicencePlate: string;
        EstateArea: number;
        UsageType: JahanJooy.RealEstateAgency.Domain.Enums.Property.UsageType;
        Address: string;
        Vicinity: JahanJooy.RealEstateAgency.Domain.Vicinities.VicinityReference;
        GeographicLocation: MongoDB.Driver.GeoJsonObjectModel.GeoJson2DCoordinates;
        UnitArea: number;
        IsArchived: boolean;
        IsHidden: boolean;
        ConversionWarning: boolean;
        NumberOfRooms: number;
        CoverImageID: MongoDB.Bson.ObjectId;
        SourceType: JahanJooy.RealEstateAgency.Domain.Enums.Property.SourceType;
        Supplies: JahanJooy.RealEstateAgency.Domain.Supply.SupplyReference[];
    }
}
declare module JahanJooy.RealEstateAgency.Domain.Customers {
    interface CustomerReference {
        ID: MongoDB.Bson.ObjectId;
        DisplayName: string;
        PhoneNumber: string;
        Email: string;
        Address: string;
        NameOfFather: string;
        Identification: number;
        IssuedIn: string;
        SocialSecurityNumber: string;
        DateOfBirth: Date;
    }
}
declare module JahanJooy.RealEstateAgency.Domain.Vicinities {
    interface VicinityReference {
        ID: MongoDB.Bson.ObjectId;
        Name: string;
        Description: string;
        OfficialLinkUrl: string;
        WikiLinkUrl: string;
        Enabled: boolean;
        Order: number;
        Region: number;
        CompleteName: string;
        Type: JahanJooy.RealEstateAgency.Domain.Enums.Vicinity.VicinityType;
        WellKnownScope: JahanJooy.RealEstateAgency.Domain.Enums.Vicinity.VicinityType;
        CanContainPropertyRecords: boolean;
        CenterPoint: MongoDB.Driver.GeoJsonObjectModel.GeoJson2DCoordinates;
        Boundary: MongoDB.Driver.GeoJsonObjectModel.GeoJsonPolygon<MongoDB.Driver.GeoJsonObjectModel.GeoJson2DCoordinates>;
        ParentID: MongoDB.Bson.ObjectId;
    }
    interface Vicinity {
        ID: MongoDB.Bson.ObjectId;
        SqlID: number;
        Name: string;
        AlternativeNames: string;
        AdditionalSearchText: string;
        Description: string;
        OfficialLinkUrl: string;
        WikiLinkUrl: string;
        AdministrativeNotes: string;
        Enabled: boolean;
        Order: number;
        Type: JahanJooy.RealEstateAgency.Domain.Enums.Vicinity.VicinityType;
        WellKnownScope: JahanJooy.RealEstateAgency.Domain.Enums.Vicinity.VicinityType;
        ShowTypeInTitle: boolean;
        ShowInHierarchy: boolean;
        ShowInSummary: boolean;
        CanContainPropertyRecords: boolean;
        CenterPoint: MongoDB.Driver.GeoJsonObjectModel.GeoJson2DCoordinates;
        Boundary: MongoDB.Driver.GeoJsonObjectModel.GeoJsonPolygon<MongoDB.Driver.GeoJsonObjectModel.GeoJson2DCoordinates>;
        BoundaryArea: number;
        Parent: JahanJooy.RealEstateAgency.Domain.Vicinities.Vicinity;
        ParentID: MongoDB.Bson.ObjectId;
        SqlParentID: number;
        Children: JahanJooy.RealEstateAgency.Domain.Vicinities.Vicinity[];
    }
}
declare module JahanJooy.RealEstateAgency.Domain.Supply {
    interface SupplyReference {
        ID: MongoDB.Bson.ObjectId;
        CreationTime: Date;
        CreatedByID: MongoDB.Bson.ObjectId;
        ExpirationTime: Date;
        IntentionOfOwner: JahanJooy.RealEstateAgency.Domain.Enums.Property.IntentionOfOwner;
        State: JahanJooy.RealEstateAgency.Domain.Workflows.SupplyState;
        IsArchived: boolean;
        IsPublic: boolean;
        PriceSpecificationType: JahanJooy.RealEstateAgency.Domain.Enums.Property.SalePriceSpecificationType;
        TotalPrice: number;
        PricePerEstateArea: number;
        PricePerUnitArea: number;
        Mortgage: number;
        Rent: number;
        HasTransferableLoan: boolean;
        TransferableLoanAmount: number;
        MortgageAndRentConvertible: boolean;
        AdditionalRentalComments: string;
        MinimumMortgage: number;
        MinimumRent: number;
        ContactToOwner: boolean;
        ContactToAgency: boolean;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.Users {
    interface ApplicationUserDetails extends JahanJooy.RealEstateAgency.Util.Models.Users.ApplicationUserSummary {
        ModificationTime: Date;
        DeletionTime: Date;
        LastLogin: Date;
        LastLoginAttempt: Date;
        FailedLoginAttempts: number;
        About: string;
        WebSiteUrl: string;
        Contact: JahanJooy.RealEstateAgency.Util.Models.Base.ContactMethodCollectionSummary;
    }
    interface ApplicationUserSummary {
        Id: string;
        Code: number;
        IsOperator: boolean;
        UserName: string;
        IsEnabled: boolean;
        ModificationTime: Date;
        LicenseType: JahanJooy.RealEstateAgency.Domain.Enums.User.LicenseType;
        LicenseActivationTime: Date;
        DisplayName: string;
        Type: JahanJooy.RealEstateAgency.Domain.Enums.User.UserType;
        ProfilePicture: JahanJooy.RealEstateAgency.Util.Models.Base.PhotoInfoSummary;
        IsVerified: boolean;
        Roles: string[];
    }
    interface ApproveApplicationUserInput {
        Ids: string[];
    }
    interface NewApplicationUserInput {
        UserName: string;
        Password: string;
        DisplayName: string;
        About: string;
        WebSiteUrl: string;
        Type: JahanJooy.RealEstateAgency.Domain.Enums.User.UserType;
        LicenseType: JahanJooy.RealEstateAgency.Domain.Enums.User.LicenseType;
        LicenseActivationTime: Date;
        ContactInfos: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactInfoInput[];
    }
    interface NewSecretForUserApplicationInput {
        UserID: string;
        ContactMethodID: string;
        ContactMethodType: JahanJooy.RealEstateAgency.Domain.Enums.User.ContactMethodType;
    }
    interface SearchApplicationUserInput {
        UserName: string;
        DisplayName: string;
        Type: JahanJooy.RealEstateAgency.Domain.Enums.User.UserType;
        ContactValues: string;
        InActive: boolean;
        StartIndex: number;
        PageSize: number;
        SortColumn: JahanJooy.RealEstateAgency.Util.Models.Users.ApplicationUserSortColumn;
        SortDirection: JahanJooy.RealEstateAgency.Util.Models.Base.SortDirectionType;
    }
    interface SignupInput {
        UserName: string;
        Password: string;
        ConfirmPassword: string;
        DisplayName: string;
        About: string;
        Address: string;
        WebSiteUrl: string;
        ProfilePictureID: MongoDB.Bson.ObjectId;
        ContactInfos: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactInfoInput[];
    }
    interface UpdateApplicationUserInput {
        ID: string;
        UserName: string;
        DisplayName: string;
        About: string;
        Address: string;
        WebSiteUrl: string;
        Type: JahanJooy.RealEstateAgency.Domain.Enums.User.UserType;
        LicenseType: JahanJooy.RealEstateAgency.Domain.Enums.User.LicenseType;
        LicenseActivationTime: Date;
        ContactInfos: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactInfoInput[];
    }
    interface UserApplicationVerifyContactMethodInput {
        UserID: string;
        ContactMethodID: string;
        VerificationSecret: string;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.Base {
    interface PhotoInfoSummary {
        ID: MongoDB.Bson.ObjectId;
        Title: string;
        Description: string;
        OriginalFileName: string;
        OriginalFileExtension: string;
        ContentType: string;
        CreationTime: Date;
        DeletionTime: Date;
    }
    interface ContactMethodCollectionSummary {
        OrganizationName: string;
        ContactName: string;
        Phones: JahanJooy.RealEstateAgency.Util.Models.Base.PhoneInfoSummary[];
        Emails: JahanJooy.RealEstateAgency.Util.Models.Base.EmailInfoSummary[];
        Addresses: JahanJooy.RealEstateAgency.Util.Models.Base.AddressInfoSummary[];
    }
    interface PhoneInfoSummary {
        ID: MongoDB.Bson.ObjectId;
        NormalizedValue: string;
        CanReceiveSms: boolean;
        IsVerifiable: boolean;
        IsVerified: boolean;
        IsActive: boolean;
        IsDeleted: boolean;
    }
    interface EmailInfoSummary {
        ID: MongoDB.Bson.ObjectId;
        NormalizedValue: string;
        IsVerifiable: boolean;
        IsVerified: boolean;
        IsActive: boolean;
        IsDeleted: boolean;
    }
    interface AddressInfoSummary {
        ID: MongoDB.Bson.ObjectId;
        NormalizedValue: string;
        IsVerifiable: boolean;
        IsVerified: boolean;
        IsActive: boolean;
        IsDeleted: boolean;
    }
    interface NewContactInfoInput {
        UserId: string;
        Value: string;
        Type: JahanJooy.RealEstateAgency.Domain.Enums.User.ContactMethodType;
    }
    interface NewContactMethodCollectionInput {
        OrganizationName: string;
        ContactName: string;
        Phones: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactInfoInput[];
        Emails: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactInfoInput[];
        Addresses: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactInfoInput[];
    }
    interface SupplyContactInfoSummary {
        SupplyID: MongoDB.Bson.ObjectId;
        OwnerCanBeContacted: boolean;
        OwnerContact: JahanJooy.RealEstateAgency.Util.Models.Base.ContactMethodCollectionSummary;
        AgencyContact: JahanJooy.RealEstateAgency.Util.Models.Base.ContactMethodCollectionSummary;
    }
    interface SearchResultSummary {
        ID: MongoDB.Bson.ObjectId;
        DataType: string;
        Title: string;
        DetailState: string;
        DisplayText: string;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.UserActivities {
    interface SearchInput {
        CorrelationId: string;
        HasTargetState: boolean;
        ApplicationType: JahanJooy.RealEstateAgency.Domain.Enums.UserActivity.ApplicationType;
        HasComment: boolean;
        AllActivity: boolean;
        UserId: MongoDB.Bson.ObjectId;
        Controller: string;
        ActionName: string;
        TargetType: JahanJooy.RealEstateAgency.Domain.Enums.UserActivity.EntityType;
        TargetId: MongoDB.Bson.ObjectId;
        TargetState: string;
        Succeeded: boolean;
        ActivityType: JahanJooy.RealEstateAgency.Domain.Enums.UserActivity.UserActivityType;
        ActivitySubType: string;
        ParentType: JahanJooy.RealEstateAgency.Domain.Enums.UserActivity.EntityType;
        ParentId: MongoDB.Bson.ObjectId;
        DetailType: JahanJooy.RealEstateAgency.Domain.Enums.UserActivity.EntityType;
        DetailId: MongoDB.Bson.ObjectId;
        StartIndex: number;
        PageSize: number;
        SortColumn: JahanJooy.RealEstateAgency.Util.Models.UserActivities.UserActivitySortColumn;
        SortDirection: JahanJooy.RealEstateAgency.Util.Models.Base.SortDirectionType;
        FromActivityTime: Date;
        FromActivityTimeMinute: number;
        FromActivityTimeHour: number;
        ToActivityTime: Date;
        ToActivityTimeMinute: number;
        ToActivityTimeHour: number;
    }
    interface SearchOutput {
        UserActivities: JahanJooy.Common.Util.ApiModel.Pagination.PagedListOutput<JahanJooy.RealEstateAgency.Util.Models.UserActivities.UserActivitySummary>;
    }
    interface UserActivitySummary {
        ID: MongoDB.Bson.ObjectId;
        ActivityTime: Date;
        ApplicationType: JahanJooy.RealEstateAgency.Domain.Enums.UserActivity.ApplicationType;
        User: JahanJooy.RealEstateAgency.Domain.Users.ApplicationUserReference;
        ControllerName: string;
        CorrelationID: string;
        ActionName: string;
        Succeeded: boolean;
        TargetType: JahanJooy.RealEstateAgency.Domain.Enums.UserActivity.EntityType;
        TargetState: string;
        ActivityType: JahanJooy.RealEstateAgency.Domain.Enums.UserActivity.UserActivityType;
        ActivitySubType: string;
        ParentType: JahanJooy.RealEstateAgency.Domain.Enums.UserActivity.EntityType;
        DetailType: JahanJooy.RealEstateAgency.Domain.Enums.UserActivity.EntityType;
        Comment: string;
    }
    interface UserActivityDetails extends JahanJooy.RealEstateAgency.Util.Models.UserActivities.UserActivitySummary {
        TargetID: MongoDB.Bson.ObjectId;
        ParentID: MongoDB.Bson.ObjectId;
        DetailID: MongoDB.Bson.ObjectId;
        RelativeActivities: JahanJooy.RealEstateAgency.Util.Models.UserActivities.UserActivitySummary[];
    }
}
declare module JahanJooy.Common.Util.ApiModel.Pagination {
    interface PagedListOutput<T> {
        PageItems: T[];
        PageNumber: number;
        TotalNumberOfPages: number;
        TotalNumberOfItems: number;
    }
}
declare module JahanJooy.RealEstateAgency.Domain.Users {
    interface ApplicationUserReference {
        Id: string;
        UserName: string;
        DisplayName: string;
    }
    interface ApplicationUser extends AspNet.Identity.MongoDB.IdentityUser {
        Code: number;
        IsOperator: boolean;
        IsEnabled: boolean;
        IsVerified: boolean;
        CreationTime: Date;
        CreatedByID: MongoDB.Bson.ObjectId;
        ModificationTime: Date;
        DeletionTime: Date;
        LastIndexingTime: Date;
        LicenseType: JahanJooy.RealEstateAgency.Domain.Enums.User.LicenseType;
        LicenseActivationTime: Date;
        LastLogin: Date;
        LastLoginAttempt: Date;
        FailedLoginAttempts: number;
        DisplayName: string;
        About: string;
        WebSiteUrl: string;
        Type: JahanJooy.RealEstateAgency.Domain.Enums.User.UserType;
        AgencyID: number;
        ProfilePicture: JahanJooy.RealEstateAgency.Domain.Base.PhotoInfo;
        Contact: JahanJooy.RealEstateAgency.Domain.Users.ContactMethodCollection;
        GeneralPermissions: JahanJooy.RealEstateAgency.Domain.Enums.User.UserGeneralPermission[];
        GeneralPermissionsValue: string;
    }
    interface ContactMethodCollection {
        OrganizationName: string;
        ContactName: string;
        Phones: JahanJooy.RealEstateAgency.Domain.Base.PhoneInfo[];
        Emails: JahanJooy.RealEstateAgency.Domain.Base.EmailInfo[];
        Addresses: JahanJooy.RealEstateAgency.Domain.Base.AddressInfo[];
    }
    interface UserContactMethodVerification {
        StartTime: Date;
        ExpirationTime: Date;
        CompletionTime: Date;
        VerificationSecret: string;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.Supplies {
    interface CompleteSupplyInput {
        ID: string;
        Reason: JahanJooy.RealEstateAgency.Domain.Workflows.SupplyCompletionReason;
    }
    interface NewSupplyInput {
        IntentionOfOwner: JahanJooy.RealEstateAgency.Domain.Enums.Property.IntentionOfOwner;
        PriceSpecificationType: JahanJooy.RealEstateAgency.Domain.Enums.Property.SalePriceSpecificationType;
        TotalPrice: number;
        PricePerEstateArea: number;
        PricePerUnitArea: number;
        HasTransferableLoan: boolean;
        TransferableLoanAmount: number;
        AdditionalRentalComments: string;
        Mortgage: number;
        Rent: number;
        MortgageAndRentConvertible: boolean;
        MinimumMortgage: number;
        MinimumRent: number;
        PropertyId: MongoDB.Bson.ObjectId;
        SwapAdditionalComments: string;
        IntentionOfCustomer: JahanJooy.RealEstateAgency.Domain.Enums.Request.IntentionOfCustomer;
        PropertyTypes: number[];
        RequestUsageType: JahanJooy.RealEstateAgency.Domain.Enums.Property.UsageType;
        ExpirationTime: Date;
        Vicinities: MongoDB.Bson.ObjectId[];
        Description: string;
        RequestEstateArea: number;
        RequestUnitArea: number;
        RequestOfficeArea: number;
        RequestTotalPrice: number;
        RequestMortgage: number;
        RequestRent: number;
        RequestMortgageAndRentConvertible: boolean;
    }
    interface PrintSuppliesInput {
        ReportTemplateID: string;
        Format: string;
        SearchInput: JahanJooy.RealEstateAgency.Util.Models.Supplies.SearchFileInput;
        Ids: string[];
    }
    interface SearchFileInput {
        UsageType: JahanJooy.RealEstateAgency.Domain.Enums.Property.UsageType;
        PropertyType: JahanJooy.RealEstateAgency.Domain.Enums.Property.PropertyType;
        IntentionOfOwner: JahanJooy.RealEstateAgency.Domain.Enums.Property.IntentionOfOwner;
        State: JahanJooy.RealEstateAgency.Domain.Workflows.SupplyState;
        SourceType: JahanJooy.RealEstateAgency.Domain.Enums.Property.SourceType;
        IsArchived: boolean;
        HasPhoto: boolean;
        IsHidden: boolean;
        IsPublic: boolean;
        HasWarning: boolean;
        EstateAreaMin: number;
        EstateAreaMax: number;
        UnitAreaMin: number;
        UnitAreaMax: number;
        NumberOfRoomsMin: number;
        NumberOfRoomsMax: number;
        MortgageMin: number;
        MortgageMax: number;
        RentMin: number;
        RentMax: number;
        PricePerEstateAreaMin: number;
        PricePerEstateAreaMax: number;
        PricePerUnitAreaMin: number;
        PricePerUnitAreaMax: number;
        PriceMin: number;
        PriceMax: number;
        Vicinity: JahanJooy.RealEstateAgency.Domain.Vicinities.Vicinity;
        VicinityID: MongoDB.Bson.ObjectId;
        Bounds: JahanJooy.Common.Util.Spatial.LatLngBounds;
        StartIndex: number;
        PageSize: number;
        SortColumn: JahanJooy.RealEstateAgency.Util.Models.Supplies.SupplySortColumn;
        SortDirection: JahanJooy.RealEstateAgency.Util.Models.Base.SortDirectionType;
    }
    interface PrintSupplyInput {
        ReportTemplateID: string;
        Format: string;
    }
    interface PublishSupplyInput {
        ID: MongoDB.Bson.ObjectId;
        ExpirationTime: Date;
        OwnerCanBeContacted: boolean;
        AgencyContact: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactMethodCollectionInput;
    }
    interface SupplyDetails extends JahanJooy.RealEstateAgency.Util.Models.Supplies.SupplySummary {
        HasTransferableLoan: boolean;
        TransferableLoanAmount: number;
        MortgageAndRentConvertible: boolean;
        AdditionalRentalComments: string;
        MinimumMortgage: number;
        MinimumRent: number;
        PropertyDetail: JahanJooy.RealEstateAgency.Util.Models.Properties.PropertyDetails;
        Request: JahanJooy.RealEstateAgency.Util.Models.Requests.RequestDetails;
        SwapAdditionalComments: string;
    }
    interface SupplySummary {
        ID: MongoDB.Bson.ObjectId;
        IntentionOfOwner: JahanJooy.RealEstateAgency.Domain.Enums.Property.IntentionOfOwner;
        State: JahanJooy.RealEstateAgency.Domain.Workflows.SupplyState;
        CreationTime: Date;
        ExpirationTime: Date;
        LastModificationTime: Date;
        DeletionTime: Date;
        IsArchived: boolean;
        IsPublic: boolean;
        PriceSpecificationType: JahanJooy.RealEstateAgency.Domain.Enums.Property.SalePriceSpecificationType;
        TotalPrice: number;
        PricePerEstateArea: number;
        PricePerUnitArea: number;
        Mortgage: number;
        Rent: number;
        Property: JahanJooy.RealEstateAgency.Util.Models.Properties.PropertySummary;
        CreatedByID: MongoDB.Bson.ObjectId;
        CreatorFullName: string;
        LastFetchTime: Date;
        ContactToOwner: boolean;
        ContactToAgency: boolean;
    }
    interface UpdateSupplyInput {
        ID: MongoDB.Bson.ObjectId;
        IntentionOfOwner: JahanJooy.RealEstateAgency.Domain.Enums.Property.IntentionOfOwner;
        PriceSpecificationType: JahanJooy.RealEstateAgency.Domain.Enums.Property.SalePriceSpecificationType;
        TotalPrice: number;
        PricePerEstateArea: number;
        PricePerUnitArea: number;
        HasTransferableLoan: boolean;
        TransferableLoanAmount: number;
        Mortgage: number;
        Rent: number;
        MortgageAndRentConvertible: boolean;
        MinimumMortgage: number;
        MinimumRent: number;
        AdditionalRentalComments: string;
        ExpirationTime: Date;
        OwnerCanBeContacted: boolean;
        OwnerContact: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactMethodCollectionInput;
        AgencyContact: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactMethodCollectionInput;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.Properties {
    interface PropertySummary {
        ID: MongoDB.Bson.ObjectId;
        PropertyType: JahanJooy.RealEstateAgency.Domain.Enums.Property.PropertyType;
        State: JahanJooy.RealEstateAgency.Domain.Workflows.PropertyState;
        IsArchived: boolean;
        CreationTime: Date;
        CreatedByID: MongoDB.Bson.ObjectId;
        LastModificationTime: Date;
        DeletionTime: Date;
        CoverImageID: MongoDB.Bson.ObjectId;
        Address: string;
        Vicinity: JahanJooy.RealEstateAgency.Domain.Vicinities.VicinityReference;
        GeographicLocation: MongoDB.Driver.GeoJsonObjectModel.GeoJson2DCoordinates;
        EstateArea: number;
        UnitArea: number;
        UsageType: JahanJooy.RealEstateAgency.Domain.Enums.Property.UsageType;
        SourceType: JahanJooy.RealEstateAgency.Domain.Enums.Property.SourceType;
        LastFetchTime: Date;
        IsHidden: boolean;
        ConversionWarning: boolean;
        NumberOfRooms: number;
        Supplies: JahanJooy.RealEstateAgency.Util.Models.Supplies.SupplySummary[];
    }
    interface PropertyDetails extends JahanJooy.RealEstateAgency.Util.Models.Properties.PropertySummary {
        Photos: JahanJooy.RealEstateAgency.Util.Models.Base.PhotoInfoSummary[];
        LicencePlate: string;
        IsAgencyListing: boolean;
        IsAgencyActivityAllowed: boolean;
        PropertyStatus: JahanJooy.RealEstateAgency.Domain.Enums.Property.PropertyStatus;
        EstateDirection: JahanJooy.RealEstateAgency.Domain.Enums.Property.EstateDirection;
        PassageEdgeLength: number;
        EstateVoucherType: JahanJooy.RealEstateAgency.Domain.Enums.Property.EstateVoucherType;
        ExternalDetails: string;
        TotalNumberOfUnits: number;
        BuildingAgeYears: number;
        NumberOfUnitsPerFloor: number;
        TotalNumberOfFloors: number;
        OfficeArea: number;
        CeilingHeight: number;
        StorageRoomArea: number;
        NumberOfParkings: number;
        UnitFloorNumber: number;
        AdditionalSpecialFeatures: string;
        NumberOfMasterBedrooms: number;
        KitchenCabinetType: JahanJooy.RealEstateAgency.Domain.Enums.Property.KitchenCabinetType;
        MainDaylightDirection: JahanJooy.RealEstateAgency.Domain.Enums.Property.DaylightDirection;
        LivingRoomFloor: JahanJooy.RealEstateAgency.Domain.Enums.Property.FloorCoverType;
        FaceType: JahanJooy.RealEstateAgency.Domain.Enums.Property.BuildingFaceType;
        IsDuplex: boolean;
        HasIranianLavatory: boolean;
        HasForeignLavatory: boolean;
        HasPrivatePatio: boolean;
        HasBeenReconstructed: boolean;
        HasElevator: boolean;
        HasGatheringHall: boolean;
        HasAutomaticParkingDoor: boolean;
        HasVideoEyePhone: boolean;
        HasSwimmingPool: boolean;
        HasSauna: boolean;
        HasJacuzzi: boolean;
        HasTransferableLoan: boolean;
        TransferableLoanAmount: number;
        MortgageAndRentConvertible: boolean;
        AdditionalRentalComments: string;
        MinimumMortgage: number;
        MinimumRent: number;
        CorrelationID: System.Guid;
        IsMasterBuiling: boolean;
        RelatedProperties: JahanJooy.RealEstateAgency.Util.Models.Properties.PropertyDetails[];
    }
    interface NewPropertyInput {
        PropertyType: JahanJooy.RealEstateAgency.Domain.Enums.Property.PropertyType;
        IntentionOfOwner: JahanJooy.RealEstateAgency.Domain.Enums.Property.IntentionOfOwner;
        Address: string;
        LicencePlate: string;
        VicinityID: MongoDB.Bson.ObjectId;
        GeographicLocation: JahanJooy.Common.Util.Spatial.LatLng;
        PropertyEstateArea: number;
        EstateDirection: JahanJooy.RealEstateAgency.Domain.Enums.Property.EstateDirection;
        PassageEdgeLength: number;
        EstateVoucherType: JahanJooy.RealEstateAgency.Domain.Enums.Property.EstateVoucherType;
        TotalNumberOfUnits: number;
        BuildingAgeYears: number;
        NumberOfUnitsPerFloor: number;
        TotalNumberOfFloors: number;
        PropertyUnitArea: number;
        PropertyOfficeArea: number;
        CeilingHeight: number;
        StorageRoomArea: number;
        PropertyUsageType: JahanJooy.RealEstateAgency.Domain.Enums.Property.UsageType;
        NumberOfRooms: number;
        NumberOfParkings: number;
        UnitFloorNumber: number;
        AdditionalSpecialFeatures: string;
        NumberOfMasterBedrooms: number;
        KitchenCabinetType: JahanJooy.RealEstateAgency.Domain.Enums.Property.KitchenCabinetType;
        MainDaylightDirection: JahanJooy.RealEstateAgency.Domain.Enums.Property.DaylightDirection;
        LivingRoomFloor: JahanJooy.RealEstateAgency.Domain.Enums.Property.FloorCoverType;
        FaceType: JahanJooy.RealEstateAgency.Domain.Enums.Property.BuildingFaceType;
        IsDuplex: boolean;
        HasIranianLavatory: boolean;
        HasForeignLavatory: boolean;
        HasPrivatePatio: boolean;
        HasBeenReconstructed: boolean;
        HasElevator: boolean;
        HasGatheringHall: boolean;
        HasAutomaticParkingDoor: boolean;
        HasVideoEyePhone: boolean;
        HasSwimmingPool: boolean;
        HasSauna: boolean;
        HasJacuzzi: boolean;
        PriceSpecificationType: JahanJooy.RealEstateAgency.Domain.Enums.Property.SalePriceSpecificationType;
        PropertyTotalPrice: number;
        PricePerEstateArea: number;
        PricePerUnitArea: number;
        HasTransferableLoan: boolean;
        TransferableLoanAmount: number;
        PropertyMortgage: number;
        PropertyRent: number;
        PropertyMortgageAndRentConvertible: boolean;
        MinimumMortgage: number;
        MinimumRent: number;
        Owner: JahanJooy.RealEstateAgency.Util.Models.Customers.NewCustomerInput;
        OwnerCanBeContacted: boolean;
        ContactInfos: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactMethodCollectionInput;
        SwapAdditionalComments: string;
        IntentionOfCustomer: JahanJooy.RealEstateAgency.Domain.Enums.Request.IntentionOfCustomer;
        PropertyTypes: number[];
        RequestUsageType: JahanJooy.RealEstateAgency.Domain.Enums.Property.UsageType;
        ExpirationTime: Date;
        Vicinities: MongoDB.Bson.ObjectId[];
        Description: string;
        RequestEstateArea: number;
        RequestUnitArea: number;
        RequestOfficeArea: number;
        RequestTotalPrice: number;
        RequestMortgage: number;
        RequestRent: number;
        RequestMortgageAndRentConvertible: boolean;
    }
    interface PrintPropertiesInput {
        ReportTemplateID: string;
        Format: string;
        SearchInput: JahanJooy.RealEstateAgency.Util.Models.Properties.SearchPropertyInput;
        Ids: string[];
    }
    interface SearchPropertyInput {
        Address: string;
        UsageType: JahanJooy.RealEstateAgency.Domain.Enums.Property.UsageType;
        PropertyType: JahanJooy.RealEstateAgency.Domain.Enums.Property.PropertyType;
        IntentionOfOwner: JahanJooy.RealEstateAgency.Domain.Enums.Property.IntentionOfOwner;
        State: JahanJooy.RealEstateAgency.Domain.Workflows.PropertyState;
        IsArchived: boolean;
        EstateAreaMin: number;
        EstateAreaMax: number;
        UnitAreaMin: number;
        UnitAreaMax: number;
        NumberOfRoomsMin: number;
        NumberOfRoomsMax: number;
        MortgageMin: number;
        MortgageMax: number;
        RentMin: number;
        RentMax: number;
        PricePerEstateAreaMin: number;
        PricePerEstateAreaMax: number;
        PricePerUnitAreaMin: number;
        PricePerUnitAreaMax: number;
        PriceMin: number;
        PriceMax: number;
        OwnerName: string;
        CreationTime: Date;
        StartIndex: number;
        PageSize: number;
        SortColumn: JahanJooy.RealEstateAgency.Util.Models.Properties.PropertySortColumn;
        SortDirection: JahanJooy.RealEstateAgency.Util.Models.Base.SortDirectionType;
    }
    interface PrintPropertyInput {
        ReportTemplateID: string;
        Format: string;
    }
    interface PropertyContactInfoSummaryForPublic {
        ContactInfos: JahanJooy.RealEstateAgency.Util.Models.Base.SupplyContactInfoSummary[];
    }
    interface PropertyContactInfoSummary {
        ContactInfos: JahanJooy.RealEstateAgency.Util.Models.Base.SupplyContactInfoSummary[];
        Owner: JahanJooy.RealEstateAgency.Util.Models.Customers.CustomerSummary;
    }
    interface PublishPropertyInput {
        ID: MongoDB.Bson.ObjectId;
        ExpirationTime: Date;
        OwnerCanBeContacted: boolean;
        AgencyContact: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactMethodCollectionInput;
    }
    interface UpdatePropertyInput {
        ID: MongoDB.Bson.ObjectId;
        PropertyType: JahanJooy.RealEstateAgency.Domain.Enums.Property.PropertyType;
        UsageType: JahanJooy.RealEstateAgency.Domain.Enums.Property.UsageType;
        Address: string;
        Vicinity: JahanJooy.RealEstateAgency.Domain.Vicinities.VicinityReference;
        GeographicLocation: JahanJooy.Common.Util.Spatial.LatLng;
        EstateArea: number;
        EstateDirection: JahanJooy.RealEstateAgency.Domain.Enums.Property.EstateDirection;
        PassageEdgeLength: number;
        EstateVoucherType: JahanJooy.RealEstateAgency.Domain.Enums.Property.EstateVoucherType;
        LicencePlate: string;
        TotalNumberOfUnits: number;
        BuildingAgeYears: number;
        NumberOfUnitsPerFloor: number;
        TotalNumberOfFloors: number;
        UnitArea: number;
        OfficeArea: number;
        CeilingHeight: number;
        StorageRoomArea: number;
        NumberOfRooms: number;
        NumberOfParkings: number;
        UnitFloorNumber: number;
        AdditionalSpecialFeatures: string;
        NumberOfMasterBedrooms: number;
        KitchenCabinetType: JahanJooy.RealEstateAgency.Domain.Enums.Property.KitchenCabinetType;
        MainDaylightDirection: JahanJooy.RealEstateAgency.Domain.Enums.Property.DaylightDirection;
        LivingRoomFloor: JahanJooy.RealEstateAgency.Domain.Enums.Property.FloorCoverType;
        FaceType: JahanJooy.RealEstateAgency.Domain.Enums.Property.BuildingFaceType;
        ConversionWarning: boolean;
        IsHidden: boolean;
        IsDuplex: boolean;
        HasIranianLavatory: boolean;
        HasForeignLavatory: boolean;
        HasPrivatePatio: boolean;
        HasBeenReconstructed: boolean;
        HasElevator: boolean;
        HasGatheringHall: boolean;
        HasAutomaticParkingDoor: boolean;
        HasVideoEyePhone: boolean;
        HasSwimmingPool: boolean;
        HasSauna: boolean;
        HasJacuzzi: boolean;
        Owner: JahanJooy.RealEstateAgency.Util.Models.Customers.NewCustomerInput;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.Requests {
    interface RequestDetails extends JahanJooy.RealEstateAgency.Util.Models.Requests.RequestSummary {
        IsArchived: boolean;
        EstateArea: number;
        EstateVoucherType: JahanJooy.RealEstateAgency.Domain.Enums.Property.EstateVoucherType;
        Description: string;
        SelectedVicinities: JahanJooy.RealEstateAgency.Util.Models.Vicinities.VicinitySummary[];
        Supply: JahanJooy.RealEstateAgency.Util.Models.Supplies.SupplySummary;
        BuildingAgeYears: number;
        TotalNumberOfUnits: number;
        NumberOfUnitsPerFloor: number;
        TotalNumberOfFloors: number;
        UnitArea: number;
        OfficeArea: number;
        CeilingHeight: number;
        NumberOfRooms: number;
        NumberOfParkings: number;
        UnitFloorNumber: number;
        IsDublex: boolean;
        HasBeenReconstructed: boolean;
        HasIranianLavatory: boolean;
        HasForeignLavatory: boolean;
        HasPrivatePatio: boolean;
        HasMasterBedroom: boolean;
        HasElevator: boolean;
        HasGatheringHall: boolean;
        HasSwimmingPool: boolean;
        HasStorageRoom: boolean;
        HasAutomaticParkingDoor: boolean;
        HasVideoEyePhone: boolean;
        HasSauna: boolean;
        HasJacuzzi: boolean;
        MortgageAndRentConvertible: boolean;
        ContactToOwner: boolean;
        ContactToAgency: boolean;
    }
    interface RequestSummary {
        ID: MongoDB.Bson.ObjectId;
        IntentionOfCustomer: JahanJooy.RealEstateAgency.Domain.Enums.Request.IntentionOfCustomer;
        UsageType: JahanJooy.RealEstateAgency.Domain.Enums.Property.UsageType;
        State: JahanJooy.RealEstateAgency.Domain.Workflows.RequestState;
        SourceType: JahanJooy.RealEstateAgency.Domain.Enums.Property.SourceType;
        PropertyTypes: number[];
        IsPublic: boolean;
        CreationTime: Date;
        LastModificationTime: Date;
        ExpirationTime: Date;
        TotalPrice: number;
        Mortgage: number;
        Rent: number;
        CreatedByID: MongoDB.Bson.ObjectId;
        CreatorFullName: string;
        Vicinities: string[];
    }
    interface NewRequestInput {
        IntentionOfCustomer: JahanJooy.RealEstateAgency.Domain.Enums.Request.IntentionOfCustomer;
        PropertyTypes: number[];
        UsageType: JahanJooy.RealEstateAgency.Domain.Enums.Property.UsageType;
        ExpirationTime: Date;
        Vicinities: MongoDB.Bson.ObjectId[];
        Description: string;
        EstateArea: number;
        EstateVoucherType: JahanJooy.RealEstateAgency.Domain.Enums.Property.EstateVoucherType;
        BuildingAgeYears: number;
        TotalNumberOfUnits: number;
        NumberOfUnitsPerFloor: number;
        TotalNumberOfFloors: number;
        UnitArea: number;
        OfficeArea: number;
        CeilingHeight: number;
        NumberOfRooms: number;
        NumberOfParkings: number;
        UnitFloorNumber: number;
        IsDublex: boolean;
        HasBeenReconstructed: boolean;
        HasIranianLavatory: boolean;
        HasForeignLavatory: boolean;
        HasPrivatePatio: boolean;
        HasMasterBedroom: boolean;
        HasElevator: boolean;
        HasGatheringHall: boolean;
        HasSwimmingPool: boolean;
        HasStorageRoom: boolean;
        HasAutomaticParkingDoor: boolean;
        HasVideoEyePhone: boolean;
        HasSauna: boolean;
        HasJacuzzi: boolean;
        TotalPrice: number;
        Mortgage: number;
        Rent: number;
        MortgageAndRentConvertible: boolean;
        Owner: JahanJooy.RealEstateAgency.Util.Models.Customers.NewCustomerInput;
        OwnerCanBeContacted: boolean;
        ContactInfos: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactMethodCollectionInput;
    }
    interface PrintRequestInput {
        ReportTemplateID: string;
        Format: string;
    }
    interface PrintRequestsInput {
        ReportTemplateID: string;
        Format: string;
        SearchInput: JahanJooy.RealEstateAgency.Util.Models.Requests.SearchRequestInput;
        Ids: string[];
    }
    interface SearchRequestInput {
        IntentionOfCustomer: JahanJooy.RealEstateAgency.Domain.Enums.Request.IntentionOfCustomer;
        State: JahanJooy.RealEstateAgency.Domain.Workflows.RequestState;
        PropertyType: JahanJooy.RealEstateAgency.Domain.Enums.Property.PropertyType;
        IsArchived: boolean;
        IsPublic: boolean;
        Vicinity: JahanJooy.RealEstateAgency.Domain.Vicinities.Vicinity;
        VicinityID: MongoDB.Bson.ObjectId;
        EstateAreaMin: number;
        EstateAreaMax: number;
        UnitAreaMin: number;
        UnitAreaMax: number;
        MortgageMin: number;
        MortgageMax: number;
        RentMin: number;
        RentMax: number;
        PriceMin: number;
        PriceMax: number;
        StartIndex: number;
        PageSize: number;
        SortColumn: JahanJooy.RealEstateAgency.Util.Models.Requests.RequestSortColumn;
        SortDirection: JahanJooy.RealEstateAgency.Util.Models.Base.SortDirectionType;
    }
    interface RequestContactInfoSummaryForPublic {
        RequestID: MongoDB.Bson.ObjectId;
        OwnerCanBeContacted: boolean;
        OwnerContact: JahanJooy.RealEstateAgency.Util.Models.Base.ContactMethodCollectionSummary;
        AgencyContact: JahanJooy.RealEstateAgency.Util.Models.Base.ContactMethodCollectionSummary;
    }
    interface RequestContactInfoSummary {
        RequestID: MongoDB.Bson.ObjectId;
        Owner: JahanJooy.RealEstateAgency.Util.Models.Customers.CustomerSummary;
        OwnerCanBeContacted: boolean;
        OwnerContact: JahanJooy.RealEstateAgency.Util.Models.Base.ContactMethodCollectionSummary;
        AgencyContact: JahanJooy.RealEstateAgency.Util.Models.Base.ContactMethodCollectionSummary;
    }
    interface PublishRequestInput {
        ID: MongoDB.Bson.ObjectId;
        ExpirationTime: Date;
        OwnerCanBeContacted: boolean;
        AgencyContact: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactMethodCollectionInput;
    }
    interface UpdateRequestInput {
        ID: MongoDB.Bson.ObjectId;
        PropertyTypes: number[];
        ExpirationTime: Date;
        OwnerCanBeContacted: boolean;
        OwnerContact: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactMethodCollectionInput;
        AgencyContact: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactMethodCollectionInput;
        Vicinities: MongoDB.Bson.ObjectId[];
        Description: string;
        EstateArea: number;
        EstateVoucherType: JahanJooy.RealEstateAgency.Domain.Enums.Property.EstateVoucherType;
        BuildingAgeYears: number;
        TotalNumberOfUnits: number;
        NumberOfUnitsPerFloor: number;
        TotalNumberOfFloors: number;
        UnitArea: number;
        OfficeArea: number;
        CeilingHeight: number;
        NumberOfRooms: number;
        NumberOfParkings: number;
        UnitFloorNumber: number;
        IsDublex: boolean;
        HasBeenReconstructed: boolean;
        HasIranianLavatory: boolean;
        HasForeignLavatory: boolean;
        HasPrivatePatio: boolean;
        HasMasterBedroom: boolean;
        HasElevator: boolean;
        HasGatheringHall: boolean;
        HasSwimmingPool: boolean;
        HasStorageRoom: boolean;
        HasAutomaticParkingDoor: boolean;
        HasVideoEyePhone: boolean;
        HasSauna: boolean;
        HasJacuzzi: boolean;
        TotalPrice: number;
        HasTransferableLoan: boolean;
        TransferableLoanAmount: number;
        Mortgage: number;
        Rent: number;
        MortgageAndRentConvertible: boolean;
        Owner: JahanJooy.RealEstateAgency.Util.Models.Customers.NewCustomerInput;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.Shared {
    interface ApiValidationResult {
        Success: boolean;
        Errors: JahanJooy.RealEstateAgency.Util.Models.Shared.ApiValidationError[];
    }
    interface ApiValidationError {
        PropertyPath: string;
        ErrorKey: string;
        ErrorParameters: string[];
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.Customers {
    interface NewCustomerInput {
        ID: MongoDB.Bson.ObjectId;
        DisplayName: string;
        Email: string;
        ContactInfos: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactInfoInput[];
        Description: string;
        IsMarried: boolean;
        NameOfFather: string;
        Identification: number;
        IssuedIn: string;
        SocialSecurityNumber: string;
        DateOfBirth: Date;
        Deputy: JahanJooy.RealEstateAgency.Domain.Customers.CustomerReference;
    }
    interface CustomerSummary {
        ID: MongoDB.Bson.ObjectId;
        DisplayName: string;
        PhoneNumber: string;
        Description: string;
        RequestCount: number;
        PropertyCount: number;
        CreatedByID: MongoDB.Bson.ObjectId;
        CreatorFullName: string;
        LastVisitTime: Date;
    }
    interface SearchCustomerInput {
        DisplayName: string;
        IntentionOfVisit: number;
        Email: string;
        PhoneNumber: string;
        IsArchived: boolean;
        IsDeleted: boolean;
        StartIndex: number;
        PageSize: number;
        SortColumn: JahanJooy.RealEstateAgency.Util.Models.Customers.CustomerSortColumn;
        SortDirection: JahanJooy.RealEstateAgency.Util.Models.Base.SortDirectionType;
    }
    interface UpdateCustomerInput {
        ID: MongoDB.Bson.ObjectId;
        DisplayName: string;
        Age: number;
        ContactInfos: JahanJooy.RealEstateAgency.Util.Models.Base.NewContactInfoInput[];
        Description: string;
        RequestCount: number;
        PropertyCount: number;
        IsMarried: boolean;
        NameOfFather: string;
        Identification: number;
        IssuedIn: string;
        SocialSecurityNumber: string;
        DateOfBirth: Date;
        Deputy: JahanJooy.RealEstateAgency.Domain.Customers.CustomerReference;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.Report {
    interface AddParameterInput {
        ID: MongoDB.Bson.ObjectId;
        Parameter: JahanJooy.RealEstateAgency.Domain.MasterData.ReportTemplateParameter;
    }
    interface NewReportTemplateInput {
        DataSourceType: JahanJooy.RealEstateAgency.Domain.MasterData.ReportDataSourceType;
        ApplicationImplementedDataSourceType: JahanJooy.RealEstateAgency.Domain.MasterData.ApplicationImplementedReportDataSourceType;
        Name: string;
        Key: string;
        Description: string;
        Order: number;
        Definition: number[];
        Parameters: JahanJooy.RealEstateAgency.Domain.MasterData.ReportTemplateParameter[];
    }
    interface RemoveParameterInput {
        ReportTemplateID: MongoDB.Bson.ObjectId;
        ParameterID: MongoDB.Bson.ObjectId;
    }
    interface ReportListOutput {
        Templates: JahanJooy.RealEstateAgency.Util.Models.Report.ReportTemplateSummary[];
    }
    interface ReportTemplateSummary {
        ID: MongoDB.Bson.ObjectId;
        DataSourceType: JahanJooy.RealEstateAgency.Domain.MasterData.ReportDataSourceType;
        ApplicationImplementedDataSourceType: JahanJooy.RealEstateAgency.Domain.MasterData.ApplicationImplementedReportDataSourceType;
        Name: string;
        Key: string;
        Description: string;
        Order: number;
        Definition: number[];
        Parameters: JahanJooy.RealEstateAgency.Domain.MasterData.ReportTemplateParameter[];
    }
    interface ReportTemplateParameterOutput {
        ReportTemplateParameterSummaries: JahanJooy.RealEstateAgency.Util.Models.Report.ReportTemplateParameterSummary[];
    }
    interface ReportTemplateParameterSummary {
        ID: MongoDB.Bson.ObjectId;
        ParameterName: string;
        DisplayText: string;
        ParameterType: JahanJooy.RealEstateAgency.Domain.MasterData.ParameterType;
        DefaultValue: string;
        Max: string;
        Min: string;
        Required: boolean;
    }
    interface SearchReportTemplateInput {
        DataSourceType: JahanJooy.RealEstateAgency.Domain.MasterData.ReportDataSourceType;
        ApplicationImplementedDataSourceType: JahanJooy.RealEstateAgency.Domain.MasterData.ApplicationImplementedReportDataSourceType;
        Name: string;
        Key: string;
        Description: string;
        Order: number;
        Definition: number[];
        Parameters: JahanJooy.RealEstateAgency.Domain.MasterData.ReportTemplateParameter[];
    }
    interface SetParametersInput {
        ID: MongoDB.Bson.ObjectId;
        Parameters: JahanJooy.RealEstateAgency.Domain.MasterData.ReportTemplateParameter[];
    }
    interface UpdateReportTemplateInput {
        ID: MongoDB.Bson.ObjectId;
        Name: string;
        Key: string;
        Description: string;
        Definition: number[];
        Order: number;
    }
}
declare module JahanJooy.RealEstateAgency.Domain.MasterData {
    interface ReportTemplateParameter {
        ID: MongoDB.Bson.ObjectId;
        ParameterName: string;
        DisplayText: string;
        ParameterType: JahanJooy.RealEstateAgency.Domain.MasterData.ParameterType;
        DefaultValue: string;
        Max: string;
        Min: string;
        Required: boolean;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.Map {
    interface GeoSearchResult {
        LargestContainedRect: JahanJooy.Common.Util.Spatial.LatLngBounds;
        LargestContainedRectArea: number;
        MinimumDistinguishedArea: number;
        SupplyGroupsSummaries: JahanJooy.RealEstateAgency.Util.Models.Map.GeoSearchVicinityResult[];
        SupplySummaries: JahanJooy.RealEstateAgency.Util.Models.Supplies.SupplySummary[];
        ReachedMaxResult: boolean;
    }
    interface GeoSearchVicinityResult {
        VicinityID: MongoDB.Bson.ObjectId;
        Name: string;
        GeographicLocation: JahanJooy.Common.Util.Spatial.LatLng;
        NumberOfPropertyListings: number;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.Logs {
    interface AppendToDebugAppenderInput {
        Logger: string;
        Append: boolean;
    }
    interface ChangeDebugModeInput {
        Logger: string;
        DebugEnabled: boolean;
    }
    interface GetAllOutput {
        LogFiles: JahanJooy.RealEstateAgency.Util.Models.Logs.LogFilesOutput[];
        Loggers: JahanJooy.RealEstateAgency.Util.Models.Logs.LoggersOutput[];
    }
    interface LogFilesOutput {
        Name: string;
        Size: number;
        CreationTime: Date;
        LastWriteTime: Date;
        CanBeDeleted: boolean;
        CanBeArchived: boolean;
    }
    interface LoggersOutput {
        Name: string;
        Additivity: boolean;
        EffectiveLevel: string;
        IsDebugEnabled: boolean;
        AssociatedToDebugAppender: boolean;
        CanChangeLevelAndAssociation: boolean;
    }
    interface ViewLogInput {
        LogName: string;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.Dashboard {
    interface DashboardOutput {
    }
    interface QuickSearchInput {
        Text: string;
        StartIndex: number;
        PageSize: number;
        DataTypes: JahanJooy.RealEstateAgency.Domain.Enums.UserActivity.EntityType[];
        SortColumn: JahanJooy.RealEstateAgency.Util.Models.Dashboard.DashboardSortColumn;
        SortDirection: JahanJooy.RealEstateAgency.Util.Models.Base.SortDirectionType;
    }
    interface QuickSearchOutput {
        SearchResults: JahanJooy.Common.Util.ApiModel.Pagination.PagedListOutput<JahanJooy.RealEstateAgency.Util.Models.Base.SearchResultSummary>;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.Contracts {
    interface ContractSummary {
        ID: MongoDB.Bson.ObjectId;
        ContractDate: Date;
        State: JahanJooy.RealEstateAgency.Domain.Workflows.ContractState;
        TrackingID: number;
        SupplySummary: JahanJooy.RealEstateAgency.Util.Models.Supplies.SupplySummary;
        PropertySummary: JahanJooy.RealEstateAgency.Util.Models.Properties.PropertySummary;
        TotalPrice: number;
        Mortgage: number;
        Rent: number;
        Seller: JahanJooy.RealEstateAgency.Util.Models.Customers.CustomerSummary;
        Buyer: JahanJooy.RealEstateAgency.Util.Models.Customers.CustomerSummary;
        CreatedByID: MongoDB.Bson.ObjectId;
        CreatorFullName: string;
    }
    interface NewContractInput {
        ContractDate: Date;
        DeliveryDate: Date;
        PropertyID: MongoDB.Bson.ObjectId;
        SupplyID: MongoDB.Bson.ObjectId;
        SellerID: string;
        BuyerID: string;
        RequestID: MongoDB.Bson.ObjectId;
        UsageType: JahanJooy.RealEstateAgency.Domain.Enums.Property.UsageType;
        PropertyType: JahanJooy.RealEstateAgency.Domain.Enums.Property.PropertyType;
        IntentionOfOwner: JahanJooy.RealEstateAgency.Domain.Enums.Property.IntentionOfOwner;
        LicencePlate: string;
        Address: string;
        Portion: string;
        District: string;
        RegistrationZone: string;
        OwnershipEvidenceSerialNumber: string;
        NotaryPublicPageNumber: string;
        NotaryPublic: string;
        PublicSpace: string;
        Description: string;
        ContractTotalPrice: number;
        ContractMortgage: number;
        ContractRent: number;
        ContractEstateArea: number;
        ContractUnitArea: number;
    }
    interface PrintContractInput {
        ReportTemplateID: string;
        Format: string;
    }
    interface PrintContractsInput {
        ReportTemplateID: string;
        Format: string;
        SearchInput: JahanJooy.RealEstateAgency.Util.Models.Contracts.SearchContractInput;
        Ids: string[];
    }
    interface SearchContractInput {
        UsageType: JahanJooy.RealEstateAgency.Domain.Enums.Property.UsageType;
        PropertyType: JahanJooy.RealEstateAgency.Domain.Enums.Property.PropertyType;
        IntentionOfOwner: JahanJooy.RealEstateAgency.Domain.Enums.Property.IntentionOfOwner;
        State: JahanJooy.RealEstateAgency.Domain.Workflows.ContractState;
        FromDate: Date;
        ToDate: Date;
        StartIndex: number;
        PageSize: number;
        SortColumn: JahanJooy.RealEstateAgency.Util.Models.Contracts.ContractSortColumn;
        SortDirection: JahanJooy.RealEstateAgency.Util.Models.Base.SortDirectionType;
    }
    interface UpdateContractInput {
        ID: MongoDB.Bson.ObjectId;
        ContractDate: Date;
        DeliveryDate: Date;
        Portion: string;
        District: string;
        RegistrationZone: string;
        OwnershipEvidenceSerialNumber: string;
        NotaryPublicPageNumber: string;
        NotaryPublic: string;
        PublicSpace: string;
        Description: string;
        ContractTotalPrice: number;
        ContractMortgage: number;
        ContractRent: number;
        ContractEstateArea: number;
        ContractUnitArea: number;
        LastIndexingTime: Date;
        LastModificationTime: Date;
        LastModifiedTimeByID: MongoDB.Bson.ObjectId;
        DeletionTime: Date;
        DeletedByID: MongoDB.Bson.ObjectId;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.ConfigureDataItem {
    interface AddNewInput {
        Identifier: string;
        Value: string;
    }
    interface GetAllOutput {
        Items: JahanJooy.RealEstateAgency.Util.Models.ConfigureDataItem.ConfigureDataItemSummary[];
    }
    interface ConfigureDataItemSummary {
        ID: MongoDB.Bson.ObjectId;
        Identifier: string;
        Value: string;
    }
    interface GetOutput {
        Item: JahanJooy.RealEstateAgency.Util.Models.ConfigureDataItem.ConfigureDataItemSummary;
    }
    interface UpdateInput {
        ID: MongoDB.Bson.ObjectId;
        Identifier: string;
        Value: string;
    }
}
declare module JahanJooy.RealEstateAgency.Util.Models.Account {
    interface CompeleteRegistrationOutput {
        User: JahanJooy.RealEstateAgency.Domain.Users.ApplicationUser;
        Token: string;
    }
    interface GetAccountPermissionsOutput {
        User: JahanJooy.RealEstateAgency.Util.Models.Users.ApplicationUserSummary;
        Roles: JahanJooy.RealEstateAgency.Domain.Enums.User.BuiltInRole[];
        RoleNames: string[];
    }
}
declare module AspNet.Identity.MongoDB {
    interface IdentityUser {
        Id: string;
        UserName: string;
        SecurityStamp: string;
        Email: string;
        EmailConfirmed: boolean;
        PhoneNumber: string;
        PhoneNumberConfirmed: boolean;
        TwoFactorEnabled: boolean;
        LockoutEndDateUtc: Date;
        LockoutEnabled: boolean;
        AccessFailedCount: number;
        Roles: string[];
        PasswordHash: string;
        Logins: Microsoft.AspNet.Identity.UserLoginInfo[];
        Claims: AspNet.Identity.MongoDB.IdentityUserClaim[];
    }
    interface IdentityUserClaim {
        Type: string;
        Value: string;
    }
}
declare module Microsoft.AspNet.Identity {
    interface UserLoginInfo {
        LoginProvider: string;
        ProviderKey: string;
    }
}
declare module JahanJooy.RealEstateAgency.Domain.Base {
    interface PhotoInfo {
        ID: MongoDB.Bson.ObjectId;
        ExternalID: number;
        Title: string;
        Description: string;
        OriginalFileName: string;
        OriginalFileExtension: string;
        ContentType: string;
        CreationTime: Date;
        DeletionTime: Date;
    }
    interface PhoneInfo extends JahanJooy.RealEstateAgency.Domain.Base.ContactInfo {
        CountryCode: string;
        AreaCode: string;
        CanReceiveSms: boolean;
    }
    interface ContactInfo {
        ID: MongoDB.Bson.ObjectId;
        Value: string;
        NormalizedValue: string;
        IsVerifiable: boolean;
        IsVerified: boolean;
        IsActive: boolean;
        IsDeleted: boolean;
        UserContactMethodVerification: JahanJooy.RealEstateAgency.Domain.Users.UserContactMethodVerification;
    }
    interface EmailInfo extends JahanJooy.RealEstateAgency.Domain.Base.ContactInfo {
    }
    interface AddressInfo extends JahanJooy.RealEstateAgency.Domain.Base.ContactInfo {
    }
}
declare module JahanJooy.RealEstateAgency.Domain.Request {
    interface RequestReference {
        ID: MongoDB.Bson.ObjectId;
        CreationTime: Date;
        ExpirationTime: Date;
        UsageType: JahanJooy.RealEstateAgency.Domain.Enums.Property.UsageType;
        State: JahanJooy.RealEstateAgency.Domain.Workflows.RequestState;
        IsArchived: boolean;
        IsPublic: boolean;
        SourceType: JahanJooy.RealEstateAgency.Domain.Enums.Property.SourceType;
        Vicinities: MongoDB.Bson.ObjectId[];
        PropertyTypes: number[];
        IntentionOfCustomer: JahanJooy.RealEstateAgency.Domain.Enums.Request.IntentionOfCustomer;
        Mortgage: number;
        Rent: number;
        TotalPrice: number;
    }
}
declare module JahanJooy.RealEstateAgency.Domain.Contract {
    interface ContractReference {
        ID: MongoDB.Bson.ObjectId;
        ContractDate: Date;
        TotalPrice: number;
        Mortgage: number;
        Rent: number;
        TrackingID: number;
        PropertyOwner: string;
        RequestOwner: string;
    }
}


