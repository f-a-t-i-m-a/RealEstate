module JahanJooy.HaftDong.File {
    export var appModule = angular.module("haftdongFiles", [
        "ui.router", "ui.bootstrap", "toastr", "jjCommon",
        "ui.bootstrap.persian.datepicker", "ui.bootstrap.datepicker",
        "haftdongComponents", "ngFileUpload",
        "haftdongApi"
    ]);
    appModule.config([
        "$stateProvider",
        ($stateProvider: angular.ui.IStateProvider) => {
            $stateProvider
                .state("files", {
                    url: "/app/files",
                    templateUrl: "Application/userApplication/views/Supplies/supplies.html",
                    controller: "SupplyController",
                    data: {
                        title: "لیست فایل ها"
                    }
                })
                .state("files.new", {
                    url: "/new/{customerId}",
                    templateUrl: "Application/userApplication/views/Properties/properties.new.html",
                    controller: "NewPropertyController",
                    data: {
                        title: "ملک جدید"
                    }
                })
                .state("files.details", {
                    url: "/details/{id}",
                    templateUrl: "Application/userApplication/views/Properties/properties.details.html",
                    controller: "PropertyDetailsController",
                    data: {
                        title: "جزئیات ملک"
                    }
                })
                .state("files.details.edit", {
                    url: "/edit",
                    templateUrl: "Application/userApplication/views/Properties/properties.edit.html",
                    controller: "EditPropertyController",
                    data: {
                        title: "ویرایش ملک"
                    }
                })
                .state("myfiles", {
                    url: "/app/myfiles",
                    templateUrl: "Application/userApplication/views/Profile/my.supplies.html",
                    controller: "MySuppliesController",
                    data: {
                        title: "فایل های من"
                    }
                })
                .state("mypublicfiles", {
                    url: "/app/mypublicfiles",
                    templateUrl: "Application/userApplication/views/Profile/my.public.supplies.html",
                    controller: "MyPublicSuppliesController",
                    data: {
                        title: "فایل های عمومی من"
                    }
                });
        }
    ]);
}