module JahanJooy.ShishDong.File {
    export var appModule = angular.module("shishDongFiles", [
        "ui.router", "ui.bootstrap", "toastr", "jjCommon",
        "ui.bootstrap.persian.datepicker", "ui.bootstrap.datepicker",
        "realEstateComponents", "ngFileUpload",
        "shishDongApi"
    ]);
    appModule.config([
        "$stateProvider",
        ($stateProvider: angular.ui.IStateProvider) => {
            $stateProvider
                .state("welcome.files", {
                    url: "/files",
                    templateUrl: "Application/userApplication/views/Files/files.html",
                    controller: "FileController",
                    data: {
                        title: "لیست فایل ها"
                    }
                })
                .state("welcome.files.details", {
                    url: "/details/{id}",
                    templateUrl: "Application/userApplication/views/Files/files.details.html",
                    controller: "FileDetailsController",
                    data: {
                        title: "جزئیات فایل"
                    }
                })
                .state("welcome.files.new", {
                    url: "/new",
                    templateUrl: "Application/userApplication/views/Files/files.new.html",
                    controller: "NewFileController",
                    data: {
                        title: "ملک جدید"
                    }
                })
                .state("welcome.files.details.edit", {
                    url: "/edit",
                    templateUrl: "Application/userApplication/views/Files/files.edit.html",
                    controller: "EditFileController",
                    data: {
                        title: "ویرایش ملک"
                    }
                })
                .state("welcome.myfiles", {
                    url: "/myfiles",
                    templateUrl: "Application/userApplication/views/Files/my.files.html",
                    controller: "MyFilesController",
                    data: {
                        title: "فایل های من"
                    }
                });
        }
    ]);
}