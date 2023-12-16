using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.SavedSearch
{
	public class SavedPropertySearchConfiguration : EntityTypeConfiguration<SavedPropertySearch>
	{
		public SavedPropertySearchConfiguration()
		{
			Property(sps => sps.Title).HasMaxLength(100);
			Property(sps => sps.AdditionalFilters).HasMaxLength(2000);

			HasRequired(sps => sps.User)
				.WithMany()
				.HasForeignKey(sps => sps.UserID)
				.WillCascadeOnDelete(true);

			HasOptional(sps => sps.CreatorSession)
				.WithMany()
				.HasForeignKey(sps => sps.CreatorSessionID)
				.WillCascadeOnDelete(false);

			HasOptional(sps => sps.EmailNotificationTarget)
				.WithMany()
				.HasForeignKey(sps => sps.EmailNotificationTargetID)
				.WillCascadeOnDelete(false);

			HasOptional(sps => sps.SmsNotificationTarget)
				.WithMany()
				.HasForeignKey(sps => sps.SmsNotificationTargetID)
				.WillCascadeOnDelete(false);
		}
	}
}