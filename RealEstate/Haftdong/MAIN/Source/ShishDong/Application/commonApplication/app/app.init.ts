module JahanJooy.ShishDong {
    export interface IAppRootScopeService extends ng.IRootScopeService {
        $state: ng.ui.IStateService;
        $stateParams: ng.ui.IStateParamsService;
        globalErrors: ServerErrorResponse[];
        inProgressRequests: number;
    }

    appModule.config([
        "$stateProvider", "$urlRouterProvider", "$locationProvider", "$httpProvider",
        ($stateProvider: ng.ui.IStateProvider, $urlRouterProvider: ng.ui.IUrlRouterProvider, $locationProvider: ng.ILocationProvider, $httpProvider: ng.IHttpProvider) => {

            $urlRouterProvider.otherwise("/app/welcome");
            $locationProvider.html5Mode(true);

            $httpProvider.interceptors.push("counterHttpInterceptor");
            $httpProvider.interceptors.push("errorsHttpInterceptor");
            $httpProvider.interceptors.push("authHttpInterceptor");
        }
    ]);

    appModule.run([
        "$rootScope", "$state", "$stateParams", "toastr",
        ($rootScope: IAppRootScopeService, $state: ng.ui.IStateService, $stateParams: ng.ui.IStateParamsService, toastr) => {
            $rootScope.$state = $state;
            $rootScope.$stateParams = $stateParams;
            $rootScope.globalErrors = [];
            $rootScope.inProgressRequests = 0;

            moment.loadPersian();
        }
    ]);
}
 