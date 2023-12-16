using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Messages;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Messages
{
	public class NotificationMessageConfiguration : EntityTypeConfiguration<NotificationMessage>
	{
		public NotificationMessageConfiguration()
		{
			HasRequired(n => n.TargetUser)
				.WithMany()
				.HasForeignKey(n => n.TargetUserID)
				.WillCascadeOnDelete(false);

			HasMany(n => n.SmsMessages)
				.WithOptional(m => m.Notification)
				.HasForeignKey(m => m.NotificationID)
				.WillCascadeOnDelete(false);

			Property(n => n.Text).HasMaxLength(1000);

		}
	}
}