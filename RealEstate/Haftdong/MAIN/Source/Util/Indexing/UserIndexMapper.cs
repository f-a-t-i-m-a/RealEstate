using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Users;

namespace JahanJooy.RealEstateAgency.Util.Indexing
{
    [Component]
    public class UserIndexMapper : IIndexMapper<ApplicationUser, ApplicationUserIE>
    {
        public ApplicationUserIE Map(ApplicationUser entity)
        {
            var description = "";
            entity.Contact?.Phones?.ForEach(p =>
            {
                description += p.Value + " ";
            });

            entity.Contact?.Emails?.ForEach(e =>
            {
                description += e.Value + " ";
            });

            entity.Contact?.Addresses?.ForEach(a =>
            {
                description += a.Value + " ";
            });

            return new ApplicationUserIE
            {
                ID = entity.Id,
                UserName = entity.UserName,
                DisplayName = entity.DisplayName,
                Type = entity.Type.ToString(),
                ContactValues = description,
                CreationTime = entity.CreationTime.Ticks,
                DeletionTime = entity.DeletionTime?.Ticks ?? 0,
                LicenseActivationTime = entity.LicenseActivationTime?.Ticks ?? 0,
                LicenseType = entity.LicenseType?.ToString(),
                CreatedByID = entity.CreatedByID?.ToString(),
            };
        }
    }
}