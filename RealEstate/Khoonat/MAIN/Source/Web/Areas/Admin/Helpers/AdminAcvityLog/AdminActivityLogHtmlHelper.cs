using System.Web.Mvc;
using JahanJooy.RealEstate.Core.Services.Dto.Audit;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Web.Areas.Admin.Resources.AdminActivityLog;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Helpers.AdminAcvityLog
{
	public static class AdminActivityLogHtmlHelper
	{
		public static string ActivityLogBackgroundColor(this HtmlHelper html, ActivityLogDisplayInfo log)
		{
			if (!log.ApprovalDate.HasValue)
				return "#aaffaa";

			if (!log.ReviewDate.HasValue)
			{
				double colorIndex = log.ReviewWeight;
				if (colorIndex > 2000)
					colorIndex = 2000;
				if (colorIndex < 0)
					colorIndex = 0;

				colorIndex = 255 - (colorIndex * 192) / 2000;

				var colorString = ((int)colorIndex).ToString("X2");
				return "#FF" + colorString + colorString;
			}

			return "white";
		}

		public static string ActivityLogAction(this HtmlHelper html, ActivityLogDisplayInfo log)
		{
			string result;

			var actionString = log.Action.ToString();
			if (actionString.Equals(log.ActionDetails))
				result = ActivityLogActionResources.ResourceManager.GetString("Action_" + actionString) ?? actionString;
			else
				result = ActivityLogActionResources.ResourceManager.GetString("ActionDetail_" + log.TargetEntity.ToString() + "_" + log.ActionDetails) ?? actionString + ": " + log.ActionDetails;

			return result;
		}

		public static string ActivityLogTargetShortcutHref(this UrlHelper url, TargetEntityType? target, long? targetId)
		{
			string result = null;

			if (target.HasValue && targetId.HasValue)
			{
				switch (target)
				{
					case TargetEntityType.User:
						result = url.Action("Details", "AdminUsers", new {id = targetId});
						break;

					case TargetEntityType.PropertyListing:
						result = url.Action("Details", "AdminProperties", new {id = targetId});
						break;

					case TargetEntityType.PropertyListingPhoto:
						result = url.Action("Details", "AdminPropertyPhotos", new {id = targetId});
						break;

					case TargetEntityType.AbuseFlag:
						result = url.Action("Details", "AdminAbuseFlags", new {id = targetId});
						break;

					case TargetEntityType.SavedPropertySearch:
						result = url.Action("Details", "AdminSavedPropertySearches", new {id = targetId});
						break;
				}
			}

			return result;
		}

		public static string ActivityLogAuditShortcutHref(this UrlHelper url, AuditEntityType? auditEntity, long? auditEntityId)
		{
			string result = null;

			if (auditEntity.HasValue && auditEntityId.HasValue)
			{
				switch (auditEntity.Value)
				{
					case AuditEntityType.PropertyListingUpdateHistory:
						result = url.Action("Details", "AdminPropertyUpdates", new { id = auditEntityId });
						break;

				}
			}

			return result;
		}
	}
}