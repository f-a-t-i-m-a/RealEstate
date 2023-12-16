module JahanJooy.HaftDong.Customer {

    appModule.controller("CustomerDetailsController", [
        '$scope', '$timeout', '$state', '$http', "$rootScope", "$stateParams", "messageBoxService", "toastr", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $rootScope, $stateParams, messageBoxService, toastr, scopes) => {
            
            $scope.GetTitle = () => {
                return 'جزئیات مشتری';
            }
            scopes.store('CustomerDetailsController', $scope);

            $scope.isMoreDetailsCollapsed = true;
            $scope.hasDeputy = true;
            $scope.moreInfo = true;

            function getThumbnails() {
                $scope.Properties.forEach(p => {
                    if (p.CoverImageID != null) {
                        $http.get("/api/web/properties/getThumbnail/" + p.CoverImageID, { responseType: "blob" })
                            .success((data: Blob) => {
                                p.blob = URL.createObjectURL(data);
                            });
                    }
                });
            }

            function initialize() {
                $http.get("/api/web/customers/get/" + $stateParams.id, null).success((data: any) => {
                    $scope.customer = data;
                    $http.get("/api/web/properties/getcustomerproperties/" + $stateParams.id, null
                    ).success((data: any) => {
                        $scope.Properties = data;
                        getThumbnails();
                    });
                    $http.get("/api/web/supplies/getcustomersupplies/" + $stateParams.id, null
                    ).success((data: any) => {
                        $scope.Supplies = data;
                    });
                    $http.get("/api/web/requests/getcustomerallrequests/" + $stateParams.id, null
                    ).success((data: any) => {
                        $scope.Requests = data;
                    });
                });

            }

            $scope.computePrice = (entity) => {
                if (entity.PriceSpecificationType === null) {
                    return 0;
                } else if (entity.PriceSpecificationType === 1) {
                    return entity.TotalPrice;
                } else if (entity.PriceSpecificationType === 2 && entity.EstateArea != null) {
                    return entity.PricePerEstateArea * entity.EstateArea;
                } else if (entity.PriceSpecificationType === 3 && entity.UnitArea != null) {
                    return entity.PricePerUnitArea * entity.UnitArea;
                }
                return 0;
            }

            $scope.onDeleteClick = () => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این مشتری را حذف کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/customers/delete/" + $scope.customer.ID, null).success((data: any) => {
                        toastr.success("مشتری با موفقیت حذف شد.");
                        $scope.$emit('EntityUpdated');
                        $state.go("^");
                    });
                });
            }

            $scope.onArchivedClick = () => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این مشتری را آرشیو کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/customers/archived/" + $scope.customer.ID, null).success((data: any) => {
                        toastr.success("مشتری آرشیو شد.");
                        $scope.$emit('EntityUpdated');
                    });
                });
            }
            $scope.onUnArchivedClick = () => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این مشتری را از آرشیو حذف کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/customers/unarchived/" + $scope.customer.ID, null).success((data: any) => {
                        toastr.success("مشتری از آرشیو حذف شد.");
                        $scope.$emit('EntityUpdated');
                    });
                });
            }

            $scope.onBackToListClick = () => {
                $state.go("customers");
            }

            initialize();

            var listener = $rootScope.$on('EntityUpdated', () => {
                initialize();
            });

            $scope.$on('$destroy', listener);
        }
    ]);

}