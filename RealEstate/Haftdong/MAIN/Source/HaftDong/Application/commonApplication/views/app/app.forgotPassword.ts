module JahanJooy.HaftDong {
    import ContactMethodType = RealEstateAgency.Domain.Enums.User.ContactMethodType;
    appModule.controller("ForgotPasswordController",
    [
        '$scope', '$timeout', '$state', '$http', 'authService', 'scopes',
        ($scope, $timeout, $state: angular.ui.IStateService, $http, authService, scopes) => {

            $scope.onPasswordRecoveryBySmsClick = () => {

                $http.post("/api/web/users/ForgotPasswordBySms", $scope.user)
                    .success((data: any) => {
                        $state.go("resetPassword");
//                        $scope.property.Owner = data;
                    });
            };
            $scope.onPasswordRecoveryByEmailClick = () => {

                $http.post("/api/web/users/ForgotPasswordByEmail", $scope.user)
                    .success((data: any) => {
                        $state.go("resetPassword");
//                        $scope.property.Owner = data;
                    });
            };
        }
    ]);

}