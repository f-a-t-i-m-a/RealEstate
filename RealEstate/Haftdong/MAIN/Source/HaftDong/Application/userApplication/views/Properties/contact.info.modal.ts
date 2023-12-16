module JahanJooy.HaftDong.File {
    appModule.controller("ContactInfoModalController", [
        '$scope', '$timeout', '$state', '$http', "toastr", "$modalInstance",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, toastr, $modalInstance) => {
            $scope.onBackClick = () => {
                $modalInstance.dismiss('cancel');
            }
        }
    ]);
}