using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Audit
{
	public class ActivityLogConfiguration : EntityTypeConfiguration<ActivityLog>
	{
		public ActivityLogConfiguration()
		{
			Property(l => l.ActionDetails)
				.IsRequired()
				.HasMaxLength(80);

			HasOptional(l => l.Session)
				.WithMany(s => s.ActivityLogs)
				.HasForeignKey(l => l.SessionID)
				.WillCascadeOnDelete(false);

			HasOptional(l => l.AuthenticatedUser)
				.WithMany()
				.HasForeignKey(l => l.AuthenticatedUserID)
				.WillCascadeOnDelete(false);

			HasOptional(l => l.ReviewedBy)
				.WithMany()
				.HasForeignKey(l => l.ReviewedByID)
				.WillCascadeOnDelete(false);
		}
	}
}