module JahanJooy.HaftDong.Contract {

    appModule.controller("ContractController", [
        '$scope', '$timeout', '$state', '$http', '$rootScope', '$q', 'scopes',
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $rootScope, $q, scopes) => {

            $scope.GetTitle = () => {
                return 'لیست قراردادها';
            }
            scopes.store('ContractController', $scope);

            $scope.FirstPage = 0;
            $scope.PageSize = 10;
            $scope.PageNumber = 1;
            $scope.TotalNumberOfItems = 0;
            $scope.SortDirection = true;
            $scope.ids = new Array();

            $scope.allUsageTypeOptions = Common.EnumExtentions.getValues(RealEstateAgency.Domain.Enums.Property.UsageType).select(i => { return { id: i, text: Common.localization.translateEnum("UsageType", i) } }).toArray();
            $scope.allPropertyTypeOptions = Common.EnumExtentions.getValues(RealEstateAgency.Domain.Enums.Property.PropertyType).select(i => { return { id: i, text: Common.localization.translateEnum("PropertyType", i) } }).toArray();
            $scope.allIntentionOfOwnerOptions = Common.EnumExtentions.getValues(RealEstateAgency.Domain.Enums.Property.IntentionOfOwner).select(i => { return { id: i, text: Common.localization.translateEnum("IntentionOfOwner", i) } }).toArray();
            $scope.allUsageTypes = $scope.allUsageTypeOptions;
            $scope.allPropertyTypes = $scope.allPropertyTypeOptions;
            $scope.allIntentionOfOwners = $scope.allIntentionOfOwnerOptions;

            $scope.searchInput = {};

            $scope.$watch("Contracts", () => {
                $scope.NumberOfPagesArray = new Array($scope.NumberOfPages);
            });

            $scope.getDataDown = (index) => {
                var promise = $q.defer();

                if ($scope.SortDirection)
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                else
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();

                $http.post("/api/web/contracts/search", {
                    StartIndex: ((index - 1) * $scope.PageSize),
                    PageSize: $scope.PageSize,
                    UsageType: $scope.searchInput.UsageType,
                    PropertyType: $scope.searchInput.PropertyType,
                    IntentionOfOwner: $scope.searchInput.IntentionOfOwner,
                    FromDate: $scope.searchInput.FromDate,
                    ToDate: $scope.searchInput.ToDate,
                    State: $scope.searchInput.State,
                    SortColumn: $scope.SortColumn,
                    SortDirection: $scope.sortDirection
                }).success((data: any) => {
                    $scope.Contracts = data;

                    if (data.Contracts.PageItems == null)
                        $scope.Contracts = [];
                    else
                        $scope.Contracts = data.Contracts.PageItems;

                    $scope.TotalNumberOfItems = data.Contracts.TotalNumberOfItems;
                    $scope.NumberOfPages = data.Contracts.TotalNumberOfPages;
                    $scope.PageNumber = data.Contracts.PageNumber;

                    promise.resolve();
                }).error(() => {
                    promise.reject();
                });
                return promise.promise;
            };

            $scope.onSearch = () => {
                $scope.getDataDown(1);
            }

//            $scope.ondelete = () => {
//                    $http.post("/api/contracts/deleteallindices", null).success((data: any) => {
//                        $http.get("/api/customers/all", null).success((data: any) => {
//                            $scope.AllCustomers = data;
//                        });
//                        $scope.$emit('EntityUpdated');
//                    });
//            }
            $scope.$watchGroup(["SortColumn", "SortDirection"], s => {
                $scope.getDataDown(1);
            });

            $scope.resetIdsList = (contact) => {
                if (contact.Selected) {
                    $scope.ids.push(contact.ID);
                } else {
                    var index = $scope.ids.indexOf(contact.ID);
                    if (index > -1)
                        $scope.ids.splice(index, 1);
                }
            };

            $scope.$watch("selectAllContacts", p => {
                if (p != null) {
                    angular.forEach($scope.Contacts, item => {
                        item.Selected = p;
                    });
                }
            });

            function initialize() {
                if ($scope.SortDirection)
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                else
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();
            }

            initialize();

            var listener = $rootScope.$on('EntityUpdated', () => {
                initialize();
            });

            $scope.$on('$destroy', listener);
        }
    ]);

}