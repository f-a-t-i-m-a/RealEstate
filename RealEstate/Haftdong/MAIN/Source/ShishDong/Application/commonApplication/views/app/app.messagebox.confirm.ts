"use strict";

module JahanJooy.ShishDong {
    appModule.controller('ConfirmModalController', [
        "$scope", "$modalInstance", "message", "messageType",
        ($scope, $modalInstance, message, messageType) => {
            $scope.message = message;
            $scope.messageType = messageType;
        }
    ]);
}
