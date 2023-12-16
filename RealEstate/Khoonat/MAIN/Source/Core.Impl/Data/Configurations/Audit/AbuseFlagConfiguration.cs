using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Audit
{
	public class AbuseFlagConfiguration : EntityTypeConfiguration<AbuseFlag>
	{
		public AbuseFlagConfiguration()
		{
			Property(f => f.Comments).HasMaxLength(500);

			HasOptional(f => f.ReportedBy)
				.WithMany()
				.HasForeignKey(f => f.ReportedByID)
				.WillCascadeOnDelete(false);

			HasOptional(f => f.ReportedInSession)
				.WithMany()
				.HasForeignKey(f => f.ReportedInSessionID)
				.WillCascadeOnDelete(false);

			HasOptional(f => f.ApprovedBy)
				.WithMany()
				.HasForeignKey(f => f.ApprovedByID)
				.WillCascadeOnDelete(false);
		}
	}
}