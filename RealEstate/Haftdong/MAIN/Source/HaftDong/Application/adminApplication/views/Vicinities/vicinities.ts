module JahanJooy.HaftDong.Vicinity {
    appModule.controller("VicinityController", [
        '$scope', '$rootScope', '$http', 'messageBoxService', 'toastr', '$stateParams', '$filter', '$modal', 'scopes',
        ($scope, $rootScope, $http: ng.IHttpService, messageBoxService, toastr, $stateParams, $filter, $modal, scopes) => {

            $scope.GetTitle = () => {
                return 'لیست محله ها';
            }
            scopes.store('VicinityController', $scope);

            $scope.parentId = $stateParams.id;
            $scope.searchResult = null;
            $scope.hierarchyList = [];
            $scope.PageSize = 10;
            $scope.PageNumber = 1;
            $scope.TotalNumberOfItems = 0;

            $scope.onRemoveClick = (id) => {
                var ids;
                var confirmResult;
                if (id != null) {
                    ids = id;
                    confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید این محله را حذف کنید؟", "danger");
                } else {
                    if (Enumerable.from($scope.items).count(i => i.Selected) === 0) {
                        return;
                    }
                    ids = Enumerable.from($scope.items).where(i => i.Selected).select(i => i.ID).toArray();
                    confirmResult = messageBoxService.confirm("آیا مطمئن هستید که می خواهید تمام محله های انتخاب شده را حذف کنید؟", "danger");
                }

                confirmResult.then(() => {
                    $http.post("/api/web/vicinities/delete", {
                        Ids: ids
                    }).success(() => {
                        toastr.success("محله (های) انتخاب شده با موفقیت حذف شد(ند).");
                        $scope.selectAllVicinities = null;
                        $scope.$emit('EntityUpdated');
                    });

                });
            }

            $scope.onEnableClick = () => {
                if (Enumerable.from($scope.items).count(i => i.Selected) === 0) {
                    return;
                }

                var ids = Enumerable.from($scope.items).where(i => i.Selected).select(i => i.ID).toArray();
                var confirmResult = messageBoxService.confirm("آیا می خواهید تمام محله های انتخاب شده را فعال کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/vicinities/enable", {
                        Ids: ids,
                        Value: true
                    }).success((data: any) => {
                        toastr.success("تمام محله های انتخاب شده با موفقیت فعال شدند.");
                        $scope.selectAllVicinities = null;
                        $scope.$emit('EntityUpdated');
                    });
                });
            }

            $scope.onDisableClick = () => {
                if (Enumerable.from($scope.items).count(i => i.Selected) === 0) {
                    return;
                }

                var ids = Enumerable.from($scope.items).where(i => i.Selected).select(i => i.ID).toArray();
                var confirmResult = messageBoxService.confirm("آیا می خواهید تمام محله های انتخاب شده را غیر فعال کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/vicinities/enable", {
                        Ids: ids,
                        Value: false
                    }).success((data: any) => {
                        toastr.success("تمام محله های انتخاب شده با موفقیت غیر فعال شدند.");
                        $scope.selectAllVicinities = null;
                        $scope.$emit('EntityUpdated');
                    });
                });
            }

            $scope.onContainPropertyClick = () => {
                if (Enumerable.from($scope.items).count(i => i.Selected) === 0) {
                    return;
                }

                var ids = Enumerable.from($scope.items).where(i => i.Selected).select(i => i.ID).toArray();
                var confirmResult = messageBoxService.confirm("آیا می خواهید امکان ثبت ملک در تمام محله های انتخاب شده را فعال کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/vicinities/containproperty", {
                        Ids: ids,
                        Value: true
                    }).success((data: any) => {
                        toastr.success("امکان ثبت ملک در تمام محله های انتخاب شده با موفقیت فعال شد.");
                        $scope.selectAllVicinities = null;
                        $scope.$emit('EntityUpdated');
                    });
                });
            }

            $scope.onDoesNotContainPropertyClick = () => {
                if (Enumerable.from($scope.items).count(i => i.Selected) === 0) {
                    return;
                }

                var ids = Enumerable.from($scope.items).where(i => i.Selected).select(i => i.ID).toArray();
                var confirmResult = messageBoxService.confirm("آیا می خواهید امکان ثبت ملک در تمام محله های انتخاب شده را غیر فعال کنید؟");
                confirmResult.then(() => {
                    $http.post("/api/web/vicinities/containproperty", {
                        Ids: ids,
                        Value: false
                    }).success((data: any) => {
                        toastr.success("امکان ثبت ملک در تمام محله های انتخاب شده با موفقیت غیر فعال شد.");
                        $scope.selectAllVicinities = null;
                        $scope.$emit('EntityUpdated');
                    });
                });
            }

            $scope.onMoveClick = () => {
                if (Enumerable.from($scope.items).count(i => i.Selected) === 0) {
                    return;
                }

                $scope.ids = Enumerable.from($scope.items).where(i => i.Selected).select(i => i.ID).toArray();

                var modalInstance = $modal.open({
                    templateUrl: "Application/adminApplication/views/Vicinities/vicinity.move.modal.html",
                    controller: "VicinityMoveModalController",
                    scope: $scope,
                    size: "lg"
                });
                modalInstance.result.then((data: any) => {
                    $scope.parentId = data.parentID;
                    $scope.$emit('EntityUpdated');
                });
            }

            $scope.onMapClick = (vicinity) => {
                $scope.vicinity = vicinity;
                var modalInstance = $modal.open({
                    templateUrl: "Application/adminApplication/views/Vicinities/vicinity.map.modal.html",
                    controller: "VicinityMapModalController",
                    scope: $scope,
                    size: "lg"
                });
            }

            $scope.onSearchClick = () => {
                $scope.getSearchData(1);
            }

            $scope.getSearchData = (index) => {
                if ($scope.searchText != null && $scope.searchText.length >= 2) {
                    $http.post("/api/web/vicinities/search", {
                        SearchText: $scope.searchText,
                        CanContainPropertyRecordsOnly: false,
                        ParentId: $scope.parentId,
                        StartIndex: ((index - 1) * $scope.PageSize),
                        PageSize: $scope.PageSize,
                        SortColumn: $scope.SortColumn,
                        SortDirection: $scope.SortDirectionName
                    }).success((data: any) => {
                        if (data.Vicinities.PageItems == null)
                            $scope.items = [];
                        else
                            $scope.items = data.Vicinities.PageItems;

                        $scope.TotalNumberOfItems = data.Vicinities.TotalNumberOfItems;
                        $scope.NumberOfPages = data.Vicinities.TotalNumberOfPages;
                        $scope.PageNumber = data.Vicinities.PageNumber;
                        $scope.searchResult = true;
                    });
                }
            }

            function getData() {
                var path: string;
                if ($scope.parentId === "0" || $scope.parentId == null)
                    path = "api/web/vicinities/list";
                else
                    path = "api/web/vicinities/list/" + $scope.parentId;

                $http.get(path, null).success((data: any) => {
                    $scope.items = data.Vicinities;
                    $scope.hierarchy = data.Hierarchy;
                    $scope.currentVicinity = data.CurrentVicinity;
                    $scope.searchResult = false;

                    $scope.hierarchyList = [
                        {
                            ID: 0,
                            Text: "همه محله ها"
                        }
                    ];

                    $scope.hierarchy.forEach(h => {
                        $scope.hierarchyList.push({
                            ID: h.ID,
                            Text: $filter("VicinityType")(h.Type) + " " + h.Name
                        });
                    });
                });
            };

            $scope.onRetrieveClick = () => {
                $http.get("/api/web/vicinities/retrieve", null).success((data: any) => {
                    toastr.success("تمام محله ها با موفقیت بازیابی شدند.");
                    $scope.$emit('EntityUpdated');
                });
            }

            $scope.$watch("selectAllVicinities", v => {
                if (v != null) {
                    angular.forEach($scope.items, item => {
                        item.Selected = v;
                    });
                }
            });

            getData();

            var listener = $rootScope.$on('EntityUpdated', () => {
                getData();
            });

            $scope.$on('$destroy', listener);
        }
    ]);

}