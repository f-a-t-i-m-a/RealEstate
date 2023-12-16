module JahanJooy.HaftDong.Log {
    ngModule.controller("LogController", [
        '$scope', '$rootScope', '$http', '$state', '$stateParams', '$modal', 'toastr', 'messageBoxService', '$q', 'scopes',
        ($scope, $rootScope, $http: ng.IHttpService, $state, $stateParams, $modal, toastr, messageBoxService, $q, scopes) => {

            $scope.GetTitle = () => {
                return 'لاگ ها';
            }
            scopes.store('LogController', $scope);

            $scope.searchInput = {
                LogType: ""
            }

            $scope.onSearch = () => {
                $http.get("/api/web/log/all", null).success((data: any) => {
                    $scope.logFiles = data.LogFiles;
                    $scope.loggers = data.Loggers;
                    $scope.loggers.forEach(l => l.IsInfoEnabled = (l.EffectiveLevel === "INFO"));
                });
            };

            $scope.onBackClick = () => {
                $state.go("^");
            }

            $scope.changeDebugLevel = (logger, value) => {
                $http.post("/api/web/log/debug", {
                    Logger: logger.Name,
                    DebugEnabled: value
                }).success(() => {
                    $scope.onSearch();
                });
            }

            $scope.appendDebugAppender = (logger, value) => {
                $http.post("/api/web/log/append", {
                    Logger: logger.Name,
                    Append: value
                }).success(() => {
                    $scope.onSearch();
                });
            }

            $scope.viewLogFile = (log) => {
                $http.post("/api/web/log/view", {
                    LogName: log.Name
                }).success((data: any) => {
                    $scope.LogFileData = data;
                    var modalInstance = $modal.open({
                        templateUrl: 'Application/adminApplication/views/Log/view.log.modal.html',
                        controller: 'ViewLogModalController',
                        scope: $scope,
                        size: 'lg'
                    });
                });
            }

            function initialize() {
                $scope.onSearch();
            };

            initialize();

            var listener = $rootScope.$on('EntityUpdated', () => {
                $scope.onSearch();
            });

            $scope.$on('$destroy', listener);
        }
    ]);

}