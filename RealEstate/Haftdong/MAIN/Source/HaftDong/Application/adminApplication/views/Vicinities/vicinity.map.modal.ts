module JahanJooy.HaftDong.Customers {
    appModule.controller("VicinityMapModalController", [
        '$scope', '$http', '$state', '$modalInstance', 'messageBoxService', 'toastr', "mapApiKey",
        ($scope, $http: ng.IHttpService, $state, $modalInstance, messageBoxService, toastr, mapApiKey) => {

            $scope.vicinity = $scope.vicinity;
            if ($scope.vicinity != null && $scope.vicinity.CenterPoint != null) {
                $scope.mapStr = "http://maps.googleapis.com/maps/api/staticmap?center=" +
                $scope.vicinity.CenterPoint.Y + "," + $scope.vicinity.CenterPoint.X
                + "&zoom=13&size=640x500&key=" + mapApiKey + "&markers=color:red%7C" +
                $scope.vicinity.CenterPoint.Y + "," + $scope.vicinity.CenterPoint.X;
            }

            $scope.cancel = () => {
                $modalInstance.dismiss('cancel');
            };
        }
    ]);

}