module JahanJooy.HaftDong.Property {

    appModule.controller("NewPropertyController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "toastr", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'ملک جدید';
            }
            scopes.store('NewPropertyController', $scope);

            $scope.property = {
                VicinityID:0
            };

            if ($stateParams.customerId !== null && $stateParams.customerId !== "") {
                $http.get("/api/web/customers/get/" + $stateParams.customerId, null)
                    .success((data: any) => {
                        $scope.property.Owner = data;
                    });
            }

            $scope.onSaveClick = () => {
                if ($scope.property.Vicinity != null && $scope.property.Vicinity != undefined) {
                    $scope.property.VicinityID = $scope.property.Vicinity.ID;
                }
                $http.post("/api/web/properties/save", $scope.property).success((data: any) => {
                    toastr.success("ملک با موفقیت ثبت شد.");
                    $scope.$emit('EntityUpdated');
                    $state.go("files.details", { id: data.ID});
                });
            }

            $scope.onBackClick = () => {
                $state.go("^");
            }
        }
    ]);

}