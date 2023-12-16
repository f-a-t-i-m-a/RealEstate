using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Cms;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Cms
{
    public class ArticleSetConfiguration : EntityTypeConfiguration<ArticleSet>
    {
        public ArticleSetConfiguration()
        {
            Property(s => s.Identifier).HasMaxLength(200);

            HasRequired(s => s.Category)
                .WithMany(c => c.ArticleSets)
                .HasForeignKey(s => s.CategoryID)
                .WillCascadeOnDelete(false);

            Property(s => s.DefaultStyleClass).HasMaxLength(50);
            Property(s => s.Name).HasMaxLength(100);
            Property(s => s.Description).HasMaxLength(500);

            HasOptional(s => s.OwnerUser)
                .WithMany()
                .HasForeignKey(s => s.OwnerUserID)
                .WillCascadeOnDelete(false);

            HasRequired(s => s.CreatedByUser)
                .WithMany()
                .HasForeignKey(s => s.CreatedByUserID)
                .WillCascadeOnDelete(false);

            HasRequired(s => s.LastModifiedByUser)
                .WithMany()
                .HasForeignKey(s => s.LastModifiedByUserID)
                .WillCascadeOnDelete(false);

            HasMany(s => s.Articles)
                .WithOptional(a => a.Set)
                .HasForeignKey(a => a.SetID)
                .WillCascadeOnDelete(false);
        }
    }
}