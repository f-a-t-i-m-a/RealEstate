"use strict";

module JahanJooy.ShishDong {
    appModule.controller('ChangePasswordModalController', [
        "$scope", "apiAccount",
        ($scope, apiAccount) => {

            $scope.changePassword = (current, changed, confirmation) => {
                apiAccount.changePassword(current, changed, confirmation)
                    .success(() => {
                        $scope.$close();
                    });
            };
        }
    ]);
}
 