module JahanJooy.HaftDong.ConfigureDataItem {
    ngModule.directive('jjConfigure', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/adminApplication/views/ConfigureDataItem/configure.data.item.directive.html',
            scope: {
                item: "=",
                onSaveClick: '&'
            },
            controller: ($scope, $http: ng.IHttpService, $state) => {

                $scope.onBackClick = () => {
                    $state.go("^");
                };
            }
        }
    });
}