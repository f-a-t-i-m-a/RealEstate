module JahanJooy.HaftDong.Operation {

    appModule.controller("OperationController", [
        '$scope', '$timeout', '$state', '$http', '$rootScope', 'toastr', 'scopes',
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $rootScope, toastr, scopes) => {

            $scope.GetTitle = () => {
                return 'عملیات';
            }
            scopes.store('OperationController', $scope);

            $http.get("/api/web/operations/all", null).success((data: any) => {
                $scope.Types = data;
            });

            $scope.onDeleteAndCreate = () => {
                $http.post("/api/web/operations/deleteindex", null).success(() => {
                        toastr.success("ایندکس با موفقیت حذف و دوباره ایجاد شد");
                    });
            }

            $scope.onDeleteAndReindex = () => {
                $http.post("/api/web/operations/deleteandreindexall", null).success(() => {
                    toastr.success("ایندکس با موفقیت حذف و دوباره ایجاد شد و همه مپینگ ها با موفقیت در صف ایجاد قرار گرفتند");
                });
            }

            $scope.onReIndex = (type) => {
                if (!type) {
                    $http.post("/api/web/operations/reindexall", null).success(() => {
                        toastr.success("همه مپینگ ها با موفقیت در صف ایجاد قرار گرفتند");
                    });
                } else {
                    $http.post("/api/web/operations/reindex/" + type, null).success(() => {
                        toastr.success(type + " با موفقیت در صف ایجاد قرار گرفت");
                    });
                }
            }
        }
    ]);

} 