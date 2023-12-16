using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Messages;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Messages
{
    public class OutgoingSmsMessageConfiguration : EntityTypeConfiguration<OutgoingSmsMessage>
    {
        public OutgoingSmsMessageConfiguration()
        {
            HasOptional(m => m.TargetUser)
                .WithMany()
                .HasForeignKey(m => m.TargetUserID)
                .WillCascadeOnDelete(false);

            HasOptional(m => m.RetryForMessage)
                .WithMany()
                .HasForeignKey(m => m.RetryForMessageID)
                .WillCascadeOnDelete(false);

			HasOptional(m => m.Notification)
				.WithMany(n => n.SmsMessages)
				.HasForeignKey(m => m.NotificationID)
				.WillCascadeOnDelete(false);

            Property(m => m.SenderNumber).HasMaxLength(20);
            Property(m => m.TargetNumber).HasMaxLength(20);
            Property(m => m.MessageText).HasMaxLength(500);

            Property(m => m.OperatorAssignedID).HasMaxLength(50);
        }
    }
}