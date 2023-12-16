module JahanJooy.HaftDong.Vicinity {
    appModule.controller("NewVicinityController", [
        '$scope', '$http', '$stateParams', '$state', "toastr", "scopes",
        ($scope, $http: ng.IHttpService, $stateParams, $state, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'محله جدید';
            }
            scopes.store('NewVicinityController', $scope);

            $scope.parentId = $stateParams.id;
            $scope.item = {
                CanContainPropertyRecords: true
            }

            $scope.onSaveClick = () => {
                $http.post("api/web/vicinities/save", {
                    Name: $scope.item.Name,
                    Type: $scope.item.Type,
                    ShowTypeInTitle: $scope.item.ShowTypeInTitle,
                    AlternativeNames: $scope.item.AlternativeNames,
                    AdditionalSearchText: $scope.item.AdditionalSearchText,
                    WellKnownScope: $scope.item.WellKnownScope,
                    ShowInSummary: $scope.item.ShowInSummary,
                    CanContainPropertyRecords: $scope.item.CanContainPropertyRecords,
                    CurrentVicinityID: $scope.parentId
                }).success(() => {
                    toastr.success("محله با موفقیت ثبت شد");
                    $state.go("^", { id: $scope.parentId });
                    $scope.$emit('EntityUpdated');
                });
            };
        }
    ]);

}