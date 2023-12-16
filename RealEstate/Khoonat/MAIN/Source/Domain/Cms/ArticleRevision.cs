using System;

namespace JahanJooy.RealEstate.Domain.Cms
{
    public class ArticleRevision
    {
        public long ID { get; set; }

        public int RevisionNumber { get; set; }
        public long ArticleID { get; set; }
        public Article Article { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }
        public DateTime? FinalizationTime { get; set; }
        public DateTime? PublishTime { get; set; }

        public long CreatedByUserID { get; set; }
        public long LastModifiedByUserID { get; set; }
        public long? FinalizedByUserID { get; set; }
        public long? PublishedByUserID { get; set; }

        public User CreatedByUser { get; set; }
        public User LastModifiedByUser { get; set; }
        public User FinalizedByUser { get; set; }
        public User PublishedByUser { get; set; }

        public string LinkText { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Markdown { get; set; }
    }
}