using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Web;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.DataAccess;

namespace JahanJooy.RealEstateAgency.Util.UserActivityLog
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    
    public class UserActivityLoggerMiddleware
    {
        private readonly AppFunc _next;
        protected IComposer Composer { get; set; }
        public DbManager DbManager { get; set; }

        public UserActivityLoggerMiddleware(AppFunc next)
        {
            _next = next;
            DbManager = new DbManager();
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var context = OwinRequestScopeContext.Current;
            var entry = context.EnsureUserActivityEntry();
            List<UserActivity> activityList = new List<UserActivity>();

            if (!context.IsGet())
                entry.EnsureMainActivity();

            await _next(environment);

            var activities = entry.GetAllActivities();
            if (activities.Any())
            {
                    activities.SafeForEach(a =>
                    {
                        SetActivityProperties(a, entry, context);

                        // ReSharper disable once AccessToDisposedClosure - Runs immediately
                        activityList.Add(a);
                    });
                await DbManager.UserActivity.InsertManyAsync(activityList);
            }
        }

        private void SetActivityProperties(UserActivity activity, UserActivityOwinEntry entry, OwinRequestScopeContext context)
        {
            activity.CorrelationID = context.GetCorrelationId();
            activity.ActivityTime = DateTime.Now;
            if (OwinRequestScopeContext.Current.GetUser() != null && OwinRequestScopeContext.Current.GetUserId() != null)
            {
                activity.User = new ApplicationUserReference
                {
                    Id = OwinRequestScopeContext.Current.GetUserId(),
                    UserName = OwinRequestScopeContext.Current.GetUser().Identity.Name
                };
            }
            activity.ControllerName = (activity.ControllerName ?? entry.ControllerName) ?? "";
            activity.ActionName = (activity.ActionName ?? entry.ActionName) ?? "";
            activity.Succeeded = context.IsSuccess();
        }
    }
}
