using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JahanJooy.RealEstate.Domain.Cms
{
    public class ArticleCategory
    {
        public long ID { get; set; }
        public string Identifier { get; set; }

        public ArticleDisplayType DefaultDisplayType { get; set; }
        public string DefaultStyleClass { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }
        public DateTime? DeleteTime { get; set; }

        public long? OwnerUserID { get; set; }
        public long CreatedByUserID { get; set; }
        public long LastModifiedByUserID { get; set; }

        public User OwnerUser { get; set; }
        public User CreatedByUser { get; set; }
        public User LastModifiedByUser { get; set; }

        public ICollection<Article> Articles { get; set; }
        public ICollection<ArticleSet> ArticleSets { get; set; }
    }
}