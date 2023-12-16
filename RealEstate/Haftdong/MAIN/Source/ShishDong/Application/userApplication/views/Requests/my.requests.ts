module JahanJooy.ShishDong.Request {
    import PropertyType = RealEstateAgency.Domain.Enums.Property.PropertyType;
    appModule.controller("MyRequestController",
        [
            '$scope', '$timeout', '$state', '$http', '$rootScope', '$q', 'scopes', 'authService',
            "allMortgageRanges", "allRentRanges", "allPriceRanges", "allDailyRentRanges",
            "allEstateAreaRanges", "allUnitAreaRanges",
            ($scope,
                $timeout,
                $state: angular.ui.IStateService,
                $http,
                $rootScope,
                $q,
                scopes,
                authService,
                allMortgageRanges,
                allRentRanges,
                allPriceRanges,
                allDailyRentRanges,
                allEstateAreaRanges,
                allUnitAreaRanges) => {

                $scope.GetTitle = () => {
                    return 'درخواست های من';
                }
                scopes.store('MyRequestController', $scope);

                $scope.FirstPage = 0;
                $scope.PageSize = 10;
                $scope.PageNumber = 1;
                $scope.TotalNumberOfItems = 0;
                $scope.SortDirection = true;
                $scope.ids = new Array();

                $scope.allEstateAreaRanges = allEstateAreaRanges;
                $scope.allUnitAreaRanges = allUnitAreaRanges;
                $scope.allMortgageRanges = allMortgageRanges;
                $scope.allRentRanges = allRentRanges;
                $scope.allDailyRentRanges = allDailyRentRanges;
                $scope.allPriceRanges = allPriceRanges;

                $scope.currentDate = moment(new Date()).format("YYYY/MM/DD");
                $scope.showCreator = authService.isAdministrator();
                $scope.isVerified = authService.isVerified();

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

                $scope.searchInput = {
                    OwnerName: ""
                }

                $scope.$watch("searchInput.PropertyTypeID",
                    p => {
                        $scope.showEstateAreaSearch = Enumerable.from<any>($scope.allowances)
                            .any(a => a.propertyType === p &&
                                Enumerable.from<any>(a.panels).any(at => at === $scope.allSearchPanels.estateAreaSearch));
                        $scope.searchInput.EstateArea = "";

                        $scope.showUnitAreaSearch = Enumerable.from<any>($scope.allowances)
                            .any(a => a.propertyType === p &&
                                Enumerable.from<any>(a.panels).any(at => at === $scope.allSearchPanels.unitAreaSearch));
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
                            rentFrom = Enumerable.from<any>($scope.allRentRanges)
                                .where(rr => rr.id === $scope.searchInput.Rent)
                                .select(rr => rr.min)
                                .singleOrDefault();
                            rentTo = Enumerable.from<any>($scope.allRentRanges)
                                .where(rr => rr.id === $scope.searchInput.Rent)
                                .select(rr => rr.max)
                                .singleOrDefault();
                        } else if ($scope.searchInput.IntentionOfCustomer === 4) {
                            rentFrom = Enumerable.from<any>($scope.allDailyRentRanges)
                                .where(rr => rr.id === $scope.searchInput.Rent)
                                .select(rr => rr.min)
                                .singleOrDefault();
                            rentTo = Enumerable.from<any>($scope.allDailyRentRanges)
                                .where(rr => rr.id === $scope.searchInput.Rent)
                                .select(rr => rr.max)
                                .singleOrDefault();
                        }

                        $http.post("/api/haftdong/requests/myrequests",
                            {
                                StartIndex: ((index - 1) * $scope.PageSize),
                                PageSize: $scope.PageSize,
                                IntentionOfCustomer: $scope.searchInput.IntentionOfCustomer,
                                PropertyType: $scope.searchInput.PropertyTypeID,
                                Vicinity: $scope.searchInput.Vicinity,
                                EstateAreaMin: Enumerable.from<any>($scope.allEstateAreaRanges).where(ear => ear.id === $scope.searchInput.EstateArea)
                                    .select(ear => ear.min).singleOrDefault(),
                                EstateAreaMax: Enumerable.from<any>($scope.allEstateAreaRanges).where(ear => ear.id === $scope.searchInput.EstateArea)
                                    .select(ear => ear.max).singleOrDefault(),
                                UnitAreaMin: Enumerable.from<any>($scope.allUnitAreaRanges).where(uar => uar.id === $scope.searchInput.UnitArea)
                                    .select(uar => uar.min).singleOrDefault(),
                                UnitAreaMax: Enumerable.from<any>($scope.allUnitAreaRanges).where(uar => uar.id === $scope.searchInput.UnitArea)
                                    .select(uar => uar.max).singleOrDefault(),
                                MortgageMin: Enumerable.from<any>($scope.allMortgageRanges)
                                    .where(mr => mr.id === $scope.searchInput.Mortgage)
                                    .select(mr => mr.min)
                                    .singleOrDefault(),
                                MortgageMax: Enumerable.from<any>($scope.allMortgageRanges)
                                    .where(mr => mr.id === $scope.searchInput.Mortgage)
                                    .select(mr => mr.max)
                                    .singleOrDefault(),
                                RentMin: rentFrom,
                                RentMax: rentTo,
                                PriceMin: Enumerable.from<any>($scope.allPriceRanges)
                                    .where(pr => pr.id === $scope.searchInput.Price)
                                    .select(pr => pr.min)
                                    .singleOrDefault(),
                                PriceMax: Enumerable.from<any>($scope.allPriceRanges)
                                    .where(pr => pr.id === $scope.searchInput.Price)
                                    .select(pr => pr.max)
                                    .singleOrDefault(),
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

                                Enumerable.from<any>($scope.Requests).forEach(r => {
                                    r.IsExpired = moment(r.ExpirationTime).format("YYYY/MM/DD") < $scope.currentDate;
                                });

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
                };


                $scope.$watchGroup(["SortColumn", "SortDirection"],
                    () => {
                        $scope.getDataDown(1);
                    });

                $scope.$watch("selectAllProperties",
                    p => {
                        if (p != null) {
                            angular.forEach($scope.Requests,
                                item => {
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