module JahanJooy.HaftDong.Contract {

    import EnumExtentions = Common.EnumExtentions;
    import PropertyType = RealEstateAgency.Domain.Enums.Property.PropertyType;
    appModule.directive('jjContract', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/userApplication/views/Contracts/contracts.directive.html',
            scope: {
                contract: "=",
                onBackClick: '&',
                onSaveClick: '&'
            },

            controller: ($scope, $http: ng.IHttpService, $state, $modal, messageBoxService, $stateParams, toastr, $q, allPropertiesPerUsage) => {

                $scope.isCollapse = true;
                $scope.FirstPage = 0;
                $scope.PageSize = 3;
                $scope.PageNumber = 1;
                $scope.DataIsCompeleted = false;
                $scope.TotalNumberOfItems = 0;
                $scope.searchInput = {};
                $scope.SortDirection = true;
                $scope.update = false;
//                $scope.showPropertyPanel = false;
                var firstTime = true;
                $scope.isPropertySelected = false;

                $scope.hasDeputyForSeller = true;
                $scope.hasDeputyForBuyer = true;
                $scope.editMode = false;
                $scope.disabledUnitArea = true;
                $scope.disabledEstateArea = true;

                $scope.areas = [
                    {
                        propertyType: PropertyType.Land,
                        area: [
                            "EstateArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.AgriculturalLand,
                        area: [
                            "EstateArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Garden,
                        area: [
                            "EstateArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.House,
                        area: [
                            "EstateArea",
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.OldHouse,
                        area: [
                            "EstateArea",
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Shed,
                        area: [
                            "EstateArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Tenement,
                        area: [
                            "EstateArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Villa,
                        area: [
                            "EstateArea",
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Apartment,
                        area: [
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Complex,
                        area: [
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Tower,
                        area: [
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.GardenTower,
                        area: [
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Office,
                        area: [
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.OfficialResidency,
                        area: [
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Commercial,
                        area: [
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.CommercialResidency,
                        area: [
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Suite,
                        area: [
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Penthouse,
                        area: [
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Shop,
                        area: [
                            "UnitArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Factory,
                        area: [
                            "EstateArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.WorkShop,
                        area: [
                            "EstateArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.RepairShop,
                        area: [
                            "EstateArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.StoreHouse,
                        area: [
                            "EstateArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Parking,
                        area: [
                            "EstateArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.Gym,
                        area: [
                            "EstateArea"
                        ]
                    },
                    {
                        propertyType: PropertyType.CityService,
                        area: [
                            "EstateArea"
                        ]
                    }
                ];

                $scope.$watch("contract.UsageType", u => {
                    if (!$scope.isPropertySelected) {
                        if ($scope.contract != null) {
                            if (!firstTime) {
                                $scope.contract.PropertyType = "";
                            }
                            firstTime = false;
                            var allPropertyTypes = EnumExtentions.getValues(PropertyType).select(i => { return { id: i, text: Common.localization.translateEnum("PropertyType", i) } }).toArray();
                            $scope.propertyTypeOptions = Enumerable.from(allPropertyTypes).where(p => Enumerable.from(allPropertiesPerUsage).any(a => a.usage === u
                                && Enumerable.from(a.properties).any(up => up === p.id))).toArray();
                        }
                    } else {
                        var allPropertyTypes2 = EnumExtentions.getValues(PropertyType).select(i => { return { id: i, text: Common.localization.translateEnum("PropertyType", i) } }).toArray();
                        $scope.propertyTypeOptions = Enumerable.from(allPropertyTypes2).where(p => Enumerable.from(allPropertiesPerUsage).any(a => a.usage === u
                            && Enumerable.from(a.properties).any(up => up === p.id))).toArray();
                    }

                });

                $scope.$watch("contract.PropertyType", p => {
                    $scope.disabledUnitArea = !Enumerable.from($scope.areas).any(a => a.propertyType === p
                        && Enumerable.from(a.area).any(ar => ar === "UnitArea"));
                    $scope.disabledEstateArea = !Enumerable.from($scope.areas).any(a => a.propertyType === p
                        && Enumerable.from(a.area).any(ar => ar === "EstateArea"));
                });


                $scope.$watch("contract.Seller.ID", s => {
                    if (s != null) {
                        $http.get("/api/web/supplies/getcustomersupplies/" + s, null).success((data: any) => {
                            $scope.Supplies = data;
                        });
                    }
                });
                $scope.$watch("contract.Buyer.ID", b => {
                    if (b != null) {
                        $http.get("/api/web/requests/getcustomernewrequests/" + b, null).success((data: any) => {
                            $scope.Requests = data;
                        });
                    }
                });

//                $scope.onNewSeller = () => {
//                    $scope.title = 'فروشنده جدید';
//                    $scope.update = false;
//                    var modalInstance = $modal.open({
//                        templateUrl: "Application/userApplication/views/Customers/customers.new.modal.html",
//                        controller: "NewCustomerModalController",
//                        scope: $scope,
//                        size: "lg"
//                    });
//                    modalInstance.result.then((data: any) => {
//                        $scope.$emit('EntityUpdated');
//                        $scope.update = true;
//                        $scope.contract.Seller = data.newCustomer;
//                        $scope.newSeller = [data.newCustomer];
//                    });
//                }

//                $scope.onNewBuyer = () => {
//                    $scope.title = 'خریدار جدید';
//                    $scope.update = false;
//                    var modalInstance = $modal.open({
//                        templateUrl: "Application/userApplication/views/Customers/customers.new.modal.html",
//                        controller: "NewCustomerModalController",
//                        scope: $scope,
//                        size: "lg"
//                    });
//                    modalInstance.result.then((data: any) => {
//                        $scope.contract.Buyer = data.newCustomer;
//                        $scope.newBuyer = [data.newCustomer];
//                        $scope.update = true;
//                        $scope.$emit('EntityUpdated');
//                    });
//                }


                $scope.onSearchPropertyOwnerClick = (index) => {
                    var promise = $q.defer();

                    if ($scope.SortDirection)
                        $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                    else
                        $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();

                    $http.post("/api/web/properties/search", {
                        StartIndex: ((index - 1) * $scope.PageSize),
                        PageSize: $scope.PageSize,
                        OwnerName: $scope.searchInput.OwnerName,
                        SortColumn: $scope.SortColumn,
                        SortDirection: $scope.SortDirectionName
                    }).success((data: any) => {
                        if (data.Properties.PageItems == null)
                            $scope.Properties = [];
                        else
                            $scope.Properties = data.Properties.PageItems;

                        $scope.TotalNumberOfItems = data.Properties.TotalNumberOfItems;
                        $scope.NumberOfPages = data.Properties.TotalNumberOfPages;
                        $scope.PageNumber = data.Properties.PageNumber;
                        if ($scope.searchInput.IntentionOfOwner != null)
                            $scope.showSortablePrice = true;
                        else
                            $scope.showSortablePrice = false;

                        promise.resolve();
                    }).error(() => {
                        promise.reject();
                    });
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
                }

                $scope.$watch("contract.IntentionOfOwner", i => {
                    if (i != null && i === 2) {
                        $scope.showSalePanel = true;
                        $scope.showFullMortgagePanel = false;
                        $scope.showRentPanel = false;
                        $scope.showDailyRentPanel = false;
                        $scope.contract.Rent = "";
                    } else if (i != null && i === 3) {
                        $scope.showFullMortgagePanel = true;
                        $scope.showSalePanel = false;
                        $scope.showRentPanel = false;
                        $scope.showDailyRentPanel = false;
                        $scope.contract.Rent = 0;
                    } else if (i != null && i === 1) {
                        $scope.showRentPanel = true;
                        $scope.showFullMortgagePanel = false;
                        $scope.showSalePanel = false;
                        $scope.showDailyRentPanel = false;
                    } else if (i != null && i === 4) {
                        $scope.showDailyRentPanel = true;
                        $scope.showRentPanel = false;
                        $scope.showFullMortgagePanel = false;
                        $scope.showSalePanel = false;
                    } else {
                        $scope.showSalePanel = false;
                        $scope.showRentPanel = false;
                        $scope.showFullMortgagePanel = false;
                        $scope.showDailyRentPanel = false;
                        $scope.contract.Rent = "";
                    }


                });

                $scope.onSupplyClick = (supply) => {
//                    if ($scope.isPropertySelected) {
//                        $scope.isPropertySelected = false;
//                    } else {
                    $scope.disabledUnitArea = false;
                    $scope.disabledEstateArea = false;

                    $scope.contract.PropertyID = supply.Property.ID;
                    $scope.contract.SupplyID = supply.ID;
                    $scope.contract.UsageType = supply.Property.UsageType;
                    $scope.contract.IntentionOfOwner = supply.IntentionOfOwner;
                    $scope.contract.PropertyType = supply.Property.PropertyType;
                    $scope.contract.LicencePlate = supply.Property.LicencePlate;
                    $scope.contract.ContractEstateArea = supply.Property.EstateArea;
                    $scope.contract.ContractUnitArea = supply.Property.UnitArea;
                    $scope.contract.Address = supply.Property.Address;
                    $scope.contract.ContractTotalPrice = supply.TotalPrice;
                    $scope.contract.ContractMortgage = supply.Mortgage;
                    $scope.contract.ContractRent = supply.Rent;
                    $scope.isPropertySelected = true;

                    if ($scope.contract.PropertyType === PropertyType.Land ||
                        $scope.contract.PropertyType === PropertyType.Garden ||
                        $scope.contract.PropertyType === PropertyType.Tenement ||
                        $scope.contract.PropertyType === PropertyType.Shed ||
                        $scope.contract.PropertyType === PropertyType.AgriculturalLand ||
                        $scope.contract.PropertyType === PropertyType.Factory ||
                        $scope.contract.PropertyType === PropertyType.WorkShop ||
                        $scope.contract.PropertyType === PropertyType.RepairShop ||
                        $scope.contract.PropertyType === PropertyType.StoreHouse ||
                        $scope.contract.PropertyType === PropertyType.Parking ||
                        $scope.contract.PropertyType === PropertyType.Gym ||
                        $scope.contract.PropertyType === PropertyType.CityService) {
                        $scope.disabledUnitArea = true;
                    }

                    if ($scope.contract.PropertyType === PropertyType.Complex ||
                        $scope.contract.PropertyType === PropertyType.Tower ||
                        $scope.contract.PropertyType === PropertyType.GardenTower ||
                        $scope.contract.PropertyType === PropertyType.Office ||
                        $scope.contract.PropertyType === PropertyType.OfficialResidency ||
                        $scope.contract.PropertyType === PropertyType.Apartment ||
                        $scope.contract.PropertyType === PropertyType.Penthouse ||
                        $scope.contract.PropertyType === PropertyType.Suite ||
                        $scope.contract.PropertyType === PropertyType.Shop ||
                        $scope.contract.PropertyType === PropertyType.OfficialResidency ||
                        $scope.contract.PropertyType === PropertyType.Commercial ||
                        $scope.contract.PropertyType === PropertyType.CommercialResidency) {
                        $scope.disabledEstateArea = true;
                    }
//                    }
                };

                $scope.onSupplyClear = () => {
                    $scope.disabledUnitArea = true;
                    $scope.disabledEstateArea = true;

                    $scope.contract.PropertyID = null;
                    $scope.contract.SupplyID = null;
                    $scope.contract.UsageType = null;
                    $scope.contract.IntentionOfOwner = null;
                    $scope.contract.PropertyType = null;
                    $scope.contract.LicencePlate = null;
                    $scope.contract.ContractEstateArea = null;
                    $scope.contract.ContractUnitArea = null;
                    $scope.contract.Address = null;
                    $scope.contract.ContractTotalPrice = null;
                    $scope.contract.ContractMortgage = null;
                    $scope.contract.ContractRent = null;
                    $scope.isPropertySelected = false;
                }

                $scope.onRequestClick = (request) => {
                    $scope.contract.RequestID = request.ID;
                };

                $scope.onRequestClear = () => {
                    $scope.contract.RequestID = null;
                };
            }
        }
    });
}