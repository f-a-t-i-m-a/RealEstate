module JahanJooy.HaftDong.File {
    appModule.controller("SupplyDetailModalController", [
        '$scope', '$timeout', '$state', '$http', "toastr", "$modalInstance",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, toastr, $modalInstance) => {

            $scope.inEditMode = !($scope.supply.ID == null);

            $scope.onBackClick = () => {
                $modalInstance.dismiss('cancel');
            }
        }
    ]);
}