using System;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.Resources;

namespace JahanJooy.RealEstateAgency.Util.Indexing
{
    [Component]
    public class UserSearchIndexMapper : ISearchIndexMapper<ApplicationUser, SearchIE>
    {
        public SearchIE SearchMap(ApplicationUser entity)
        {
            var indexText = entity.UserName
                              + " " + entity.DisplayName
                              + " " + entity.Code
                              + " " + entity.About
                              + " " + entity.WebSiteUrl
                              + " " + entity.Type;

            entity.Contact?.Phones?.ForEach(p =>
            {
                indexText += p.Value + " ";
            });

            entity.Contact?.Emails?.ForEach(e =>
            {
                indexText += e.Value + " ";
            });

            entity.Contact?.Addresses?.ForEach(a =>
            {
                indexText += a.Value + " ";
            });

            var title = entity.UserName;
            var displayText = "نام کامل: " + entity.DisplayName
                              + Environment.NewLine
                              + "نوع مجوز: " +
                              StaticEnumResources.ResourceManager.GetString(nameof(LicenseType) + "_" +
                                                                            entity.LicenseType);

            return new SearchIE
            {
                ID = entity.Id,
                DataType = EntityType.ApplicationUser.ToString(),
                IndexedText = indexText,
                Title = title,
                DisplayText = displayText,
                CreatedByID = entity.CreatedByID?.ToString(),
                CreationTime = entity.CreationTime.Ticks,
                ModificationTime = entity.ModificationTime?.Ticks
            };
        }
    }
}