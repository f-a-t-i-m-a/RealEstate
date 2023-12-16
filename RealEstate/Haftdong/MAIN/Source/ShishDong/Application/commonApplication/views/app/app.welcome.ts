module JahanJooy.ShishDong {
    import EnumExtentions = Common.EnumExtentions;
    import PropertyType = RealEstateAgency.Domain.Enums.Property.PropertyType;
    import UsageType = RealEstateAgency.Domain.Enums.Property.UsageType;
    import IntentionOfOwner = RealEstateAgency.Domain.Enums.Property.IntentionOfOwner;
    import IntentionOfCustomer = RealEstateAgency.Domain.Enums.Request.IntentionOfCustomer;
    import SalePriceSpecificationType = RealEstateAgency.Domain.Enums.Property.SalePriceSpecificationType;

    appModule.controller("WelcomeController",
    [
        '$scope', '$timeout', '$state', 'authService', "$http", "$rootScope", "$q",
        "allEstateAreaRanges", "allUnitAreaRanges", "allNumberOfRoomRanges", "allMortgageRanges",
        "allRentRanges", "allPriceRanges", "allPropertiesPerUsage", "allPropertiesPerIntention", "allDailyRentRanges",
        "allPricePerEstateAreaRanges", "allPricePerUnitAreaRanges", "allPriceSpecificationTypePerProperty", "toastr",
        ($scope,
            $timeout,
            $state: angular.ui.IStateService,
            authService,
            $http,
            $rootScope,
            $q,
            allEstateAreaRanges,
            allUnitAreaRanges,
            allNumberOfRoomRanges,
            allMortgageRanges,
            allRentRanges,
            allPriceRanges,
            allPropertiesPerUsage,
            allPropertiesPerIntention,
            allDailyRentRanges,
            allPricePerEstateAreaRanges,
            allPricePerUnitAreaRanges,
            allPriceSpecificationTypePerProperty,
            toastr) => {

            var firstTime = true;

//            $timeout(() => {
//                if (authService.isAnonymous()) {
//                    authService.showLogin().then(() => {
//                        location.reload();
//                        $state.go("dashboard");
//                    }
//                )} else {
//                    $state.go("dashboard");
//                }
//            }, 500);


            //request Start

            $scope.$watch("request.UsageType",
                u => {
                    if ($scope.request != null) {
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

            $scope.$watch("request.IntentionOfCustomer",
                r => {
                    if (r != null && r === 2) {
                        $scope.showRequestBuyPanel = true;
                        $scope.showRequestFullMortgagePanel = false;
                        $scope.showRequestRentPanel = false;
                        $scope.showRequestDailyRentPanel = false;
                    } else if (r != null && r === 3) {
                        $scope.showRequestFullMortgagePanel = true;
                        $scope.showRequestBuyPanel = false;
                        $scope.showRequestRentPanel = false;
                        $scope.showRequestDailyRentPanel = false;
                    } else if (r != null && r === 1) {
                        $scope.showRequestRentPanel = true;
                        $scope.showRequestFullMortgagePanel = false;
                        $scope.showRequestBuyPanel = false;
                        $scope.showRequestDailyRentPanel = false;
                    } else if (r != null && r === 4) {
                        $scope.showRequestDailyRentPanel = true;
                        $scope.showRequestRentPanel = false;
                        $scope.showRequestFullMortgagePanel = false;
                        $scope.showRequestBuyPanel = false;
                    } else if (r != null && r === 5) {
                        $scope.showRequestDailyRentPanel = false;
                        $scope.showRequestRentPanel = false;
                        $scope.showRequestFullMortgagePanel = false;
                        $scope.showRequestBuyPanel = false;
                    } else if (r != null && r === 6) {
                        $scope.showRequestDailyRentPanel = false;
                        $scope.showRequestRentPanel = false;
                        $scope.showRequestFullMortgagePanel = false;
                        $scope.showRequestBuyPanel = false;
                    } else {
                        $scope.showRequestBuyPanel = false;
                        $scope.showRequestRentPanel = false;
                        $scope.showRequestFullMortgagePanel = false;
                        $scope.showRequestDailyRentPanel = false;
                    }
                });

            $scope.$watch('propertyTypeItems',
                newValue => {
                    $scope.selectedPropertyTypeCount = 0;
                    if (newValue != null) {
                        newValue.forEach(n => {
                            if (n.isSelected) {
                                $scope.request.PropertyTypes.forEach(pt => {
                                    if (pt.id === n.id) {
                                        pt.isSelected = true;
                                    }
                                });
                            } else {
                                $scope.request.PropertyTypes.forEach(pt => {
                                    if (pt.id === n.id) {
                                        pt.isSelected = false;
                                    }
                                });
                            }
                        });
                    }
                },
                true);

            $scope.$watch("request",
                r => {
                    r.PropertyTypes = EnumExtentions.getValues(PropertyType)
                        .select(i => { return { id: i, isSelected: false } })
                        .toArray();
                });
            //request End

            $scope.showSecondTab = "all";
            $scope.showPropertySecondTab = "propertysale";
            $scope.showFirstTab = "search";
            $scope.searchInput = {};
            $scope.property = {};
            $scope.request = {};
            $scope.searchInput.IntentionOfOwner = null;
            $scope.property.IntentionOfOwner = IntentionOfOwner.ForSale;
            $scope.request.IntentionOfCustomer = IntentionOfCustomer.ForBuy;

            $scope.onFirstTabClick = (tabName) => {
                $scope.showFirstTab = tabName;
                Enumerable.from(document.getElementsByName("firstTab")).forEach(el => el.removeAttribute("class"));
                document.getElementById(tabName).setAttribute("class", "active");
            }

            $scope.onSecondTabClick = (tabName) => {
                $scope.showSecondTab = tabName;
                Enumerable.from(document.getElementsByName("secondTab")).forEach(el => el.removeAttribute("class"));
                document.getElementById(tabName).setAttribute("class", "active");

                switch (tabName) {
                case "all":
                    $scope.searchInput.IntentionOfOwner = null;
                    break;
                case "sale":
                    $scope.searchInput.IntentionOfOwner = IntentionOfOwner.ForSale;
                    break;
                case "rent":
                    $scope.searchInput.IntentionOfOwner = IntentionOfOwner.ForRent;
                    break;
                case "fullmortgage":
                    $scope.searchInput.IntentionOfOwner = IntentionOfOwner.ForFullMortgage;
                    break;
                case "dailyrent":
                    $scope.searchInput.IntentionOfOwner = IntentionOfOwner.ForDailyRent;
                    break;
                case "exchange":
                    $scope.searchInput.IntentionOfOwner = IntentionOfOwner.ForSwap;
                    break;
                case "cooperate":
                    $scope.searchInput.IntentionOfOwner = IntentionOfOwner.ForCooperation;
                    break;
                }
            }

            $scope.onPropertySecondTabClick = (tabName) => {
                $scope.showSecondTab = tabName;
                Enumerable.from(document.getElementsByName("propertysecondTab"))
                    .forEach(el => el.removeAttribute("class"));
                document.getElementById(tabName).setAttribute("class", "active");

                switch (tabName) {
                case "propertysale":
                    $scope.property.IntentionOfOwner = IntentionOfOwner.ForSale;
                    break;
                case "propertyrent":
                    $scope.property.IntentionOfOwner = IntentionOfOwner.ForRent;
                    break;
                case "propertyfullmortgage":
                    $scope.property.IntentionOfOwner = IntentionOfOwner.ForFullMortgage;
                    break;
                case "propertydailyrent":
                    $scope.property.IntentionOfOwner = IntentionOfOwner.ForDailyRent;
                    break;
                case "propertyexchange":
                    $scope.property.IntentionOfOwner = IntentionOfOwner.ForSwap;
                    break;
                case "propertycooperate":
                    $scope.property.IntentionOfOwner = IntentionOfOwner.ForCooperation;
                    break;
                }
            }

            $scope.onRequestSecondTabClick = (tabName) => {
                $scope.showSecondTab = tabName;
                Enumerable.from(document.getElementsByName("requestsecondTab"))
                    .forEach(el => el.removeAttribute("class"));
                document.getElementById(tabName).setAttribute("class", "active");

                switch (tabName) {
                case "requestbuy":
                    $scope.request.IntentionOfCustomer = IntentionOfCustomer.ForBuy;
                    break;
                case "requestrent":
                    $scope.request.IntentionOfCustomer = IntentionOfCustomer.ForRent;
                    break;
                case "requestfullmortgage":
                    $scope.request.IntentionOfCustomer = IntentionOfCustomer.ForFullMortgage;
                    break;
                case "requestdailyrent":
                    $scope.request.IntentionOfCustomer = IntentionOfCustomer.ForDailyRent;
                    break;
                case "requestexchange":
                        $scope.request.IntentionOfCustomer = IntentionOfCustomer.ForSwap;
                    break;
                case "requestcooperate":
                        $scope.request.IntentionOfCustomer = IntentionOfCustomer.ForCooperation;
                    break;
                }
            }

            $scope.allEstateAreaRanges = allEstateAreaRanges;
            $scope.allUnitAreaRanges = allUnitAreaRanges;
            $scope.allNumberOfRoomRanges = allNumberOfRoomRanges;
            $scope.allMortgageRanges = allMortgageRanges;
            $scope.allRentRanges = allRentRanges;
            $scope.allDailyRentRanges = allDailyRentRanges;
            $scope.allPriceRanges = allPriceRanges;
            $scope.allPricePerEstateAreaRanges = allPricePerEstateAreaRanges;
            $scope.allPricePerUnitAreaRanges = allPricePerUnitAreaRanges;
            $scope.allUsageTypeOptions = EnumExtentions.getValues(UsageType)
                .select(i => { return { id: i, text: Common.localization.translateEnum("UsageType", i) } })
                .toArray();
            $scope.allPropertyTypeOptions = EnumExtentions.getValues(PropertyType)
                .select(i => { return { id: i, text: Common.localization.translateEnum("PropertyType", i) } })
                .toArray();
            $scope.allIntentionOfOwnerOptions = EnumExtentions.getValues(IntentionOfOwner)
                .select(i => { return { id: i, text: Common.localization.translateEnum("IntentionOfOwner", i) } })
                .toArray();
            $scope.allUsageTypes = $scope.allUsageTypeOptions;
            $scope.allPropertyTypes = $scope.allPropertyTypeOptions;
            $scope.allIntentionOfOwners = $scope.allIntentionOfOwnerOptions;

            $scope.allSearchPanels = {
                estateAreaSearch: "showEstateAreaSearch",
                unitAreaSearch: "showUnitAreaSearch",
                numberOfRoomsSearch: "showNumberOfRoomsSearch",
                pricePerEstateAreaSearch: "showPricePerEstateAreaSearch",
                pricePerUnitAreaSearch: "showPricePerUnitAreaSearch"
            };

            $scope.allPanels = {
                estatePanel: "showEstatePanel",
                housePanel: "showHousePanel",
                unitPanel: "showUnitPanel",
                industryPanel: "showIndustryPanel",
                extraHousePanel: "showHousePanel",
                shopPanel: "showShopPanel"
            }

            $scope.searchAllowances = [
                {
                    propertyType: PropertyType.Land,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.AgriculturalLand,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Garden,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.House,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.numberOfRoomsSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.OldHouse,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.numberOfRoomsSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Shed,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Tenement,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Villa,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.numberOfRoomsSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Apartment,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.numberOfRoomsSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Complex,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.numberOfRoomsSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Tower,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.numberOfRoomsSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.GardenTower,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.numberOfRoomsSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.OfficialResidency,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.numberOfRoomsSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Office,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.numberOfRoomsSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Commercial,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.numberOfRoomsSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.CommercialResidency,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.numberOfRoomsSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Suite,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.numberOfRoomsSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Penthouse,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.numberOfRoomsSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Shop,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch,
                        $scope.allSearchPanels.pricePerUnitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Factory,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.WorkShop,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.RepairShop,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.StoreHouse,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Parking,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Gym,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.CityService,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.pricePerEstateAreaSearch
                    ]
                }
            ];

            $scope.propertyAllowances = [
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

            $scope.$watch("searchInput.UsageType",
                u => {
                    var usagePropertyTypes = {};
                    if (u != null) {
                        usagePropertyTypes = Enumerable.from<any>(allPropertiesPerUsage)
                            .singleOrDefault(m => m.usage === u)
                            .properties;
                    }

                    var intentionPropertyTypes;
                    if ($scope.searchInput.IntentionOfOwner != null) {
                        intentionPropertyTypes = Enumerable.from<any>(allPropertiesPerIntention)
                            .singleOrDefault(m => m.intention === $scope.searchInput.IntentionOfOwner)
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

                });

            $scope.$watch("searchInput.PropertyType",
                p => {
                    $scope.showEstateAreaSearch = Enumerable.from<any>($scope.searchAllowances)
                        .any(a => a.propertyType === p &&
                            Enumerable.from(a.panels).any(at => at === $scope.allSearchPanels.estateAreaSearch));
                    $scope.searchInput.EstateArea = "";

                    $scope.showUnitAreaSearch = Enumerable.from<any>($scope.searchAllowances)
                        .any(a => a.propertyType === p &&
                            Enumerable.from(a.panels).any(at => at === $scope.allSearchPanels.unitAreaSearch));
                    $scope.searchInput.UnitArea = "";

                    $scope.showNumberOfRoomsSearch = Enumerable.from<any>($scope.searchAllowances)
                        .any(a => a.propertyType === p &&
                            Enumerable.from(a.panels).any(at => at === $scope.allSearchPanels.numberOfRoomsSearch));
                    $scope.searchInput.NumberOfRooms = "";
                });

            $scope.$watch("searchInput.IntentionOfOwner",
                i => {
                    if (i != null && i === 2) {
                        $scope.showMortgageSearch = false;
                        $scope.showRentSearch = false;
                        $scope.showDailyRentSearch = false;
                        $scope.showPriceSearch = true;
                        $scope.searchInput.Rent = "";
                        $scope.searchInput.Mortgage = "";
                        $scope.searchInput.Price = "";

                        Enumerable.from(document.getElementsByName("secondTab"))
                            .forEach(el => el.removeAttribute("class"));
                        document.getElementById("sale").setAttribute("class", "active");
                    } else if (i != null && i === 3) {
                        $scope.showMortgageSearch = true;
                        $scope.showRentSearch = false;
                        $scope.showDailyRentSearch = false;
                        $scope.showPriceSearch = false;
                        $scope.searchInput.Rent = "";
                        $scope.searchInput.Mortgage = "";
                        $scope.searchInput.Price = "";

                        Enumerable.from(document.getElementsByName("secondTab"))
                            .forEach(el => el.removeAttribute("class"));
                        document.getElementById("rent").setAttribute("class", "active");
                    } else if (i != null && i === 1) {
                        $scope.showMortgageSearch = true;
                        $scope.showRentSearch = true;
                        $scope.showDailyRentSearch = false;
                        $scope.showPriceSearch = false;
                        $scope.searchInput.Rent = "";
                        $scope.searchInput.Mortgage = "";
                        $scope.searchInput.Price = "";

                        Enumerable.from(document.getElementsByName("secondTab"))
                            .forEach(el => el.removeAttribute("class"));
                        document.getElementById("fullmortgage").setAttribute("class", "active");
                    } else if (i != null && i === 4) {
                        $scope.showMortgageSearch = false;
                        $scope.showRentSearch = false;
                        $scope.showDailyRentSearch = true;
                        $scope.showPriceSearch = false;
                        $scope.searchInput.Rent = "";
                        $scope.searchInput.Mortgage = "";
                        $scope.searchInput.Price = "";

                        Enumerable.from(document.getElementsByName("secondTab"))
                            .forEach(el => el.removeAttribute("class"));
                        document.getElementById("dailyrent").setAttribute("class", "active");
                    } else if (i != null && i === 5) {
                        $scope.showSalePanelProperty = true;
                        $scope.showRentPanelProperty = false;
                        $scope.showDailyRentPanelProperty = false;
                        $scope.showSwapPanel = true;
                        $scope.showCooperationPanel = false;

                        $scope.showSalePanelRequest = true;
                        Enumerable.from(document.getElementsByName("secondTab"))
                            .forEach(el => el.removeAttribute("class"));
                        document.getElementById("exchange").setAttribute("class", "active");
                    } else if (i != null && i === 6) {
                        $scope.showSalePanelProperty = true;
                        $scope.showRentPanelProperty = false;
                        $scope.showDailyRentPanelProperty = false;
                        $scope.showSwapPanel = false;
                        $scope.showCooperationPanel = true;
                        Enumerable.from(document.getElementsByName("secondTab"))
                            .forEach(el => el.removeAttribute("class"));
                        document.getElementById("cooperate").setAttribute("class", "active");
                    } else {
                        $scope.showMortgageSearch = false;
                        $scope.showRentSearch = false;
                        $scope.showDailyRentSearch = false;
                        $scope.showPriceSearch = false;
                        $scope.searchInput.Rent = "";
                        $scope.searchInput.Mortgage = "";
                        $scope.searchInput.Price = "";

                        Enumerable.from(document.getElementsByName("secondTab"))
                            .forEach(el => el.removeAttribute("class"));
                        document.getElementById("all").setAttribute("class", "active");
                    }

                    var intentionPropertyTypes = {};
                    if (i != null) {
                        intentionPropertyTypes = Enumerable.from<any>(allPropertiesPerIntention)
                            .singleOrDefault(m => m.intention === i)
                            .properties;
                    }

                    var usagePropertyTypes;
                    if ($scope.searchInput.UsageType != null) {
                        usagePropertyTypes = Enumerable.from<any>(allPropertiesPerUsage)
                            .singleOrDefault(m => m.usage === $scope.searchInput.UsageType)
                            .properties;
                    } else {
                        usagePropertyTypes = intentionPropertyTypes;
                    }

                    var viablePropertyTypes;
                    if (i != null) {
                        viablePropertyTypes = Enumerable.from(intentionPropertyTypes)
                            .intersect(usagePropertyTypes);
                    } else {
                        viablePropertyTypes = Enumerable.from(usagePropertyTypes);
                    }

                    $scope.propertyTypeOptions = viablePropertyTypes.select(i => {
                            return { id: i, text: Common.localization.translateEnum("PropertyType", i) }
                        })
                        .toArray();
                });

            $scope.$watch("property.PropertyType",
                p => {
                    if ($scope.property != null) {
                        $scope.showEstatePanel = Enumerable.from<any>($scope.propertyAllowances)
                            .any(a => a.propertyType === p &&
                                Enumerable.from(a.panels).any(at => at === $scope.allPanels.estatePanel));

                        $scope.showHousePanel = Enumerable.from<any>($scope.propertyAllowances)
                            .any(a => a.propertyType === p &&
                                Enumerable.from(a.panels).any(at => at === $scope.allPanels.housePanel));

                        $scope.showUnitPanel = Enumerable.from<any>($scope.propertyAllowances)
                            .any(a => a.propertyType === p &&
                                Enumerable.from(a.panels).any(at => at === $scope.allPanels.unitPanel));

                        $scope.showShedPanel = Enumerable.from<any>($scope.propertyAllowances)
                            .any(a => a.propertyType === p &&
                                Enumerable.from(a.panels).any(at => at === $scope.allPanels.shedPanel));

                        $scope.showExtraHousePanel = Enumerable.from<any>($scope.propertyAllowances)
                            .any(a => a.propertyType === p &&
                                Enumerable.from(a.panels).any(at => at === $scope.allPanels.extraHousePanel));

                        $scope.showShopPanel = Enumerable.from<any>($scope.propertyAllowances)
                            .any(a => a.propertyType === p &&
                                Enumerable.from(a.panels).any(at => at === $scope.allPanels.shopPanel));

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
                            Enumerable.from<any>(allSalePriceSpecificationTypes)
                            .where(pr => Enumerable.from<any>(allPriceSpecificationTypePerProperty)
                                .any(a => a.propertyType === p &&
                                    Enumerable.from<any>(a.priceTypes).any(pt => pt === pr.id)))
                            .toArray();
                        $scope.property.PriceSpecificationType = "";
                    }
                });

            $scope.$watch("property.UsageType",
                u => {
                    if ($scope.property != null) {
                        if (!firstTime) {
                            $scope.property.PropertyType = "";
                        }
                        firstTime = false;
                        var usagePropertyTypes = {};
                        if (u != null) {
                            usagePropertyTypes = Enumerable.from<any>(allPropertiesPerUsage)
                                .singleOrDefault(m => m.usage === u)
                                .properties;
                        }
//
                        var intentionPropertyTypes;
                        if ($scope.property.IntentionOfOwner != null) {
                            intentionPropertyTypes = Enumerable.from<any>(allPropertiesPerIntention)
                                .singleOrDefault(m => m.intention === $scope.property.IntentionOfOwner)
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

            $scope.$watch("property.IntentionOfOwner",
                p => {
                    if ($scope.property != null) {
                        if (p != null && p === 2) {
                            $scope.showRentPanel = false;
                            $scope.showSalePanel = true;
                            $scope.showDailyRentPanel = false;
                        } else if (p != null && p === 3) {
                            $scope.showSalePanel = false;
                            $scope.showRentPanel = true;
                            $scope.showDailyRentPanel = false;
                        } else if (p != null && p === 1) {
                            $scope.showRentPanel = true;
                            $scope.showSalePanel = false;
                            $scope.showDailyRentPanel = false;
                        } else if (p != null && p === 4) {
                            $scope.showRentPanel = false;
                            $scope.showDailyRentPanel = true;
                            $scope.showSalePanel = false;
                        } else if (p != null && p === 5) {
                            $scope.showRentPanel = false;
                            $scope.showDailyRentPanel = false;
                            $scope.showSalePanel = true;
                        } else if (p != null && p === 6) {
                            $scope.showRentPanel = false;
                            $scope.showDailyRentPanel = false;
                            $scope.showSalePanel = true;
                        } else {
                            $scope.showSalePanel = false;
                            $scope.showRentPanel = false;
                            $scope.showDailyRentPanel = false;
                        }

                        var intentionPropertyTypes = {};
                        if (p != null) {
                            intentionPropertyTypes = Enumerable.from<any>(allPropertiesPerIntention)
                                .singleOrDefault(m => m.intention === p)
                                .properties;
                        }

                        var usagePropertyTypes;
                        if ($scope.property.UsageType != null) {
                            usagePropertyTypes = Enumerable.from<any>(allPropertiesPerUsage)
                                .singleOrDefault(m => m.usage === $scope.property.UsageType)
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

            $scope.onSavePropertyClick = () => {
                if ($scope.property.PriceSpecificationType === null) {
                    $scope.property.TotalPrice = 0;
                } else if ($scope.property.PriceSpecificationType === 1) {
                    $scope.property.TotalPrice = $scope.property.Price;
                } else if ($scope.property.PriceSpecificationType === 2) {
                    $scope.property.PricePerEstateArea = $scope.property.Price;
                } else if ($scope.property.PriceSpecificationType === 3) {
                    $scope.property.PricePerUnitArea = $scope.property.Price;
                }

                $http.post("/api/haftdong/files/save", $scope.property)
                    .success((data: any) => {
                        toastr.success("ملک با موفقیت ثبت شد.");
                        $scope.$emit('EntityUpdated');
                        $state.go("welcome.files.details", { id: data.ID });
                    });
            };

            $scope.onSaveRequestClick = () => {
                $scope.request.PropertyTypes = Enumerable.from<any>($scope.request.PropertyTypes)
                    .where(pt => pt.isSelected)
                    .select(pt => pt.id)
                    .toArray();
                $http.post("/api/haftdong/requests/save", $scope.request)
                    .success((data: any) => {
                        toastr.success("درخواست با موفقیت ثبت شد.");
                        $scope.$emit('EntityUpdated');
                        $state.go("welcome.requests.details", { id: data.ID });
                    });
            };
        }
    ]);

}