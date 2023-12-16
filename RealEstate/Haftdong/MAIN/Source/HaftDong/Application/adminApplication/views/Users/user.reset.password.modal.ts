module JahanJooy.HaftDong.Customers {
    appModule.controller("UserResetPasswordModalController", [
        '$scope', '$http', '$state', '$modalInstance', 'messageBoxService','toastr',
        ($scope, $http: ng.IHttpService, $state, $modalInstance, messageBoxService, toastr) => {

            $scope.title = $scope.title;
            $scope.InvalidPassword = false;

            $scope.cancel = () => {
                $modalInstance.dismiss('cancel');
            };

            $scope.onSave = () => {
                $http.post("/api/web/account/resetpasswordbyadmin", {
                    UserID: $scope.UserId,
                    NewPassword:$scope.NewPassword
                }).success((data: any) => {
                    toastr.success("رمز با موفقیت تغییر کرد.");
                    $modalInstance.close({ newCustomer: data });
                });
            };

            $scope.confirmPassword = () => {
                if ($scope.NewPassword !== $scope.ConfirmPassword)
                    $scope.InvalidPassword = true;
                else
                    $scope.InvalidPassword = false;
            }

        }
    ]);

}