module JahanJooy.HaftDong.File {

    appModule.controller("NewSupplyModalController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "toastr", "$modalInstance",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams, toastr, $modalInstance) => {

            $scope.supply = {
                Property: $scope.property,
                OwnerCanBeContacted: true,
                AgencyContact: {
                    Phones: [],
                    Emails: [],
                    Addresses: []
                }
            };

            $scope.onSaveClick = () => {
                $scope.supply.PropertyId = $scope.property.ID;
                $http.post("/api/web/supplies/save", $scope.supply).success(() => {
                    toastr.success("آگهی با موفقیت ثبت شد.");
                    $modalInstance.close();
                });
            }

            $scope.onBackClick = () => {
                $modalInstance.dismiss('cancel');
            }
        }
    ]);

}