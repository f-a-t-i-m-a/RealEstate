using System;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Enums.Vicinity;
using JahanJooy.RealEstateAgency.Domain.Workflows;

namespace JahanJooy.RealEstateAgency.Util.Resources
{
    public static class StaticEnumResourcesMetadata
    {
        public static readonly Type[] TranslatedEnumTypes =
        {
            typeof (PropertyType),
            typeof (PropertyStatus),
            typeof (PropertyState),
            typeof (SupplyState),
            typeof (IntentionOfOwner),
            typeof (BuildingFaceType),
            typeof (DaylightDirection),
            typeof (EstateDirection),
            typeof (EstateVoucherType),
            typeof (FloorCoverType),
            typeof (KitchenCabinetType),
            typeof (SalePriceSpecificationType),
            typeof (PropertyState),
            typeof (UsageType),
            typeof (IntentionOfCustomer),
            typeof (UserType),
            typeof (BuiltInRole),
            typeof (ContactMethodType),
            typeof (LicenseType),
            typeof (UserActivityType),
            typeof (EntityType),
            typeof (ContractState),
            typeof (RequestState),
            typeof (SupplyCompletionReason),
            typeof (VicinityType),
            typeof (SourceType),
            typeof (ApplicationType)
        };
    }
}