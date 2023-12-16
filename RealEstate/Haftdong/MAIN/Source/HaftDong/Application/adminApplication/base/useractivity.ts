module JahanJooy.HaftDong.UserActivities {
    export var ngModule = angular.module("haftdongUserActivities", ["ui.router", "ui.select", "ngSanitize",
        "ui.bootstrap.persian.datepicker", "ui.bootstrap.datepicker",
        "haftdongComponents", "haftdongApi", "toastr"]);
    ngModule.config([
        '$stateProvider',
        ($stateProvider: ng.ui.IStateProvider) => {
            $stateProvider
                .state("useractivities", {
                    url: "/adminapp/useractivities",
                    templateUrl: "Application/adminApplication/views/Users/user.activities.html",
                    controller: "UserActivityController",
                    data: {
                        title: "فعالیت های کاربران"
                    }
                })
                .state("useractivities.details", {
                    url: "/details/{correlationid}",
                    templateUrl: "Application/adminApplication/views/Users/user.activities.details.html",
                    controller: "UserActivityDetailsController",
                    data: {
                        title: "جزئیات فعالیت های کاربران"
                    }
                });
        }
    ]);
}