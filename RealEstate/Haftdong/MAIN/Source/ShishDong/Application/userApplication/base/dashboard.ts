module JahanJooy.ShishDong.Dashboard {
    export var ngModule = angular.module("shishdongDashboard",
    ['ui.router', 'ui.select', 'ngSanitize', 'ui.bootstrap.persian.datepicker', 'ui.bootstrap.datepicker']);
    ngModule.config([
        '$stateProvider',
        ($stateProvider: ng.ui.IStateProvider) => {
            $stateProvider
                .state('profile',
                {
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