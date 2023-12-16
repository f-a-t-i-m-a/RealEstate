using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain.Messages;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminNotificationMessage;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
    public class AdminNotificationMessageController : AdminControllerBase
    {
        #region Component plugs

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        #endregion

        [HttpGet]
        public ActionResult List(AdminNotificationMessageListModel model)
        {
            if (model == null)
                model = new AdminNotificationMessageListModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            IQueryable<NotificationMessage> query = DbManager.Db.NotificationMessagesDbSet;
            query = ApplyFilterQuery(model, query);

            model.NotificationMessages = PagedList<NotificationMessage>.BuildUsingPageNumber(query.Count(), 20, pageNum);
            model.NotificationMessages.FillFrom(query.OrderByDescending(c => c.CreationTime));

            return View(model);
        }

        #region Private helper methods

        private static IQueryable<NotificationMessage> ApplyFilterQuery(AdminNotificationMessageListModel model,
            IQueryable<NotificationMessage> notifications)
        {
            if (model.TargetUserIDFilter.HasValue)
                notifications = notifications.Where(n => n.TargetUserID == model.TargetUserIDFilter.Value);

            if (model.ReasonFilter.HasValue)
                notifications = notifications.Where(n => n.Reason == model.ReasonFilter.Value);

            if (model.SeverityFilter.HasValue)
                notifications = notifications.Where(n => n.Severity == model.SeverityFilter.Value);

            if (model.SourceEntityTypeFilter.HasValue)
                notifications = notifications.Where(n => n.SourceEntityType == model.SourceEntityTypeFilter.Value);

            if (model.SourceEntityIDFilter.HasValue)
                notifications = notifications.Where(n => n.SourceEntityID == model.SourceEntityIDFilter.Value);

            if (model.SeenTimeFilter.HasValue)
                notifications = model.SeenTimeFilter.Value
                    ? notifications.Where(n => n.SeenTime.HasValue)
                    : notifications.Where(n => !n.SeenTime.HasValue);

            if (model.AddressedTimeFilter.HasValue)
                notifications = model.AddressedTimeFilter.Value
                    ? notifications.Where(n => n.AddressedTime.HasValue)
                    : notifications.Where(n => !n.AddressedTime.HasValue);

            return notifications;
        }

        #endregion
    }
}