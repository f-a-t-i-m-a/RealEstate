module JahanJooy.HaftDong.User {
    import ContactMethodType = RealEstateAgency.Domain.Enums.User.ContactMethodType;
    appModule.controller("NewUserController", [
        '$scope', '$timeout', '$state', '$http', 'scopes',
        ($scope, $timeout, $state: angular.ui.IStateService, $http, scopes) => {

            $scope.GetTitle = () => {
                return 'کاربر جدید';
            }
            scopes.store('NewUserController', $scope);

            $scope.user = {};

            $scope.onSaveClick = () => {
                $http.post("/api/web/users/save", $scope.user).success((data: any) => {
                    $scope.$emit('EntityUpdated');
                    $state.go("users.details", { id: data.Id });
                });
            }

            $scope.onBackClick = () => {
                window.history.back();
            }
        }
    ]);

}