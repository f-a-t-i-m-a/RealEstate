using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Audit
{
	public class UserContactMethodVerificationConfiguration : EntityTypeConfiguration<UserContactMethodVerification>
	{
		public UserContactMethodVerificationConfiguration()
		{
			Property(l => l.VerificationSecret)
				.IsRequired()
				.HasMaxLength(20);

			HasRequired(l => l.Session)
				.WithMany()
				.HasForeignKey(l => l.SessionID)
				.WillCascadeOnDelete(false);

			HasRequired(l => l.TargetUser)
				.WithMany()
				.HasForeignKey(l => l.TargetUserID)
				.WillCascadeOnDelete(false);

			HasRequired(l => l.TargetContactMethod)
				.WithMany()
				.HasForeignKey(l => l.TargetContactMethodID)
				.WillCascadeOnDelete(true);

		}
	}
}