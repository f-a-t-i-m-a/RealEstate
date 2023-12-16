module JahanJooy.HaftDong.UserActivities {
    appModule.controller("UserActivityDetailsController", [
        '$scope', '$timeout', '$state', '$http', "$stateParams", "$rootScope", "messageBoxService", "toastr", "$modal",
        "Upload", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $stateParams, $rootScope, messageBoxService, toastr,
            $modal, Upload, scopes) => {

            $scope.GetTitle = () => {
                return 'جزئیات فعالیت های کاربران';
            }
            scopes.store('UserActivityDetailsController', $scope);

            $scope.isContactCollapsed = true;
            $scope.isLoginCollapsed = true;

            function initialize() {
                $http.get("/api/web/useractivity/get/" + $stateParams.correlationid, null).success((data: any) => {
                    $scope.useractivity = data;
                });
            }

            initialize();

            var listener = $rootScope.$on('EntityUpdated', () => {
                initialize();
            });

            $scope.$on('$destroy', listener);

        }
    ]);

}