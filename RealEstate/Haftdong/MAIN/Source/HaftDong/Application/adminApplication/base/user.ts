module JahanJooy.HaftDong.User {
    export var appModule = angular.module("haftdongUsers", [
        "ui.router", "ui.bootstrap", "toastr", "jjCommon",
        "ui.bootstrap.persian.datepicker", "ui.bootstrap.datepicker",
        "haftdongComponents", "ngFileUpload",
        "haftdongApi"
    ]);
    appModule.config([
        "$stateProvider",
        ($stateProvider: angular.ui.IStateProvider) => {
            $stateProvider
                .state("users", {
                    url: "/adminapp/users",
                    templateUrl: "Application/adminApplication/views/Users/users.html",
                    controller: "UserController",
                    data: {
                        title: "لیست کاربران"
                    }
                })
                .state("users.new", {
                    url: "/new",
                    templateUrl: "Application/adminApplication/views/Users/users.new.html",
                    controller: "NewUserController",
                    data: {
                        title: "کاربر جدید"
                    }
                })
                .state("users.details", {
                    url: "/details/{id}",
                    templateUrl: "Application/adminApplication/views/Users/users.details.html",
                    controller: "UserDetailsController",
                    data: {
                        title: "جزئیات کاربر"
                    }
                })
                .state("users.details.edit", {
                    url: "/edit",
                    templateUrl: "Application/adminApplication/views/Users/users.edit.html",
                    controller: "EditUserController",
                    data: {
                        title: "ویرایش کاربر"
                    }
                });
        }
    ]);
}