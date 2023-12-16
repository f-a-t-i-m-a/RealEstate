module JahanJooy.HaftDong.User {
    appModule.controller("UserController", [
        '$scope', '$timeout', '$state', '$http', '$rootScope', '$q', 'messageBoxService', 'toastr', "$stateParams", "scopes",
        ($scope, $timeout, $state: angular.ui.IStateService, $http, $rootScope, $q, messageBoxService, toastr, $stateParams, scopes) => {

            $scope.GetTitle = () => {
                return 'لیست کاربران';
            }
            scopes.store('UserController', $scope);

            $scope.isCollapse = true;
            $scope.FirstPage = 0;
            $scope.PageSize = 10;
            $scope.PageNumber = 1;
            $scope.DataIsCompeleted = false;
            $scope.TotalNumberOfItems = 0;
            $scope.searchInput = {};
            $scope.SortDirection = true;

            $scope.contact = {
                mobileNumber: "",
                address: "",
                email: ""
            }
            function getThumbnails() {
                $scope.Users.forEach(u => {
                    if (u.ProfilePicture != null && u.ProfilePicture.ID != null && u.ProfilePicture.DeletionTime == null) {
                        $http.get("/api/web/users/getThumbnail/" + u.ProfilePicture.ID, { responseType: "blob" })
                            .success((data: Blob) => {
                                u.blob = URL.createObjectURL(data);
                            });
                    }
                });
            }

            $scope.getDataDown = (index) => {
                var promise = $q.defer();

                if ($scope.SortDirection)
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                else
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();

                $http.post("/api/web/users/search", {
                    StartIndex: ((index - 1) * $scope.PageSize),
                    PageSize: $scope.PageSize,
                    UserName: $scope.searchInput.UserName,
                    DisplayName: $scope.searchInput.DisplayName,
                    Type: $scope.searchInput.Type,
                    InActive: $scope.searchInput.InActive,
                    ContactMethods: $scope.searchInput.ContactMethods,
                    SortColumn: $scope.SortColumn,
                    SortDirection: $scope.SortDirectionName
                }).success((data: any) => {
                    if (data.Users.PageItems == null)
                        $scope.Users = [];
                    else
                        $scope.Users = data.Users.PageItems;

                    getThumbnails();
                    $scope.TotalNumberOfItems = data.Users.TotalNumberOfItems;
                    $scope.NumberOfPages = data.Users.TotalNumberOfPages;
                    $scope.PageNumber = data.Users.PageNumber;

                    promise.resolve();
                }).error(() => {
                    promise.reject();
                });
                return promise.promise;
            };

            $scope.onSearch = () => {
                $scope.getDataDown(1);
            };

            function initialize() {
                if ($scope.SortDirection)
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                else
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();
            }

            $scope.$watch("selectAllUser", u => {
                if (u != null) {
                    angular.forEach($scope.Users, item => {
                        item.Selected = u;
                    });
                }
            });

            $scope.$watch("Users", () => {
                $scope.NumberOfPagesArray = new Array($scope.NumberOfPages);
            });

            $scope.$watchGroup(["SortColumn", "SortDirection"], () => {
                $scope.getDataDown(1);
            });

            initialize();

            var listener = $rootScope.$on('EntityUpdated', () => {
                initialize();
            });

            $scope.$on('$destroy', listener);
        }
    ]);

}