module JahanJooy.HaftDong.Log {
    export var ngModule = angular.module("haftdongLog", ['ui.router', 'ui.select', 'ngSanitize',
        "haftdongComponents", 'ui.bootstrap.persian.datepicker',
        'ui.bootstrap.datepicker', "ngFileUpload"]);
    ngModule.config([
        '$stateProvider',
        ($stateProvider: ng.ui.IStateProvider) => {
            $stateProvider
                .state('log', {
                    url: "/adminapp/log",
                    templateUrl: "Application/adminApplication/views/Log/log.html",
                    controller: "LogController",
                    data: {
                        title: "لاگ ها"
                    }
                });
        }
    ]);
}