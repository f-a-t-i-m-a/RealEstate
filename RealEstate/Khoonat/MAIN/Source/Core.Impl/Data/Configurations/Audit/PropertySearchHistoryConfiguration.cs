using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Audit
{
	public class PropertySearchHistoryConfiguration : EntityTypeConfiguration<PropertySearchHistory>
	{
		public PropertySearchHistoryConfiguration()
		{
			Property(h => h.SearchQuery).HasMaxLength(2000);

			HasOptional(h => h.Session)
				.WithMany()
				.HasForeignKey(h => h.SessionID)
				.WillCascadeOnDelete(false);

			HasOptional(h => h.User)
				.WithMany()
				.HasForeignKey(h => h.UserID)
				.WillCascadeOnDelete(false);
		}
	}
}