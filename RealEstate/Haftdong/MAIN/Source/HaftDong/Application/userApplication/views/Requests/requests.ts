module JahanJooy.HaftDong.Request {
    import PropertyType = RealEstateAgency.Domain.Enums.Property.PropertyType;
    import UsageType = RealEstateAgency.Domain.Enums.Property.UsageType;
    import EnumExtentions = Common.EnumExtentions;
    appModule.controller("RequestController",
    [
        '$scope', '$timeout', '$state', '$http', '$rootScope', '$q', 'scopes', "allMortgageRanges", "allRentRanges",
        "allPriceRanges", "allDailyRentRanges", "allEstateAreaRanges", "allUnitAreaRanges","allPropertiesPerUsage",
        ($scope,
            $timeout,
            $state: angular.ui.IStateService,
            $http,
            $rootScope,
            $q,
            scopes,
            allMortgageRanges,
            allRentRanges,
            allPriceRanges,
            allDailyRentRanges,
            allEstateAreaRanges,
            allUnitAreaRanges,
            allPropertiesPerUsage) => {

            $scope.GetTitle = () => {
                return 'لیست درخواست ها';
            }
            scopes.store('RequestController', $scope);

            $scope.FirstPage = 0;
            $scope.PageSize = 10;
            $scope.PageNumber = 1;
            $scope.TotalNumberOfItems = 0;
            $scope.SortDirection = true;
            $scope.ids = new Array();
            $scope.searchInput = {};

            $scope.allEstateAreaRanges = allEstateAreaRanges;
            $scope.allUnitAreaRanges = allUnitAreaRanges;
            $scope.allMortgageRanges = allMortgageRanges;
            $scope.allRentRanges = allRentRanges;
            $scope.allDailyRentRanges = allDailyRentRanges;
            $scope.allPriceRanges = allPriceRanges;
            $scope.allPropertyTypeOptions = EnumExtentions.getValues(PropertyType).select(i => { return { id: i, text: Common.localization.translateEnum("PropertyType", i) } }).toArray();
            $scope.allUsageTypeOptions = EnumExtentions.getValues(UsageType).select(i => { return { id: i, text: Common.localization.translateEnum("UsageType", i) } }).toArray();
            $scope.allUsageTypes = $scope.allUsageTypeOptions;
            $scope.allPropertyTypes = $scope.allPropertyTypeOptions;
            $scope.isCollapse = false;
          
           

            $scope.allSearchPanels = {
                estateAreaSearch: "showEstateAreaSearch",
                unitAreaSearch: "showUnitAreaSearch"
            };

            $scope.allowances = [
                {
                    propertyType: PropertyType.Land,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.AgriculturalLand,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Garden,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.House,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.OldHouse,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Shed,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Tenement,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Villa,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch,
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Apartment,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Complex,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Tower,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.GardenTower,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.OfficialResidency,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Office,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Commercial,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.CommercialResidency,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Suite,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Penthouse,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Shop,
                    panels: [
                        $scope.allSearchPanels.unitAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Factory,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.WorkShop,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.RepairShop,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.StoreHouse,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Parking,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.Gym,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch
                    ]
                },
                {
                    propertyType: PropertyType.CityService,
                    panels: [
                        $scope.allSearchPanels.estateAreaSearch
                    ]
                }
            ];


            $scope.$watch("searchInput.UsageType", u => {
                $scope.allPropertyTypes = Enumerable.from($scope.allPropertyTypeOptions).where(p => Enumerable.from(allPropertiesPerUsage).any(a => a.usage === u
                    && Enumerable.from(a.properties).any(up => up === p.id))).toArray();
                $scope.searchInput.PropertyType = "";
            });

            $scope.$watch("searchInput.PropertyType",
                p => {
                    $scope.showEstateAreaSearch = Enumerable.from($scope.allowances)
                        .any(a => a.propertyType === p &&
                            Enumerable.from(a.panels).any(at => at === $scope.allSearchPanels.estateAreaSearch));
                    $scope.searchInput.EstateArea = "";

                    $scope.showUnitAreaSearch = Enumerable.from($scope.allowances)
                        .any(a => a.propertyType === p &&
                            Enumerable.from(a.panels).any(at => at === $scope.allSearchPanels.unitAreaSearch));
                    $scope.searchInput.UnitArea = "";
                });

            $scope.$watch("searchInput.IntentionOfCustomer",
                p => {
                    if (p != null && p === 2) {
                        $scope.showMortgageSearch = false;
                        $scope.showRentSearch = false;
                        $scope.showDailyRentSearch = false;
                        $scope.showPriceSearch = true;
                        $scope.searchInput.Rent = "";
                        $scope.searchInput.Mortgage = "";
                        $scope.searchInput.Price = "";
                    } else if (p != null && p === 3) {
                        $scope.showMortgageSearch = true;
                        $scope.showRentSearch = false;
                        $scope.showDailyRentSearch = false;
                        $scope.showPriceSearch = false;
                        $scope.searchInput.Rent = "";
                        $scope.searchInput.Mortgage = "";
                        $scope.searchInput.Price = "";
                    } else if (p != null && p === 1) {
                        $scope.showMortgageSearch = true;
                        $scope.showRentSearch = true;
                        $scope.showDailyRentSearch = false;
                        $scope.showPriceSearch = false;
                        $scope.searchInput.Rent = "";
                        $scope.searchInput.Mortgage = "";
                        $scope.searchInput.Price = "";
                    } else if (p != null && p === 4) {
                        $scope.showMortgageSearch = false;
                        $scope.showRentSearch = false;
                        $scope.showDailyRentSearch = true;
                        $scope.showPriceSearch = false;
                        $scope.searchInput.Rent = "";
                        $scope.searchInput.Mortgage = "";
                        $scope.searchInput.Price = "";
                    } else {
                        $scope.showMortgageSearch = false;
                        $scope.showRentSearch = false;
                        $scope.showDailyRentSearch = false;
                        $scope.showPriceSearch = false;
                        $scope.searchInput.Rent = "";
                        $scope.searchInput.Mortgage = "";
                        $scope.searchInput.Price = "";
                    }
                });

            function initialize() {
                if ($scope.SortDirection)
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                else
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();
            }

            $scope.getDataDown = (index) => {
                if (!$scope.DataIsCompeleted) {
                    var promise = $q.defer();

                    var sortDirection;
                    if (!$scope.SortDirection)
                        sortDirection = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                    else
                        sortDirection = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();

                    var rentFrom = null;
                    var rentTo = null;
                    if ($scope.searchInput.IntentionOfCustomer === 1) {
                        rentFrom = Enumerable.from($scope.allRentRanges)
                            .where(rr => rr.id === $scope.searchInput.Rent)
                            .select(rr => rr.min)
                            .singleOrDefault();
                        rentTo = Enumerable.from($scope.allRentRanges)
                            .where(rr => rr.id === $scope.searchInput.Rent)
                            .select(rr => rr.max)
                            .singleOrDefault();
                    } else if ($scope.searchInput.IntentionOfCustomer === 4) {
                        rentFrom = Enumerable.from($scope.allDailyRentRanges)
                            .where(rr => rr.id === $scope.searchInput.Rent)
                            .select(rr => rr.min)
                            .singleOrDefault();
                        rentTo = Enumerable.from($scope.allDailyRentRanges)
                            .where(rr => rr.id === $scope.searchInput.Rent)
                            .select(rr => rr.max)
                            .singleOrDefault();
                    }

                    $http.post("/api/web/requests/search",
                        {
                            StartIndex: ((index - 1) * $scope.PageSize),
                            PageSize: $scope.PageSize,
                            IntentionOfCustomer: $scope.searchInput.IntentionOfCustomer,
                            PropertyType: $scope.searchInput.PropertyType,
                            UsageType: $scope.searchInput.UsageType,
                            Vicinity: $scope.searchInput.Vicinity,
                            EstateAreaMin: Enumerable.from($scope.allEstateAreaRanges).where(ear => ear.id === $scope.searchInput.EstateArea)
                                .select(ear => ear.min).singleOrDefault(),
                            EstateAreaMax: Enumerable.from($scope.allEstateAreaRanges).where(ear => ear.id === $scope.searchInput.EstateArea)
                                .select(ear => ear.max).singleOrDefault(),
                            UnitAreaMin: Enumerable.from($scope.allUnitAreaRanges).where(uar => uar.id === $scope.searchInput.UnitArea)
                                .select(uar => uar.min).singleOrDefault(),
                            UnitAreaMax: Enumerable.from($scope.allUnitAreaRanges).where(uar => uar.id === $scope.searchInput.UnitArea)
                                .select(uar => uar.max).singleOrDefault(),
                            MortgageMin: Enumerable.from($scope.allMortgageRanges)
                                .where(mr => mr.id === $scope.searchInput.Mortgage)
                                .select(mr => mr.min)
                                .singleOrDefault(),
                            MortgageMax: Enumerable.from($scope.allMortgageRanges)
                                .where(mr => mr.id === $scope.searchInput.Mortgage)
                                .select(mr => mr.max)
                                .singleOrDefault(),
                            RentMin: rentFrom,
                            RentMax: rentTo,
                            PriceMin: Enumerable.from($scope.allPriceRanges)
                                .where(pr => pr.id === $scope.searchInput.Price)
                                .select(pr => pr.min)
                                .singleOrDefault(),
                            PriceMax: Enumerable.from($scope.allPriceRanges)
                                .where(pr => pr.id === $scope.searchInput.Price)
                                .select(pr => pr.max)
                                .singleOrDefault(),
                            IsArchived: $scope.searchInput.IsArchived,
                            IsPublic: $scope.searchInput.IsPublic,
                            State: $scope.searchInput.State,
                            SortColumn: $scope.SortColumn,
                            SortDirection: sortDirection
                        })
                        .success((data: any) => {
                            $scope.extendMenu = data.ExtendMenu;
                            $scope.Requests = data.Requests;
                            if (data.Requests.PageItems == null)
                                $scope.Requests = [];
                            else
                                $scope.Requests = data.Requests.PageItems;

                            $scope.TotalNumberOfItems = data.Requests.TotalNumberOfItems;
                            $scope.NumberOfPages = data.Requests.TotalNumberOfPages;
                            $scope.PageNumber = data.Requests.PageNumber;

                            promise.resolve();

                        })
                        .error(() => {
                            promise.reject();
                        });
                    return promise.promise;
                }
                return null;
            };


            $scope.$watchGroup(["SortColumn", "SortDirection"],
            () => {
                $scope.getDataDown(1);
            });

            $scope.$watch("selectAllRequests",
                p => {
                    if (p != null) {angular.forEach($scope.Requests,item => {
                                item.Selected = p;
                            });
                    }
                });

            $scope.resetIdsList = (request) => {
                if (request.Selected) {
                    $scope.ids.push(request.ID);
                } else {
                    var index = $scope.ids.indexOf(request.ID);
                    if (index > -1)
                        $scope.ids.splice(index, 1);
                }
            }

            $scope.onSearch = () => {
                $scope.getDataDown(1);
            }

            initialize();

            var listener = $rootScope.$on('EntityUpdated',
            () => {
                initialize();
            });

            $scope.$on('$destroy', listener);
        }
    ]);

}