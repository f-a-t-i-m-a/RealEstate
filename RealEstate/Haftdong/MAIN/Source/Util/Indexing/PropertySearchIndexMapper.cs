using System;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.Resources;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Util.Indexing
{
    [Component]
    public class PropertySearchIndexMapper : ISearchIndexMapper<Property, SearchIE>
    {
        public SearchIE SearchMap(Property entity)
        {
            var indexText = StaticEnumResources.ResourceManager.GetString(nameof(UsageType) + "_" + entity.UsageType) 
                              + " " + StaticEnumResources.ResourceManager.GetString(nameof(PropertyType) + "_" + entity.PropertyType) 
                              + " " + StaticEnumResources.ResourceManager.GetString(nameof(PropertyState) + "_" + entity.State) 
                              + " " + entity.Address
                              + " " + entity.Vicinity?.Name
                              + " " + entity.Owner?.DisplayName
                              + " " + entity.Owner?.PhoneNumber
                              + " " + entity.Owner?.Address
                              + " " + entity.Owner?.Email;

            var suppliesText = "";
            entity.Supplies?.ForEach(s =>
            {
                suppliesText += " " + StaticEnumResources.ResourceManager.GetString(nameof(IntentionOfOwner) + "_" + s.IntentionOfOwner);
            });

            indexText += suppliesText;

            var title = "اطلاعات " + StaticEnumResources.ResourceManager.GetString(nameof(PropertyType) + "_" + entity.PropertyType)
                + " با کاربری " + StaticEnumResources.ResourceManager.GetString(nameof(UsageType) + "_" + entity.UsageType);

            var displayText = "مالک: " + entity.Owner?.DisplayName
                + Environment.NewLine
                + (!suppliesText.IsNullOrEmpty() ? "نوع معامله: " + suppliesText.Replace(" ", "، ").Substring(2) + ""
                + Environment.NewLine : "")
                + "آدرس: " + entity.Address;

            return new SearchIE
            {
                ID = entity.ID.ToString(),
                DataType = EntityType.Property.ToString(),
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