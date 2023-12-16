using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations
{
	public class UserConfiguration : EntityTypeConfiguration<User>
	{
		public UserConfiguration()
		{
			Property(u => u.Code).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
			Property(u => u.LoginName).IsRequired().HasMaxLength(80);
			Property(u => u.PasswordHash).IsRequired().HasMaxLength(50);
			Property(u => u.PasswordSalt).IsRequired().HasMaxLength(20);
			Property(u => u.DisplayName).IsRequired().HasMaxLength(80);
			Property(u => u.FullName).IsRequired().HasMaxLength(80);
			Property(u => u.GeneralPermissionsValue).HasMaxLength(1000);

            Property(u => u.Activity).HasMaxLength(200);
            Property(u => u.About).HasMaxLength(2000);
            Property(u => u.Services).HasMaxLength(2000);
            Property(u => u.WorkBackground).HasMaxLength(2000);
            Property(u => u.Address).HasMaxLength(300);
            Property(u => u.WebSiteUrl).HasMaxLength(250);

			HasRequired(u => u.CreatorSession)
				.WithMany()
				.HasForeignKey(u => u.CreatorSessionID)
				.WillCascadeOnDelete(false);

            HasOptional(u => u.Agency)
                .WithMany(a => a.MemberUsers)
                .HasForeignKey(u => u.AgencyID)
                .WillCascadeOnDelete(false);
		}
	}
}