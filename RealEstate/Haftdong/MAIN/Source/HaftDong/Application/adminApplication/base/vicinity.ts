module JahanJooy.HaftDong.Vicinity {
    export var appModule = angular.module("haftdongVicinity", [
        "ui.router", "ui.bootstrap", "toastr", "jjCommon",
        "ui.bootstrap.persian.datepicker", "ui.bootstrap.datepicker",
        "haftdongComponents", "ngFileUpload",
        "haftdongApi"
    ]);
    appModule.config([
        "$stateProvider",
        ($stateProvider: angular.ui.IStateProvider) => {
            $stateProvider
                .state("vicinities", {
                    url: "/adminapp/vicinities/{id}",
                    templateUrl: "Application/adminApplication/views/Vicinities/vicinities.html",
                    controller: "VicinityController",
                    data: {
                        title: "لیست محله ها"
                    }
                })
                .state("vicinities.new", {
                    url: "/new",
                    templateUrl: "Application/adminApplication/views/Vicinities/vicinities.new.html",
                    controller: "NewVicinityController",
                    data: {
                        title: "محله جدید"
                    }
                })
                .state("vicinities.edit", {
                    url: "/edit",
                    templateUrl: "Application/adminApplication/views/Vicinities/vicinities.edit.html",
                    controller: "EditVicinityController",
                    data: {
                        title: "ویرایش محله"
                    }
                });
        }
    ]);
}