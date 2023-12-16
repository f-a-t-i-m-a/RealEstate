using System;
using System.Collections.Generic;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Util.Models.Contracts;
using JahanJooy.RealEstateAgency.Util.Models.Requests;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using JahanJooy.RealEstateAgency.Util.Report.ApplicationImplemented;
using MongoDB.Bson;
using Stimulsoft.Report;

namespace JahanJooy.RealEstateAgency.ReportDesigner
{
    public static class ReportDictionaryFiller
    {
        public static void Fill(StiReport report, ReportDesignerParams parameters)
        {
            switch (parameters.DataSourceType)
            {
//                    case ReportDataSourceType.DirectDbConnection:
//                        FillDirectDbConnection(report, parameters);
//                        break;

                case ReportDataSourceType.ApplicationImplemented:
                    FillApplicationImplemented(report, parameters);
                    break;
            }
        }

//        public static void Fill(string reportTemplateId, ReportDesignerParams parameters)
//        {
//            switch (parameters.DataSourceType)
//            {
//                //                    case ReportDataSourceType.DirectDbConnection:
//                //                        FillDirectDbConnection(report, parameters);
//                //                        break;
//
//                case ReportDataSourceType.ApplicationImplemented:
//                    FillApplicationImplemented(reportTemplateId, parameters);
//                    break;
//            }
//        }

        //        public static void FillDirectDbConnection(StiReport report, ReportDesignerParams parameters)
        //        {
        //            Program.Composer.GetComponent<DirectDbReport>().PopulateConnectionStrings(report);
        //        }

        public static void FillApplicationImplemented(StiReport report, ReportDesignerParams parameters)
        {
            if (!parameters.ApplicationImplementedDataSourceType.HasValue)
                throw new ArgumentException("ApplicationImplementedDataSourceType is required");

            switch (parameters.ApplicationImplementedDataSourceType.Value)
            {
                case ApplicationImplementedReportDataSourceType.Property:
                    Program.Composer.GetComponent<PropertyReport>().PopulatePropertyDetails(report, parameters.ID, false);
                    break;
                case ApplicationImplementedReportDataSourceType.Requests:
                    Program.Composer.GetComponent<RequestReport>()
                        .PopulateRequestList(report, new SearchRequestInput(), new List<ObjectId>(), false);
                    break;
                case ApplicationImplementedReportDataSourceType.Request:
                    Program.Composer.GetComponent<RequestReport>().PopulateRequestDetails(report, parameters.ID);
                    break;
                case ApplicationImplementedReportDataSourceType.Supplies:
                    Program.Composer.GetComponent<SupplyReport>()
                        .PopulateSupplyList(report, new SearchFileInput(), new List<ObjectId>(), true);
                    break;
                case ApplicationImplementedReportDataSourceType.Contracts:
                    Program.Composer.GetComponent<ContractReport>()
                        .PopulateContractList(report, new SearchContractInput(), new List<ObjectId>());
                    break;
                case ApplicationImplementedReportDataSourceType.Contract:
                    Program.Composer.GetComponent<ContractReport>().PopulateContractDetails(report, parameters.ID);
                    break;
            }
        }

//        public static void FillApplicationImplemented(string reportTemplateId, ReportDesignerParams parameters)
//        {
//            if (!parameters.ApplicationImplementedDataSourceType.HasValue)
//                throw new ArgumentException("ApplicationImplementedDataSourceType is required");
//
//            string url = "";
//            var input = new object();
//            switch (parameters.ApplicationImplementedDataSourceType.Value)
//            {
//                case ApplicationImplementedReportDataSourceType.Properties:
//                    input = new PrintPropertiesInput
//                    {
//                        ReportTemplateID = reportTemplateId,
//                        SearchInput = new SearchPropertyInput(),
//                        Ids = new List<string>()
//                    };
//                    url = "/properties/print";
//                    break;
//                case ApplicationImplementedReportDataSourceType.Property:
//                    input = new PrintPropertyInput
//                    {
//                        ReportTemplateID = reportTemplateId
//                    };
//                    if (!string.IsNullOrEmpty(parameters.ID))
//                    {
//                        url = "/properties/details/" + parameters.ID + "/print";
//                    }
//                    else
//                    {
//                        url = "/properties/printfirst";
//                    }
//                    break;
//            }
//            var content = new StringContent(input.ToJson(), Encoding.UTF8, "application/json");
//            HttpClient client = new HttpClient {BaseAddress = new Uri(LoginForm.BaseAddress)};
//            client.DefaultRequestHeaders.Accept.Clear();
//            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginForm.OathToken);
//            var response = client.PostAsync(LoginForm.UrlPrefix + url, content).Result;
//            var body = response.Content.ReadAsStringAsync().Result;
//        }
    }
}