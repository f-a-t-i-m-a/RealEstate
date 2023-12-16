//using AppsOn.Automation.Core.Impl.Identity;
//using Compositional.Composer;
//using Compositional.Composer.Web.Cache;
//using Microsoft.AspNet.Identity;
//
//namespace JahanJooy.RealEstateAgency.Api.Web.Identity
//{
//    [Contract]
//    [Component]
//    [ComponentCache(typeof(OwinContextComponentCache))]
//    public class ApplicationRoleManager : RoleManager<ApplicationRole, long>
//    {
//        [CompositionConstructor]
//        public ApplicationRoleManager(ApplicationRoleStore store) : base(store)
//        {
//        }
//    }
//}