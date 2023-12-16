module JahanJooy.HaftDong.File {
    import SalePriceSpecificationType = RealEstateAgency.Domain.Enums.Property.SalePriceSpecificationType;
    import EnumExtentions = Common.EnumExtentions;
    import PropertyType = RealEstateAgency.Domain.Enums.Property.PropertyType;
    appModule.directive('jjSupply',
    () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/userApplication/views/Supplies/supplies.directive.modal.html',
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
                allPriceSpecificationTypePerProperty) => {

                $scope.inEditMode = !($scope.input.ID == null);

                $scope.TotalCalculatedPrice = 0;
                $scope.isCollapsed = true;
                $scope.isFirstTime = true;
                $scope.swapType = "Other";

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

                $scope.$watch("input",
                    s => {
                        if (s != null) {
                            s.OwnerCanBeContacted = s.ContactToOwner;

                            if (s.AgencyContact != null) {
                                s.AgencyContact.Phones.forEach(p => {
                                    p.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone;
                                });
                                s.AgencyContact.Emails.forEach(e => {
                                    e.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Email;
                                });
                                s.AgencyContact.Addresses.forEach(a => {
                                    a.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Address;
                                });
                            }

                            if (s.OwnerContact != null) {
                                s.OwnerContact.Phones.forEach(p => {
                                    p.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone;
                                });
                                s.OwnerContact.Emails.forEach(e => {
                                    e.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Email;
                                });
                                s.OwnerContact.Addresses.forEach(a => {
                                    a.Type = RealEstateAgency.Domain.Enums.User.ContactMethodType.Address;
                                });
                            }
                        };
                    });

                $scope.$watch("input.Property.PropertyType",
                    p => {
                        if ($scope.input != null) {
                            var allSalePriceSpecificationTypes =
                                EnumExtentions.getValues(SalePriceSpecificationType)
                                    .select(i => {
                                        return {
                                            id: i,
                                            text: Common.localization.translateEnum("SalePriceSpecificationType", i)
                                        }
                                    })
                                    .toArray();
                            $scope
                                .priceSpecificationTypeOptions =
                                Enumerable.from(allSalePriceSpecificationTypes)
                                .where(pr => Enumerable.from(allPriceSpecificationTypePerProperty)
                                    .any(a => a.propertyType === p &&
                                        Enumerable.from(a.priceTypes).any(pt => pt === pr.id)))
                                .toArray();
                            if (!$scope.inEditMode) {
                                $scope.input.PriceSpecificationType = "";
                            }
                        }
                    });

                $scope.$watch("input.IntentionOfOwner",
                    p => {
                        if ($scope.input != null) {
                            if (p != null && p === 2) {
                                $scope.showRentPanel = false;
                                $scope.showSalePanel = true;
                                $scope.showDailyRentPanel = false;
                                $scope.showSwapPanel = false;
                            } else if (p != null && p === 3) {
                                $scope.showSalePanel = false;
                                $scope.showRentPanel = true;
                                $scope.showDailyRentPanel = false;
                                $scope.showSwapPanel = false;
                            } else if (p != null && p === 1) {
                                $scope.showRentPanel = true;
                                $scope.showSalePanel = false;
                                $scope.showDailyRentPanel = false;
                                $scope.showSwapPanel = false;
                            } else if (p != null && p === 4) {
                                $scope.showRentPanel = false;
                                $scope.showDailyRentPanel = true;
                                $scope.showSalePanel = false;
                                $scope.showSwapPanel = false;
                            } else if (p != null && p === 5) {
                                $scope.showSalePanel = true;
                                $scope.showRentPanel = false;
                                $scope.showDailyRentPanel = false;
                                $scope.showSwapPanel = true;

                                $scope.showSalePanelRequest = true;
                                $scope.input.IntentionOfCustomer = JahanJooy.RealEstateAgency.Domain.Enums.Request
                                    .IntentionOfCustomer.ForSwap;
                            } else if (p != null && p === 6) {
                                $scope.showSalePanel = true;
                                $scope.showRentPanel = false;
                                $scope.showDailyRentPanel = false;
                                $scope.showSwapPanel = false;
                            } else {
                                $scope.showSalePanel = false;
                                $scope.showRentPanel = false;
                                $scope.showDailyRentPanel = false;
                                $scope.showSwapPanel = false;
                            }
                        }
                    });

                $scope.clearPrice = () => {
                    $scope.input.Price = null;
                }

                function calculatePrice() {
                    if ($scope.input.PriceSpecificationType === null) {
                        return 0;
                    }
                    if ($scope.input.PriceSpecificationType === 1) {
                        return $scope.input.Price;
                    } else if ($scope.input.PriceSpecificationType === 2 && $scope.input.Property.EstateArea != null) {
                        return $scope.input.Price * $scope.input.Property.EstateArea;
                    } else if ($scope.input.PriceSpecificationType === 3 && $scope.input.Property.UnitArea != null) {
                        return $scope.input.Price * $scope.input.Property.UnitArea;
                    }
                    return 0;
                }

                $scope.$watchGroup([
                    "input.PriceSpecificationType", "input.Price"
                ],
                () => {
                    if ($scope.input != null)
                        $scope.TotalCalculatedPrice = calculatePrice();
                });

//                $scope.$watch("input.OwnerCanBeContacted",
//                    o => {
//                        if (o == true) {
//                            if ($scope.input.OwnerContact == null) {
//                                $scope.input.AgencyContact = {
//                                    Phones: [
//                                        {
//                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
//                                        }
//                                    ],
//                                    Emails: [
//                                        {
//                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
//                                        }
//                                    ],
//                                    Addresses: [
//                                        {
//                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Address
//                                        }
//                                    ]
//                                };
//                            } else {
//                                if ($scope.input.AgencyContact.Phones == null ||
//                                    $scope.input.AgencyContact.Phones.length === 0) {
//                                    $scope.input.AgencyContact.Phones = [
//                                        {
//                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
//                                        }
//                                    ];
//                                }
//
//                                if ($scope.input.AgencyContact.Emails == null ||
//                                    $scope.input.AgencyContact.Emails.length === 0) {
//                                    $scope.input.AgencyContact.Emails = [
//                                        {
//                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
//                                        }
//                                    ];
//                                }
//
//                                if ($scope.input.AgencyContact.Addresses == null ||
//                                    $scope.input.AgencyContact.Addresses.length === 0) {
//                                    $scope.input.AgencyContact.Addresses = [
//                                        {
//                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Address
//                                        }
//                                    ];
//                                }
//                            }
//                        }
//                        else if (o == false) {
//                            if ($scope.input.AgencyContact == null) {
//                                $scope.input.AgencyContact = {
//                                    Phones: [
//                                        {
//                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
//                                        }
//                                    ],
//                                    Emails: [
//                                        {
//                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
//                                        }
//                                    ],
//                                    Addresses: [
//                                        {
//                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Address
//                                        }
//                                    ]
//                                };
//                            } else {
//                                if ($scope.input.AgencyContact.Phones == null ||
//                                    $scope.input.AgencyContact.Phones.length === 0) {
//                                    $scope.input.AgencyContact.Phones = [
//                                        {
//                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Phone
//                                        }
//                                    ];
//                                }
//
//                                if ($scope.input.AgencyContact.Emails == null ||
//                                    $scope.input.AgencyContact.Emails.length === 0) {
//                                    $scope.input.AgencyContact.Emails = [
//                                        {
//                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Email
//                                        }
//                                    ];
//                                }
//
//                                if ($scope.input.AgencyContact.Addresses == null ||
//                                    $scope.input.AgencyContact.Addresses.length === 0) {
//                                    $scope.input.AgencyContact.Addresses = [
//                                        {
//                                            Type: RealEstateAgency.Domain.Enums.User.ContactMethodType.Address
//                                        }
//                                    ];
//                                }
//                            }
//                        }
//                    });

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
                                            Enumerable.from($scope.allowancesRequest)
                                            .any(a => a.propertyType === n.id &&
                                                Enumerable
                                                .from(a.panels)
                                                .any(at => at === $scope.allRequestPanels.estatePanel));

                                        $scope.showHousePanelRequest =
                                            Enumerable.from($scope.allowancesRequest)
                                            .any(a => a.propertyType === n.id &&
                                                Enumerable
                                                .from(a.panels)
                                                .any(at => at === $scope.allRequestPanels.housePanel));

                                        $scope.showUnitPanelRequest =
                                            Enumerable.from($scope.allowancesRequest)
                                            .any(a => a.propertyType === n.id &&
                                                Enumerable.from(a.panels)
                                                .any(at => at === $scope.allRequestPanels.unitPanel));

                                        $scope.showExtraHousePanelRequest =
                                            Enumerable.from($scope.allowancesRequest)
                                            .any(a => a.propertyType === n.id &&
                                                Enumerable.from(a.panels)
                                                .any(at => at === $scope.allRequestPanels.extraHousePanel));

                                        $scope.showShopPanelRequest =
                                            Enumerable.from($scope.allowancesRequest)
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
                                            .where(u => Enumerable.from($scope.allUsages)
                                                .any(a => a.propertyType === n.id &&
                                                    Enumerable.from(a.usages).any(uu => uu === u.id)))
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
                                .where(p => Enumerable.from(allPropertiesPerUsage)
                                    .any(a => a.usage === u && Enumerable.from(a.properties).any(up => up === p.id)))
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

                $scope.onSave = () => {
                    if ($scope.input.PriceSpecificationType === null) {
                        $scope.input.TotalPrice = 0;
                    } else if ($scope.input.PriceSpecificationType === 1) {
                        $scope.input.TotalPrice = $scope.input.Price;
                    } else if ($scope.input.PriceSpecificationType === 2) {
                        $scope.input.PricePerEstateArea = $scope.input.Price;
                    } else if ($scope.input.PriceSpecificationType === 3) {
                        $scope.input.PricePerUnitArea = $scope.input.Price;
                    }

                    $scope.input.PropertyTypes = Enumerable.from($scope.input.PropertyTypeObjects)
                        .where(pt => pt.isSelected)
                        .select(pt => pt.id)
                        .toArray();
                    $scope.input.Vicinities = Enumerable.from($scope.input.SelectedVicinities)
                        .select(v => v.ID)
                        .toArray();

                    $scope.onSaveClick();
                }
            }
        }
    });
}