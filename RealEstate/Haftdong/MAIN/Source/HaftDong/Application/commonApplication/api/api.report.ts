module JahanJooy.HaftDong.Api {
    import ListOutput = RealEstateAgency.Util.Models.Report.ReportListOutput;

    export interface IReportApi {
        list(): ng.IHttpPromise<ListOutput>;
    }

    class ReportApi implements IReportApi {
        static $name = "reportApi";
        static $inject = ["$http", "$q"];
        constructor(private $http: ng.IHttpService, private $q: ng.IQService) {
        }

        list(): any {
            return this.$http.get("/api/web/report/list");
        }
    }

    ngModule.service(ReportApi.$name, ReportApi);
}
