using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Requests
{
    [TsClass]
    [AutoMapperConfig]
    public class NewRequestInput
    {
        
        public IntentionOfCustomer IntentionOfCustomer { get; set; }
        public long[] PropertyTypes { get; set; }
        public UsageType? UsageType { get; set; }
        public DateTime? ExpirationTime { get; set; }


        #region Location properties
        public ObjectId[] Vicinities { get; set; }
        public string Description { get; set; }

        #endregion


        #region Estate properties

        public decimal? EstateArea { get; set; }
        public EstateVoucherType? EstateVoucherType { get; set; }

        #endregion


        #region Building properties

        public int? BuildingAgeYears { get; set; }
        public int? TotalNumberOfUnits { get; set; }
        public int? NumberOfUnitsPerFloor { get; set; }
        public int? TotalNumberOfFloors { get; set; }

        #endregion


        #region Unit properties

        public decimal? UnitArea { get; set; }
        public decimal? OfficeArea { get; set; }
        public decimal? CeilingHeight { get; set; }
        public int? NumberOfRooms { get; set; }
        public int? NumberOfParkings { get; set; }
        public int? UnitFloorNumber { get; set; }

        #endregion


        #region Extra unit properties

        public bool? IsDublex { get; set; }
        public bool? HasBeenReconstructed { get; set; }
        public bool? HasIranianLavatory { get; set; }
        public bool? HasForeignLavatory { get; set; }
        public bool? HasPrivatePatio { get; set; }
        public bool? HasMasterBedroom { get; set; }

        #endregion


        #region Extra building properties

        public bool? HasElevator { get; set; }
        public bool? HasGatheringHall { get; set; }
        public bool? HasSwimmingPool { get; set; }
        public bool? HasStorageRoom { get; set; }
        public bool? HasAutomaticParkingDoor { get; set; }
        public bool? HasVideoEyePhone { get; set; }
        public bool? HasSauna { get; set; }
        public bool? HasJacuzzi { get; set; }

        #endregion

        #region Sales price

        public decimal? TotalPrice { get; set; }

        #endregion


        #region Rent price

        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }
        public bool MortgageAndRentConvertible { get; set; }

        #endregion


        #region Owner Contacts

        public NewCustomerInput Owner { get; set; }
        public bool? OwnerCanBeContacted { get; set; }
        public NewContactMethodCollectionInput ContactInfos { get; set; }
        #endregion


        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<NewRequestInput, Request>()
                .IgnoreUnmappedProperties();
        }
    }
}
