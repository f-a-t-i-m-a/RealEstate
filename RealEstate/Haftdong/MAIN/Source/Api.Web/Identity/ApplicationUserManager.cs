using Compositional.Composer;
using Compositional.Composer.Web.Cache;
using JahanJooy.Common.Util.Identity;
using JahanJooy.RealEstateAgency.Domain.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace JahanJooy.RealEstateAgency.Api.Web.Identity
{
    [Contract]
    [Component]
    [ComponentCache(typeof (OwinContextComponentCache))]
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        [CompositionConstructor]
        public ApplicationUserManager(ApplicationUserStore store,
            IDataProtectionProviderContract dataProtectionProvider)
            : base(store)
        {
            UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
//                RequireUniqueEmail = true
            };

            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            if (dataProtectionProvider != null)
            {
                UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(
                        dataProtectionProvider.Create("AppsOn Application Identity"));
            }
        }
    }
}