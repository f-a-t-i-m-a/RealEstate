module JahanJooy.HaftDong.User {
    appModule.controller("UserImageModalController", [
        '$scope', '$http', '$state', '$modalInstance', 'messageBoxService', 'toastr', '$rootScope',
        ($scope, $http: ng.IHttpService, $state, $modalInstance, messageBoxService, toastr, $rootScope) => {
            $scope.image = $scope.selectedImage;
            $scope.showPrevious = false;
            $scope.showNext = false;

            function initialize() {
                $http.get("/api/web/users/getMediumSize/" + $scope.image.ID, { responseType: "blob" })
                    .success((data: Blob) => {
                        $scope.image.Blob = URL.createObjectURL(data);
                    });
            }

            $scope.cancel = () => {
                $modalInstance.dismiss('cancel');
            };

            $scope.onRemoveImageClick = (image) => {
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این عکس را پاک کنید؟", "danger");
                confirmResult.then(() => {
                    $http.post("/api/web/users/deleteimage/" + image.ID, null).success(() => {
                        toastr.success("عکس پاک شد.");
                        $scope.$emit('EntityUpdated');
                        $modalInstance.dismiss('cancel');
                    });

                });
            };

            $scope.onDownloadImageClick = (image) => {
                $http.get("/api/web/users/download/" + image.ID, { responseType: "blob" })
                    .success((data: Blob) => {
                        Common.DownloadUtils.downloadBlob(data, image.Title + image.OriginalFileExtension);
                    });
            };

            initialize();
        }
    ]);

}