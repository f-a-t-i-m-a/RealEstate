
module JahanJooy.HaftDong.Operation {
    export var appModule = angular.module("haftdongOperations", [
        "ui.router", "ui.bootstrap", "toastr", "jjCommon",
        "haftdongApi"]);
    appModule.config([
        "$stateProvider",
        ($stateProvider: angular.ui.IStateProvider) => {
            $stateProvider
                .state("operations", {
                    url: "/app/operations",
                    templateUrl: "Application/userApplication/views/Operations/operations.html",
                    controller: "OperationController",
                    data: {
                        title: "عملیات"
                    }
                });
        }
    ]);
}