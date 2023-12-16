module JahanJooy.HaftDong.Components {
    import Common = JahanJooy.Common;
    ngModule.filter('PropertyType', () => (input) => {
        return Common.localization.translateEnum("PropertyType", input);
    });
    ngModule.filter('IntentionOfOwner', () => (input) => {
        return Common.localization.translateEnum("IntentionOfOwner", input);
    });
    ngModule.filter('IntentionOfCustomer', () => (input) => {
        return Common.localization.translateEnum("IntentionOfCustomer", input);
    });
    ngModule.filter('EstateVoucherType', () => (input) => {
        return Common.localization.translateEnum("EstateVoucherType", input);
    });
    ngModule.filter('UsageType', () => (input) => {
        return Common.localization.translateEnum("UsageType", input);
    });
    ngModule.filter('BuildingFaceType', () => (input) => {
        return Common.localization.translateEnum("BuildingFaceType", input);
    });
    ngModule.filter('DaylightDirection', () => (input) => {
        return Common.localization.translateEnum("DaylightDirection", input);
    });
    ngModule.filter('EstateDirection', () => (input) => {
        return Common.localization.translateEnum("EstateDirection", input);
    });
    ngModule.filter('FloorCoverType', () => (input) => {
        return Common.localization.translateEnum("FloorCoverType", input);
    });
    ngModule.filter('KitchenCabinetType', () => (input) => {
        return Common.localization.translateEnum("KitchenCabinetType", input);
    });
    ngModule.filter('SalePriceSpecificationType', () => (input) => {
        return Common.localization.translateEnum("SalePriceSpecificationType", input);
    });
    ngModule.filter('PropertyState', () => (input) => {
        return Common.localization.translateEnum("PropertyState", input);
    });
    ngModule.filter('SupplyState', () => (input) => {
        return Common.localization.translateEnum("SupplyState", input);
    });
    ngModule.filter('UserType', () => (input) => {
        return Common.localization.translateEnum("UserType", input);
    });
    ngModule.filter('ContactMethodType', () => (input) => {
        return Common.localization.translateEnum("ContactMethodType", input);
    });
    ngModule.filter('BuiltInRole', () => (input) => {
        return Common.localization.translateEnum("BuiltInRole", input);
    });
    ngModule.filter('EntityType', () => (input) => {
        return Common.localization.translateEnum("EntityType", input);
    });
    ngModule.filter('UserActivityType', () => (input) => {
        return Common.localization.translateEnum("UserActivityType", input);
    });
    ngModule.filter('ContractState', () => (input) => {
        return Common.localization.translateEnum("ContractState", input);
    });
    ngModule.filter('RequestState', () => (input) => {
        return Common.localization.translateEnum("RequestState", input);
    });
    ngModule.filter('VicinityType', () => (input) => {
        return Common.localization.translateEnum("VicinityType", input);
    });
    ngModule.filter('LicenseType', () => (input) => {
        return Common.localization.translateEnum("LicenseType", input);
    });
    ngModule.filter('PropertyStatus', () => (input) => {
        return Common.localization.translateEnum("PropertyStatus", input);
    });
    ngModule.filter('SourceType', () => (input) => {
        return Common.localization.translateEnum("SourceType", input);
    });
    ngModule.filter('PhoneType', () => (input) => {
        return Common.localization.translateEnum("PhoneType", input);
    });

    ngModule.filter('IsHidden', () => (input) => {
        var booleanOptions = [
            { id: true, text: "مخفی" },
            { id: false, text: "" },
            { id: null, text: "" }
        ];

        var result = input;
        booleanOptions.forEach((o) => {
            if (input === o.id)
                result = o.text;
        });

        return result;
    });

    ngModule.filter('Approval', () => (input) => {
        var booleanOptions = [
            { id: true, text: "تائید شده" },
            { id: false, text: "عدم تائید" },
            { id: null, text: "نا مشخص" }
        ];

        var result = input;
        booleanOptions.forEach((o) => {
            if (input === o.id)
                result = o.text;
        });

        return result;
    });

    ngModule.filter('Success', () => (input) => {
        var booleanOptions = [
            { id: true, text: "موفق" },
            { id: false, text: "نا موفق" }
        ];

        var result = input;
        booleanOptions.forEach((o) => {
            if (input === o.id)
                result = o.text;
        });

        return result;
    });

    ngModule.filter('IsEnabled', () => (input) => {
        var booleanOptions = [
            { id: true, text: "فعال" },
            { id: false, text: "غیر فعال" },
            { id: null, text: "نا مشخص" }
        ];

        var result = input;
        booleanOptions.forEach((o) => {
            if (input === o.id)
                result = o.text;
        });

        return result;
    });

    ngModule.filter('IsDeleted', () => (input) => {
        var booleanOptions = [
            { id: true, text: "حذف شده" },
            { id: false, text: "حذف نشده" },
            { id: null, text: "نا مشخص" }
        ];

        var result = input;
        booleanOptions.forEach((o) => {
            if (input === o.id)
                result = o.text;
        });

        return result;
    });

    ngModule.filter('Boolean', () => (input) => {
        var booleanOptions = [
            { id: true, text: "بله" },
            { id: false, text: "خیر" }
        ];

        var result = input;
        booleanOptions.forEach((o) => {
            if (input === o.id)
                result = o.text;
        });

        return result;
    });
}