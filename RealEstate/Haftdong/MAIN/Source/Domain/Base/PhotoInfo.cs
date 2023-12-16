using System;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Domain.Base
{
    public class PhotoInfo
    {
        public ObjectId ID { get; set; }
        public long? ExternalID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string OriginalFileName { get; set; }
        public string OriginalFileExtension { get; set; }
        public string ContentType { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime? DeletionTime { get; set; }
    }
}