module JahanJooy.HaftDong.Customer {
    export var appModule = angular.module("haftdongCustomers", [
        "ui.router", "ui.bootstrap", "toastr", "jjCommon",
        "haftdongApi"]);
    appModule.config([
        "$stateProvider",
        ($stateProvider: angular.ui.IStateProvider) => {
            $stateProvider
                .state("customers", {
                    url: "/app/customers",
                    templateUrl: "Application/userApplication/views/Customers/customers.html",
                    controller: "CustomerController",
                    data: {
                        title: "لیست مشتریان"
                    }
                })
                .state("customers.new", {
                    url: "/new",
                    templateUrl: "Application/userApplication/views/Customers/customers.new.html",
                    controller: "NewCustomerController",
                    data: {
                        title: "مشتری جدید"
                    }
                })
                .state("customers.details", {
                    url: "/details/{id}",
                    templateUrl: "Application/userApplication/views/Customers/customers.details.html",
                    controller: "CustomerDetailsController",
                    data: {
                        title: "جزئیات مشتری"
                    }
                })
                .state("customers.details.edit", {
                    url: "/edit",
                    templateUrl: "Application/userApplication/views/Customers/customers.edit.html",
                    controller: "EditCustomerController",
                    data: {
                        title: "ویرایش مشتری"
                    }
                });
        }
    ]);
}