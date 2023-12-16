using System.Collections.Generic;
using System.Linq;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstateAgency.Domain.Audit;

namespace JahanJooy.RealEstateAgency.Util.UserActivityLog
{
    public class UserActivityOwinEntry
    {
        public UserActivity MainActivity { get; set; }
        public List<UserActivity> AdditionalActivities { get; set; }

        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public void Cancel()
        {
            MainActivity = null;
            AdditionalActivities = null;
        }

        public void EnsureMainActivity()
        {
            if (MainActivity == null)
                MainActivity = new UserActivity();
        }

        public void ClearMainActivity()
        {
            MainActivity = null;
        }

        public void ReportAdditionalActivity(UserActivity activity)
        {
            if (AdditionalActivities == null)
                AdditionalActivities = new List<UserActivity>();

            AdditionalActivities.Add(activity);
        }

        public UserActivity[] GetAllActivities()
        {
            return (MainActivity == null ? Enumerable.Empty<UserActivity>() : MainActivity.Yield())
                .Union(AdditionalActivities.EmptyIfNull())
                .ToArray();
        }
    }
}