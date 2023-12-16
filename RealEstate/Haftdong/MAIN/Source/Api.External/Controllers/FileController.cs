using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Api.External.Base;
using JahanJooy.RealEstateAgency.Api.External.Models.File;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Util.Attachments;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.Api.External.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("externalfiles")]
    public class FileController : ExternalSourceExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public PhotoGridStore PhotoGridStore { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        #endregion

        #region Action methods

        [HttpPost, Route("save")]
        [SkipUserActivityLogging]
        public IHttpActionResult SaveExternalFile(ExternalNewFileInput input)
        {
            if (input == null)
            {
                ApplicationStaticLogs.ExternalServiceCallLog.Error("Single saving input is null!");
                return BadRequest();
            }

            if (input.Property == null || input.Supply == null)
                return BadRequest();

            var result = SaveFile(Mapper.Map<Property>(input.Property), Mapper.Map<Supply>(input.Supply));

            if (!result)
            {
                ApplicationStaticLogs.ExternalServiceCallLog.ErrorFormat("An error ocurred during property with ID {0} saving!", input.Property?.ID);
                return BadRequest();
            }

            ApplicationStaticLogs.ExternalServiceCallLog.DebugFormat("Property with ID {0} saved successfuly", input.Property?.ID);
            return Ok();
        }

        [HttpPost, Route("save/batch")]
        [SkipUserActivityLogging]
        public IHttpActionResult SaveBatchExternalFile(ExternalNewFilesInput input)
        {
            if (input.Files == null)
            {
                ApplicationStaticLogs.ExternalServiceCallLog.Error("Batch saving input is null!");
                return BadRequest();
            }

            var successCount = 0;
            input.Files.ForEach(f =>
            {
                if(f.Property == null || f.Supply == null)
                    return;

                var result = SaveFile(Mapper.Map<Property>(f.Property), Mapper.Map<Supply>(f.Supply));
                if (!result)
                {
                    ApplicationStaticLogs.ExternalServiceCallLog.ErrorFormat(
                        "An error ocurred during property with ID {0} saving!", f.Property?.ID);
                }
                else
                {
                    successCount++;
                }
            });

            ApplicationStaticLogs.ExternalServiceCallLog.DebugFormat("{0} property(ies) saved successfuly and {1} property(ies) failed to save.", successCount, input.Files.Count - successCount);
            return Ok();
        }

        #endregion

        #region Private helper methods 

        private bool SaveFile(Property property, Supply supply)
        {
            supply.LastIndexingTime = DateTime.Now;
            ApplicationStaticLogs.ExternalServiceCallLog.Debug("********** Start of saving file from Naroon **********");
            var saveSupplyResult = AddSupply(supply);
            ApplicationStaticLogs.ExternalServiceCallLog.DebugFormat("Supply with ID {0} saved", supply.ID);

            var supplyReference = Mapper.Map<SupplyReference>(supply);
            property.Supplies = new List<SupplyReference> { supplyReference };
            var savePropertyResult = AddProperty(property);
            ApplicationStaticLogs.ExternalServiceCallLog.DebugFormat("Property with ID {0} saved", property.ID);

            var propertyReference = Mapper.Map<PropertyReference>(property);
            var updateSupplyResult = UpdateSupply(supply.ID, propertyReference);
            ApplicationStaticLogs.ExternalServiceCallLog.DebugFormat("Supply with ID {0} updated", supply.ID);

            ApplicationStaticLogs.ExternalServiceCallLog.Debug("********** End of saving file from Naroon **********");
            return saveSupplyResult && savePropertyResult && updateSupplyResult;
        }

        private bool AddSupply(Supply supply)
        {
            try
            {
                supply.IsPublic = true;
                supply.OwnerCanBeContacted = true;
                LocalContactMethodUtil.PrepareContactMethods(supply.OwnerContact, true, false, true);

                var supplyFilter = Builders<Supply>.Filter.Eq("ExternalID", supply.ExternalID);
                var existingSupply = DbManager.Supply.Find(supplyFilter).SingleOrDefaultAsync();
                if (existingSupply?.Result != null) //Existing item
                {
                    var updateSupply = Builders<Supply>.Update
                        .Set("LastIndexingTime", supply.LastIndexingTime)
                        .Set("LastModificationTime", supply.LastModificationTime)
                        .Set("LastFetchTime", DateTime.Now)
                        .Set("IntentionOfOwner", supply.IntentionOfOwner)
                        .Set("State", supply.State)
                        .Set("PriceSpecificationType", supply.PriceSpecificationType)
                        .Set("TotalPrice", supply.TotalPrice)
                        .Set("PricePerEstateArea", supply.PricePerEstateArea)
                        .Set("PricePerUnitArea", supply.PricePerUnitArea)
                        .Set("HasTransferableLoan", supply.HasTransferableLoan)
                        .Set("TransferableLoanAmount", supply.TransferableLoanAmount)
                        .Set("Mortgage", supply.Mortgage)
                        .Set("Rent", supply.Rent)
                        .Set("MortgageAndRentConvertible", supply.MortgageAndRentConvertible)
                        .Set("MinimumMortgage", supply.MinimumMortgage)
                        .Set("MinimumRent", supply.MinimumRent)
                        .Set("OwnerCanBeContacted", supply.OwnerCanBeContacted)
                        .Set("OwnerContact", supply.OwnerContact)
                        .Set("AgencyContact", supply.AgencyContact)
                        .Set("IsPublic", supply.IsPublic)
                        .Set("CreationTime", supply.CreationTime);

                    var updatedSupply = DbManager.Supply.FindOneAndUpdateAsync(supplyFilter, updateSupply);
                    if (updatedSupply?.Result == null)
                    {
                        ApplicationStaticLogs.ExternalServiceCallLog.ErrorFormat(
                            "Unexpected error while updating supply with External ID {0}", supply.ExternalID);
                        return false;
                    }

                    supply.ID = updatedSupply.Result.ID;
                    return true;
                }

                //New One
                DbManager.Supply.InsertOneAsync(supply);
                return true;
            }
            catch (Exception e)
            {
                ApplicationStaticLogs.ExternalServiceCallLog.ErrorFormat(
                            "Unexpected error occured while adding supply with External ID {0}, exception: {1}", supply.ExternalID, e);
                return false;
            }
        }

        private bool AddProperty(Property property)
        {
            try
            {
                RefindVicinity(property);
                property.LastIndexingTime = null;

                var propertyFilter = Builders<Property>.Filter.Eq("ExternalID", property.ExternalID);
                var existingProperty = DbManager.Property.Find(propertyFilter).SingleOrDefaultAsync();
                if (existingProperty?.Result != null) //Existing item
                {
                    var updateProperty = Builders<Property>.Update
                        .Set("LastIndexingTime", property.LastIndexingTime)
                        .Set("LastModificationTime", property.LastModificationTime)
                        .Set("LastFetchTime", DateTime.Now)
                        .Set("Address", property.Address)
                        .Set("GeographicLocation", property.GeographicLocation)
                        .Set("State", property.State)
                        .Set("IsHidden", property.IsHidden)
                        .Set("ConversionWarning", property.ConversionWarning)
                        .Set("ExternalDetails", property.ExternalDetails)
                        .Set("IsAgencyListing", property.IsAgencyListing)
                        .Set("IsAgencyActivityAllowed", property.IsAgencyActivityAllowed)
                        .Set("PropertyType", property.PropertyType)
                        .Set("PropertyStatus", property.PropertyStatus)
                        .Set("Vicinity", property.Vicinity)
                        .Set("EstateArea", property.EstateArea)
                        .Set("EstateDirection", property.EstateDirection)
                        .Set("PassageEdgeLength", property.PassageEdgeLength)
                        .Set("EstateVoucherType", property.EstateVoucherType)
                        .Set("TotalNumberOfUnits", property.TotalNumberOfUnits)
                        .Set("BuildingAgeYears", property.BuildingAgeYears)
                        .Set("NumberOfUnitsPerFloor", property.NumberOfUnitsPerFloor)
                        .Set("TotalNumberOfFloors", property.TotalNumberOfFloors)
                        .Set("UnitArea", property.UnitArea)
                        .Set("StorageRoomArea", property.StorageRoomArea)
                        .Set("UsageType", property.UsageType)
                        .Set("NumberOfRooms", property.NumberOfRooms)
                        .Set("NumberOfParkings", property.NumberOfParkings)
                        .Set("UnitFloorNumber", property.UnitFloorNumber)
                        .Set("NumberOfMasterBedrooms", property.NumberOfMasterBedrooms)
                        .Set("KitchenCabinetType", property.KitchenCabinetType)
                        .Set("MainDaylightDirection", property.MainDaylightDirection)
                        .Set("LivingRoomFloor", property.LivingRoomFloor)
                        .Set("FaceType", property.FaceType)
                        .Set("IsDuplex", property.IsDuplex)
                        .Set("HasIranianLavatory", property.HasIranianLavatory)
                        .Set("HasForeignLavatory", property.HasForeignLavatory)
                        .Set("HasPrivatePatio", property.HasPrivatePatio)
                        .Set("HasBeenReconstructed", property.HasBeenReconstructed)
                        .Set("HasElevator", property.HasElevator)
                        .Set("HasGatheringHall", property.HasGatheringHall)
                        .Set("HasAutomaticParkingDoor", property.HasAutomaticParkingDoor)
                        .Set("HasVideoEyePhone", property.HasVideoEyePhone)
                        .Set("HasSwimmingPool", property.HasSwimmingPool)
                        .Set("HasSauna", property.HasSauna)
                        .Set("HasJacuzzi", property.HasJacuzzi)
                        .Set("Supplies", property.Supplies)
                        .Set("CorrelationID", property.CorrelationID)
                        .Set("IsMasterBuiling", property.IsMasterBuiling)
                        .Set("SourceType", property.SourceType)
                        .Set("CreationTime", property.CreationTime);

                    var updatedProperty = DbManager.Property.FindOneAndUpdateAsync(propertyFilter, updateProperty);
                    if (updatedProperty?.Result == null)
                    {
                        ApplicationStaticLogs.ExternalServiceCallLog.ErrorFormat(
                            "Unexpected error while updating property with External ID {0}", property.ExternalID);
                        return false;
                    }

                    property.ID = updatedProperty.Result.ID;
                    return true;
                }

                //New One
                DbManager.Property.InsertOneAsync(property);
                return true;
            }
            catch (Exception e)
            {
                ApplicationStaticLogs.ExternalServiceCallLog.ErrorFormat(
                            "Unexpected error occured while adding property with External ID {0}, exception: {1}", property.ExternalID, e);
                return false;
            }
        }

        private void RefindVicinity(Property property)
        {
            if (property?.Vicinity?.Region == null)
                return;

            List<Vicinity> response;
            var maxVicinitySize = 100;
            var tehranid = ConfigurationManager.AppSettings["tehranid"];
            if (string.IsNullOrEmpty(tehranid))
            {
                return;
            }

            var parentId = ObjectId.Parse(tehranid);//Tehran
            switch (property.Vicinity.Region)
            {
                case 0:
                    break;
                case 1:
                    response = VicinityCache.Search("منطقه یک", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه یک")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 2:
                    response = VicinityCache.Search("تهران منطقه دو", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه دو")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 3:
                    response = VicinityCache.Search("تهران منطقه سه", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه سه")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 4:
                    response = VicinityCache.Search("تهران منطقه چهار", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه چهار")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 5:
                    response = VicinityCache.Search("تهران منطقه پنج", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه پنج")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 6:
                    response = VicinityCache.Search("تهران منطقه شش", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه شش")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 7:
                    response = VicinityCache.Search("تهران منطقه هفت", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه هفت")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 8:
                    response = VicinityCache.Search("تهران منطقه هشت", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه هشت")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 9:
                    response = VicinityCache.Search("تهران منطقه نه", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه نه")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 10:
                    response = VicinityCache.Search("تهران منطقه ده", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه ده")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 11:
                    response = VicinityCache.Search("تهران منطقه یازده", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه یازده")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 12:
                    response = VicinityCache.Search("تهران منطقه دوازده", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه دوازده")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 13:
                    response = VicinityCache.Search("تهران منطقه سیزده", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه سیزده")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 14:
                    response = VicinityCache.Search("تهران منطقه چهارده", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه چهارده")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 15:
                    response = VicinityCache.Search("تهران منطقه پانزده", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه پانزده")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 16:
                    response = VicinityCache.Search("تهران منطقه شانزده", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه شانزده")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 17:
                    response = VicinityCache.Search("تهران منطقه هفده", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه هفده")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 18:
                    response = VicinityCache.Search("تهران منطقه هجده", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه هجده")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 19:
                    response = VicinityCache.Search("تهران منطقه نوزده", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه نوزده")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 20:
                    response = VicinityCache.Search("تهران منطقه بیست", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه بیست")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 21:
                    response = VicinityCache.Search("تهران منطقه بیست و یک", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه بیست و یک")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
                case 22:
                    response = VicinityCache.Search("تهران منطقه بیست و دو", false, 0, maxVicinitySize, parentId);
                    property.Vicinity = response.Where(v => v.Name.Equals("منطقه بیست و دو")).Select(Mapper.Map<VicinityReference>).FirstOrDefault();
                    break;
            }

            if (property.Vicinity?.CenterPoint != null)
            {
                property.GeographicLocation = property.Vicinity.CenterPoint;
                property.GeographicLocationType = GeographicLocationSpecificationType.InferredFromVicinity;
            }
        }

        private bool UpdateSupply(ObjectId supplyID, PropertyReference property)
        {
            try
            {
                var supplyFilter = Builders<Supply>.Filter.Eq("ID", supplyID);
                var existingSupply = DbManager.Supply.Find(supplyFilter).SingleOrDefaultAsync();
                if (existingSupply?.Result != null) //Existing item
                {
                    var updateSupply = Builders<Supply>.Update
                        .Set("LastIndexingTime", BsonNull.Value) //For indexing
                        .Set("Property", property);
                    var result = DbManager.Supply.UpdateOneAsync(supplyFilter, updateSupply).Result;

                    if (result.MatchedCount != 1)
                        ApplicationStaticLogs.ExternalServiceCallLog.ErrorFormat(
                            "Could not find supply with ID {0} while updating process", supplyID);

                    if (result.MatchedCount == 1 && result.ModifiedCount != 1)
                        ApplicationStaticLogs.ExternalServiceCallLog.ErrorFormat(
                            "Could not update supply with ID {0} while updating process", supplyID);
                    return true;
                }

                ApplicationStaticLogs.ExternalServiceCallLog.ErrorFormat(
                    "Could not find supply with ID {0} while updating process", supplyID);
                return false;
            }
            catch (Exception e)
            {
                ApplicationStaticLogs.ExternalServiceCallLog.ErrorFormat(
                            "Unexpected error occured while updating supply with External ID {0}, exception: {1}", supplyID, e);
                return false;
            }
        }

        #endregion
    }
}