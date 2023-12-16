"use strict";

module JahanJooy.HaftDong {
    appModule.controller('ConfirmModalController', [
        "$scope", "$modalInstance", "message", "messageType",
        ($scope, $modalInstance, message, messageType) => {
            $scope.message = message;
            $scope.messageType = messageType;
        }
    ]);
}
