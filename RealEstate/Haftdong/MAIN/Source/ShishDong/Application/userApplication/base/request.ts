
module JahanJooy.ShishDong.Request {
    export var appModule = angular.module("shishdongRequests", [
        "ui.router", "ui.bootstrap", "toastr", "jjCommon",
        "shishDongApi"
    ]);
    appModule.config([
        "$stateProvider",
        ($stateProvider: angular.ui.IStateProvider) => {
            $stateProvider
                .state("welcome.requests", {
                    url: "/app/requests",
                    templateUrl: "Application/userApplication/views/Requests/requests.html",
                    controller: "RequestController",
                    data: {
                        title: "لیست درخواست ها"
                    }
                })
                .state("welcome.requests.new", {
                    url: "/new",
                    templateUrl: "Application/userApplication/views/Requests/requests.new.html",
                    controller: "NewRequestController",
                    data: {
                        title: "درخواست جدید"
                    }
                })
                .state("welcome.requests.details.edit", {
                    url: "/edit",
                    templateUrl: "Application/userApplication/views/Requests/requests.edit.html",
                    controller: "EditRequestController",
                    data: {
                        title: "ویرایش درخواست"
                    }
                })
                .state("welcome.requests.details", {
                    url: "/details/{id}",
                    templateUrl: "Application/userApplication/views/Requests/requests.details.html",
                    controller: "RequestDetailsController",
                    data: {
                        title: "جزئیات درخواست"
                    }
                })
                .state("welcome.myrequests", {
                    url: "/myrequests",
                    templateUrl: "Application/userApplication/views/Requests/my.requests.html",
                    controller: "MyRequestController",
                    data: {
                        title: "درخواست های من"
                    }
                });
        }
    ]);
}