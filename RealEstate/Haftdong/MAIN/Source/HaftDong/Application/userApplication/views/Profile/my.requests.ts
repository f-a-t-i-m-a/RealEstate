module JahanJooy.HaftDong.Request {

    appModule.controller("MyRequestController", [
        '$scope', '$timeout', '$state', '$http', '$rootScope', '$q', 'scopes',
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $rootScope, $q, scopes) => {

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

            $scope.isCollapse = true;

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

                    $http.post("/api/web/requests/myrequests", {
                        StartIndex: ((index - 1) * $scope.PageSize),
                        PageSize: $scope.PageSize,
                        IsPublic: false,
                        SortColumn: $scope.SortColumn,
                        SortDirection: sortDirection
                    }).success((data: any) => {
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

                    }).error(() => {
                        promise.reject();
                    });
                    return promise.promise;
                }
                return null;
            };

            
            $scope.$watchGroup(["SortColumn", "SortDirection"], () => {
                $scope.getDataDown(1);
            });

            $scope.$watch("selectAllRequests", p => {
                if (p != null) {
                    angular.forEach($scope.Requests, item => {
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

            var listener = $rootScope.$on('EntityUpdated', () => {
                initialize();
            });

            $scope.$on('$destroy', listener);
        }
    ]);

}