using System;
using System.Globalization;
using Compositional.Composer;
using FluentScheduler;
using JahanJooy.Common.Util.Components;
using JahanJooy.RealEstateAgency.Naroon.Util;
using log4net;

namespace JahanJooy.RealEstateAgency.Naroon.Component
{
    [Contract]
    [Component]
    public class ScheduledTaskRegistry : Registry, IEagerComponent
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ScheduledTaskRegistry));

        public ScheduledTaskRegistry()
        {
            DefaultAllTasksAsNonReentrant();
        }

        [ComponentPlug]
        public NaroonUtil NaroonUtil { get; set; }

        [OnCompositionComplete]
        public void OnCompositionComplete()
        {
            Schedule(() =>
            {
                try
                {
                    var yesterday = DateTime.Now.AddDays(-1);
                    var calendar = new GregorianCalendar();
                    var fromDate = calendar.GetYear(yesterday) +
                                  (calendar.GetMonth(yesterday).ToString().Length <= 1
                                      ? "-0" + calendar.GetMonth(yesterday)
                                      : "-" + calendar.GetMonth(yesterday)) +
                                  (calendar.GetDayOfMonth(yesterday).ToString().Length <= 1
                                      ? "-0" + calendar.GetDayOfMonth(yesterday)
                                      : "-" + calendar.GetDayOfMonth(yesterday));
                    var toDate = calendar.GetYear(yesterday) +
                                 (calendar.GetMonth(yesterday).ToString().Length <= 1
                                     ? "-0" + calendar.GetMonth(yesterday)
                                     : "-" + calendar.GetMonth(yesterday)) +
                                 (calendar.GetDayOfMonth(yesterday).ToString().Length <= 1
                                     ? "-0" + calendar.GetDayOfMonth(yesterday)
                                     : "-" + calendar.GetDayOfMonth(yesterday));

                    NaroonUtil.RetrieveFromNaroon(fromDate, toDate);
                }
                catch (Exception e)
                {
                    Log.Error("Unhandled exception in Scheduled Task Registry", e);
                }
            }).ToRunOnceIn(1).Days().At(5, 00);

            Schedule(() =>
            {
                try
                {
                    var today = DateTime.Now;
                    var calendar = new GregorianCalendar();
                    var fromDate = calendar.GetYear(today) +
                                   (calendar.GetMonth(today).ToString().Length <= 1
                                       ? "-0" + calendar.GetMonth(today)
                                       : "-" + calendar.GetMonth(today)) +
                                   (calendar.GetDayOfMonth(today).ToString().Length <= 1
                                       ? "-0" + calendar.GetDayOfMonth(today)
                                       : "-" + calendar.GetDayOfMonth(today)) ;
                    var toDate = calendar.GetYear(today) +
                                 (calendar.GetMonth(today).ToString().Length <= 1
                                     ? "-0" + calendar.GetMonth(today)
                                     : "-" + calendar.GetMonth(today)) +
                                 (calendar.GetDayOfMonth(today).ToString().Length <= 1
                                     ? "-0" + calendar.GetDayOfMonth(today)
                                     : "-" + calendar.GetDayOfMonth(today));

                    NaroonUtil.RetrieveFromNaroon(fromDate, toDate);
                }
                catch (Exception e)
                {
                    Log.Error("Unhandled exception in Scheduled Task Registry", e);
                }
            }).ToRunEvery(2).Minutes();

            TaskManager.Initialize(this);
        }
    }
}