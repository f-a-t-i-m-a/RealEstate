module JahanJooy.HaftDong.Log {
    ngModule.controller("ViewLogModalController", [
        '$scope', '$http', '$state', '$modalInstance', 'toastr',
        ($scope, $http: ng.IHttpService, $state, $modalInstance, toastr) => {

            $scope.onCancel = () => {
                $modalInstance.dismiss('cancel');
            };
        }
    ]);

}