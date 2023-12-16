module JahanJooy.HaftDong.ConfigureDataItem {
    ngModule.controller("ConfigureDataItemController", [
        '$scope', '$rootScope', '$http', 'messageBoxService', 'toastr', 'scopes',
        ($scope, $rootScope, $http: ng.IHttpService, messageBoxService, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'پیکربندی ها';
            }
            scopes.store('ConfigureDataItemController', $scope);

            $scope.onRemoveItemClick = (id) => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این داده ی پیکربندی را پاک کنید؟", "danger");
                confirmResult.then(() => {
                    $http.post("/api/web/configure/remove/" + id, null).success(() => {
                        toastr.success("داده ی پیکربندی پاک شد.");
                        $scope.$emit('EntityUpdated');
                    });

                });
            }

            function getData() {
                $http.get("api/web/configure/all", null).success((data: any) => {
                    $scope.items = data.Items;
                });
            };

            getData();

            var listener = $rootScope.$on('EntityUpdated', () => {
                getData();
            });

            $scope.$on('$destroy', listener);
        }
    ]);

}