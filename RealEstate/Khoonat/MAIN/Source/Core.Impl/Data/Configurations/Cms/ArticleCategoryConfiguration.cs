using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Cms;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Cms
{
    public class ArticleCategoryConfiguration : EntityTypeConfiguration<ArticleCategory>
    {
        public ArticleCategoryConfiguration()
        {
            Property(s => s.Identifier).HasMaxLength(200);
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

            HasMany(c => c.Articles)
                .WithRequired(a => a.Category)
                .HasForeignKey(a => a.CategoryID)
                .WillCascadeOnDelete(false);

            HasMany(c => c.ArticleSets)
                .WithRequired(s => s.Category)
                .HasForeignKey(s => s.CategoryID)
                .WillCascadeOnDelete(false);
        }
    }
}