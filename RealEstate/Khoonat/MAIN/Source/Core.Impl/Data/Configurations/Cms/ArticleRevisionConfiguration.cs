using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Cms;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Cms
{
    public class ArticleRevisionConfiguration : EntityTypeConfiguration<ArticleRevision>
    {
        public ArticleRevisionConfiguration()
        {
            HasRequired(ar => ar.Article)
                .WithMany(a => a.Revisions)
                .HasForeignKey(ar => ar.ArticleID)
                .WillCascadeOnDelete(false);

            HasRequired(ar => ar.CreatedByUser)
                .WithMany()
                .HasForeignKey(ar => ar.CreatedByUserID)
                .WillCascadeOnDelete(false);

            HasRequired(ar => ar.LastModifiedByUser)
                .WithMany()
                .HasForeignKey(ar => ar.LastModifiedByUserID)
                .WillCascadeOnDelete(false);

            HasOptional(ar => ar.FinalizedByUser)
                .WithMany()
                .HasForeignKey(ar => ar.FinalizedByUserID)
                .WillCascadeOnDelete(false);

            HasOptional(ar => ar.PublishedByUser)
                .WithMany()
                .HasForeignKey(ar => ar.PublishedByUserID)
                .WillCascadeOnDelete(false);

            Property(ar => ar.LinkText).HasMaxLength(100);
            Property(ar => ar.Title).HasMaxLength(200);
            Property(ar => ar.Description).HasMaxLength(500);
            Property(ar => ar.Markdown).HasMaxLength(null);
        }
    }
}