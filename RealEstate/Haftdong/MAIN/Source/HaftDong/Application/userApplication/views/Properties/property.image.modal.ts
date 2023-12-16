module JahanJooy.HaftDong.Property {
    appModule.controller("PropertyImageModalController", [
        '$scope', '$http', '$state', '$modalInstance', 'messageBoxService', 'toastr', '$rootScope',
        ($scope, $http: ng.IHttpService, $state, $modalInstance, messageBoxService, toastr, $rootScope) => {
            $scope.image = $scope.selectedImage;
            $scope.showPrevious = false;
            $scope.showNext = false;

            function initialize() {
                $http.get("/api/web/properties/getMediumSize/" + $scope.image.ID, { responseType: "blob" })
                    .success((data: Blob) => {
                        $scope.image.Blob = URL.createObjectURL(data);
                        var currentIndex = Enumerable.from($scope.notDeletedImages).indexOf(i => i.ID === $scope.image.ID);
                        if (currentIndex === 0)
                            $scope.showPrevious = false;
                        else
                            $scope.showPrevious = true;
                        if (currentIndex === Enumerable.from($scope.notDeletedImages).count() - 1)
                            $scope.showNext = false;
                        else
                            $scope.showNext = true;
                    });
            }

            $scope.cancel = function () {
                $modalInstance.dismiss('cancel');
            };

            $scope.onRemoveImageClick = (image) => {
                var coverChange;
                if (image.ID === $scope.property.CoverImageID) {
                    coverChange = true;
                } else {
                    coverChange = false;
                }
                var confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این عکس را پاک کنید؟", "danger");
                confirmResult.then(() => {
                    $http.post("/api/web/properties/deleteimage/" + image.ID, null).success(() => {
                        toastr.success("عکس پاک شد.");
                        $scope.$emit('EntityUpdated');
                        if (coverChange && Enumerable.from($scope.notDeletedImages).count() > 1) {
                            toastr.warning("برای تصاویر کاور جدیدی انتخاب شده است.");
                        }
                        $modalInstance.dismiss('cancel');
                    });

                });
            };

            $scope.onDownloadImageClick = (image) => {
                $http.get("/api/web/properties/download/" + image.ID, { responseType: "blob" })
                    .success((data: Blob) => {
                        Common.DownloadUtils.downloadBlob(data, image.Title + image.OriginalFileExtension);
                    });
            };

            $scope.onChangeCoverClick = image => {
                $http.post("/api/web/properties/changecover/" + image.ID, null).success(() => {
                    $scope.image.cover = true;
                    toastr.success("کاور ملک با موفقیت تغییر کرد.");
                    $scope.$emit('EntityUpdated');
                });
            }

            $scope.onNextClick = image => {
                var currentIndex = Enumerable.from($scope.notDeletedImages).indexOf(i => i.ID === image.ID);
                $scope.image = Enumerable.from($scope.notDeletedImages).skip(currentIndex + 1).firstOrDefault();
                initialize();
            }

            $scope.onPreviousClick = image => {
                var currentIndex = Enumerable.from($scope.notDeletedImages).indexOf(i => i.ID === image.ID);
                $scope.image = Enumerable.from($scope.notDeletedImages).skip(currentIndex - 1).firstOrDefault();
                initialize();
            }

            initialize();
        }
    ]);

}