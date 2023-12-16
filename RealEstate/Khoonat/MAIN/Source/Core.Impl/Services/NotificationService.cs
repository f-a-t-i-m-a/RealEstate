using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Messages;
using log4net;
using ServiceStack;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
	[Component]
	public class NotificationService : INotificationService
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(NotificationService));

		private const int BatchSize = 20;

		#region Injected dependencies

		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public IEmailNotificationService EmailNotificationService { get; set; }

		[ComponentPlug]
		public ISmsNotificationService SmsNotificationService { get; set; }

		#endregion

		#region Implementation of INotificationService

		public bool SendEmailAndSms()
		{
			var now = DateTime.Now;
			var notificationMessages = DbManager.Db.NotificationMessagesDbSet
				.Where(n => n.NextMessageTransmissionDue <= now && !n.AddressedTime.HasValue)
				.Include(n => n.TargetUser.ContactMethods)
				.Take(BatchSize)
				.ToList();

			Log.DebugFormat("Starting to process and send SMS/Email for notifications {0}", notificationMessages.Select(nm => nm.ID.ToString(CultureInfo.InvariantCulture)).Join());

			foreach (var notification in notificationMessages)
			{
				Log.DebugFormat("Processing NotificationMessage.ID={0} with Reason={1}", notification.ID, notification.Reason);

				var currentNextTransmissionDue = notification.NextMessageTransmissionDue;

				switch (notification.Reason)
				{
					case NotificationReason.BalanceRunningLow:
						NotifyBalanceRuningLow(notification);
						break;

					case NotificationReason.BalanceDepleted:
						NotifyBalanceDepleted(notification);
						break;
				}

				VerifyNextMessageTransmissionDue(notification, currentNextTransmissionDue);
			}

			return notificationMessages.Count >= BatchSize;
		}

		#endregion

		#region Reason: BalanceRunningLow

		private void NotifyBalanceRuningLow(NotificationMessage notification)
		{
			SendLowBalanceEmail(notification, true);
			notification.NextMessageTransmissionDue = null;
		}

		private void SendLowBalanceEmail(NotificationMessage notification, bool isBalanceDepleted)
		{
			var verifiedEmail = notification.TargetUser.ContactMethods
				.FirstOrDefault(cm => cm.IsVerified && !cm.IsDeleted && cm.ContactMethodType == ContactMethodType.Email);

			if (verifiedEmail != null)
			{
				Log.DebugFormat("For notification ID {0}, attempting to send email to user {1}, contact method {2}, depleted {3}", 
					notification.ID, verifiedEmail.UserID, verifiedEmail.ID, isBalanceDepleted);

				EmailNotificationService.SendBalanceNotification(notification.TargetUser, verifiedEmail, isBalanceDepleted);
			}
			else
			{
				Log.WarnFormat("Could not send email for NotificationMessage ID {0} because no verified email addresses are registered for the user.", notification.ID);
			}
		}

		#endregion

		#region Reason: BalanceDepleted

		private void NotifyBalanceDepleted(NotificationMessage notification)
		{
			var now = DateTime.Now;
			var threeDays = TimeSpan.FromDays(3);
			var twentyDays = TimeSpan.FromDays(20);
			var fromCreationTillNow = (notification.CreationTime - now).Duration();
			var maxTransmissionDue = notification.CreationTime.AddMonths(1);

			// Send SMS if it is first notification, or last (after 24 days)
			if (fromCreationTillNow < threeDays || fromCreationTillNow > twentyDays)
				SendBalanceDepletedSms(notification);

			// Send Email everytime
			SendLowBalanceEmail(notification, false);

			notification.NextMessageTransmissionDue =
				fromCreationTillNow < threeDays
					? now + threeDays
					: now + fromCreationTillNow;

			if (notification.NextMessageTransmissionDue > maxTransmissionDue)
				notification.NextMessageTransmissionDue = null;
		}

		private void SendBalanceDepletedSms(NotificationMessage notification)
		{
			var verifiedPhone = notification.TargetUser.ContactMethods
				.FirstOrDefault(cm => cm.IsVerified && !cm.IsDeleted && cm.ContactMethodType == ContactMethodType.Phone);

			if (verifiedPhone != null)
			{
				Log.DebugFormat("For notification ID {0}, attempting to send SMS to user {1}, contact method {2}",
					notification.ID, notification.TargetUserID, verifiedPhone.ID);

				SmsNotificationService.NotifyBalanceDepleted(verifiedPhone, notification);
			}
			else
			{
				Log.WarnFormat("Could not send SMS for NotificationMessage ID {0} because no verified phone numbers are registered for the user.", notification.ID);
			}
		}

		#endregion

		#region Helper methods

		private void VerifyNextMessageTransmissionDue(NotificationMessage notification, DateTime? currentNextTransmissionDue)
		{
			if (notification.NextMessageTransmissionDue.HasValue)
			{
				var tomorrow = DateTime.Now.AddDays(1);
				if (notification.NextMessageTransmissionDue.Value < tomorrow)
					notification.NextMessageTransmissionDue = tomorrow;
			}

			Log.DebugFormat("For NotificationMessage.ID={0}, NextMsgTxDue changed from {1} => {2}",
				notification.ID,
				currentNextTransmissionDue.IfHasValue(dt => dt.ToString(CultureInfo.InvariantCulture), "<NULL>"),
				notification.NextMessageTransmissionDue.IfHasValue(dt => dt.ToString(CultureInfo.InvariantCulture), "<NULL>"));
		}

		#endregion
	}
}