using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Property
{
	public class PropertyRequestConfiguration : EntityTypeConfiguration<PropertyRequest>
	{
		public PropertyRequestConfiguration()
		{
			Ignore(pr => pr.Content);
			Property(pr => pr.ContentString).HasMaxLength(null);

			Property(pr => pr.Code).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
			Property(pr => pr.EditPassword).HasMaxLength(20);

			HasOptional(pr => pr.CreatorSession)
				.WithMany()
				.HasForeignKey(pr => pr.CreatorSessionID)
				.WillCascadeOnDelete(false);
			HasOptional(pr => pr.CreatorUser)
				.WithMany()
				.HasForeignKey(pr => pr.CreatorUserID)
				.WillCascadeOnDelete(false);
			HasOptional(pr => pr.OwnerUser)
				.WithMany()
				.HasForeignKey(pr => pr.OwnerUserID)
				.WillCascadeOnDelete(false);
		}
	}
}