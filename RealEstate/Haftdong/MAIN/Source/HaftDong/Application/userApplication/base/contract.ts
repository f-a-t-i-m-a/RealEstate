module JahanJooy.HaftDong.Contract {
    export var appModule = angular.module("haftdongContracts", [
        "ui.router", "ui.bootstrap", "toastr", "jjCommon",
        "haftdongApi", "haftdongComponents"]);
    appModule.config([
        "$stateProvider",
        ($stateProvider: angular.ui.IStateProvider) => {
            $stateProvider
                .state("contracts", {
                    url: "/app/contracts",
                    templateUrl: "Application/userApplication/views/Contracts/contracts.html",
                    controller: "ContractController",
                    data: {
                        title: "لیست قراردادها"
                    }
                })
                .state("contracts.new", {
                    url: "/new",
                    templateUrl: "Application/userApplication/views/Contracts/contracts.new.html",
                    controller: "NewContractController",
                    data: {
                        title: "قرارداد جدید"
                    }
                })
                .state("contracts.details", {
                    url: "/details/{id}",
                    templateUrl: "Application/userApplication/views/Contracts/contracts.details.html",
                    controller: "ContractDetailsController",
                    data: {
                        title: "جزئیات قرارداد"
                    }
                })
                .state("contracts.details.edit", {
                    url: "/edit",
                    templateUrl: "Application/userApplication/views/Contracts/contracts.edit.html",
                    controller: "EditContractController",
                    data: {
                        title: "ویرایش قرارداد"
                    }
                });
        }
    ]);
}