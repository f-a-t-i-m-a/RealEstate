using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Util.Models.Base
{
    public class SearchResultSummary
    {
        public ObjectId ID { get; set; }
        public string DataType { get; set; }
        public string Title { get; set; }
        public string DetailState { get; set; }
        public string DisplayText { get; set; }
    }
}