using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Supplies
{
    [TsClass]
    [AutoMapperConfig]
    public class NewSupplyInput
    {
        public IntentionOfOwner IntentionOfOwner { get; set; }

        #region Sales price

        public SalePriceSpecificationType? PriceSpecificationType { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? PricePerEstateArea { get; set; }
        public decimal? PricePerUnitArea { get; set; }
        public bool HasTransferableLoan { get; set; }
        public decimal? TransferableLoanAmount { get; set; }
        public string AdditionalRentalComments { get; set; }

        #endregion

        #region Rent price

        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }
        public bool MortgageAndRentConvertible { get; set; }
        public decimal? MinimumMortgage { get; set; }
        public decimal? MinimumRent { get; set; }

        #endregion
        public ObjectId PropertyId { get; set; }

        #region Request

        public string SwapAdditionalComments { get; set; }
        public IntentionOfCustomer IntentionOfCustomer { get; set; }
        public long[] PropertyTypes { get; set; }
        public UsageType? RequestUsageType { get; set; }
        public DateTime? ExpirationTime { get; set; }

        #region Location properties
        public ObjectId[] Vicinities { get; set; }
        public string Description { get; set; }

        #endregion

        #region Estate properties

        public decimal? RequestEstateArea { get; set; }

        #endregion

        #region Unit properties

        public decimal? RequestUnitArea { get; set; }
        public decimal? RequestOfficeArea { get; set; }

        #endregion

        #region Sales price

        public decimal? RequestTotalPrice { get; set; }

        #endregion


        #region Rent price

        public decimal? RequestMortgage { get; set; }
        public decimal? RequestRent { get; set; }
        public bool RequestMortgageAndRentConvertible { get; set; }

        #endregion

        #endregion

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<NewSupplyInput, Supply>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<NewSupplyInput, Request>()
                .ForMember(p => p.UsageType, opt => opt.MapFrom(pp => pp.RequestUsageType))
                .ForMember(p => p.EstateArea, opt => opt.MapFrom(pp => pp.RequestEstateArea))
                .ForMember(p => p.UnitArea, opt => opt.MapFrom(pp => pp.RequestUnitArea))
                .ForMember(p => p.OfficeArea, opt => opt.MapFrom(pp => pp.RequestOfficeArea))
                .ForMember(p => p.TotalPrice, opt => opt.MapFrom(pp => pp.RequestTotalPrice))
                .ForMember(p => p.Mortgage, opt => opt.MapFrom(pp => pp.RequestMortgage))
                .ForMember(p => p.Rent, opt => opt.MapFrom(pp => pp.RequestRent))
                .ForMember(p => p.MortgageAndRentConvertible, opt => opt.MapFrom(pp => pp.RequestMortgageAndRentConvertible))
                .IgnoreUnmappedProperties();
        }
    }
}
