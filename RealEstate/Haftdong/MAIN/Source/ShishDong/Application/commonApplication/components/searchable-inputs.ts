module JahanJooy.ShishDong.Components {
    ngModule.directive('jjSelectVicinity', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-data/vicinity.html',
            scope: {
                model: '=',
                jjRequired: '=',
                jjAllowClear: '=',
                onSelect: '&',
                jjDisabled: '=',
                jjVicinities: '='
            },
            controller: ($scope, $http) => {

                $scope.PageNumber = 0;
                $scope.PageSize = 5;

                $scope.$watch("jjVicinities", v => {
                    if (v != null && v.length !== 0) {
                        $scope.vicinities = v;
                    } else {
                        $scope.vicinities = [];
                    }
                });

                $scope.fetch = function (searchText, $event) {
                    // no event means first load!
                    if ($event) {
                        $scope.PageNumber ++;
                        $scope.searchVicinities(searchText);
                    }
                }


                $scope.searchVicinities = (searchText) => {
                    if (searchText != null && searchText.length >= 2) {
                        $http.post("/api/haftdong/vicinities/search", {
                            SearchText: searchText,
                            CanContainPropertyRecordsOnly: true,
                            StartIndex: $scope.PageNumber * $scope.PageSize,
                            PageSize: $scope.PageSize
                        }).success((data: any) => {
                            $scope.vicinities = $scope.vicinities.concat(data.Vicinities.PageItems);
                        });
                    }
                }
            }
        };
    });

    ngModule.directive('jjSelectMultipleVicinity', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-select-data/vicinities.html',
            scope: {
                model: '=',
                jjRequired: '=',
                jjAllowClear: '=',
                onSelect: '&',
                jjDisabled: '=',
                jjVicinities: '='
            },
            controller: ($scope, $http) => {

                $scope.PageNumber = 0;
                $scope.PageSize = 5;

                $scope.vicinities = [];
                $scope.$watch("jjVicinities", v => {
                    if (v != null && v.length !== 0) {
                        $scope.vicinities = v;
                    } else {
                        $scope.vicinities = [];
                    }
                });

                $scope.fetch = function (searchText, $event) {
                    // no event means first load!
                    if ($event) {
                        $scope.PageNumber ++;
                        $scope.searchVicinities(searchText);
                    }
                }

                $scope.onSelected = function () {
                    $scope.vicinities = [];
                }
                $scope.searchVicinities = (searchText) => {
                    if (searchText != null && searchText.length >= 2) {
                        $http.post("/api/haftdong/vicinities/search", {
                            SearchText: searchText,
                            CanContainPropertyRecordsOnly: true,
                            StartIndex: $scope.PageNumber * $scope.PageSize,
                            PageSize: $scope.PageSize
                        }).success((data: any) => {
                              $scope.vicinities = $scope.vicinities.concat(data.Vicinities.PageItems);
                        });
                    }
                }
            }
        };
    });
}