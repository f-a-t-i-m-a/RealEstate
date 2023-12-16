module JahanJooy.HaftDong.ConfigureDataItem {
    ngModule.controller("EditConfigureDataItemController", [
        '$scope', '$rootScope', '$http', '$stateParams', '$state', "toastr", "scopes",
        ($scope, $rootScope, $http: ng.IHttpService, $stateParams, $state, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'ویرایش پیکربندی';
            }
            scopes.store('EditConfigureDataItemController', $scope);

            function getData() {
                $http.get("api/web/configure/details?identifier=" + $stateParams.identifier, null).success((data: any) => {
                    $scope.item = data.Item;
                });
            }

            $scope.onSaveClick = () => {
                $http.post("api/web/configure/update", {
                        Identifier: $scope.item.Identifier,
                        Value: $scope.item.Value,
                        ID: $scope.item.ID
                    })
                    .success(() => {
                        toastr.success("داده با موفقیت ویرایش شد");
                        $state.go("^");
                        $scope.$emit('EntityUpdated');
                    });
            }

            getData();
        }
    ]);

}