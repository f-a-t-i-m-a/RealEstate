using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Base
{
    [TsClass]
    [AutoMapperConfig]
    public class AppPhotoInfoSummary
    {
        public ObjectId ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string OriginalFileName { get; set; }
        public string OriginalFileExtension { get; set; }
        public string ContentType { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime? DeletionTime { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<PhotoInfo, AppPhotoInfoSummary>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<PhotoInfoSummary, AppPhotoInfoSummary>()
                .IgnoreUnmappedProperties();
        }
    }
}