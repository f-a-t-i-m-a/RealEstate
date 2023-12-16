module JahanJooy.HaftDong.UserActivities {
    ngModule.controller("UserActivityController",
    [
        "$scope", "$rootScope", "$http", "$state", "$modal", "toastr", "$q", "scopes",
        ($scope, $rootScope, $http: ng.IHttpService, $state, $modal, toastr, $q, scopes) => {

            $scope.GetTitle = () => {
                return 'فعالیت های کاربران';
            }
            scopes.store('UserActivityController', $scope);

            $scope.FirstPage = 0;
            $scope.PageSize = 10;
            $scope.PageNumber = 1;
            $scope.DataIsCompeleted = false;
            $scope.TotalNumberOfItems = 0;
            $scope.searchInput = {};
            $scope.ids = new Array();
            $scope.SortDirection = false;
            $scope.isCollapse = true;
            $scope.AllActivity = true;
            $scope.SuccessListOptions = [
                { id: true, text: "موفقیت آمیز" },
                { id: false, text: "با شکست" }
            ];

            $http.get("/api/web/users/all", null)
                .success((data: any) => {
                    $scope.Users = data;
                    $scope.UserListOptions = [];
                    for (var i = 0; i < $scope.Users.length; i++) {
                        $scope.UserListOptions.push({ id: $scope.Users[i].Id, text: $scope.Users[i].DisplayName });
                    }
                });

//            $http.get("/api/users/allroles", null).success((data: any) => {
//                $scope.Roles = data;
//                $scope.UserRoleListOptions = [];
//                for (var i = 0; i < $scope.Roles.length; i++) {
//                    $scope.UserRoleListOptions.push({ id: $scope.Roles[i].Id, text: $scope.Roles[i].Name });
//                }
//            });

            $scope.onSearch = (index) => {
                if (!$scope.DataIsCompeleted) {
                    var promise = $q.defer();

                    var userId = null;
                    if ($scope.UserFilter != null)
                        userId = $scope.UserFilter.id;
                    var roleId = null;
                    if ($scope.UserRoleFilter != null)
                        roleId = $scope.UserRoleFilter.id;

                    if ($scope.SortDirection)
                        $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                    else
                        $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();

                    $http.post("/api/web/useractivity/search",
                        {
                            StartIndex: ((index - 1) * $scope.PageSize),
                            PageSize: $scope.PageSize,
                            PrizeName: $scope.PrizeNameFilter,
                            CorrelationId: $scope.CorrelationIdFilter,
                            HasTargetState: $scope.HasTargetStateFilter,
                            HasComment: $scope.HasCommentFilter,
                            AllActivity: $scope.AllActivity,
                            UserId: userId,
                            RoleId: roleId,
                            Controller: $scope.ControllerFilter,
                            ActionName: $scope.ActionNameFilter,
                            ApplicationType: $scope.ApplicationTypeFilter,
                            TargetType: $scope.TargetTypeFilter,
                            TargetId: $scope.TargetIDFilter,
                            TargetState: $scope.TargetStateFilter,
                            Succeeded: $scope.IsSuccessfulFilter,
                            ActivityType: $scope.UserActivityTypeFilter,
                            ActivitySubType: $scope.UserActivitySubTypeFilter,
                            ParentType: $scope.ParentTypeFilter,
                            ParentId: $scope.ParentIDFilter,
                            DetailType: $scope.DetailTypeFilter,
                            DetailId: $scope.DetailIDFilter,
                            FromActivityTime: $scope.FromActivityTimeFilter,
                            FromActivityTimeMinute: $scope.FromActivityTimeMinuteFilter,
                            FromActivityTimeHour: $scope.FromActivityTimeHourFilter,
                            ToActivityTime: $scope.ToActivityTimeFilter,
                            ToActivityTimeMinute: $scope.ToActivityTimeMinuteFilter,
                            ToActivityTimeHour: $scope.ToActivityTimeHourFilter,
                            SortColumn: $scope.SortColumn,
                            SortDirection: $scope.SortDirectionName
                        })
                        .success((data: RealEstateAgency.Util.Models.UserActivities.SearchOutput) => {
                            if (data.UserActivities.PageItems == null)
                                $scope.userActivities = [];
                            else
                                $scope.userActivities = data.UserActivities.PageItems;

                            $scope.TotalNumberOfItems = data.UserActivities.TotalNumberOfItems;
                            $scope.NumberOfPages = data.UserActivities.TotalNumberOfPages;
                            $scope.PageNumber = data.UserActivities.PageNumber;
                            promise.resolve();
                        })
                        .error(error => {
                            promise.reject();
                        });
                    return promise.promise;
                }
            };

            function initialize() {
                if ($scope.SortDirection)
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Asc.toString();
                else
                    $scope.SortDirectionName = RealEstateAgency.Util.Models.Base.SortDirectionType.Desc.toString();
            }

            $scope.$watchGroup(["SortColumn", "SortDirection"],
            () => {
                $scope.onSearch(1);
            });

            initialize();

            var listener = $rootScope.$on("EntityUpdated",
            () => {
                initialize();
            });

            $scope.$on("$destroy", listener);
        }
    ]);

}