/// <reference path="../site-base/AutoRun.ts" />
/// <reference path="../TypeLite.Net4.d.ts" />
/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/angularjs/angular.d.ts" />

module Admin {
    AutoRun.register(AutoRun.Scope.Page, "mainadminhomeindex", {
        onBeforeInit: () => { },
        onDoInit: (root: JQuery) => {
             setupAngular();
        },
        onAfterInit: () => { }
    });

    interface IHomeScope extends ng.IScope {
        status: AdminHome.AdminHomeIndexModel;
    }


    function setupAngular() {
        var adminHomeApp = angular.module('adminHomeApp', []);

        adminHomeApp.controller('AdminHomeController', ['$scope', '$http', ($scope: IHomeScope, $http: ng.IHttpService) => {
            $http.post("/admin/adminhome/GetStatusSummary", null).success((data: AdminHome.AdminHomeIndexModel) => {
                $scope.status = data;
            });
        }]);
    }
} 
