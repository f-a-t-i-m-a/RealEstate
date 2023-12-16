module JahanJooy.HaftDong.Components {
    appModule.directive('jjRelatedProperties', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-directives/related-properties.html',
            scope: {
                propertyId: "="
                
            },
            controller: ($scope, $http: ng.IHttpService, $state, $modal, $stateParams, toastr, allPropertiesPerUsage, allPriceSpecificationTypePerProperty) => {

                $scope.$watch("propertyId", id => {
                    if (id !== null) {
                        $http.get("/api/web/properties/getrelatedproperties/" + id, null).success((data: any) => {
                            $scope.relatedProperties = data;
                        });
                    }
                });
               
            }
        }
    });
}