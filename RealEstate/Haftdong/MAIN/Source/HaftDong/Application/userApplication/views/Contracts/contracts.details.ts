module JahanJooy.HaftDong.Property {
    appModule.controller("ContractDetailsController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams",
        "allPropertiesPerUsage", "allPriceSpecificationTypePerProperty", "$rootScope", "messageBoxService",
        "toastr", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams,
            allPropertiesPerUsage, allPriceSpecificationTypePerProperty, $rootScope, messageBoxService, toastr, scopes
        ) => {

            $scope.GetTitle = () => {
                return 'جزئیات قرارداد';
            }
            scopes.store('ContractDetailsController', $scope);

            $scope.hasDeputyForSeller = true;
            $scope.hasDeputyForBuyer = true;

            $scope.isEstateCollapsed = true;
            $scope.isUnitCollapsed = true;
            $scope.isHouseCollapsed = true;
            $scope.isIndustryCollapsed = true;
            $scope.isExtraHouseCollapsed = true;
            $scope.isShopCollapsed = true;
            $scope.isSaleCollapsed = true;
            $scope.isRentCollapsed = true;
            $scope.isDailyRentCollapsed = true;
            $scope.newImage = {
                title: "",
                description: ""
            }
            $scope.isCollapsed = true;

            $scope.notDeletedImages = [];

            function initialize() {
                $http.get("/api/web/contracts/get/" + $stateParams.id, null).success((data: any) => {
                    $scope.contract = data;
                    $scope.seller = $scope.contract.Seller;
                    $scope.buyer = $scope.contract.Buyer;
                    $scope.supply = $scope.contract.SupplyDetails;
                    $scope.property = $scope.supply.PropertyDetail;
                    $scope.request = $scope.contract.RequestSummary;
//                    $http.get("/api/customers/get/" + $scope.contract.SellerReference.ID, null).success((data: any) => {
//                        $scope.seller = data;
//                    });
//                    $http.get("/api/customers/get/" + $scope.contract.BuyerReference.ID, null).success((data: any) => {
//                        $scope.buyer = data;
//                    });
                });
            }

            $scope.onCancelClick = () => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این قرارداد را فسخ کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/contracts/cancel/" + $scope.contract.ID, null).success(() => {
                        toastr.success("قرارداد با موفقیت فسخ شد.");
                        $scope.$emit('EntityUpdated');
                    });
                });
            }

            $scope.onCloseClick = () => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این قرارداد را ببندید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/contracts/close/" + $scope.contract.ID, null).success(() => {
                        toastr.success("قرارداد با موفقیت بسته شد.");
                        $scope.$emit('EntityUpdated');
                    });
                });
            }

            $scope.onBackToListClick = () => {
                $state.go("contracts");
            }

            initialize();

            var listener = $rootScope.$on('EntityUpdated', () => {
                initialize();
            });

            $scope.$on('$destroy', listener);
        }
    ]);

}