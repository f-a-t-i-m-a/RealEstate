using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Audit
{
    public class HttpSessionConfiguration : EntityTypeConfiguration<HttpSession>
    {
        public HttpSessionConfiguration()
        {
	        Property(s => s.HttpSessionID).HasMaxLength(50);
	        Property(s => s.PrevHttpSessionID).HasMaxLength(50);
	        Property(s => s.UserAgent).HasMaxLength(400);
	        Property(s => s.StartupUri).HasMaxLength(2000);
	        Property(s => s.ReferrerUri).HasMaxLength(2000);
	        Property(s => s.ClientAddress).HasMaxLength(50);

            HasOptional(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserID)
                .WillCascadeOnDelete(false);

			HasRequired(s => s.UniqueVisitor)
				.WithMany(v => v.HttpSessions)
				.HasForeignKey(s => s.UniqueVisitorID)
				.WillCascadeOnDelete(false);
        }
    }
}