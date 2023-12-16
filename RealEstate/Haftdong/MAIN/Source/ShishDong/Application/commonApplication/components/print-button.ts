module JahanJooy.ShishDong.Components {
    import DownloadUtils = Common.DownloadUtils;
    ngModule.directive('jjPrintButton', () => {
        return {
            restrict: 'E',
            templateUrl: 'Application/commonApplication/components/tmpl-print-button/print-button.html',
            scope: {
                sourceType: "=",
                endpoint: "@",
                filename: "@",
                showOfficeType: "@",
                searchInput: "=",
                ids: "="
            },
            controller: ($scope, $http, reportTemplateCache: Components.IReportTemplateCache) => {

                $scope.print = (templateId: string, format: string, extension: string) => {
                    $http.post($scope.endpoint, {
                        reportTemplateId: templateId,
                        format: format,
                        searchInput: $scope.searchInput,
                        ids: $scope.ids
                    }, { responseType: "blob" }).success((data: Blob) => {
                        DownloadUtils.downloadBlob(data, $scope.filename + "." + extension);
                    });
                };

                reportTemplateCache.ensureLoaded().then(() => {
                    $scope.templates = reportTemplateCache.appImplementedList($scope.sourceType);
                });
            }
        };
    });

}