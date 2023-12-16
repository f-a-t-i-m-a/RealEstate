module JahanJooy.ShishDong.Components {
    import ReportTemplateSummary = RealEstateAgency.Util.Models.Report.ReportTemplateSummary;
    import ApplicationImplementedReportDataSourceType = RealEstateAgency.Domain.MasterData.ApplicationImplementedReportDataSourceType;
    import IReportApi = ShishDong.Api.IReportApi;
    import ListOutput = RealEstateAgency.Util.Models.Report.ReportListOutput;

    export interface IReportTemplateCache {
        refresh(): ng.IPromise<any>;
        ensureLoaded(): ng.IPromise<any>;

        all(): ReportTemplateSummary[];
        id(i: number): ReportTemplateSummary;
        appImplementedList(t: ApplicationImplementedReportDataSourceType): ReportTemplateSummary[];
        generalList(): ReportTemplateSummary[];
    }

    export interface IReportTemplateIndexByAppImpl {
        [t: number]: ReportTemplateSummary[];
    }

    export interface IReportTemplateIndexById {
        [id: number]: ReportTemplateSummary;
    }

    export interface IReportTemplateIndexByAppImplAndId {
        [t: number]: IReportTemplateIndexById;
    }

    class ReportTemplateCache implements IReportTemplateCache {
        static $name = "reportTemplateCache";
        static $inject = ["$q", "reportApi"];
        constructor(private $q: ng.IQService, private reportApi: IReportApi) {
        }

        private loadState = 0;

        private listAll: ReportTemplateSummary[] = [];
        private listGeneral: ReportTemplateSummary[] = [];
        private listByAppImpl: IReportTemplateIndexByAppImpl = {};
        private indexById: IReportTemplateIndexById = {};
        private indexByAppImplAndId = {};

        private pendingDefers = [];

        private rebuildIndexes(loadedItems: ReportTemplateSummary[]) {
            this.listAll = loadedItems;
            this.listGeneral = [];
            this.listByAppImpl = {};
            this.indexById = {};
            this.indexByAppImplAndId = {};

            this.listAll.forEach((item) => {

                this.indexById[item.ID.toString()] = item;

                if (item.ApplicationImplementedDataSourceType) {

                    if (!(this.listByAppImpl[item.ApplicationImplementedDataSourceType]))
                        this.listByAppImpl[item.ApplicationImplementedDataSourceType] = [];
                    if (!(this.indexByAppImplAndId[item.ApplicationImplementedDataSourceType]))
                        this.indexByAppImplAndId[item.ApplicationImplementedDataSourceType] = {};

                    this.listByAppImpl[item.ApplicationImplementedDataSourceType].push(item);
                    this.indexByAppImplAndId[item.ApplicationImplementedDataSourceType][item.ID.toString()] = item;
                } else {
                    this.listGeneral.push(item);
                }
            });
        }

        refresh(): any {
            var deferred = this.$q.defer();
            this.pendingDefers.push(deferred);

            this.loadState = 1;
            this.reportApi.list().success((data: ListOutput) => {
                this.rebuildIndexes(data.Templates);
                this.loadState = 2;
                this.pendingDefers.forEach(d => { d.resolve(); });
                this.pendingDefers = [];
            }).error(() => {
                this.loadState = 0;
                this.pendingDefers.forEach(d => { d.reject(); });
                this.pendingDefers = [];
            });

            return deferred.promise;
        }

        ensureLoaded(): any {
            if (!this.loadState)
                return this.refresh();

            var deferred = this.$q.defer();

            if (this.loadState === 1) {
                this.pendingDefers.push(deferred);
            } else {
                deferred.resolve();
            }

            return deferred.promise;
        }

        all(): ReportTemplateSummary[] {
            return this.listAll;
        }

        id(i: number): ReportTemplateSummary {
            return this.indexById[i];
        }

        appImplementedList(t: ApplicationImplementedReportDataSourceType): ReportTemplateSummary[] {
            return this.listByAppImpl[t];
        }

        generalList(): ReportTemplateSummary[] {
            return this.listGeneral;
        }
    }

    appModule.service(ReportTemplateCache.$name, ReportTemplateCache);
    appModule.run([
        ReportTemplateCache.$name, "authService",
        (reportTemplateCache: IReportTemplateCache, authService) => {
            if (!authService.isAnonymous())
                reportTemplateCache.ensureLoaded();
        }
    ]);
}
