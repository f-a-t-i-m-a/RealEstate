module JahanJooy.HaftDong.Supply {
    import EnumExtentions = Common.EnumExtentions;
    import PropertyType = RealEstateAgency.Domain.Enums.Property.PropertyType;
    import UsageType = RealEstateAgency.Domain.Enums.Property.UsageType;
    import IntentionOfOwner = RealEstateAgency.Domain.Enums.Property.IntentionOfOwner;

    appModule.controller("SupplyController", [
        "$scope", "$timeout", "$state", "$http", "$rootScope", "$q",
        "allEstateAreaRanges", "allUnitAreaRanges", "allNumberOfRoomRanges", "allMortgageRanges",
        "allRentRanges", "allPriceRanges", "allPropertiesPerUsage", "allDailyRentRanges",
        "allPricePerEstateAreaRanges", "allPricePerUnitAreaRanges", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $rootScope, $q,
            allEstateAreaRanges, allUnitAreaRanges, allNumberOfRoomRanges, allMortgageRanges,
            allRentRanges, allPriceRanges, allPropertiesPerUsage, allDailyRentRanges,
            allPricePerEstateAreaRanges, allPricePerUnitAreaRanges, scopes) => {

            $scope.GetTitle = () => {
                return "لیست فایل ها";
            };
            scopes.store("SupplyController", $scope);

            $scope.isCollapsed = false;
            $scope.showMap = false;
            $scope.boundary = null;
            $scope.reload = true;
            $scope.FirstPage = 0;
            $scope.PageSize = 7;
            $scope.PageNumber = 1;
            $scope.DataIsCompeleted = false;
            $scope.TotalNumberOfItems = 0;
            $scope.searchInput = {};
            $scope.ids = new Array();
            $scope.SortDirection = true;
            $scope.showOtherSupplies = false;
            $scope.allSupplies = null;
//            $scope.searchInput.SourceType = SourceType.Haftdong;

            $scope.currentDateMinusTwoWeeks = moment(new Date()).add(-2, "w").format("YYYY/MM/DD");
            $scope.isTwoWeeksAgo = (supply) => {
                return moment(supply.CreationTime).format("YYYY/MM/DD") >= $scope.currentDateMinusTwoWeeks;
            };
            $scope.allEstateAreaRanges = allEstateAreaRanges;
            $scope.allUnitAreaRanges = allUnitAreaRanges;
            $scope.allNumberOfRoomRanges = allNumberOfRoomRanges;
            $scope.allMortgageRanges = allMortgageRanges;
            $scope.allRentRanges = allRentRanges;
            $scope.allDailyRentRanges = allDailyRentRanges;
            $scope.allPriceRanges = allPriceRanges;
            $scope.allPricePerEstateAreaRanges = allPricePerEstateAreaRanges;
            $scope.allPricePerUnitAreaRanges = allPricePerUnitAreaRanges;
            $scope.allUsageTypeOptions = EnumExtentions.getValues(UsageType).select(i => { return { id: i, text: Common.localization.translateEnum("UsageType", i) } }).toArray();
            $scope.allPropertyTypeOptions = EnumExtentions.getValues(PropertyType).select(i => { return { id: i, text: Common.localization.translateEnum("PropertyType", i) } }).toArray();
            $scope.allIntentionOfOwnerOptions = EnumExtentions.getValues(IntentionOfOwner).select(i => { return { id: i, text: Common.localization.translateEnum("IntentionOfOwner", i) } }).toArray();
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

            $scope.allowances = [
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

            $scope.getDataDown = (index, isCluster) => {
                var promise = $q.defer();

                if (isCluster === false) {
                    $scope.searchInput.VicinityID = null;
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
                    State: $scope.searchInput.State,
                    Vicinity: $scope.searchInput.Vicinity,
                    VicinityID: $scope.searchInput.VicinityID,
                    SearchBox: null,
                    IsArchived: $scope.searchInput.IsArchived,
                    IsHidden: $scope.searchInput.IsHidden,
                    IsPublic: $scope.searchInput.IsPublic,
                    HasWarning: $scope.searchInput.HasWarning,
                    HasPhoto: $scope.searchInput.HasPhoto,
                    Address: $scope.searchInput.Address,
                    EstateAreaMin: Enumerable.from($scope.allEstateAreaRanges).where(ear => ear.id === $scope.searchInput.EstateArea)
                        .select(ear => ear.min).singleOrDefault(),
                    EstateAreaMax: Enumerable.from($scope.allEstateAreaRanges).where(ear => ear.id === $scope.searchInput.EstateArea)
                        .select(ear => ear.max).singleOrDefault(),
                    UnitAreaMin: Enumerable.from($scope.allUnitAreaRanges).where(uar => uar.id === $scope.searchInput.UnitArea)
                        .select(uar => uar.min).singleOrDefault(),
                    UnitAreaMax: Enumerable.from($scope.allUnitAreaRanges).where(uar => uar.id === $scope.searchInput.UnitArea)
                        .select(uar => uar.max).singleOrDefault(),
                    NumberOfRoomsMin: Enumerable.from($scope.allNumberOfRoomRanges).where(nrr => nrr.id === $scope.searchInput.NumberOfRooms)
                        .select(ear => ear.min).singleOrDefault(),
                    NumberOfRoomsMax: Enumerable.from($scope.allNumberOfRoomRanges).where(nrr => nrr.id === $scope.searchInput.NumberOfRooms)
                        .select(nrr => nrr.max).singleOrDefault(),
                    MortgageMin: Enumerable.from($scope.allMortgageRanges).where(mr => mr.id === $scope.searchInput.Mortgage)
                        .select(mr => mr.min).singleOrDefault(),
                    MortgageMax: Enumerable.from($scope.allMortgageRanges).where(mr => mr.id === $scope.searchInput.Mortgage)
                        .select(mr => mr.max).singleOrDefault(),
                    RentMin: Enumerable.from($scope.allRentRanges).where(rr => rr.id === $scope.searchInput.Rent)
                        .select(rr => rr.min).singleOrDefault(),
                    RentMax: Enumerable.from($scope.allRentRanges).where(rr => rr.id === $scope.searchInput.Rent)
                        .select(rr => rr.max).singleOrDefault(),
                    PriceMin: Enumerable.from($scope.allPriceRanges).where(pr => pr.id === $scope.searchInput.Price)
                        .select(pr => pr.min).singleOrDefault(),
                    PriceMax: Enumerable.from($scope.allPriceRanges).where(pr => pr.id === $scope.searchInput.Price)
                        .select(pr => pr.max).singleOrDefault(),
                    PricePerEstateAreaMin: Enumerable.from($scope.allPricePerEstateAreaRanges).where(pr => pr.id === $scope.searchInput.PricePerEstateArea)
                        .select(pr => pr.min).singleOrDefault(),
                    PricePerEstateAreaMax: Enumerable.from($scope.allPricePerEstateAreaRanges).where(pr => pr.id === $scope.searchInput.PricePerEstateArea)
                        .select(pr => pr.max).singleOrDefault(),
                    PricePerUnitAreaMin: Enumerable.from($scope.allPricePerUnitAreaRanges).where(pr => pr.id === $scope.searchInput.PricePerUnitArea)
                        .select(pr => pr.min).singleOrDefault(),
                    PricePerUnitAreaMax: Enumerable.from($scope.allPricePerUnitAreaRanges).where(pr => pr.id === $scope.searchInput.PricePerUnitArea)
                        .select(pr => pr.max).singleOrDefault(),
                    SortColumn: $scope.SortColumn,
                    SortDirection: $scope.SortDirectionName
                };

                if ($scope.SortDirection)
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                else
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();

                $http.post("/api/web/files/search", searchQuery).success((data: any) => {
                    if (data.Supplies.PageItems == null || data.Supplies.PageItems.length === 0)
                        $scope.Supplies = [];
                    else
                        $scope.Supplies = data.Supplies.PageItems;

                    prepareData(data.Supplies);
                    if (isCluster == null || !isCluster) {
                        $scope.allSupplies = data.Supplies;
                    }
                    promise.resolve();
                }).error(() => {
                    promise.reject();
                });

                return promise.promise;
            };

            $scope.getMapData = () => {
                var searchQuery = {
                    StartIndex: 0,
                    PageSize: $scope.PageSize,
                    UsageType: $scope.searchInput.UsageType,
                    PropertyType: $scope.searchInput.PropertyType,
                    SourceType: $scope.searchInput.SourceType,
                    IntentionOfOwner: $scope.searchInput.IntentionOfOwner,
                    OwnerName: $scope.searchInput.OwnerName,
                    State: $scope.searchInput.State,
                    Vicinity: $scope.searchInput.Vicinity,
                    Bounds: {
                        NorthEast: $scope.boundary.getNorthEast(),
                        SouthWest: $scope.boundary.getSouthWest()
                    },
                    IsArchived: $scope.searchInput.IsArchived,
                    IsHidden: $scope.searchInput.IsHidden,
                    IsPublic: $scope.searchInput.IsPublic,
                    HasWarning: $scope.searchInput.HasWarning,
                    HasPhoto: $scope.searchInput.HasPhoto,
                    Address: $scope.searchInput.Address,
                    EstateAreaMin: Enumerable.from($scope.allEstateAreaRanges).where(ear => ear.id === $scope.searchInput.EstateArea)
                        .select(ear => ear.min).singleOrDefault(),
                    EstateAreaMax: Enumerable.from($scope.allEstateAreaRanges).where(ear => ear.id === $scope.searchInput.EstateArea)
                        .select(ear => ear.max).singleOrDefault(),
                    UnitAreaMin: Enumerable.from($scope.allUnitAreaRanges).where(uar => uar.id === $scope.searchInput.UnitArea)
                        .select(uar => uar.min).singleOrDefault(),
                    UnitAreaMax: Enumerable.from($scope.allUnitAreaRanges).where(uar => uar.id === $scope.searchInput.UnitArea)
                        .select(uar => uar.max).singleOrDefault(),
                    NumberOfRoomsMin: Enumerable.from($scope.allNumberOfRoomRanges).where(nrr => nrr.id === $scope.searchInput.NumberOfRooms)
                        .select(ear => ear.min).singleOrDefault(),
                    NumberOfRoomsMax: Enumerable.from($scope.allNumberOfRoomRanges).where(nrr => nrr.id === $scope.searchInput.NumberOfRooms)
                        .select(nrr => nrr.max).singleOrDefault(),
                    MortgageMin: Enumerable.from($scope.allMortgageRanges).where(mr => mr.id === $scope.searchInput.Mortgage)
                        .select(mr => mr.min).singleOrDefault(),
                    MortgageMax: Enumerable.from($scope.allMortgageRanges).where(mr => mr.id === $scope.searchInput.Mortgage)
                        .select(mr => mr.max).singleOrDefault(),
                    RentMin: Enumerable.from($scope.allRentRanges).where(rr => rr.id === $scope.searchInput.Rent)
                        .select(rr => rr.min).singleOrDefault(),
                    RentMax: Enumerable.from($scope.allRentRanges).where(rr => rr.id === $scope.searchInput.Rent)
                        .select(rr => rr.max).singleOrDefault(),
                    PriceMin: Enumerable.from($scope.allPriceRanges).where(pr => pr.id === $scope.searchInput.Price)
                        .select(pr => pr.min).singleOrDefault(),
                    PriceMax: Enumerable.from($scope.allPriceRanges).where(pr => pr.id === $scope.searchInput.Price)
                        .select(pr => pr.max).singleOrDefault(),
                    PricePerEstateAreaMin: Enumerable.from($scope.allPricePerEstateAreaRanges).where(pr => pr.id === $scope.searchInput.PricePerEstateArea)
                        .select(pr => pr.min).singleOrDefault(),
                    PricePerEstateAreaMax: Enumerable.from($scope.allPricePerEstateAreaRanges).where(pr => pr.id === $scope.searchInput.PricePerEstateArea)
                        .select(pr => pr.max).singleOrDefault(),
                    PricePerUnitAreaMin: Enumerable.from($scope.allPricePerUnitAreaRanges).where(pr => pr.id === $scope.searchInput.PricePerUnitArea)
                        .select(pr => pr.min).singleOrDefault(),
                    PricePerUnitAreaMax: Enumerable.from($scope.allPricePerUnitAreaRanges).where(pr => pr.id === $scope.searchInput.PricePerUnitArea)
                        .select(pr => pr.max).singleOrDefault(),
                    SortColumn: $scope.SortColumn,
                    SortDirection: $scope.SortDirectionName
                }
                
                $http.post("/api/web/files/searchinmap", searchQuery).success((data: any) => {
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

            $scope.$watch("searchInput.UsageType", u => {
                $scope.allPropertyTypes = Enumerable.from($scope.allPropertyTypeOptions).where(p => Enumerable.from(allPropertiesPerUsage).any(a => a.usage === u
                    && Enumerable.from(a.properties).any(up => up === p.id))).toArray();
                $scope.searchInput.PropertyType = "";
            });

            $scope.$watch("searchInput.PropertyType", p => {
                $scope.showEstateAreaSearch = Enumerable.from($scope.allowances).any(a => a.propertyType === p
                    && Enumerable.from(a.panels).any(at => at === $scope.allSearchPanels.estateAreaSearch));
                $scope.searchInput.EstateArea = "";

                $scope.showUnitAreaSearch = Enumerable.from($scope.allowances).any(a => a.propertyType === p
                    && Enumerable.from(a.panels).any(at => at === $scope.allSearchPanels.unitAreaSearch));
                $scope.searchInput.UnitArea = "";

                $scope.showNumberOfRoomsSearch = Enumerable.from($scope.allowances).any(a => a.propertyType === p
                    && Enumerable.from(a.panels).any(at => at === $scope.allSearchPanels.numberOfRoomsSearch));
                $scope.searchInput.NumberOfRooms = "";
            });

            $scope.$watch("searchInput.IntentionOfOwner", p => {
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

            $scope.$watch("Supplies", () => {
                $scope.NumberOfPagesArray = new Array($scope.NumberOfPages);
            });

            $scope.$watchGroup(["SortColumn", "SortDirection"], () => {
                $scope.getDataDown(1);
            });
            
            var mapListener = $scope.$on("MapIsReady", () => {
                if ($state.current.name === "files") {
                    $timeout($scope.getMapData(), 0);
//                    $scope.getMapData();
                }
            });

            $scope.$watch("selectAllSupplies", s => {
                if (s != null) {
                    angular.forEach($scope.Supplies, item => {
                        item.Selected = s;
                    });
                }
            });

            $scope.$watchGroup(["searchInput.PropertyType", "searchInput.IntentionOfOwner"], (i) => {
                $scope.showPricePerEstateAreaSearch = Enumerable.from($scope.allowances).any(a => a.propertyType === i[0]
                        && Enumerable.from(a.panels).any(at => at === $scope.allSearchPanels.pricePerEstateAreaSearch))
                    && (i[1] === 1);
                $scope.searchInput.PricePerEstateArea = "";

                $scope.showPricePerUnitAreaSearch = Enumerable.from($scope.allowances).any(a => a.propertyType === i[0]
                        && Enumerable.from(a.panels).any(at => at === $scope.allSearchPanels.pricePerUnitAreaSearch))
                    && (i[1] === 1);
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
                    $scope.TotalNumberOfItems = $scope.allSupplies.TotalNumberOfItems != null ? $scope.allSupplies.TotalNumberOfItems : 1;
                    $scope.NumberOfPages = $scope.allSupplies.TotalNumberOfPages != null ? $scope.allSupplies.TotalNumberOfPages : 1;
                    $scope.PageNumber = $scope.allSupplies.PageNumber != null ? $scope.allSupplies.PageNumber : 1;
                    $scope.searchInput.VicinityID = null;
                }
            }

            initialize();

            var listener = $rootScope.$on("EntityUpdated", () => {
                initialize();
            });

            $scope.$on("$destroy", listener);
            $scope.$on("$destroy", mapListener);
        }
    ]);

}