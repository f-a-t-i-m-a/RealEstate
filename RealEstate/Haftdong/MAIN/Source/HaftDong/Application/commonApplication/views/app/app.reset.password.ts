module JahanJooy.HaftDong {
    import ContactMethodType = RealEstateAgency.Domain.Enums.User.ContactMethodType;
    appModule.controller("ResetPasswordController",
    [
        '$scope', '$timeout', '$state', '$http', 'authService', 'scopes',
        ($scope, $timeout, $state: angular.ui.IStateService, $http, authService, scopes) => {


            $scope.showIcon = false;

            $scope.checkPassword = () => {
                if ($scope.user.NewPassword == null ||
                    ($scope.user.NewPassword != null && $scope.user.NewPassword.length < 6))
                    $scope.PasswordLengthMinimumRequirement = true;
                else
                    $scope.PasswordLengthMinimumRequirement = false;

                if ($scope.user.NewPassword != null && $scope.user.NewPassword !== "")
                    $scope.showIcon = true;
                else
                    $scope.showIcon = false;
            }

            $scope.onResetPasswordClick = () => {
                $http.post("/api/web/account/resetpassword", $scope.user)
                    .success((data: any) => {
                        $state.go("welcome");
                    });
            };
        }
    ]);

}