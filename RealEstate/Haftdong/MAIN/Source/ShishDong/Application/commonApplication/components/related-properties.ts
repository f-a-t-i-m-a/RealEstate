module JahanJooy.ShishDong.Components {
    appModule.directive('jjRelatedProperties', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-directives/related-properties.html',
            scope: {
                propertyId: "="
            },
            controller: ($scope, $http: ng.IHttpService) => {

                $scope.$watch("propertyId", id => {
                    if (id !== null) {
                        $http.get("/api/haftdong/files/getrelatedproperties/" + id, null).success((data: any) => {
                            $scope.relatedProperties = data;
                        });
                    }
                });
               
            }
        }
    });
}