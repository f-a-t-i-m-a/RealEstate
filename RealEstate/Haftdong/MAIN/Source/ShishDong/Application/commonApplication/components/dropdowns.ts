module JahanJooy.ShishDong.Components {
    import EnumExtentions = Common.EnumExtentions; 
    import PropertyType = RealEstateAgency.Domain.Enums.Property.PropertyType;
    import VicinityType = RealEstateAgency.Domain.Enums.Vicinity.VicinityType;
    import PropertyState = RealEstateAgency.Domain.Workflows.PropertyState;
    import SupplyState = RealEstateAgency.Domain.Workflows.SupplyState;
    import IntentionOfOwner = RealEstateAgency.Domain.Enums.Property.IntentionOfOwner;
    import IntentionOfCustomer = RealEstateAgency.Domain.Enums.Request.IntentionOfCustomer;
    import BuildingFaceType = RealEstateAgency.Domain.Enums.Property.BuildingFaceType;
    import DaylightDirection = RealEstateAgency.Domain.Enums.Property.DaylightDirection;
    import EstateDirection = RealEstateAgency.Domain.Enums.Property.EstateDirection;
    import EstateVoucherType = RealEstateAgency.Domain.Enums.Property.EstateVoucherType;
    import FloorCoverType = RealEstateAgency.Domain.Enums.Property.FloorCoverType;
    import KitchenCabinetType = RealEstateAgency.Domain.Enums.Property.KitchenCabinetType;
    import SalePriceSpecificationType = RealEstateAgency.Domain.Enums.Property.SalePriceSpecificationType;
    import UsageType = RealEstateAgency.Domain.Enums.Property.UsageType;
    import UserType = RealEstateAgency.Domain.Enums.User.UserType;
    import LicenseType = RealEstateAgency.Domain.Enums.User.LicenseType;
    import EntityType = RealEstateAgency.Domain.Enums.UserActivity.EntityType;
    import UserActivityType = RealEstateAgency.Domain.Enums.UserActivity.UserActivityType;
    import ContractState = RealEstateAgency.Domain.Workflows.ContractState;
    import RequestState = RealEstateAgency.Domain.Workflows.RequestState;
    import SourceType = RealEstateAgency.Domain.Enums.Property.SourceType;

//    ngModule.directive('apsSelectContestGroup', () => {
//        return {
//            restrict: 'E',
//            templateUrl: 'Application/components/tmpl-select-data/contest-group.html',
//            scope: {
//                model: '=',
//                apsRequired: '=',
//                onSelect: '&'
//            },
//            controller: ($scope, $http) => {
//                $http.get("/api/contests/allcontestgroups", null).success((data: AppsOn.Automation.Web.Models.Contests.GetAllContestGroupOutput) => {
//                    $scope.contestGroups = data.ContestGroups;
//                });
//            }
//        };
//    });

    ngModule.directive('jjSelectUserType', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/user-type.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.userTypeOptions = EnumExtentions.getValues(UserType).select(i => { return { id: i, text: Common.localization.translateEnum("UserType", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectLicenseType', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/license-type.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.licenseTypeOptions = EnumExtentions.getValues(LicenseType).select(i => { return { id: i, text: Common.localization.translateEnum("LicenseType", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectPropertyType', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/property-type.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.propertyTypeOptions = EnumExtentions.getValues(PropertyType).select(i => { return { id: i, text: Common.localization.translateEnum("PropertyType", i)}}).toArray();
            }
        };
    });

    ngModule.directive('jjSelectPropertyState', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/property-state.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.propertyStateOptions = EnumExtentions.getValues(PropertyState).select(i => { return { id: i, text: Common.localization.translateEnum("PropertyState", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectRequestState', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/request-state.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.requestStateOptions = EnumExtentions.getValues(RequestState).select(i => { return { id: i, text: Common.localization.translateEnum("RequestState", i) } }).toArray();
            }
        };
    });
    ngModule.directive('jjSelectSupplyState', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/supply-state.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.supplyStateOptions = EnumExtentions.getValues(SupplyState).select(i => { return { id: i, text: Common.localization.translateEnum("SupplyState", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectIntentionOfOwner', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/intention-of-owner.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.intentionOfOwnerOptions = EnumExtentions.getValues(IntentionOfOwner).select(i => { return { id: i, text: Common.localization.translateEnum("IntentionOfOwner", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectIntentionOfCustomer', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/intention-of-customer.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.intentionOfCustomerOptions = EnumExtentions.getValues(IntentionOfCustomer).select(i => { return { id: i, text: Common.localization.translateEnum("IntentionOfCustomer", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectBuildingFaceType', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/building-face-type.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.buildingFaceTypeOptions = EnumExtentions.getValues(BuildingFaceType).select(i => { return { id: i, text: Common.localization.translateEnum("BuildingFaceType", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectDaylightDirection', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/daylight-direction.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.daylightDirectionOptions = EnumExtentions.getValues(DaylightDirection).select(i => { return { id: i, text: Common.localization.translateEnum("DaylightDirection", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectEstateDirection', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/estate-direction.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.estateDirectionOptions = EnumExtentions.getValues(EstateDirection).select(i => { return { id: i, text: Common.localization.translateEnum("EstateDirection", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectEstateVoucherType', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/estate-voucher-type.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.estateVoucherTypeOptions = EnumExtentions.getValues(EstateVoucherType).select(i => { return { id: i, text: Common.localization.translateEnum("EstateVoucherType", i) } }).toArray();
            }
        };
    });
    
    ngModule.directive('jjSelectFloorCoverType', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/floor-cover-type.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.floorCoverTypeOptions = EnumExtentions.getValues(FloorCoverType).select(i => { return { id: i, text: Common.localization.translateEnum("FloorCoverType", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectKitchenCabinetType', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/kitchen-cabinet-type.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.kitchenCabinetTypeOptions = EnumExtentions.getValues(KitchenCabinetType).select(i => { return { id: i, text: Common.localization.translateEnum("KitchenCabinetType", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectSalePriceSpecificationType', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/sale-price-specification-type.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.salePriceSpecificationTypeOptions = EnumExtentions.getValues(SalePriceSpecificationType).select(i => { return { id: i, text: Common.localization.translateEnum("SalePriceSpecificationType", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectUsageType', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/unit-usage-type.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.UsageTypeOptions = EnumExtentions.getValues(UsageType).select(i => { return { id: i, text: Common.localization.translateEnum("UsageType", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjPropertyTypeCheckboxes', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/property-type-checkboxes.html',
            scope: {
                propertyTypeItems: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&'
            },
            controller: $scope => {
                $scope.propertyTypeItems = EnumExtentions.getValues(PropertyType).select(i => { return { id: i, text: Common.localization.translateEnum("PropertyType", i)}}).toArray();
            }
        };
    });

    ngModule.directive('jjSelectContractState', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/contract-state.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.contractStateOptions = EnumExtentions.getValues(ContractState).select(i => { return { id: i, text: Common.localization.translateEnum("ContractState", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectEntityType', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/entity-type.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '='
            },
            controller: $scope => {
                $scope.entityTypeOptions = EnumExtentions.getValues(EntityType).select(i => { return { id: i, text: Common.localization.translateEnum("EntityType", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectUserActivityType', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/user-activity-type.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '='
            },
            controller: $scope => {
                $scope.userActivityTypeOptions = EnumExtentions.getValues(UserActivityType).select(i => { return { id: i, text: Common.localization.translateEnum("UserActivityType", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectVicinityType', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/vicinity-type.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.vicinityTypeOptions = EnumExtentions.getValues(VicinityType).select(i => { return { id: i, text: Common.localization.translateEnum("VicinityType", i) } }).toArray();
            }
        };
    });

    ngModule.directive('jjSelectSourceType', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-enum/source-type.html',
            scope: {
                model: '=',
                jjAllowClear: "=",
                jjRequired: '=',
                onSelect: '&',
                jjDisabled: '='
            },
            controller: $scope => {
                $scope.sourceTypeOptions = EnumExtentions.getValues(SourceType).select(i => { return { id: i, text: Common.localization.translateEnum("SourceType", i) } }).toArray();
            }
        };
    });
}