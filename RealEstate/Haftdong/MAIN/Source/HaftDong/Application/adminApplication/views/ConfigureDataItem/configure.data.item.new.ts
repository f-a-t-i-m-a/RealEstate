module JahanJooy.HaftDong.ConfigureDataItem {
    ngModule.controller("AddNewConfigureDataItemController", [
        '$scope', '$http', '$stateParams', '$state', "toastr", "scopes",
        ($scope, $http: ng.IHttpService, $stateParams, $state, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'پیکربندی جدید';
            }
            scopes.store('AddNewConfigureDataItemController', $scope);

            $scope.item = {
                Identifier: "",
                Value: "",
                ID: ""
            }

            $scope.onSaveClick = () => {
                $http.post("api/web/configure/add", {
                    Identifier: $scope.item.Identifier,
                    Value: $scope.item.Value
                }).success(() => {
                    toastr.success("داده با موفقیت ثبت شد");
                    $state.go("^");
                    $scope.$emit('EntityUpdated');
                });
            };
        }
    ]);

}