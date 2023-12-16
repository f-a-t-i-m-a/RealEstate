using System;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Contract;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.Resources;

namespace JahanJooy.RealEstateAgency.Util.Indexing
{
    [Component]
    public class ContractSearchIndexMapper : ISearchIndexMapper<Contract, SearchIE>
    {
        public SearchIE SearchMap(Contract entity)
        {
            var indexText = StaticEnumResources.ResourceManager.GetString(nameof(UsageType) + "_" + entity.PropertyReference?.UsageType)
                + " " + StaticEnumResources.ResourceManager.GetString(nameof(PropertyType) + "_" + entity.PropertyReference?.PropertyType)
                + " " + StaticEnumResources.ResourceManager.GetString(nameof(IntentionOfOwner) + "_" + entity.SupplyReference?.IntentionOfOwner)
                + " " + StaticEnumResources.ResourceManager.GetString(nameof(SupplyState) + "_" + entity.SupplyReference?.State)
                + " " + StaticEnumResources.ResourceManager.GetString(nameof(PropertyState) + "_" + entity.PropertyReference?.State)
                + " " + StaticEnumResources.ResourceManager.GetString(nameof(ContractState) + "_" + entity.State)
                + " " + StaticEnumResources.ResourceManager.GetString(nameof(RequestState) + "_" + entity.RequestReference?.State)
                + " " + StaticEnumResources.ResourceManager.GetString(nameof(IntentionOfCustomer) + "_" + entity.RequestReference?.IntentionOfCustomer)
                + " " + entity.PropertyReference?.Address
                + " " + entity.SellerReference?.DisplayName
                + " " + entity.SellerReference?.PhoneNumber
                + " " + entity.SellerReference?.Address
                + " " + entity.SellerReference?.Email
                + " " + entity.BuyerReference?.DisplayName
                + " " + entity.BuyerReference?.PhoneNumber
                + " " + entity.BuyerReference?.Address
                + " " + entity.BuyerReference?.Email;

            var title = "قرارداد " + StaticEnumResources.ResourceManager.GetString(nameof(IntentionOfOwner) + "_" + entity.SupplyReference?.IntentionOfOwner)
                + " " + StaticEnumResources.ResourceManager.GetString(nameof(PropertyType) + "_" + entity.PropertyReference?.PropertyType)
                + " با کاربری " + StaticEnumResources.ResourceManager.GetString(nameof(UsageType) + "_" + entity.PropertyReference?.UsageType);

            var displayText = "فروشنده: " + entity.SellerReference?.DisplayName
                + Environment.NewLine
                + "خریدار: " + entity.BuyerReference?.DisplayName;

            return new SearchIE
            {
                ID = entity.ID.ToString(),
                DataType = EntityType.Contract.ToString(),
                IndexedText = indexText,
                Title = title,
                DisplayText = displayText,
                CreatedByID = entity.CreatedByID.ToString(),
                CreationTime = entity.CreationTime.Ticks,
                ModificationTime = entity.LastModificationTime?.Ticks
            };
        }
    }
}