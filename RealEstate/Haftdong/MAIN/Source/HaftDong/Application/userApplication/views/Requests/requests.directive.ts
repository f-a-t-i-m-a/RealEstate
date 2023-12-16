module JahanJooy.HaftDong.Request {
    import EnumExtentions = Common.EnumExtentions;
    import PropertyType = RealEstateAgency.Domain.Enums.Property.PropertyType;

    appModule.directive('jjRequests', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/userApplication/views/Requests/requests.directive.html',
            scope: {
                request: "=",
                onBackClick: '&',
                onSaveClick: '&'
            },
            controller: ($scope, $http: ng.IHttpService, $state, $modal, messageBoxService, $stateParams, $rootScope, toastr, allPropertiesPerUsage) => {
                if ($scope.request != null) {
                    $scope.inEditMode = !($scope.request.ID == null);
                }
                if ($scope.inEditMode) {
                    $scope.vicinities = $scope.request.SelectedVicinities;
                    $scope.customers = [$scope.request.Owner];
                } else {
                    $scope.vicinities = [];
                }

                var setOwnerFirstTime = true;
                $scope.$watch("request.Owner", ro => {
                    if (setOwnerFirstTime) {
                        if (ro !== null && ro !== undefined && Object.getOwnPropertyNames(ro).length > 0) {
                            $scope.originOwner = JSON.parse(JSON.stringify(ro));
                            setOwnerFirstTime = false;
                        } else {
                            $scope.originOwner = null;
                        }
                    }
                });

                var listener = $rootScope.$on("SetOriginOwner", () => {
                    $scope.request.Owner = $scope.originOwner;
                });

                $scope.$on("$destroy", listener);

                $scope.allPanels = {
                    estatePanel: "showEstatePanel",
                    housePanel: "showHousePanel",
                    unitPanel: "showUnitPanel",
                    industryPanel: "showIndustryPanel",
                    extraHousePanel: "showHousePanel",
                    shopPanel: "showShopPanel"
                };

                $scope.allowances = [
                    {
                        propertyType: PropertyType.Land,
                        panels: [
                            $scope.allPanels.estatePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.AgriculturalLand,
                        panels: [
                            $scope.allPanels.estatePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Garden,
                        panels: [
                            $scope.allPanels.estatePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.House,
                        panels: [
                            $scope.allPanels.estatePanel,
                            $scope.allPanels.housePanel,
                            $scope.allPanels.unitPanel,
                            $scope.allPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.OldHouse,
                        panels: [
                            $scope.allPanels.estatePanel,
                            $scope.allPanels.housePanel,
                            $scope.allPanels.unitPanel,
                            $scope.allPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Shed,
                        panels: [
                            $scope.allPanels.estatePanel,
                            $scope.allPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Tenement,
                        panels: [
                            $scope.allPanels.estatePanel,
                            $scope.allPanels.housePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Villa,
                        panels: [
                            $scope.allPanels.estatePanel,
                            $scope.allPanels.housePanel,
                            $scope.allPanels.unitPanel,
                            $scope.allPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Apartment,
                        panels: [
                            $scope.allPanels.housePanel,
                            $scope.allPanels.unitPanel,
                            $scope.allPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Complex,
                        panels: [
                            $scope.allPanels.housePanel,
                            $scope.allPanels.unitPanel,
                            $scope.allPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Tower,
                        panels: [
                            $scope.allPanels.housePanel,
                            $scope.allPanels.unitPanel,
                            $scope.allPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.GardenTower,
                        panels: [
                            $scope.allPanels.housePanel,
                            $scope.allPanels.unitPanel,
                            $scope.allPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Office,
                        panels: [
                            $scope.allPanels.housePanel,
                            $scope.allPanels.unitPanel,
                            $scope.allPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.OfficialResidency,
                        panels: [
                            $scope.allPanels.housePanel,
                            $scope.allPanels.unitPanel,
                            $scope.allPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Commercial,
                        panels: [
                            $scope.allPanels.housePanel,
                            $scope.allPanels.unitPanel,
                            $scope.allPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.CommercialResidency,
                        panels: [
                            $scope.allPanels.housePanel,
                            $scope.allPanels.unitPanel,
                            $scope.allPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Suite,
                        panels: [
                            $scope.allPanels.housePanel,
                            $scope.allPanels.unitPanel,
                            $scope.allPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Penthouse,
                        panels: [
                            $scope.allPanels.housePanel,
                            $scope.allPanels.unitPanel,
                            $scope.allPanels.extraHousePanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Shop,
                        panels: [
                            $scope.allPanels.shopPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Factory,
                        panels: [
                            $scope.allPanels.estatePanel,
                            $scope.allPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.WorkShop,
                        panels: [
                            $scope.allPanels.estatePanel,
                            $scope.allPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.RepairShop,
                        panels: [
                            $scope.allPanels.estatePanel,
                            $scope.allPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.StoreHouse,
                        panels: [
                            $scope.allPanels.estatePanel,
                            $scope.allPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Parking,
                        panels: [
                            $scope.allPanels.estatePanel,
                            $scope.allPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.Gym,
                        panels: [
                            $scope.allPanels.estatePanel,
                            $scope.allPanels.industryPanel
                        ]
                    },
                    {
                        propertyType: PropertyType.CityService,
                        panels: [
                            $scope.allPanels.estatePanel,
                            $scope.allPanels.industryPanel
                        ]
                    }
                ];
                
                $scope.UsageTypeOptions = [];

                $scope.isCollapsed = true;

                $scope.selectedPropertyTypeCount = 0;
                $scope.$watch('propertyTypeItems', newValue => {
                    $scope.selectedPropertyTypeCount = 0;
                    if (newValue != null) {
                        newValue.forEach(n => {
                            if (n.isSelected) {
                                $scope.request.PropertyTypeObjects.forEach(pt => {
                                    if (pt.id === n.id) {
                                        pt.isSelected = true;
                                    }
                                });
                                $scope.selectedPropertyTypeCount++;
                                if ($scope.selectedPropertyTypeCount === 1) {
                                    $scope.showEstatePanel = Enumerable.from($scope.allowances).any(a => a.propertyType === n.id
                                        && Enumerable.from(a.panels).any(at => at === $scope.allPanels.estatePanel));

                                    $scope.showHousePanel = Enumerable.from($scope.allowances).any(a => a.propertyType === n.id
                                        && Enumerable.from(a.panels).any(at => at === $scope.allPanels.housePanel));

                                    $scope.showUnitPanel = Enumerable.from($scope.allowances).any(a => a.propertyType === n.id
                                        && Enumerable.from(a.panels).any(at => at === $scope.allPanels.unitPanel));

                                    $scope.showExtraHousePanel = Enumerable.from($scope.allowances).any(a => a.propertyType === n.id
                                        && Enumerable.from(a.panels).any(at => at === $scope.allPanels.extraHousePanel));

                                    $scope.showShopPanel = Enumerable.from($scope.allowances).any(a => a.propertyType === n.id
                                        && Enumerable.from(a.panels).any(at => at === $scope.allPanels.shopPanel));

                                    var allUsageTypes = Common.EnumExtentions.getValues(RealEstateAgency.Domain.Enums.Property.UsageType).select(i => { return { id: i, text: Common.localization.translateEnum("UsageType", i) } }).toArray();
                                    $scope.UsageTypeOptions = Enumerable.from(allUsageTypes).where(u => Enumerable.from($scope.allUsages).any(a => a.propertyType === n.id
                                        && Enumerable.from(a.usages).any(uu => uu === u.id))).toArray();

                                }
                            } else {
                                $scope.request.PropertyTypeObjects.forEach(pt => {
                                    if (pt.id === n.id) {
                                        pt.isSelected = false;
                                    }
                                });
                            }
                        });
                    }
                    if ($scope.selectedPropertyTypeCount === 0 || $scope.selectedPropertyTypeCount > 1) {
                        $scope.UsageTypeOptions = [];
                        $scope.showEstatePanel = false;
                        $scope.showUnitPanel = false;
                        $scope.showHousePanel = false;
                        $scope.showExtraHousePanel = false;
                        $scope.showShopPanel = false;
                    }
                }, true);

                $scope.$watch("request.UsageType", u => {
                    if ($scope.request != null) {
                        var allPropertyTypes = EnumExtentions.getValues(PropertyType).select(i => { return { id: i, text: Common.localization.translateEnum("PropertyType", i) } }).toArray();
                        $scope.propertyTypeItems = Enumerable.from(allPropertyTypes).where(p => Enumerable.from(allPropertiesPerUsage).any(a => a.usage === u
                            && Enumerable.from(a.properties).any(up => up === p.id))).toArray();
                    }
                });

                $scope.$watch("request", r => {
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

                        if (r.Owner != null && r.Owner.ID !== "" && r.Owner.ID != null) {
                            $http.get("/api/web/customers/get/" + r.Owner.ID, null)
                                .success((data: any) => {
                                    $scope.customer = data;
                                    $scope.customers = [$scope.customer];
                                });
                        }
                    }
                });

                $scope.$watch("request.IntentionOfCustomer", r => {
                    if (r != null && r === 2) {
                        $scope.showSalePanel = true;
                        $scope.showFullMortgagePanel = false;
                        $scope.showRentPanel = false;
                        $scope.showDailyRentPanel = false;
                    } else if (r != null && r === 3) {
                        $scope.showFullMortgagePanel = true;
                        $scope.showSalePanel = false;
                        $scope.showRentPanel = false;
                        $scope.showDailyRentPanel = false;
                    } else if (r != null && r === 1) {
                        $scope.showRentPanel = true;
                        $scope.showFullMortgagePanel = false;
                        $scope.showSalePanel = false;
                        $scope.showDailyRentPanel = false;
                    } else if (r != null && r === 4) {
                        $scope.showDailyRentPanel = true;
                        $scope.showRentPanel = false;
                        $scope.showFullMortgagePanel = false;
                        $scope.showSalePanel = false;
                    } else {
                        $scope.showSalePanel = false;
                        $scope.showRentPanel = false;
                        $scope.showFullMortgagePanel = false;
                        $scope.showDailyRentPanel = false;
                    }
                });
               
                $scope.onSave = () => {
                    if ($scope.request.Owner != null && $scope.request.Owner.DisplayName != null
                        && $scope.request.Owner.Number != null) {
                        $scope.request.Owner.Contact = {
                            Phones: [
                                {
                                    Value: $scope.request.Owner.Number,
                                    Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
                                }
                            ]
                        };
                    }

                    if ($scope.request.Contact != null) {
                        $scope.request.OwnerCanBeContacted = $scope.request.Contact.OwnerCanBeContacted;
                        if ($scope.request.Contact.OwnerCanBeContacted === true) {
                            $scope.request.OwnerContact = $scope.request.Contact.OwnerContact;
                        } else if ($scope.request.Contact.OwnerCanBeContacted === false) {
                            $scope.request.AgencyContact = $scope.request.Contact.AgencyContact;
                        }
                    }

                    $scope.request.PropertyTypes = Enumerable.from($scope.request.PropertyTypeObjects).where(pt => pt.isSelected).select(pt => pt.id).toArray();
                    $scope.request.Vicinities = Enumerable.from($scope.request.SelectedVicinities)
                        .select(v => v.ID)
                        .toArray();

                    $scope.onSaveClick();
                }

            }
        }
    });
}