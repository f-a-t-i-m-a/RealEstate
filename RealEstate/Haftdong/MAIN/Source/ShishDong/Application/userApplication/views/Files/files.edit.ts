module JahanJooy.ShishDong.File {
    appModule.controller("EditFileController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "toastr", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'ویرایش ملک';
            }
            scopes.store('EditFileController', $scope);

            $scope.$watch("property", p => {
                if (p != null && p.PriceSpecificationType === 1) {
                    p.Price = p.TotalPrice;
                } else if (p != null && p.PriceSpecificationType === 2) {
                    p.Price = p.PricePerEstateArea;
                } else if (p != null && p.PriceSpecificationType === 3) {
                    p.Price = p.PricePerUnitArea;
                }
            });

            $scope.onSaveClick = () => {
                if ($scope.property.GeographicLocation != null &&
                    $scope.property.GeographicLocation.X != null &&
                    $scope.property.GeographicLocation.Y != null) {
                    $scope.property.GeographicLocation = {
                        Lat: $scope.property.GeographicLocation.X,
                        Lng: $scope.property.GeographicLocation.Y
                    };
                }

                $http.post("/api/haftdong/files/update", $scope.property).success(() => {
                    toastr.success("ملک با موفقیت ویرایش شد.");
                    $state.go("^");
                    $scope.$emit('EntityUpdated');
                });
            }

            $scope.onBackClick = () => {
                $state.go("^");
            }
        }
    ]);

}