using System;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Util.Resources;
using JahanJooy.RealEstateAgency.Util.VicinityCache;

namespace JahanJooy.RealEstateAgency.Util.Indexing
{
    [Component]
    public class RequestSearchIndexMapper : ISearchIndexMapper<Request, SearchIE>
    {
        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        public SearchIE SearchMap(Request entity)
        {
            var indexText = StaticEnumResources.ResourceManager.GetString(nameof(IntentionOfCustomer) + "_" + entity.IntentionOfCustomer)
                              + " " + StaticEnumResources.ResourceManager.GetString(nameof(UsageType) + "_" + entity.UsageType)
                              + " " + entity.Owner?.DisplayName
                              + " " + entity.Owner?.PhoneNumber
                              + " " + entity.Owner?.Address
                              + " " + entity.Owner?.Email;

            entity.Vicinities?.ForEach(v =>
            {
                indexText += " " + VicinityCache[v]?.Name;
            });

            var title = "درخواست " + StaticEnumResources.ResourceManager.GetString(nameof(IntentionOfCustomer) + "_" + entity.IntentionOfCustomer)
                + " ملک با کاربری " + StaticEnumResources.ResourceManager.GetString(nameof(UsageType) + "_" + entity.UsageType);

            var displayText = "درخواست " + StaticEnumResources.ResourceManager.GetString(nameof(IntentionOfCustomer) + "_" + entity.IntentionOfCustomer)
                + " ملک با کاربری " + StaticEnumResources.ResourceManager.GetString(nameof(UsageType) + "_" + entity.UsageType)
                + Environment.NewLine;

            return new SearchIE
            {
                ID = entity.ID.ToString(),
                DataType = EntityType.Request.ToString(),
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