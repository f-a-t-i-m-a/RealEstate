module JahanJooy.HaftDong.Request {
    appModule.controller("NewRequestController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "toastr", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'درخواست جدید';
            }
            scopes.store('NewRequestController', $scope);

            $scope.request = {};
            if ($stateParams.customerId !== null && $stateParams.customerId !== "") {
                $http.get("/api/web/customers/get/" + $stateParams.customerId, null)
                    .success((data: any) => {
                        $scope.request.Owner = data;
                    });
            }

            $scope.onSaveClick = () => {
                $http.post("/api/web/requests/save", $scope.request).success((data: any) => {
                    toastr.success("درخواست با موفقیت ثبت شد.");
                    $scope.$emit('EntityUpdated');
                    $state.go("^.details", { id: data.ID });
                });
            }

            $scope.onBackClick = () => {
                $state.go("^");
            }
        }
    ]);

} 