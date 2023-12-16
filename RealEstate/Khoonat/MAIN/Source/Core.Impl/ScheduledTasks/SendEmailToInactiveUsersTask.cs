using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.Common.Util.Text;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks
{
    [Component]
    public class SendEmailToInactiveUsersTask : ScheduledTaskBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (SendEmailToInactiveUsersTask));

        #region Injected Dependencies

        [ComponentPlug]
        public IEmailNotificationService EmailNotificationService { get; set; }

        #endregion

        public override string Key
        {
            get { return ScheduledTaskKeys.SendEmailToInactiveUsersTask; }
        }

        public override int MaxIterationsPerSchedule
        {
            get { return 20; }
        }

        public override ScheduledTaskIterationResult IterateInternal(string currentProgress)
        {
            var twoWeeksAgoStart = ParseProgress(currentProgress);
            var twoWeeksAgoEnd = DateTime.Now.AddDays(-14);

            var fourWeeksAgoStart = twoWeeksAgoStart.AddDays(-14);
            var fourWeeksAgoEnd = twoWeeksAgoEnd.AddDays(-14);

            var eightWeeksAgoStart = fourWeeksAgoStart.AddDays(-28);
            var eightWeeksAgoEnd = fourWeeksAgoEnd.AddDays(-28);

            var twoWeeksAgoUsers = FindInactiveUsersBetween(twoWeeksAgoStart, twoWeeksAgoEnd);
            var fourWeeksAgoUsers = FindInactiveUsersBetween(fourWeeksAgoStart, fourWeeksAgoEnd);
            var eightWeeksAgoUsers = FindInactiveUsersBetween(eightWeeksAgoStart, eightWeeksAgoEnd);

            foreach (var inactiveUser in twoWeeksAgoUsers)
            {
                SendEmailToInactiveUsers(inactiveUser);
            }

            foreach (var inactiveUser in fourWeeksAgoUsers)
            {
                // Skip users who already received emails in this task execution
                if (twoWeeksAgoUsers.Any(u => u.ID == inactiveUser.ID))
                    continue;

                SendEmailToInactiveUsers(inactiveUser);
            }

            foreach (var inactiveUser in eightWeeksAgoUsers)
            {
                // Skip users who already received emails in this task execution
                if (twoWeeksAgoUsers.Any(u => u.ID == inactiveUser.ID))
                    continue;
                if (fourWeeksAgoUsers.Any(u => u.ID == inactiveUser.ID))
                    continue;

                SendEmailToInactiveUsers(inactiveUser);
            }
            return
                ScheduledTaskIterationResult.Success((twoWeeksAgoEnd.Ticks + 1L).ToString(CultureInfo.InvariantCulture));
        }

        private void SendEmailToInactiveUsers(User inactiveUser)
        {
            try
            {
                foreach (var contactMethod in
                    inactiveUser.ContactMethods.Where(cm =>
                        cm.ContactMethodType == ContactMethodType.Email &&
                        EmailUtils.IsValidEmail(cm.ContactMethodText) &&
                        cm.IsVerified &&
                        !cm.IsDeleted))
                {
                    EmailNotificationService.NotifyInactiveUser(contactMethod);
                }
            }
            catch (Exception e)
            {
                // Catch exceptions in email transmission to prevent one failure aborting a whole batch
                Log.Error("Caught an exception while sending  email for inactive User " + inactiveUser.ID, e);
            }
        }


        private DateTime ParseProgress(string currentProgress)
        {
            // If the current progress is null, this is the first time the schedule is running.

            if (string.IsNullOrWhiteSpace(currentProgress))
                return DateTime.Now.AddYears(-1);

            long currentProgressTicks;
            if (!long.TryParse(currentProgress, out currentProgressTicks))
                throw new ArgumentException("Invalid progress string. A valid Ticks number is expected.");

            return new DateTime(currentProgressTicks);
        }

        private List<User> FindInactiveUsersBetween(DateTime lastLoginFrom, DateTime lastLoginTo)
        {
            var s = DbManager.Db.HttpSessions
                    
                .Where(hs => (hs.UserID.HasValue))
                .GroupBy(hs => hs.UserID)
                .Select(hs => hs.OrderByDescending(h => h.Start).FirstOrDefault())
                .Where(hs=>hs.Start <= lastLoginTo && hs.Start > lastLoginFrom)
                .Include(hs => hs.User)
                .Include("User.ContactMethods")
                .ToList();

            var result = s.Select(hs => hs.User).ToList();
            return result;
        }
    }
}