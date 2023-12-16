"use strict";

module JahanJooy.HaftDong {
    appModule.controller('NavBarController', [
        "$scope","$http", "$state", "$modal", "authService",
        ($scope, $http, $state, $modal, authService) => {
            
            $scope.showLogin = () => {
                authService.showLogin().then(() => {
                    $state.go("dashboard");
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
            }

            //temporary removed FromApi
          /*  $scope.signUp = () => {
                $state.go("signup");
            }*/

//            $scope.quickSearch = () => {
//                var modalInstance = $modal.open({
//                    templateUrl: "Application/views/customers/customers.quick.search.modal.html",
//                    controller: "CustomerQuickSearchController",
//                    scope: $scope,
//                    size: 'lg'
//                });
//                modalInstance.result.then(() => {
//                    modalInstance.close();
//                });
//            }
        }
    ]);
}
 