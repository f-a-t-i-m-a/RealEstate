module JahanJooy.HaftDong.Customers {
    appModule.controller("VicinityMoveModalController", [
        '$scope', '$http', '$state', '$modalInstance', 'messageBoxService','toastr',
        ($scope, $http: ng.IHttpService, $state, $modalInstance, messageBoxService, toastr) => {

            $scope.ids = $scope.ids;
            $scope.vicinities = [];

            $scope.searchVicinities = (searchText) => {
                if (searchText != null && searchText.length >= 2) {
                    $http.post("/api/web/vicinities/search", {
                        SearchText: searchText,
                        CanContainPropertyRecordsOnly: false,
                        StartIndex: 0,
                        PageSize: 5
                    }).success((data: any) => {
                        if (data.Vicinities.PageItems == null)
                            $scope.vicinities = [];
                        else
                            $scope.vicinities = data.Vicinities.PageItems;
                    });
                }
            }

            $scope.cancel = () => {
                $modalInstance.dismiss('cancel');
            };

            $scope.onSave = () => {
                $http.post("/api/web/vicinities/move", {
                    Ids: $scope.ids,
                    ParentId: $scope.parentID
                }).success((data: any) => {
                    toastr.success("محله با موفقیت جابجا شد.");
                    $modalInstance.close({ parentID: $scope.parentID });
                });
            };
        }
    ]);

}