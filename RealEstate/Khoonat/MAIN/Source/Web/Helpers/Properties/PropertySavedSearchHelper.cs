using System;
using System.Linq.Expressions;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Web.Helpers.Properties
{
	public static class PropertySavedSearchHelper
	{
		private static readonly Expression<Func<SavedPropertySearch, bool>> NotificationErrorExpressionField;
		private static readonly Func<SavedPropertySearch, bool> NotificationErrorDelegateField;

		static PropertySavedSearchHelper()
		{
			NotificationErrorExpressionField = s => (s.SendNotificationEmails &&
			                                     (s.EmailNotificationTarget == null || s.EmailNotificationTarget.IsDeleted || !s.EmailNotificationTarget.IsVerified)) ||
			                                    ((s.SendPromotionalSmsMessages || s.SendPaidSmsMessages) &&
			                                     (s.SmsNotificationTarget == null || s.SmsNotificationTarget.IsDeleted || !s.SmsNotificationTarget.IsVerified));

			NotificationErrorDelegateField = NotificationErrorExpressionField.Compile();
		}

		public static Expression<Func<SavedPropertySearch, bool>> NotificationErrorExpression
		{
			get { return NotificationErrorExpressionField; }
		}

		public static Func<SavedPropertySearch, bool> NotificationErrorDelegate
		{
			get { return NotificationErrorDelegateField; }
		}
	}
}