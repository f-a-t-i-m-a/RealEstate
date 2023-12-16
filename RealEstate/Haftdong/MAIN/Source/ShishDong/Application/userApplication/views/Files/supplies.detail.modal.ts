module JahanJooy.ShishDong.File {
    appModule.controller("SupplyDetailModalController", [
        '$scope', '$timeout', '$state', '$http', "toastr", "$modalInstance",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, toastr, $modalInstance) => {

            $scope.inEditMode = !($scope.supply.ID == null);
            $scope.$watch("supply",
                c => {
                    var s = c;
//                    if (c !== undefined && c !== null) {
//                        c.ContactPhoneNumbers = Enumerable.from<any>(c.ContactPhoneNumbers)
//                            .where(pn => pn !== "" && pn !== null)
//                            .toArray();
//
//                        c.OwnerPhoneNumbers = Enumerable.from<any>(c.OwnerPhoneNumbers)
//                            .where(pn => pn !== "" && pn !== null)
//                            .toArray();
//                    }
                });

            $scope.onBackClick = () => {
                $modalInstance.dismiss('cancel');
            }
        }
    ]);
}