module JahanJooy.HaftDong.AdminDashboard {
    export var ngModule = angular.module("haftdongAdminDashboard",
        ['ui.router', 'ui.select', 'ngSanitize', 'ui.bootstrap.persian.datepicker', 'ui.bootstrap.datepicker']);
    ngModule.config([
        '$stateProvider',
        ($stateProvider: ng.ui.IStateProvider) => {
            $stateProvider
                .state('dashboard', {
                    url: "/adminapp/dashboard/{searchtext}",
                    templateUrl: "Application/adminApplication/views/dashboard/dashboard.html",
                    controller: "AdminDashboardController",
                    data: {
                        title: "داشبورد"
                    }
                });
        }
    ]);
}