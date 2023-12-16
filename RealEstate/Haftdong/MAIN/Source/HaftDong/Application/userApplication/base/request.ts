
module JahanJooy.HaftDong.Request {
    export var appModule = angular.module("haftdongRequests", [
        "ui.router", "ui.bootstrap", "toastr", "jjCommon",
        "haftdongApi"
    ]);
    appModule.config([
        "$stateProvider",
        ($stateProvider: angular.ui.IStateProvider) => {
            $stateProvider
                .state("requests", {
                    url: "/app/requests",
                    templateUrl: "Application/userApplication/views/Requests/requests.html",
                    controller: "RequestController",
                    data: {
                        title: "لیست درخواست ها"
                    }
                })
                .state("requests.new", {
                    url: "/new/{customerId}",
                    templateUrl: "Application/userApplication/views/Requests/requests.new.html",
                    controller: "NewRequestController",
                    data: {
                        title: "درخواست جدید"
                    }
                })
                .state("requests.details.edit", {
                    url: "/edit",
                    templateUrl: "Application/userApplication/views/Requests/requests.edit.html",
                    controller: "EditRequestController",
                    data: {
                        title: "ویرایش درخواست"
                    }
                })
                .state("requests.details", {
                    url: "/details/{id}",
                    templateUrl: "Application/userApplication/views/Requests/requests.details.html",
                    controller: "RequestDetailsController",
                    data: {
                        title: "جزئیات درخواست"
                    }
                })
                .state("myrequests", {
                    url: "/app/myrequests",
                    templateUrl: "Application/userApplication/views/Profile/my.requests.html",
                    controller: "MyRequestController",
                    data: {
                        title: "درخواست های من"
                    }
                })
                .state("mypublicrequests", {
                    url: "/app/mypublicrequests",
                    templateUrl: "Application/userApplication/views/Profile/my.public.requests.html",
                    controller: "MyPublicRequestController",
                    data: {
                        title: "درخواست های عمومی من"
                    }
                });
        }
    ]);
}