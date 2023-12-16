module JahanJooy.ShishDong.File {
    import PropertyType = RealEstateAgency.Domain.Enums.Property.PropertyType;
    import SalePriceSpecificationType = RealEstateAgency.Domain.Enums.Property.SalePriceSpecificationType;
    import EnumExtentions = Common.EnumExtentions;
    appModule.directive('jjFile',
    () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/userApplication/views/Files/files.directive.html',
            scope: {
                input: "=",
                onBackClick: '&',
                onSaveClick: '&'
            },
            controller: ($scope,
                $http: ng.IHttpService,
                $state,
                $modal,
                $stateParams,
                toastr,
                allPropertiesPerUsage,
                allPropertiesPerIntention,
                allPriceSpecificationTypePerProperty,
                $rootScope) => {

                $scope.swapType = "Other";
                var firstTime = true;
                $scope.inEditMode = !($scope.input == null || $scope.input.ID == null);
                if ($scope.inEditMode) {
                    $scope.vicinities = [$scope.input.Vicinity];
                } else {
                    $scope.vicinities = [];
                    $scope.input.Contact = {};
                }

                var setOwnerFirstTime = true;
                $scope.$watch("input.Owner",
                    po => {
                        if (setOwnerFirstTime) {
                            if (po !== null && po !== undefined && Object.getOwnPropertyNames(po).length > 0) {
                                $scope.originOwner = JSON.parse(JSON.stringify(po));
                                setOwnerFirstTime = false;
                            } else {
                                $scope.originOwner = null;
                            }
                        }
                    });

                var listener = $rootScope.$on("SetOriginOwner",
                () => {
                    $scope.input.Owner = $scope.originOwner;
                });

                $scope.$on("$destroy", listener);

                $scope.allPropertyPanels = {
                    estatePanel: "showEstatePanelProperty",
                    housePanel: "showHousePanelProperty",
                    unitPanel: "showUnitPanelProperty",
                    industryPanel: "showIndustryPanelProperty",
                    extraHousePanel: "showHousePanelProperty",
                    shopPanel: "showShopPanelProperty"
                };

                $scope.allowancesProperty = [
                    {
                        propertyType: PropertyType.Land,
                        panels: [
                            $scope.allPropertyPanels.estatePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.AgriculturalLand,
                        panels: [
                            $scope.allPropertyPanels.estatePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Garden,
                        panels: [
                            $scope.allPropertyPanels.estatePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.House,
                        panels: [
                            $scope.allPropertyPanels.estatePanel,
                            $scope.allPropertyPanels.housePanel,
                            $scope.allPropertyPanels.unitPanel,
                            $scope.allPropertyPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.OldHouse,
                        panels: [
                            $scope.allPropertyPanels.estatePanel,
                            $scope.allPropertyPanels.housePanel,
                            $scope.allPropertyPanels.unitPanel,
                            $scope.allPropertyPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Shed,
                        panels: [
                            $scope.allPropertyPanels.estatePanel,
                            $scope.allPropertyPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Tenement,
                        panels: [
                            $scope.allPropertyPanels.estatePanel,
                            $scope.allPropertyPanels.housePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Villa,
                        panels: [
                            $scope.allPropertyPanels.estatePanel,
                            $scope.allPropertyPanels.housePanel,
                            $scope.allPropertyPanels.unitPanel,
                            $scope.allPropertyPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Apartment,
                        panels: [
                            $scope.allPropertyPanels.housePanel,
                            $scope.allPropertyPanels.unitPanel,
                            $scope.allPropertyPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Complex,
                        panels: [
                            $scope.allPropertyPanels.housePanel,
                            $scope.allPropertyPanels.unitPanel,
                            $scope.allPropertyPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Tower,
                        panels: [
                            $scope.allPropertyPanels.housePanel,
                            $scope.allPropertyPanels.unitPanel,
                            $scope.allPropertyPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.GardenTower,
                        panels: [
                            $scope.allPropertyPanels.housePanel,
                            $scope.allPropertyPanels.unitPanel,
                            $scope.allPropertyPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Office,
                        panels: [
                            $scope.allPropertyPanels.housePanel,
                            $scope.allPropertyPanels.unitPanel,
                            $scope.allPropertyPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.OfficialResidency,
                        panels: [
                            $scope.allPropertyPanels.housePanel,
                            $scope.allPropertyPanels.unitPanel,
                            $scope.allPropertyPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Commercial,
                        panels: [
                            $scope.allPropertyPanels.housePanel,
                            $scope.allPropertyPanels.unitPanel,
                            $scope.allPropertyPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.CommercialResidency,
                        panels: [
                            $scope.allPropertyPanels.housePanel,
                            $scope.allPropertyPanels.unitPanel,
                            $scope.allPropertyPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Suite,
                        panels: [
                            $scope.allPropertyPanels.housePanel,
                            $scope.allPropertyPanels.unitPanel,
                            $scope.allPropertyPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Penthouse,
                        panels: [
                            $scope.allPropertyPanels.housePanel,
                            $scope.allPropertyPanels.unitPanel,
                            $scope.allPropertyPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Shop,
                        panels: [
                            $scope.allPropertyPanels.shopPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Factory,
                        panels: [
                            $scope.allPropertyPanels.estatePanel,
                            $scope.allPropertyPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.WorkShop,
                        panels: [
                            $scope.allPropertyPanels.estatePanel,
                            $scope.allPropertyPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.RepairShop,
                        panels: [
                            $scope.allPropertyPanels.estatePanel,
                            $scope.allPropertyPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.StoreHouse,
                        panels: [
                            $scope.allPropertyPanels.estatePanel,
                            $scope.allPropertyPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Parking,
                        panels: [
                            $scope.allPropertyPanels.estatePanel,
                            $scope.allPropertyPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Gym,
                        panels: [
                            $scope.allPropertyPanels.estatePanel,
                            $scope.allPropertyPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.CityService,
                        panels: [
                            $scope.allPropertyPanels.estatePanel,
                            $scope.allPropertyPanels.industryPanel
                        ]
                    }
                ];

                $scope.allRequestPanels = {
                    estatePanel: "showEstatePanelRequest",
                    housePanel: "showHousePanelRequest",
                    unitPanel: "showUnitPanelRequest",
                    industryPanel: "showIndustryPanelRequest",
                    extraHousePanel: "showHousePanelRequest",
                    shopPanel: "showShopPanelRequest"
                };

                $scope.allowancesRequest = [
                    {
                        propertyType: PropertyType.Land,
                        panels: [
                            $scope.allRequestPanels.estatePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.AgriculturalLand,
                        panels: [
                            $scope.allRequestPanels.estatePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Garden,
                        panels: [
                            $scope.allRequestPanels.estatePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.House,
                        panels: [
                            $scope.allRequestPanels.estatePanel,
                            $scope.allRequestPanels.housePanel,
                            $scope.allRequestPanels.unitPanel,
                            $scope.allRequestPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.OldHouse,
                        panels: [
                            $scope.allRequestPanels.estatePanel,
                            $scope.allRequestPanels.housePanel,
                            $scope.allRequestPanels.unitPanel,
                            $scope.allRequestPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Shed,
                        panels: [
                            $scope.allRequestPanels.estatePanel,
                            $scope.allRequestPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Tenement,
                        panels: [
                            $scope.allRequestPanels.estatePanel,
                            $scope.allRequestPanels.housePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Villa,
                        panels: [
                            $scope.allRequestPanels.estatePanel,
                            $scope.allRequestPanels.housePanel,
                            $scope.allRequestPanels.unitPanel,
                            $scope.allRequestPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Apartment,
                        panels: [
                            $scope.allRequestPanels.housePanel,
                            $scope.allRequestPanels.unitPanel,
                            $scope.allRequestPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Complex,
                        panels: [
                            $scope.allRequestPanels.housePanel,
                            $scope.allRequestPanels.unitPanel,
                            $scope.allRequestPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Tower,
                        panels: [
                            $scope.allRequestPanels.housePanel,
                            $scope.allRequestPanels.unitPanel,
                            $scope.allRequestPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.GardenTower,
                        panels: [
                            $scope.allRequestPanels.housePanel,
                            $scope.allRequestPanels.unitPanel,
                            $scope.allRequestPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Office,
                        panels: [
                            $scope.allRequestPanels.housePanel,
                            $scope.allRequestPanels.unitPanel,
                            $scope.allRequestPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.OfficialResidency,
                        panels: [
                            $scope.allRequestPanels.housePanel,
                            $scope.allRequestPanels.unitPanel,
                            $scope.allRequestPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Commercial,
                        panels: [
                            $scope.allRequestPanels.housePanel,
                            $scope.allRequestPanels.unitPanel,
                            $scope.allRequestPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.CommercialResidency,
                        panels: [
                            $scope.allRequestPanels.housePanel,
                            $scope.allRequestPanels.unitPanel,
                            $scope.allRequestPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Suite,
                        panels: [
                            $scope.allRequestPanels.housePanel,
                            $scope.allRequestPanels.unitPanel,
                            $scope.allRequestPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Penthouse,
                        panels: [
                            $scope.allRequestPanels.housePanel,
                            $scope.allRequestPanels.unitPanel,
                            $scope.allRequestPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Shop,
                        panels: [
                            $scope.allRequestPanels.shopPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Factory,
                        panels: [
                            $scope.allRequestPanels.estatePanel,
                            $scope.allRequestPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.WorkShop,
                        panels: [
                            $scope.allRequestPanels.estatePanel,
                            $scope.allRequestPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.RepairShop,
                        panels: [
                            $scope.allRequestPanels.estatePanel,
                            $scope.allRequestPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.StoreHouse,
                        panels: [
                            $scope.allRequestPanels.estatePanel,
                            $scope.allRequestPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Parking,
                        panels: [
                            $scope.allRequestPanels.estatePanel,
                            $scope.allRequestPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Gym,
                        panels: [
                            $scope.allRequestPanels.estatePanel,
                            $scope.allRequestPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.CityService,
                        panels: [
                            $scope.allRequestPanels.estatePanel,
                            $scope.allRequestPanels.industryPanel
                        ]
                    }
                ];

                $scope.UsageTypeOptions = [];

                $scope.TotalCalculatedPrice = 0;
                $scope.isCollapsed = true;

                $scope.$watch("input.PropertyType",
                    p => {
                        if ($scope.input != null) {
                            if ($scope.input.IntentionOfOwner != null && $scope.input.IntentionOfOwner === 6) {
                                $scope.showCooperationPanel = true;
                                if (p != null && p === 103) {
                                    $scope.showUnitAreaPanel = true;
                                } else {
                                    $scope.showUnitAreaPanel = false;
                                }
                                $scope.showEstatePanelProperty = false;
                                $scope.showHousePanelProperty = false;
                                $scope.showUnitPanelProperty = false;
                                $scope.showShedPanelProperty = false;
                                $scope.showExtraHousePanelProperty = false;
                                $scope.showShopPanelProperty = false;
                            } else {
                                $scope.showCooperationPanel = false;


                                $scope.showEstatePanelProperty = Enumerable.from<any>($scope.allowancesProperty)
                                    .any(a => a.propertyType === p &&
                                        Enumerable.from(a.panels).any(at => at === $scope.allPropertyPanels.estatePanel));

                                $scope.showHousePanelProperty = Enumerable.from<any>($scope.allowancesProperty)
                                    .any(a => a.propertyType === p &&
                                        Enumerable.from(a.panels).any(at => at === $scope.allPropertyPanels.housePanel));

                                $scope.showUnitPanelProperty = Enumerable.from<any>($scope.allowancesProperty)
                                    .any(a => a.propertyType === p &&
                                        Enumerable.from(a.panels).any(at => at === $scope.allPropertyPanels.unitPanel));

                                $scope.showShedPanelProperty = Enumerable.from<any>($scope.allowancesProperty)
                                    .any(a => a.propertyType === p &&
                                        Enumerable.from(a.panels).any(at => at === $scope.allPropertyPanels.shedPanel));

                                $scope.showExtraHousePanelProperty = Enumerable.from<any>($scope.allowancesProperty)
                                    .any(a => a.propertyType === p &&
                                        Enumerable.from(a.panels).any(at => at === $scope.allPropertyPanels.extraHousePanel));

                                $scope.showShopPanelProperty = Enumerable.from<any>($scope.allowancesProperty)
                                    .any(a => a.propertyType === p &&
                                        Enumerable.from(a.panels).any(at => at === $scope.allPropertyPanels.shopPanel));
                            }
                            var allSalePriceSpecificationTypes =
                                EnumExtentions.getValues(SalePriceSpecificationType)
                                    .select(i => {
                                        return {
                                            id: i,
                                            text: Common.localization.translateEnum("SalePriceSpecificationType", i)
                                        }
                                    })
                                    .toArray();
                            $scope.priceSpecificationTypeOptions =
                                Enumerable.from<any>(allSalePriceSpecificationTypes)
                                .where(pr => Enumerable.from<any>(allPriceSpecificationTypePerProperty)
                                    .any(a => a.propertyType === p &&
                                        Enumerable.from<any>(a.priceTypes).any(pt => pt === pr.id)))
                                .toArray();
                            $scope.input.PriceSpecificationType = "";
                        }
                    });

                $scope.$watch("input.UsageType",
                    u => {
                        if ($scope.input != null) {
                            if (!firstTime) {
                                $scope.input.PropertyType = "";
                            }
                            firstTime = false;
                            var usagePropertyTypes = {};
                            if (u != null) {
                                usagePropertyTypes = Enumerable.from<any>(allPropertiesPerUsage)
                                    .singleOrDefault(m => m.usage === u)
                                    .properties;
                            }

                            var intentionPropertyTypes;
                            if ($scope.input.IntentionOfOwner != null) {
                                intentionPropertyTypes = Enumerable.from<any>(allPropertiesPerIntention)
                                    .singleOrDefault(m => m.intention === $scope.input.IntentionOfOwner)
                                    .properties;
                            } else {
                                intentionPropertyTypes = usagePropertyTypes;
                            }

                            var viablePropertyTypes;
                            if (u != null) {
                                viablePropertyTypes = Enumerable.from(usagePropertyTypes)
                                    .intersect(intentionPropertyTypes);
                            } else {
                                viablePropertyTypes = Enumerable.from(intentionPropertyTypes);
                            }

                            $scope.propertyTypeOptions = viablePropertyTypes.select(i => {
                                return { id: i, text: Common.localization.translateEnum("PropertyType", i) }
                            })
                                .toArray();
                        }
                    });

                $scope.$watch("input.IntentionOfOwner",
                    p => {
                        if ($scope.input != null) {
                            if (p != null && p === 2) {
                                $scope.showRentPanelProperty = false;
                                $scope.showSalePanelProperty = true;
                                $scope.showDailyRentPanelProperty = false;
                                $scope.showSwapPanel = false;
                                $scope.showCooperationPanel = false;
                            } else if (p != null && p === 3) {
                                $scope.showSalePanelProperty = false;
                                $scope.showRentPanelProperty = true;
                                $scope.showDailyRentPanelProperty = false;
                                $scope.showSwapPanel = false;
                                $scope.showCooperationPanel = false;
                            } else if (p != null && p === 1) {
                                $scope.showRentPanelProperty = true;
                                $scope.showSalePanelProperty = false;
                                $scope.showDailyRentPanelProperty = false;
                                $scope.showSwapPanel = false;
                                $scope.showCooperationPanel = false;
                            } else if (p != null && p === 4) {
                                $scope.showRentPanelProperty = false;
                                $scope.showDailyRentPanelProperty = true;
                                $scope.showSalePanelProperty = false;
                                $scope.showSwapPanel = false;
                                $scope.showCooperationPanel = false;
                            } else if (p != null && p === 5) {
                                $scope.showSalePanelProperty = true;
                                $scope.showRentPanelProperty = false;
                                $scope.showDailyRentPanelProperty = false;
                                $scope.showSwapPanel = true;
                                $scope.showCooperationPanel = false;

                                $scope.showSalePanelRequest = true;
                                $scope.input.IntentionOfCustomer = JahanJooy.RealEstateAgency.Domain.Enums.Request
                                    .IntentionOfCustomer.ForSwap;
                            } else if (p != null && p === 6) {
                                $scope.showSalePanelProperty = true;
                                $scope.showRentPanelProperty = false;
                                $scope.showDailyRentPanelProperty = false;
                                $scope.showSwapPanel = false;
                                $scope.showCooperationPanel = true;
                            } else {
                                $scope.showSalePanelProperty = false;
                                $scope.showRentPanelProperty = false;
                                $scope.showDailyRentPanelProperty = false;
                                $scope.showSwapPanel = false;
                                $scope.showCooperationPanel = false;
                            }

                            var intentionPropertyTypes = {};
                            if (p != null) {
                                intentionPropertyTypes = Enumerable.from<any>(allPropertiesPerIntention)
                                    .singleOrDefault(m => m.intention === p)
                                    .properties;
                            }

                            var usagePropertyTypes;
                            if ($scope.input.UsageType != null) {
                                usagePropertyTypes = Enumerable.from<any>(allPropertiesPerUsage)
                                    .singleOrDefault(m => m.usage === $scope.input.UsageType)
                                    .properties;
                            } else {
                                usagePropertyTypes = intentionPropertyTypes;
                            }

                            var viablePropertyTypes;
                            if (p != null) {
                                viablePropertyTypes = Enumerable.from(intentionPropertyTypes)
                                    .intersect(usagePropertyTypes);
                            } else {
                                viablePropertyTypes = Enumerable.from(usagePropertyTypes);
                            }

                            $scope.propertyTypeOptions = viablePropertyTypes.select(i => {
                                return { id: i, text: Common.localization.translateEnum("PropertyType", i) }
                            })
                                .toArray();
                        }
                    });

                $scope.clearPrice = () => {
                    $scope.input.PropertyPrice = null;
                }

                function calculatePrice() {
                    if ($scope.input.PriceSpecificationType === null) {
                        return 0;
                    }
                    if ($scope.input.PriceSpecificationType === 1) {
                        return $scope.input.Price;
                    } else if ($scope.input.PriceSpecificationType === 2 && $scope.input.EstateArea != null) {
                        return $scope.input.Price * $scope.input.EstateArea;
                    } else if ($scope.input.PriceSpecificationType === 3 && $scope.input.UnitArea != null) {
                        return $scope.input.Price * $scope.input.UnitArea;
                    }
                    return 0;
                }

                $scope.$watchGroup([
                    "input.PriceSpecificationType", "input.Price",
                    "input.UnitArea", "input.EstateArea"
                ],
                () => {
                    if ($scope.input != null)
                        $scope.TotalCalculatedPrice = calculatePrice();
                });


                $scope.$watch("swapType",
                    st => {
                        if (st != null) {
                            if (st !== "Other") {
                                $scope.input.SwapAdditionalComments = "";
                            }
                        }
                    });

                $scope.selectedPropertyTypeCount = 0;
                $scope.$watch('propertyTypeItems',
                    newValue => {
                        $scope.selectedPropertyTypeCount = 0;
                        if (newValue != null) {
                            newValue.forEach(n => {
                                if (n.isSelected) {
                                    $scope.input.PropertyTypeObjects.forEach(pt => {
                                        if (pt.id === n.id) {
                                            pt.isSelected = true;
                                        }
                                    });
                                    $scope.selectedPropertyTypeCount++;
                                    if ($scope.selectedPropertyTypeCount === 1) {
                                        $scope.showEstatePanelRequest =
                                        Enumerable.from<any>($scope.allowancesRequest)
                                            .any(a => a.propertyType === n.id &&
                                                Enumerable
                                                    .from(a.panels)
                                                    .any(at => at === $scope.allRequestPanels.estatePanel));

                                        $scope.showHousePanelRequest =
                                        Enumerable.from<any>($scope.allowancesRequest)
                                            .any(a => a.propertyType === n.id &&
                                                Enumerable
                                                    .from(a.panels)
                                                    .any(at => at === $scope.allRequestPanels.housePanel));

                                        $scope.showUnitPanelRequest =
                                        Enumerable.from<any>($scope.allowancesRequest)
                                            .any(a => a.propertyType === n.id &&
                                                Enumerable.from(a.panels)
                                                    .any(at => at === $scope.allRequestPanels.unitPanel));

                                        $scope.showExtraHousePanelRequest =
                                        Enumerable.from<any>($scope.allowancesRequest)
                                            .any(a => a.propertyType === n.id &&
                                                Enumerable.from(a.panels)
                                                    .any(at => at === $scope.allRequestPanels.extraHousePanel));

                                        $scope.showShopPanelRequest =
                                        Enumerable.from<any>($scope.allowancesRequest)
                                            .any(a => a.propertyType === n.id &&
                                                Enumerable.from(a.panels)
                                                    .any(at => at === $scope.allRequestPanels.shopPanel));

                                        var allUsageTypes =
                                            Common.EnumExtentions
                                                .getValues(RealEstateAgency.Domain.Enums.Property.UsageType)
                                                .select(i => {
                                                    return {
                                                        id: i,
                                                        text: Common.localization.translateEnum("UsageType", i)
                                                    }
                                                })
                                                .toArray();

                                        $scope.UsageTypeOptions =
                                        Enumerable.from(allUsageTypes)
                                            .where(u => Enumerable.from<any>($scope.allUsages)
                                                .any(a => a.propertyType === n.id &&
                                                    Enumerable.from<any>(a.usages).any(uu => uu === u.id)))
                                            .toArray();

                                    }
                                } else {
                                    $scope.input.PropertyTypeObjects.forEach(pt => {
                                        if (pt.id === n.id) {
                                            pt.isSelected = false;
                                        }
                                    });
                                }
                            });
                        }
                        if ($scope.selectedPropertyTypeCount === 0 || $scope.selectedPropertyTypeCount > 1) {
                            $scope.UsageTypeOptions = [];
                            $scope.showEstatePanelRequest = false;
                            $scope.showUnitPanelRequest = false;
                            $scope.showHousePanelRequest = false;
                            $scope.showExtraHousePanelRequest = false;
                            $scope.showShopPanelRequest = false;
                        }
                    },
                    true);

                $scope.$watch("input.RequestUsageType",
                    u => {
                        if ($scope.input != null) {
                            var allPropertyTypes = EnumExtentions.getValues(PropertyType)
                                .select(i => {
                                    return { id: i, text: Common.localization.translateEnum("PropertyType", i) }
                                })
                                .toArray();
                            $scope.propertyTypeItems = Enumerable.from(allPropertyTypes)
                                .where(p => Enumerable.from<any>(allPropertiesPerUsage)
                                    .any(a => a.usage === u && Enumerable.from<any>(a.properties).any(up => up === p.id)))
                                .toArray();
                        }
                    });

                $scope.$watch("input",
                    r => {
                        if (r != null) {
                            r.PropertyTypeObjects = EnumExtentions.getValues(PropertyType)
                                .select(i => { return { id: i, isSelected: false } })
                                .toArray();

                            if (r.PropertyTypes != null && r.PropertyTypes.length !== 0) {
                                $scope.propertyTypeItems.forEach(item => {
                                    if (Enumerable.from(r.PropertyTypes).any(p => p === item.id)) {
                                        item.isSelected = true;
                                    }
                                });
                            };
                        }
                    });
                $scope.onSave = (showDetail) => {
                    if ($scope.input.PriceSpecificationType === null) {
                        $scope.input.TotalPrice = 0;
                    } else if ($scope.input.PriceSpecificationType === 1) {
                        $scope.input.TotalPrice = $scope.input.Price;
                    } else if ($scope.input.PriceSpecificationType === 2) {
                        $scope.input.PricePerEstateArea = $scope.input.Price;
                    } else if ($scope.input.PriceSpecificationType === 3) {
                        $scope.input.PricePerUnitArea = $scope.input.Price;
                    }

                    if (!$scope.inEditMode) {
                        if ($scope.input.Contact === null || Object.getOwnPropertyNames($scope.input.Contact).length <= 0) {
                            $scope.input.ContactInfos = null;
                        } else {
                            $scope.input.OwnerCanBeContacted = $scope.input.Contact.OwnerCanBeContacted;
                            if ($scope.input.Contact.OwnerCanBeContacted === true) {
                                $scope.input.ContactInfos = $scope.input.Contact.OwnerContact;
                            } else if ($scope.input.Contact.OwnerCanBeContacted === false) {
                                $scope.input.ContactInfos = $scope.input.Contact.AgencyContact;
                            }
                        }
                    }

                    $scope.input.PropertyTypes = Enumerable.from<any>($scope.input.PropertyTypeObjects)
                        .where(pt => pt.isSelected)
                        .select(pt => pt.id)
                        .toArray();
                    $scope.input.Vicinities = Enumerable.from<any>($scope.input.SelectedVicinities)
                        .select(v => v.ID)
                        .toArray();

                    $scope.onSaveClick({ showDetail: showDetail });
                }
            }
        }
    });
}