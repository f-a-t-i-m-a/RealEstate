module JahanJooy.HaftDong.Vicinity {
    appModule.directive('jjVicinity', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/adminApplication/views/Vicinities/vicinities.directive.html',
            scope: {
                item: "=",
                onSaveClick: '&'
            },
            controller: ($scope, $http: ng.IHttpService, $state) => {

                $scope.$watch("item", i => {
                    if (i != null) {
                        $scope.inEditMode = !(i.ID == null);
                    }
                });

                $scope.onBackClick = () => {
                    $state.go("^");
                };
            }
        }
    });
}