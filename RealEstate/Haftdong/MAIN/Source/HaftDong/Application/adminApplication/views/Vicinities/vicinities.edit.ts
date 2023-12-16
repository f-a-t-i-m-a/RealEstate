module JahanJooy.HaftDong.Vicinity {
    appModule.controller("EditVicinityController", [
        '$scope', '$rootScope', '$http', '$stateParams', '$state', "toastr", "scopes",
        ($scope, $rootScope, $http: ng.IHttpService, $stateParams, $state, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'ویرایش محله';
            }
            scopes.store('EditVicinityController', $scope);

            function getData() {
                $http.get("api/web/vicinities/get/" + $stateParams.id, null).success((data: any) => {
                    $scope.item = data;
                });
            }

            $scope.onSaveClick = () => {
                $http.post("api/web/vicinities/update", {
                    ID: $scope.item.ID,
                    Name: $scope.item.Name,
                    Type: $scope.item.Type,
                    ShowTypeInTitle: $scope.item.ShowTypeInTitle,
                    AlternativeNames: $scope.item.AlternativeNames,
                    AdditionalSearchText: $scope.item.AdditionalSearchText,
                    WellKnownScope: $scope.item.WellKnownScope,
                    ShowInSummary: $scope.item.ShowInSummary,
                    CanContainPropertyRecords: $scope.item.CanContainPropertyRecords,
                    Description: $scope.item.Description,
                    OfficialLinkUrl: $scope.item.OfficialLinkUrl,
                    WikiLinkUrl: $scope.item.WikiLinkUrl,
                    AdministrativeNotes: $scope.item.AdministrativeNotes,
                    Enabled: $scope.item.Enabled,
                    Order: $scope.item.Order,
                    ShowInHierarchy: $scope.item.ShowInHierarchy
                    })
                    .success(() => {
                        toastr.success("محله با موفقیت ویرایش شد");
                        $state.go("^", { id: $scope.item.ParentID});
                        $scope.$emit('EntityUpdated');
                    });
            }

            getData();
        }
    ]);

}