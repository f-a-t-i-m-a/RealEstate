using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Api.App.Models.Customer;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Request
{
    [TsClass]
    [AutoMapperConfig]
    public class AppNewRequestInput
    {
        public IntentionOfCustomer IntentionOfCustomer { get; set; }
        public long[] PropertyTypes { get; set; }
        public UsageType? UsageType { get; set; }

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

        #endregion

        #region Unit properties

        public decimal? UnitArea { get; set; }
        public int? NumberOfRooms { get; set; }
        public int? NumberOfParkings { get; set; }
        public int? UnitFloorNumber { get; set; }

        #endregion

        #region Extra building properties

        public bool? HasElevator { get; set; }
        public bool? HasAutomaticParkingDoor { get; set; }
        public bool? HasVideoEyePhone { get; set; }

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

        public AppNewCustomerInput Owner { get; set; }
        #endregion

        public bool IsPublic { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppNewRequestInput, Domain.Request.Request>()
                .ForMember(r => r.PropertyTypes, opt => opt.MapFrom(rr => rr.PropertyTypes))
                .IgnoreUnmappedProperties();
        }
    }
}
