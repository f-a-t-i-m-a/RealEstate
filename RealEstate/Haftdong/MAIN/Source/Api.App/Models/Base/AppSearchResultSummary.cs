using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Base
{
    public class AppSearchResultSummary
    {
        public ObjectId ID { get; set; }
        public string DataType { get; set; }
        public string Title { get; set; }
        public string DisplayText { get; set; }
    }
}