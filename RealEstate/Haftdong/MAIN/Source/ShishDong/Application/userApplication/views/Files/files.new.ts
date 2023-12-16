module JahanJooy.ShishDong.File {
    appModule.controller("NewFileController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "toastr", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'ملک جدید';
            }
            scopes.store('NewFileController', $scope);

            $scope.property = {
                ContactInfos: {},
                VicinityID:0
            };

            $scope.onSaveClick = (showDetail) => {
                if ($scope.property.Vicinity != null && $scope.property.Vicinity != undefined) {
                    $scope.property.VicinityID = $scope.property.Vicinity.ID;
                }
                $http.post("/api/haftdong/files/save", $scope.property).success((data: any) => {
                    toastr.success("ملک با موفقیت ثبت شد.");
                    $scope.$emit('EntityUpdated');
                    if (showDetail)
                        $state.go("welcome.files.details", { id: data.ID });
                    else
                        location.reload();
                });
            }

            $scope.onBackClick = () => {
                $state.go("^");
            }
        }
    ]);

}