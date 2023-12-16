"use strict";

module JahanJooy.HaftDong {
    appModule.controller('LoginModalController', [
        "$scope", "authService", "$state",
        ($scope, authService, $state) => {

            $scope.signUp = () => {
                $scope.$dismiss();
                $state.go("signup");
            }

            $scope._rememberMe = true;

            $scope.forgotPassword = () => {
                $scope.$dismiss();
                $state.go("forgotPassword");
            }
            $scope.checkPassword = () => {
                if ($scope._password != null && $scope._password !== "")
                    $scope.showIcon = true;
                else
                    $scope.showIcon = false;
            }

            $scope.attemptLogin = (email, password, rememberMe) => {
                authService.attemptLogin(email, password, rememberMe).then(() => {
                    $scope.$close();
                    location.reload();
                });
            };
        }
    ]);
}
 