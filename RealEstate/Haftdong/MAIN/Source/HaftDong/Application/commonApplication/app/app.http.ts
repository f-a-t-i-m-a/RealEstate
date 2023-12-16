module JahanJooy.HaftDong {

    appModule.factory("counterHttpInterceptor", [
        "$q", "$rootScope",
        ($q: ng.IQService, $rootScope: IAppRootScopeService) => {
            return {
                request(config) {
                    $rootScope.inProgressRequests++;
                    return config;
                },
                requestError(rejection) {
                    $rootScope.inProgressRequests--;
                    return $q.reject(rejection);
                },
                response(response) {
                    $rootScope.inProgressRequests--;
                    return response;
                },
                responseError(rejection) {
                    $rootScope.inProgressRequests--;
                    return $q.reject(rejection);
                }
            };
        }
    ]);
}
