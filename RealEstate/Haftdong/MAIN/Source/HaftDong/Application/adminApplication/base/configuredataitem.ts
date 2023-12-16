module JahanJooy.HaftDong.ConfigureDataItem {
    export var ngModule = angular.module("haftdongConfigureDataItem", ['ui.router']);
    ngModule.config([
        '$stateProvider',
        ($stateProvider: ng.ui.IStateProvider) => {
            $stateProvider
                .state('configure', {
                    url: "/adminapp/configuredataitem",
                    templateUrl: "Application/adminApplication/views/ConfigureDataItem/configure.data.item.html",
                    controller: "ConfigureDataItemController",
                    data: {
                        title: "پیکربندی ها"
                    }
                })
                .state('configure.new', {
                    url: "/addnew",
                    templateUrl: "Application/adminApplication/views/ConfigureDataItem/configure.data.item.new.html",
                    controller: "AddNewConfigureDataItemController",
                    data: {
                        title: "پیکربندی جدید"
                    }
                })
                .state('configure.edit', {
                    url: "/edit/{identifier}",
                    templateUrl: "Application/adminApplication/views/ConfigureDataItem/configure.data.item.edit.html",
                    controller: "EditConfigureDataItemController",
                    data: {
                        title: "ویرایش پیکربندی"
                    }
                });
        }
    ]);

}