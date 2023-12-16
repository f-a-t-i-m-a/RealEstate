using System;
using System.Collections.Generic;

namespace JahanJooy.RealEstate.Domain.Cms
{
    public class Article
    {
        public long ID { get; set; }
        public string Identifier { get; set; }
        public long Order { get; set; }

        public long CategoryID { get; set; }
        public long? SetID { get; set; }
        public long? ParentArticleID { get; set; }
        public ArticleCategory Category { get; set; }
        public ArticleSet Set { get; set; }
        public Article ParentArticle { get; set; }

        public ArticleDisplayType DisplayType { get; set; }
        public string StyleClass { get; set; }

        public string Name { get; set; }
        public bool IsLocked { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }
        public DateTime? DeleteTime { get; set; }

        public long? OwnerUserID { get; set; }
        public long CreatedByUserID { get; set; }
        public long LastModifiedByUserID { get; set; }

        public User OwnerUser { get; set; }
        public User CreatedByUser { get; set; }
        public User LastModifiedByUser { get; set; }

        public ICollection<Article> ChildArticles { get; set; }
        public ICollection<ArticleRevision> Revisions { get; set; }
    }
}