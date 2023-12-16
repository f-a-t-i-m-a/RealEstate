module JahanJooy.HaftDong.AdminDashboard {
    ngModule.controller("AdminDashboardController", [
        '$scope', '$rootScope', '$http', 'scopes',
        ($scope, $rootScope, $http: ng.IHttpService, scopes) => {

            $scope.GetTitle = () => {
                return 'داشبورد';
            }
            scopes.store('AdminDashboardController', $scope);
        }
    ]);

}