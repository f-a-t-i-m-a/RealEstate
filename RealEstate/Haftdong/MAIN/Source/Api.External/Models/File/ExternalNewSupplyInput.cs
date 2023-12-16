using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Api.External.Models.File
{
    [AutoMapperConfig]
    public class ExternalNewSupplyInput
    {
        public ObjectId ID { get; set; }
        public string ExternalID { get; set; }
        public IntentionOfOwner IntentionOfOwner { get; set; }
        public SupplyState State { get; set; }
        public SupplyCompletionReason? CompletionReason { get; set; }
        public bool IsArchived { get; set; }

        #region Sales price

        public SalePriceSpecificationType? PriceSpecificationType { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? PricePerEstateArea { get; set; }
        public decimal? PricePerUnitArea { get; set; }
        public bool HasTransferableLoan { get; set; }
        public decimal? TransferableLoanAmount { get; set; }

        #endregion

        #region Rent price

        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }
        public bool MortgageAndRentConvertible { get; set; }
        public string AdditionalRentalComments { get; set; }
        public decimal? MinimumMortgage { get; set; }
        public decimal? MinimumRent { get; set; }

        #endregion

        #region Owner Contacts

        public PropertyReference Property { get; set; }
        public NewContactMethodCollectionInput OwnerContact { get; set; }

        #endregion

        #region Indexing

        public DateTime? LastIndexingTime { get; set; }

        #endregion

        public DateTime LastFetchTime { get; set; }
        public DateTime CreationTime { get; set; }
        public ObjectId CreatedByID { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public ObjectId? LastModifiedTimeByID { get; set; }
        public DateTime? DeletionTime { get; set; }
        public ObjectId? DeletedByID { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<ExternalNewSupplyInput, Supply>()
                .IgnoreUnmappedProperties();
            Mapper.CreateMap<ExternalNewSupplyInput, SupplyReference>()
                .IgnoreUnmappedProperties();
        }
    }
}
