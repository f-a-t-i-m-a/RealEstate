module JahanJooy.HaftDong.Customer {

    appModule.controller("CustomerController", [
        '$scope', '$timeout', '$state', '$http', '$rootScope', '$q', 'scopes',
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $rootScope, $q, scopes) => {

            $scope.GetTitle = () => {
                return 'لیست مشتریان';
            }
            scopes.store('CustomerController', $scope);

            $scope.FirstPage = 0;
            $scope.PageSize = 10;
            $scope.PageNumber = 1;
            $scope.TotalNumberOfItems = 0;
            $scope.SortDirection = true;
            $scope.searchInput = {};

            $scope.currentDateMinusTwoWeeks = moment(new Date()).add(-2, "w").format("YYYY/MM/DD");
            $scope.isTwoWeeksAgo = (customer) => {
                return moment(customer.LastVisitTime).format("YYYY/MM/DD") >= $scope.currentDateMinusTwoWeeks;
            }

            $scope.ListOptions = [
                { id: 1, text: "درخواست" },
                { id: 2, text: "ملک" },
                { id: 3, text: "همه" }
            ];

            $scope.$watch("Customers", () => {
                $scope.NumberOfPagesArray = new Array($scope.NumberOfPages);
            });

            $scope.getDataDown = (index) => {
                var promise = $q.defer();

                if ($scope.SortDirection)
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                else
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();


                $http.post("/api/web/customers/search", {
                    StartIndex: ((index - 1) * $scope.PageSize),
                    PageSize: $scope.PageSize,
                    DisplayName: $scope.searchInput.DisplayName,
                    Email: $scope.searchInput.Email,
                    PhoneNumber: $scope.searchInput.PhoneNumber,
                    IsArchived: $scope.searchInput.IsArchived,
                    IsDeleted: $scope.searchInput.IsDeleted,
                    IntentionOfVisit: $scope.searchInput.IntentionOfVisit,
                    SortColumn: $scope.SortColumn,
                    SortDirection: $scope.sortDirection
                }).success((data: any) => {
                    $scope.Customers = data;

                    if (data.Customers.PageItems == null)
                        $scope.Customers = [];
                    else
                        $scope.Customers = data.Customers.PageItems;

                    $scope.TotalNumberOfItems = data.Customers.TotalNumberOfItems;
                    $scope.NumberOfPages = data.Customers.TotalNumberOfPages;
                    $scope.PageNumber = data.Customers.PageNumber;

                    promise.resolve();
                }).error(() => {
                    promise.reject();
                });
                return promise.promise;
            };

            $scope.onSearch = () => {
                $scope.getDataDown(1);
            }

            $scope.$watchGroup(["SortColumn", "SortDirection"], s => {
                $scope.getDataDown(1);
            });

            $scope.$watch("selectAllCustomer", c => {
                if (c != null) {
                    angular.forEach($scope.Customers, item => {
                        item.Selected = c;
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