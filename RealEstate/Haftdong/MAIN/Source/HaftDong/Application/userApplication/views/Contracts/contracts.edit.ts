module JahanJooy.HaftDong.Contract {

    appModule.controller("EditContractController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "$rootScope", "toastr", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams, $rootScope, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'ویرایش قرارداد';
            }
            scopes.store('EditContractController', $scope);

            $scope.$watch("contract", c => {
                if (c != null) {
                    $scope.contract.Seller = c.Seller;
                    $scope.contract.UsageType = c.SupplyDetails.PropertyDetail.UsageType;
                    $scope.contract.PropertyType = c.SupplyDetails.PropertyDetail.PropertyType;
                    $scope.contract.IntentionOfOwner = c.SupplyDetails.IntentionOfOwner;
                    $scope.contract.Address = c.SupplyDetails.PropertyDetail.Address;
                    $scope.contract.LicencePlate = c.SupplyDetails.PropertyDetail.LicencePlate;
                    $scope.contract.ContractTotalPrice = c.TotalPrice;
                    $scope.contract.ContractRent = c.Rent;
                    $scope.contract.ContractMortgage = c.Mortgage;
                    $scope.contract.ContractUnitArea = c.UnitArea;
                    $scope.contract.ContractEstateArea = c.EstateArea;
                }
            });

            $scope.onSaveClick = () => {
              
                $http.post("/api/web/contracts/update", $scope.contract).success((data: any) => {
                    toastr.success("قرارداد با موفقیت ویرایش شد.");
                    $state.go("^");
                    $scope.$emit('EntityUpdated');
                });
            }

            $scope.onBackClick = () => {
                $state.go("^");
            }

        }
    ]);

}