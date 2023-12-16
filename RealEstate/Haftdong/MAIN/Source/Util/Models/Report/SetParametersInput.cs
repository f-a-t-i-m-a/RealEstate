using System.Collections.Generic;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Report
{
    [TsClass]
    public class SetParametersInput
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public List<ReportTemplateParameter> Parameters { get; set; }
        
    }
}