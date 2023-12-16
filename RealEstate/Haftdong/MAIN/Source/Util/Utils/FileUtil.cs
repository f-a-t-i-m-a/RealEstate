using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.Common.Util.Text;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Models.Map;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using JahanJooy.RealEstateAgency.Util.Report;
using JahanJooy.RealEstateAgency.Util.Report.ApplicationImplemented;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils.Map;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using JahanJooy.Stimulsoft.Common.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceStack;
using Stimulsoft.Report;

namespace JahanJooy.RealEstateAgency.Util.Utils
{
    [Contract]
    [Component]
    public class FileUtil
    {
        #region Injected dependencies

        [ComponentPlug]
        public SupplyReport SupplyReport { get; set; }

        [ComponentPlug]
        public PropertyReport PropertyReport { get; set; }

        [ComponentPlug]
        public ReportRepository ReportRepository { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        [ComponentPlug]
        public PropertyUtil PropertyUtil { get; set; }

        [ComponentPlug]
        public RequestUtil RequestUtil { get; set; }

        [ComponentPlug]
        public CustomerUtil CustomerUtil { get; set; }

        [ComponentPlug]
        public SupplyUtil SupplyUtil { get; set; }

        [ComponentPlug]
        public VicinityUtil VicinityUtil { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

        #endregion

        #region Action methods

        public PagedListOutput<SupplySummary> Search(SearchFileInput searchInput, bool publicOnly, bool? mineOnly)
        {
            var response = SupplyReport.GetResultFromElastic(searchInput, publicOnly, mineOnly);
            var ids = response.Documents.Select(d => ObjectId.Parse(d.ID)).ToList();
            var sortedSupplies = new List<SupplySummary>();

            if (ids.Count != 0)
            {
                var filter = Builders<Supply>.Filter.In("ID", ids);
                var supplies = DbManager.Supply.Find(filter)
                    .Project(p => Mapper.Map<SupplySummary>(p))
                    .ToListAsync().Result;

                supplies.ForEach(s => s.CreatorFullName = UserUtil.GetUserName(s.CreatedByID));

                ids.ForEach(id => sortedSupplies.Add(supplies.SingleOrDefault(pr => pr.ID == id)));
                sortedSupplies.RemoveAll(s => s == null);

                sortedSupplies.ForEach(s =>
                {
                    if (s?.Property?.Vicinity != null)
                    {
                        var vicinityCompleteName = VicinityUtil.GetFullName(s.Property.Vicinity.ID);

                        if (!vicinityCompleteName.IsNullOrWhitespace())
                            s.Property.Vicinity.CompleteName = vicinityCompleteName;
                    }
                });
            }

            var result = new PagedListOutput<SupplySummary>
            {
                PageItems = sortedSupplies,
                PageNumber = searchInput.StartIndex/searchInput.PageSize + 1,
                TotalNumberOfItems = (int) response.Total,
                TotalNumberOfPages = (int) Math.Ceiling((decimal) response.Total/searchInput.PageSize)
            };

            return result;
        }

        public void SearchInMap(SearchFileInput searchInput, GeoSearchResult mapResult, bool publicOnly, bool? mineOnly)
        {
            var mapResponse = SupplyReport.GetResultFromElastic(searchInput, publicOnly, mineOnly);
            var mapIds = mapResponse.Documents.Select(d => ObjectId.Parse(d.ID)).ToList();

            if (mapIds.Count == searchInput.PageSize)
            {
                mapResult.ReachedMaxResult = true;
            }

            var supplies = new List<SupplySummary>();
            if (mapIds.Count != 0)
            {
                var mapFilter = Builders<Supply>.Filter.In("ID", mapIds)
                                & Builders<Supply>.Filter.Ne("Property.GeographicLocation", BsonNull.Value);
                supplies = DbManager.Supply.Find(mapFilter)
                    .Project(s => Mapper.Map<SupplySummary>(s))
                    .ToListAsync().Result;

                mapResult.SupplySummaries?.ForEach(s => s.CreatorFullName = UserUtil.GetUserName(s.CreatedByID));

                supplies.ForEach(s =>
                {
                    if (s?.Property?.Vicinity != null)
                    {
                        var vicinityCompleteName = VicinityUtil.GetFullName(s.Property.Vicinity.ID);

                        if (!vicinityCompleteName.IsNullOrWhitespace())
                            s.Property.Vicinity.CompleteName = vicinityCompleteName;
                    }
                });
            }

            var clusterCalculator = Composer.GetComponent<GeographicClusterCalculator>();
            clusterCalculator.SetMinimumAreaToBreakDown(mapResult.MinimumDistinguishedArea);
            clusterCalculator.ClusterProperties(supplies);
            clusterCalculator.PrepareResults(mapResult);
        }

        public ReportBinaryResult Print(string id, PrintPropertyInput input, bool publicOnly)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));

            var filter = Builders<Property>.Filter.Eq("ID", ObjectId.Parse(id));
            var property = DbManager.Property.Find(filter).SingleOrDefaultAsync().Result;

            if (publicOnly && property?.Supplies != null && property.Supplies.TrueForAll(s => !s.IsPublic))
            {
                return null;
            }

            var report = ReportRepository.GetApplicationImplemented(
                ApplicationImplementedReportDataSourceType.Property,
                input.IfNotNull(i => i.ReportTemplateID));

            if (report == null)
                return null;
            PropertyReport.PopulatePropertyDetails(report, id, publicOnly);

            StiExportFormat format;
            if (!Enum.TryParse(input.Format, true, out format))
                format = StiExportFormat.Pdf;

            return new ReportBinaryResult(report, format);
        }

        public ReportBinaryResult PrintAll(PrintSuppliesInput input, bool publicOnly)
        {
            if (input != null)
            {
                var report = ReportRepository.GetApplicationImplemented(
                    ApplicationImplementedReportDataSourceType.Supplies,
                    input.IfNotNull(i => i.ReportTemplateID));

                if (report == null)
                    return null;

                SupplyReport.PopulateSupplyList(report, input.SearchInput, input.Ids.Select(ObjectId.Parse).ToList(),
                    publicOnly);

                StiExportFormat format;
                if (!Enum.TryParse(input.Format, true, out format))
                    format = StiExportFormat.Pdf;

                return new ReportBinaryResult(report, format);
            }

            return null;
        }

        public ValidationResult SaveFile(Property property, Supply supply, Customer customer, Request request,
            bool isPublic, SourceType sourceType, ApplicationType applicationType)
        {
            if (!isPublic)
            {
                var customerResult = CustomerUtil.GetCustomerByCustomerDetails(customer);
                if (!customerResult.IsValid)
                {
                    return customerResult;
                }

                property.Owner = customerResult.Result;
            }
            else
            {
                if (supply.AgencyContact == null && supply.OwnerContact == null)
                {
                    var contactInfo = UserUtil.GetContactInfoOfCurrentUser();
                    if (contactInfo == null)
                        return ValidationResult.Failure("Supply.ContactInfo", GeneralValidationErrors.ValueNotSpecified);

                    supply.OwnerCanBeContacted = true;
                    supply.OwnerContact = contactInfo;
                }

                if (supply.OwnerCanBeContacted.HasValue && !supply.OwnerCanBeContacted.Value &&
                    (supply.AgencyContact?.Phones == null || supply.AgencyContact?.Phones.Count == 0))
                {
                    return ValidationResult.Failure("Supply.Phones", GeneralValidationErrors.ValueNotSpecified);
                }

                if (supply.OwnerCanBeContacted.HasValue && supply.OwnerCanBeContacted.Value && 
                    (supply.OwnerContact?.Phones == null || supply.OwnerContact?.Phones.Count == 0))
                {
                    return ValidationResult.Failure("Supply.Phones", GeneralValidationErrors.ValueNotSpecified);
                }
            }

            var result = PropertyUtil.ValidateForSaveOrUpdate(property, isPublic);
            if (!result.IsValid)
                return result;

            result = SupplyUtil.ValidateForSave(supply, property, isPublic);
            if (!result.IsValid)
                return result;

            if (supply.IntentionOfOwner == IntentionOfOwner.ForSwap)
            {
                result = RequestUtil.ValidateForSave(request);
                if (supply.SwapAdditionalComments.IsNullOrEmpty() && !result.IsValid)
                    return result;
            }

            var saveResult = PropertyUtil.SaveProperty(property, supply, request, isPublic, sourceType, applicationType);
            if (!saveResult.IsValid)
                return saveResult;

            return ValidationResult.Success;
        }

        #endregion

        #region Private helper methods 


        #endregion
    }
}