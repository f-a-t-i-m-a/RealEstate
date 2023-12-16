module JahanJooy.HaftDong.Dashboard {
    import EntityType = RealEstateAgency.Domain.Enums.UserActivity.EntityType;
    import SortDirectionType = RealEstateAgency.Util.Models.Base.SortDirectionType;
    import DashboardSortColumn = RealEstateAgency.Util.Models.Dashboard.DashboardSortColumn;
    ngModule.controller("DashboardController", [
        '$scope', '$rootScope', '$http', '$state', '$modal', "messageBoxService", "$stateParams",
        'toastr', 'authService', '$filter', '$sce', 'scopes',
        ($scope, $rootScope, $http: ng.IHttpService, $state, $modal, messageBoxService, $stateParams,
            toastr, authService, $filter, $sce, scopes) => {

            $scope.GetTitle = () => {
                return 'داشبورد';
            }
            scopes.store('DashboardController', $scope);

            $scope.PageSize = 5;
            $scope.PageNumber = 1;
            $scope.DataTypes = [EntityType.ApplicationUser, EntityType.Customer, EntityType.Request, EntityType.Property];
            
            $scope.onSearchClick = (index) => {
                if ($scope.searchText != null && $scope.searchText.length >= 3) {
                    $http.post("/api/web/dashboard/userquicksearch",
                    {
                        StartIndex: ((index - 1) * $scope.PageSize),
                        PageSize: $scope.PageSize,
                        Text: $scope.searchText,
                        DataTypes: $scope.DataTypes,
                        SortColumn: DashboardSortColumn.CreationTime,
                        SortDirection: SortDirectionType.Asc
                    }).success((data: any) => {
                        $scope.Items = data.SearchResults.PageItems;
                        $scope.TotalNumberOfItems = data.SearchResults.TotalNumberOfItems;
                        $scope.NumberOfPages = data.SearchResults.TotalNumberOfPages;
                        $scope.PageNumber = data.SearchResults.PageNumber;

                        $scope.Items.forEach(d => {
                            switch (d.DataType) {
                            case "Property":
                                d.DetailState = "files.details";
                                break;
                            case "Request":
                                d.DetailState = "requests.details";
                                break;
                            case "Customer":
                                d.DetailState = "customers.details";
                                break;
                            case "Contract":
                                d.DetailState = "contracts.details";
                                break;
                            case "ApplicationUser":
                                d.DetailState = "users.details";
                                break;
                            }
                        });

                        $state.transitionTo($state.$current, { searchtext: $scope.searchText }, { location: "replace", notify: false, inherit: false, reload: false });
                    });
                }
            }

            $scope.highlight = (text, search) => {
                if (!search) {
                    return $sce.trustAsHtml(text);
                }
                return $sce.trustAsHtml(text.replace(new RegExp(search, 'gi'), '<span class="highlightedText">$&</span>'));
            };

            $scope.onDetailClick = (item) => {
                $state.go(item.DetailState, { id: item.ID });
            }

            $scope.$watch("Items", () => {
                $scope.NumberOfPagesArray = new Array($scope.NumberOfPages);
            });

            function initialize() {
                var user = authService.getCurrentUser();
                if (user != null && user.user != null
                    && moment(user.user.LicenseActivationTime).format("YYYY/MM/DD") >= moment().format("YYYY/MM/DD")) {
                    $scope.activationTime = moment.duration(moment(user.user.LicenseActivationTime).diff(moment())).humanize()
                        + " مانده به پایان اعتبار مجوز شما";
                    $scope.isExpired = false;
                } else {
                    $scope.activationTime = "اعتبار مجوز شما به پایان رسیده است";
                    $scope.isExpired = true;
                }
                if ($stateParams.searchtext != null) {
                    $scope.searchText = $stateParams.searchtext;
                    $scope.onSearchClick();
                }
            }

            initialize();
        }
    ]);

}