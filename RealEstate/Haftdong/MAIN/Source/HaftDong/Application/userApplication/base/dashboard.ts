module JahanJooy.HaftDong.Dashboard {
    export var ngModule = angular.module("haftdongDashboard",
        ['ui.router', 'ui.select', 'ngSanitize', 'ui.bootstrap.persian.datepicker', 'ui.bootstrap.datepicker']);
    ngModule.config([
        '$stateProvider',
        ($stateProvider: ng.ui.IStateProvider) => {
            $stateProvider
                .state('dashboard', {
                    url: "/app/dashboard/{searchtext}",
                    templateUrl: "Application/userApplication/views/dashboard/dashboard.html",
                    controller: "DashboardController",
                    data: {
                        title: "داشبورد"
                    }
                })
                .state('profile', {
                url: "/app/profile",
                    templateUrl: "Application/userApplication/views/Profile/myprofile.html",
                    controller: "ProfileController",
                    data: {
                        title: "صفحه شخصی"
                    }
                });
        }
    ]);
}