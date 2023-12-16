"use strict";

// Code and logic inspired by:
//     - http://brewhouse.io/blog/2014/12/09/authentication-made-simple-in-single-page-angularjs-applications.html
//     - http://bitoftech.net/2014/06/09/angularjs-token-authentication-using-asp-net-web-api-2-owin-asp-net-identity/


module JahanJooy.HaftDong {

    appModule.directive("restrictTo", [
        "authService",
        authService => {
            return {
                restrict: "A",
                prioriry: 100000,
                scope: false,
                link() {
                },
                compile(element, attr, linker) {
                    if (authService.isAdministrator())
                        return;

                    var accessDenied = true;

                    var allowedRoles: string[] = attr.restrictTo.split(",");
                    if (Enumerable.from(allowedRoles).any(ar => authService.isInRole(ar.trim())))
                        accessDenied = false;

                    if (accessDenied) {
                        element.children().remove();
                        element.remove();
                    }
                }
            };
        }
    ]);

    appModule.directive("licensed", [
        "authService",
        authService => {
            return {
                restrict: "A",
                prioriry: 100000,
                scope: false,
                link() {
                },
                compile(element, attr, linker) {
                    if (authService.isAdministrator())
                        return;

                    var haveLicense = false;
                    var user = authService.getCurrentUser();
                    if (user != null && user.user != null
                        && moment(user.user.LicenseActivationTime).format("YYYY/MM/DD") >= moment().format("YYYY/MM/DD")) {
                        haveLicense = true;
                    }

                    if (!haveLicense) {
                        element.children().remove();
                        element.remove();
                    }
                }
            };
        }
    ]);

    class AuthorizedToDirectiveInstance {
        constructor(private authService) {
        }

        link(scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) {
            var accessDenied;

            scope.$watch(attrs["authorizedTo"],
                (value: any) => {
                    if (this.authService.isAnonymous()) {
                        accessDenied = true;
                    } else if (this.authService.isAdministrator() || this.authService.getCurrentUser().user.Id === value) {
                        accessDenied = false;
                    } else {
                        accessDenied = true;
                    }

                    if (accessDenied) {
                        element.hide();
                    } else {
                        element.show();
                    }
                });
        }
    }

    class AuthorizedToDirectiveDefinition implements ng.IDirective {
        static $name = "authorizedTo";
        static $inject = ["authService"];

        constructor(private authService) {
        }

        static getFactory(): ng.IDirectiveFactory {
            var factory = (authService) => new AuthorizedToDirectiveDefinition(authService);
            factory.$inject = AuthorizedToDirectiveDefinition.$inject;
            return factory;
        }

        restrict = "A";
        link = (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
            new AuthorizedToDirectiveInstance(this.authService).link(scope, element, attrs);
        }
    }

    appModule.directive(AuthorizedToDirectiveDefinition.$name, AuthorizedToDirectiveDefinition.getFactory());


    class ShowDirectiveInstance {
        link(scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) {
            var showElement;

            scope.$watch(attrs["show"],
                (value: any) => {
                    if (value == null || value === 0) {
                        showElement = false;
                    } else {
                        showElement = true;
                        element.removeClass("EmptyText");
                    }

                    if (!showElement) {
                        element.addClass("EmptyText");
                    }
                });
        }
    }

    class ShowDirectiveDefinition implements ng.IDirective {
        static $name = "show";

        static getFactory(): ng.IDirectiveFactory {
            var factory = () => new ShowDirectiveDefinition();
            return factory;
        }

        restrict = "A";
        link = (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) => {
            new ShowDirectiveInstance().link(scope, element, attrs);
        }
    }

    appModule.directive(ShowDirectiveDefinition.$name, ShowDirectiveDefinition.getFactory());


    appModule.run([
        "$rootScope", "$state", "authService", "scopes",
        ($rootScope: angular.IRootScopeService, $state, authService, scopes) => {
            $rootScope.$on("$stateChangeStart", (event, toState, toParams) => {
                var allowAnonymous = toState.data ? toState.data.allowAnonymous : false;

                if (!allowAnonymous && authService.isAnonymous()) {
                    event.preventDefault();
                    authService.showLogin()
                        //Uncomment it when the problem of main menu and refreshing it is solved
//                        .then(() => $state.go(toState.name, toParams), () => $state.go("accessDenied"))
                        .catch(() => $state.go("accessDenied"));
                }

                if (toState.controller != null)
                    document.title = toState.data.title;
            });
        }
    ]);

    appModule.factory('scopes', () => {
        var mem = {};

        return {
            store(key, value) {
                mem[key] = value;
            },
            get(key) {
                return mem[key];
            }
        };
    });

    appModule.factory("authHttpInterceptor", [
        "$q", "$timeout", "$injector",
        ($q: angular.IQService, $timeout: angular.ITimeoutService, $injector) => {
            var authService, $http, $state;

            $timeout(() => {
                authService = $injector.get("authService");
                $http = $injector.get("$http");
                $state = $injector.get("$state");
            });

            return {
                request(config) {
                    if (!authService) {
                        authService = $injector.get("authService");
                    }

                    if (authService && !authService.isAnonymous()) {
                        config.headers = config.headers || {};
                        config.headers.Authorization = "Bearer " + authService.getCurrentUser().token;
                    }

                    return config;                    
                },
                responseError(rejection) {
                    if (rejection.status !== 401) {
                        return $q.reject(rejection);
                    }

                    if (authService && !authService.isAnonymous()) {
                        return $q.reject(rejection);
                    }

                    var deferred = $q.defer();

                    authService.showLogin()
                        .then(() => {
                            deferred.resolve($http(rejection.config));
                        })
                        .catch(() => {
                            $state.go("accessDenied");
                            deferred.reject(rejection);
                        });

                    return deferred.promise;
                }
            };
        }
    ]);
}
