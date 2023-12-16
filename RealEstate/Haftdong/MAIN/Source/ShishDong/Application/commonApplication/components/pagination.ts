module JahanJooy.ShishDong.Components {
    appModule.directive('jjPagination', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-directives/pagination.html',
            scope: {
                pageNumber: "=",
                numberOfPages: "=",
                totalNumberOfItems: "=",
                pageSize: "=",
                onChangePageClick: '&'
            },
            controller: ($scope, $http: ng.IHttpService, $state, $modal, $stateParams, toastr, allPropertiesPerUsage, allPriceSpecificationTypePerProperty) => {

                $scope.MaxPageNumberToShow = 0;
                $scope.MinPageNumberToShow = 0;
                

                $scope.$watchGroup(["pageNumber", "numberOfPages"], (p) => {
                    if (p[1] != null) {
                        $scope.MaxPageNumberToShow = Math.min(p[0] + 2, p[1]);
                        $scope.MinPageNumberToShow = Math.max(p[0] - 2, 1);
                    }
                });

                $scope.$watchGroup(["pageNumber", "pageSize", "totalNumberOfItems"], (p) => {
                    $scope.TotalNumberPerPage = Math.min(p[0] * p[1], p[2]);
                });

                $scope.$watch("numberOfPages", p => {
                    $scope.NumberOfPagesArray = new Array(p);
                });

                $scope.showPrevious = () => {
                    if ($scope.pageNumber > 1)
                        $scope.onChangePageClick({ page: $scope.pageNumber - 1 });
                };

                $scope.showNext = () => {
                    if ($scope.pageNumber < $scope.numberOfPages)
                        $scope.onChangePageClick({ page: $scope.pageNumber + 1 });
                };

                $scope.showFirst = () => {
                    $scope.onChangePageClick({ page: 1 });
                };

                $scope.showLast = () => {
                    $scope.onChangePageClick({ page: $scope.numberOfPages });
                };
            }
        }
    });
}