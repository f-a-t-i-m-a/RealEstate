"use strict";

module JahanJooy.ShishDong {
    appModule.controller('NavBarController', [
        "$scope","$http", "$state", "$modal", "authService",
        ($scope, $http, $state, $modal, authService) => {
            
            $scope.showLogin = () => {
                authService.showLogin().then(() => {
                    location.reload();
                });
            };

            $scope.showChangePassword = () => {
                $modal.open({
                    templateUrl: "/Application/commonApplication/views/app/app.auth.changepassword.html",
                    controller: "ChangePasswordModalController",
                    size: "sm"
                });
            };

            $scope.logout = () => {
                authService.logout();
                $state.go("welcome");
                location.reload();
            }

            $scope.signUp = () => {
                $state.go("signup");
            }
        }
    ]);
}
 