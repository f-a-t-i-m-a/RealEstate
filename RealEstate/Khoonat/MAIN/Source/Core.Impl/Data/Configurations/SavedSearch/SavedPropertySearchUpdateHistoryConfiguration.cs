using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.SavedSearch
{
    public class SavedPropertySearchUpdateHistoryConfiguration : EntityTypeConfiguration<SavedPropertySearchUpdateHistory>
    {
        public SavedPropertySearchUpdateHistoryConfiguration()
        {
            HasRequired(h => h.SavedSearch)
                .WithMany()
                .HasForeignKey(h => h.SavedSearchID)
                .WillCascadeOnDelete(true);

            HasOptional(h => h.Session)
                .WithMany()
                .HasForeignKey(h => h.SessionID)
                .WillCascadeOnDelete(false);

            HasRequired(h => h.User)
                .WithMany()
                .HasForeignKey(h => h.UserID)
                .WillCascadeOnDelete(false);
        }
    }
}