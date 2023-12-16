using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Cms;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Cms
{
    public class ArticleConfiguration : EntityTypeConfiguration<Article>
    {
        public ArticleConfiguration()
        {
            Property(a => a.Identifier).HasMaxLength(200);

            HasRequired(a => a.Category)
                .WithMany(ac => ac.Articles)
                .HasForeignKey(a => a.CategoryID)
                .WillCascadeOnDelete(false);

            HasOptional(a => a.Set)
                .WithMany(aset => aset.Articles)
                .HasForeignKey(a => a.SetID)
                .WillCascadeOnDelete(false);

            HasOptional(a => a.ParentArticle)
                .WithMany(a => a.ChildArticles)
                .HasForeignKey(a => a.ParentArticleID)
                .WillCascadeOnDelete(false);

            Property(a => a.StyleClass).HasMaxLength(50);
            Property(a => a.Name).HasMaxLength(100);

            HasOptional(a => a.OwnerUser)
                .WithMany()
                .HasForeignKey(a => a.OwnerUserID)
                .WillCascadeOnDelete(false);

            HasRequired(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedByUserID)
                .WillCascadeOnDelete(false);

            HasRequired(a => a.LastModifiedByUser)
                .WithMany()
                .HasForeignKey(a => a.LastModifiedByUserID)
                .WillCascadeOnDelete(false);

            HasMany(a => a.Revisions)
                .WithRequired(ar => ar.Article)
                .HasForeignKey(ar => ar.ArticleID)
                .WillCascadeOnDelete(false);
        }
    }
}