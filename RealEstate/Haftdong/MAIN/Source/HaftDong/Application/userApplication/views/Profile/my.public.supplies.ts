module JahanJooy.HaftDong.Supply {
    appModule.controller("MyPublicSuppliesController", [
        "$scope", "$timeout", "$state", "$http", "$rootScope", "$q",
        "allEstateAreaRanges", "allUnitAreaRanges", "allNumberOfRoomRanges", "allMortgageRanges",
        "allRentRanges", "allPriceRanges", "allPropertiesPerUsage", "allDailyRentRanges",
        "allPricePerEstateAreaRanges", "allPricePerUnitAreaRanges", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $rootScope, $q,
            allEstateAreaRanges, allUnitAreaRanges, allNumberOfRoomRanges, allMortgageRanges,
            allRentRanges, allPriceRanges, allPropertiesPerUsage, allDailyRentRanges,
            allPricePerEstateAreaRanges, allPricePerUnitAreaRanges, scopes) => {

            $scope.GetTitle = () => {
                return "فایل های عمومی من";
            };
            scopes.store("MyPublicSuppliesController", $scope);

            $scope.FirstPage = 0;
            $scope.PageSize = 7;
            $scope.PageNumber = 1;
            $scope.DataIsCompeleted = false;
            $scope.TotalNumberOfItems = 0;
            $scope.searchInput = {};
            $scope.ids = new Array();
            $scope.SortDirection = true;
            $scope.allSupplies = null;

            $scope.currentDateMinusTwoWeeks = moment(new Date()).add(-2, "w").format("YYYY/MM/DD");
            $scope.isTwoWeeksAgo = (supply) => {
                return moment(supply.CreationTime).format("YYYY/MM/DD") >= $scope.currentDateMinusTwoWeeks;
            };

            function getThumbnails() {
                $scope.Supplies.forEach(s => {
                    if (s.Property.CoverImageID != null) {
                        $http.get("/api/web/properties/getThumbnail/" + s.Property.CoverImageID, { responseType: "blob" })
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
                    if (s.Property != null && s.Property.Vicinity != null && s.Property.Vicinity.CompleteName != null && s.Property.Vicinity.CompleteName !== ""
                        && s.Property.Address != null && s.Property.Address !== "") {
                        s.Address = s.Property.Vicinity.CompleteName + " - " + s.Property.Address;
                    } else if (s.Property != null && s.Property.Vicinity != null && s.Property.Vicinity.CompleteName != null && s.Property.Vicinity.CompleteName !== "") {
                        s.Address = s.Property.Vicinity.CompleteName;
                    } else if (s.Property != null && s.Property.Address != null && s.Property.Address !== "") {
                        s.Address = s.Property.Address;
                    } else {
                        s.Address = "";
                    }
                });

                getThumbnails();
            }

            $scope.getDataDown = (index) => {
                var promise = $q.defer();

                $scope.showOtherSupplies = false;
                var searchQuery = {
                    StartIndex: ((index - 1) * $scope.PageSize),
                    PageSize: $scope.PageSize,
                    SortColumn: $scope.SortColumn,
                    SortDirection: $scope.SortDirectionName,
                    IsPublic: true
                };

                if ($scope.SortDirection)
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                else
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();

                $http.post("/api/web/files/mypublicfiles", searchQuery).success((data: any) => {
                    if (data.Supplies.PageItems == null || data.Supplies.PageItems.length === 0)
                        $scope.Supplies = [];
                    else
                        $scope.Supplies = data.Supplies.PageItems;

                    prepareData(data.Supplies);
                    promise.resolve();
                }).error(() => {
                    promise.reject();
                });

                return promise.promise;
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
            
            $scope.$watch("Supplies", () => {
                $scope.NumberOfPagesArray = new Array($scope.NumberOfPages);
            });

            $scope.$watchGroup(["SortColumn", "SortDirection"], () => {
                $scope.getDataDown(1);
            });
            
            $scope.$watch("selectAllSupplies", s => {
                if (s != null) {
                    angular.forEach($scope.Supplies, item => {
                        item.Selected = s;
                    });
                }
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
            
            initialize();

            var listener = $rootScope.$on("EntityUpdated", () => {
                initialize();
            });

            $scope.$on("$destroy", listener);
        }
    ]);

}