module JahanJooy.ShishDong.File {
    appModule.controller("MyFilesController",
        [
            "$scope", "$timeout", "$state", "$http", "$rootScope", "$q",
            "allEstateAreaRanges", "allUnitAreaRanges", "allNumberOfRoomRanges", "allMortgageRanges",
            "allRentRanges", "allPriceRanges", "allPropertiesPerUsage", "allDailyRentRanges",
            "allPricePerEstateAreaRanges", "allPricePerUnitAreaRanges", "scopes", "authService",
            ($scope,
                $timeout,
                $state: angular.ui.IStateService,
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
                allDailyRentRanges,
                allPricePerEstateAreaRanges,
                allPricePerUnitAreaRanges,
                scopes,
                authService) => {

                $scope.GetTitle = () => {
                    return "لیست فایل ها";
                };
                scopes.store("MyFilesController", $scope);

                $scope.isCollapsed = false;
                $scope.showMap = false;
                $scope.boundary = null;
                $scope.reload = true;
                $scope.FirstPage = 0;
                $scope.PageSize = 7;
                $scope.PageNumber = 1;
                $scope.DataIsCompeleted = false;
                $scope.TotalNumberOfItems = 0;
                $scope.ids = new Array();
                $scope.SortDirection = true;
                $scope.showOtherSupplies = false;
                $scope.allSupplies = null;
                $scope.showCreator = authService.isAdministrator();
                $scope.isVerified = authService.isVerified();
                //            $scope.searchInput.SourceType = SourceType.Haftdong;

                $scope.currentDate = moment(new Date()).format("YYYY/MM/DD");
                $scope.currentDateMinusTwoWeeks = moment(new Date()).add(-2, "w").format("YYYY/MM/DD");
                $scope.isTwoWeeksAgo = (supply) => {
                    return moment(supply.CreationTime).format("YYYY/MM/DD") >= $scope.currentDateMinusTwoWeeks;
                };

                function getThumbnails() {
                    $scope.Supplies.forEach(s => {
                        if (s.Property.CoverImageID != null) {
                            $http.get("/api/haftdong/files/getThumbnail/" + s.Property.CoverImageID,
                                { responseType: "blob" })
                                .success((data: Blob) => {
                                    s.Property.blob = URL.createObjectURL(data);
                                });
                        } else {
                            s.Property.blob = null;
                        }
                    });
                }

                $scope.showCallOwner = (supply) => {
                    switch (supply.IntentionOfOwner) {
                        case 1:
                            if (supply.Mortgage === 0 && supply.Rent === 0)
                                return true;
                            else
                                return false;
                        case 2:
                            if ($scope.computePrice(supply) === 0)
                                return true;
                            else
                                return false;
                        case 3:
                            if (supply.Mortgage === 0)
                                return true;
                            else
                                return false;
                        case 4:
                            if (supply.Rent === 0)
                                return true;
                            else
                                return false;
                        default:
                            return false;
                    }
                };

                function prepareData(supplies) {
                    if ($scope.searchInput.IntentionOfOwner != null)
                        $scope.showOtherSupplies = true;

                    $scope.TotalNumberOfItems = supplies.TotalNumberOfItems != null ? supplies.TotalNumberOfItems : 1;
                    $scope.NumberOfPages = supplies.TotalNumberOfPages != null ? supplies.TotalNumberOfPages : 1;
                    $scope.PageNumber = supplies.PageNumber != null ? supplies.PageNumber : 1;
                    if ($scope.searchInput.IntentionOfOwner != null)
                        $scope.showSortablePrice = true;
                    else
                        $scope.showSortablePrice = false;

                    $scope.Supplies.forEach(s => {
                        if (s.Property != null &&
                            s.Property.Vicinity != null &&
                            s.Property.Vicinity.CompleteName != null &&
                            s.Property.Vicinity.CompleteName !== "" &&
                            s.Property.Address != null &&
                            s.Property.Address !== "") {
                            s.Address = s.Property.Vicinity.CompleteName + " - " + s.Property.Address;
                        } else if (s.Property != null &&
                            s.Property.Vicinity != null &&
                            s.Property.Vicinity.CompleteName != null &&
                            s.Property.Vicinity.CompleteName !== "") {
                            s.Address = s.Property.Vicinity.CompleteName;
                        } else if (s.Property != null && s.Property.Address != null && s.Property.Address !== "") {
                            s.Address = s.Property.Address;
                        } else {
                            s.Address = "";
                        }

                        s.IsExpired = moment(s.ExpirationTime).format("YYYY/MM/DD") < $scope.currentDate;
                    });

                    getThumbnails();
                }

                $scope.getDataDown = (index, isCluster) => {
                    var promise = $q.defer();

                    if (isCluster === false) {
                        $scope.searchInput.VicinityID = null;
                    }

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

                    $scope.showOtherSupplies = false;
                    var searchQuery = {
                        StartIndex: ((index - 1) * $scope.PageSize),
                        PageSize: $scope.PageSize,
                        UsageType: $scope.searchInput.UsageType,
                        PropertyType: $scope.searchInput.PropertyType,
                        SourceType: $scope.searchInput.SourceType,
                        IntentionOfOwner: $scope.searchInput.IntentionOfOwner,
                        OwnerName: $scope.searchInput.OwnerName,
                        Vicinity: $scope.searchInput.Vicinity,
                        VicinityID: $scope.searchInput.VicinityID,
                        SearchBox: null,
                        IsHidden: $scope.searchInput.IsHidden,
                        HasWarning: $scope.searchInput.HasWarning,
                        HasPhoto: $scope.searchInput.HasPhoto,
                        MineOnly: $scope.searchInput.MineOnly,
                        Address: $scope.searchInput.Address,
                        EstateAreaMin: Enumerable.from<any>($scope.allEstateAreaRanges)
                            .where(ear => ear.id === $scope.searchInput.EstateArea)
                            .select(ear => ear.min)
                            .singleOrDefault(),
                        EstateAreaMax: Enumerable.from<any>($scope.allEstateAreaRanges)
                            .where(ear => ear.id === $scope.searchInput.EstateArea)
                            .select(ear => ear.max)
                            .singleOrDefault(),
                        UnitAreaMin: Enumerable.from<any>($scope.allUnitAreaRanges)
                            .where(uar => uar.id === $scope.searchInput.UnitArea)
                            .select(uar => uar.min)
                            .singleOrDefault(),
                        UnitAreaMax: Enumerable.from<any>($scope.allUnitAreaRanges)
                            .where(uar => uar.id === $scope.searchInput.UnitArea)
                            .select(uar => uar.max)
                            .singleOrDefault(),
                        NumberOfRoomsMin: Enumerable.from<any>($scope.allNumberOfRoomRanges)
                            .where(nrr => nrr.id === $scope.searchInput.NumberOfRooms)
                            .select(ear => ear.min)
                            .singleOrDefault(),
                        NumberOfRoomsMax: Enumerable.from<any>($scope.allNumberOfRoomRanges)
                            .where(nrr => nrr.id === $scope.searchInput.NumberOfRooms)
                            .select(nrr => nrr.max)
                            .singleOrDefault(),
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
                        PricePerEstateAreaMin: Enumerable.from<any>($scope.allPricePerEstateAreaRanges)
                            .where(pr => pr.id === $scope.searchInput.PricePerEstateArea)
                            .select(pr => pr.min)
                            .singleOrDefault(),
                        PricePerEstateAreaMax: Enumerable.from<any>($scope.allPricePerEstateAreaRanges)
                            .where(pr => pr.id === $scope.searchInput.PricePerEstateArea)
                            .select(pr => pr.max)
                            .singleOrDefault(),
                        PricePerUnitAreaMin: Enumerable.from<any>($scope.allPricePerUnitAreaRanges)
                            .where(pr => pr.id === $scope.searchInput.PricePerUnitArea)
                            .select(pr => pr.min)
                            .singleOrDefault(),
                        PricePerUnitAreaMax: Enumerable.from<any>($scope.allPricePerUnitAreaRanges)
                            .where(pr => pr.id === $scope.searchInput.PricePerUnitArea)
                            .select(pr => pr.max)
                            .singleOrDefault(),
                        SortColumn: $scope.SortColumn,
                        SortDirection: $scope.SortDirectionName
                    };

                    if ($scope.SortDirection)
                        $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                    else
                        $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();

                    $http.post("/api/haftdong/files/myfiles", searchQuery)
                        .success((data: any) => {
                            if (data.Supplies.PageItems == null || data.Supplies.PageItems.length === 0)
                                $scope.Supplies = [];
                            else
                                $scope.Supplies = data.Supplies.PageItems;

                            prepareData(data.Supplies);
                            if (isCluster == null || !isCluster) {
                                $scope.allSupplies = data.Supplies;
                            }
                            promise.resolve();
                        })
                        .error(() => {
                            promise.reject();
                        });

                    return promise.promise;
                };

                $scope.getMapData = () => {
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

                    var searchQuery = {
                        StartIndex: 0,
                        PageSize: $scope.PageSize,
                        UsageType: $scope.searchInput.UsageType,
                        PropertyType: $scope.searchInput.PropertyType,
                        SourceType: $scope.searchInput.SourceType,
                        IntentionOfOwner: $scope.searchInput.IntentionOfOwner,
                        OwnerName: $scope.searchInput.OwnerName,
                        Vicinity: $scope.searchInput.Vicinity,
                        Bounds: {
                            NorthEast: $scope.boundary.getNorthEast(),
                            SouthWest: $scope.boundary.getSouthWest()
                        },
                        IsHidden: $scope.searchInput.IsHidden,
                        HasWarning: $scope.searchInput.HasWarning,
                        HasPhoto: $scope.searchInput.HasPhoto,
                        MineOnly: $scope.searchInput.MineOnly,
                        Address: $scope.searchInput.Address,
                        EstateAreaMin: Enumerable.from<any>($scope.allEstateAreaRanges)
                            .where(ear => ear.id === $scope.searchInput.EstateArea)
                            .select(ear => ear.min)
                            .singleOrDefault(),
                        EstateAreaMax: Enumerable.from<any>($scope.allEstateAreaRanges)
                            .where(ear => ear.id === $scope.searchInput.EstateArea)
                            .select(ear => ear.max)
                            .singleOrDefault(),
                        UnitAreaMin: Enumerable.from<any>($scope.allUnitAreaRanges)
                            .where(uar => uar.id === $scope.searchInput.UnitArea)
                            .select(uar => uar.min)
                            .singleOrDefault(),
                        UnitAreaMax: Enumerable.from<any>($scope.allUnitAreaRanges)
                            .where(uar => uar.id === $scope.searchInput.UnitArea)
                            .select(uar => uar.max)
                            .singleOrDefault(),
                        NumberOfRoomsMin: Enumerable.from<any>($scope.allNumberOfRoomRanges)
                            .where(nrr => nrr.id === $scope.searchInput.NumberOfRooms)
                            .select(ear => ear.min)
                            .singleOrDefault(),
                        NumberOfRoomsMax: Enumerable.from<any>($scope.allNumberOfRoomRanges)
                            .where(nrr => nrr.id === $scope.searchInput.NumberOfRooms)
                            .select(nrr => nrr.max)
                            .singleOrDefault(),
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
                        PricePerEstateAreaMin: Enumerable.from<any>($scope.allPricePerEstateAreaRanges)
                            .where(pr => pr.id === $scope.searchInput.PricePerEstateArea)
                            .select(pr => pr.min)
                            .singleOrDefault(),
                        PricePerEstateAreaMax: Enumerable.from<any>($scope.allPricePerEstateAreaRanges)
                            .where(pr => pr.id === $scope.searchInput.PricePerEstateArea)
                            .select(pr => pr.max)
                            .singleOrDefault(),
                        PricePerUnitAreaMin: Enumerable.from<any>($scope.allPricePerUnitAreaRanges)
                            .where(pr => pr.id === $scope.searchInput.PricePerUnitArea)
                            .select(pr => pr.min)
                            .singleOrDefault(),
                        PricePerUnitAreaMax: Enumerable.from<any>($scope.allPricePerUnitAreaRanges)
                            .where(pr => pr.id === $scope.searchInput.PricePerUnitArea)
                            .select(pr => pr.max)
                            .singleOrDefault(),
                        SortColumn: $scope.SortColumn,
                        SortDirection: $scope.SortDirectionName
                    }

                    $http.post("/api/haftdong/files/myfilesinmap", searchQuery)
                        .success((data: any) => {
                            if (data == null)
                                $scope.Properties = [];
                            else
                                $scope.Properties = data;
                        });
                };

                $scope.onSearch = () => {
                    $scope.getDataDown(1, false);
                    $scope.getMapData();
                };

                function initialize() {
                    if ($scope.SortDirection)
                        $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                    else
                        $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();
                }

                $scope.computePrice = (entity) => {
                    if (entity.PriceSpecificationType === null) {
                        return 0;
                    } else if (entity.PriceSpecificationType === 1) {
                        return entity.TotalPrice;
                    } else if (entity.PriceSpecificationType === 2 && entity.Property.EstateArea != null) {
                        return entity.PricePerEstateArea * entity.Property.EstateArea;
                    } else if (entity.PriceSpecificationType === 3 && entity.Property.UnitArea != null) {
                        return entity.PricePerUnitArea * entity.Property.UnitArea;
                    }
                    return 0;
                };

                $scope.$watch("Supplies",
                    () => {
                        $scope.NumberOfPagesArray = new Array($scope.NumberOfPages);
                    });

                $scope.$watchGroup(["SortColumn", "SortDirection"],
                    () => {
                        $scope.getDataDown(1);
                    });

                var mapListener = $scope.$on("MapIsReady",
                    () => {
                        if ($state.current.name === "welcome.files") {
                            //                    $timeout($scope.getMapData(), 0);
                            $scope.getMapData();
                        }
                    });

                $scope.$watch("selectAllSupplies",
                    s => {
                        if (s != null) {
                            angular.forEach($scope.Supplies,
                                item => {
                                    item.Selected = s;
                                });
                        }
                    });

                $scope.$watchGroup(["searchInput.PropertyType", "searchInput.IntentionOfOwner"],
                    (i) => {
                        $scope.showPricePerEstateAreaSearch = Enumerable.from<any>($scope.allowances)
                            .any(a => a.propertyType === i[0] &&
                                Enumerable.from(a.panels).any(at => at === $scope.allSearchPanels.pricePerEstateAreaSearch)) &&
                        (i[1] === 1);
                        $scope.searchInput.PricePerEstateArea = "";

                        $scope.showPricePerUnitAreaSearch = Enumerable.from<any>($scope.allowances)
                            .any(a => a.propertyType === i[0] &&
                                Enumerable.from(a.panels).any(at => at === $scope.allSearchPanels.pricePerUnitAreaSearch)) &&
                        (i[1] === 1);
                        $scope.searchInput.PricePerUnitArea = "";
                    });

                $scope.resetIdsList = (supply) => {
                    if (supply.Selected) {
                        $scope.ids.push(supply.ID);
                    } else {
                        var index = $scope.ids.indexOf(supply.ID);
                        if (index > -1)
                            $scope.ids.splice(index, 1);
                    }
                };

                function setReloadTrue() {
                    if (!$scope.reload)
                        $scope.reload = true;
                }

                $scope.setReloadFalse = () => {
                    $scope.reload = false;
                    $timeout(setReloadTrue, 1000);
                }

                $scope.clickOnPropertyMarker = (supply) => {
                    $scope.Supplies = supply;
                    prepareData($scope.Supplies);
                    $scope.$apply();
                }

                $scope.clickOnVicinityMarker = (vicinityId) => {
                    $scope.searchInput.VicinityID = vicinityId;
                    $scope.getDataDown(1, true);
                    $scope.$apply();
                }

                $scope.backToAllProperties = () => {
                    if ($scope.allSupplies != null) {
                        $scope.Supplies = $scope.allSupplies.PageItems;
                        $scope.TotalNumberOfItems = $scope.allSupplies.TotalNumberOfItems != null
                            ? $scope.allSupplies.TotalNumberOfItems
                            : 1;
                        $scope.NumberOfPages = $scope.allSupplies.TotalNumberOfPages != null
                            ? $scope.allSupplies.TotalNumberOfPages
                            : 1;
                        $scope.PageNumber = $scope.allSupplies.PageNumber != null ? $scope.allSupplies.PageNumber : 1;
                        $scope.searchInput.VicinityID = null;
                    }
                }

                initialize();

                var listener = $rootScope.$on("EntityUpdated",
                    () => {
                        initialize();
                    });

                $scope.$on("$destroy", listener);
                $scope.$on("$destroy", mapListener);
            }
        ]);

}