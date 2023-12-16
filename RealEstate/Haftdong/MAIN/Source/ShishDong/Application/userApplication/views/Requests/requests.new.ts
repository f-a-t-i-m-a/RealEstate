module JahanJooy.ShishDong.Request {
    appModule.controller("NewRequestController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "toastr", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'درخواست جدید';
            }
            scopes.store('NewRequestController', $scope);

            $scope.request = {
                ContactInfos: {}
            };
            
            $scope.onSaveClick = () => {
                $scope.request.PropertyTypes = Enumerable.from<any>($scope.request.PropertyTypeObjects).where(pt => pt.isSelected).select(pt => pt.id).toArray();
                $http.post("/api/haftdong/requests/save", $scope.request).success((data: any) => {
                    toastr.success("درخواست با موفقیت ثبت شد.");
                    $scope.$emit('EntityUpdated');
                    $state.go("^.details", { id: data.ID });
                });
            }

            $scope.onBackClick = () => {
                $state.go("^");
            }
        }
    ]);

} 