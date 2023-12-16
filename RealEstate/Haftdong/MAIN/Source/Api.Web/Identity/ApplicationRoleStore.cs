//using AppsOn.Automation.Core.Impl.Data;
//using AppsOn.Automation.Domain.Identity;
//using Compositional.Composer;
//using Compositional.Composer.Web.Cache;
//using Microsoft.AspNet.Identity.EntityFramework;
//
//namespace AppsOn.Automation.Core.Impl.Identity
//{
//    [Contract]
//    [Component]
//    [ComponentCache(typeof(OwinContextComponentCache))]
//    public class ApplicationRoleStore : RoleStore<ApplicationRole, long, ApplicationUserRole>
//    {
//        [CompositionConstructor]
//        public ApplicationRoleStore(Db db) : base(db)
//        {
//        }
//    }
//}