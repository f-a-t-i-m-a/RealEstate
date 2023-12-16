using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.VicinityCache;

namespace JahanJooy.RealEstateAgency.Util.Indexing
{
    [Component]
    public class RequestIndexMapper : IIndexMapper<Request, RequestIE>
    {
        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [ComponentPlug]
        public VicinityUtil VicinityUtil { get; set; }

        public RequestIE Map(Request entity)
        {
            var description = entity.IntentionOfCustomer + " " + entity.State +
                " "+ entity.UsageType;

            var vicinityStr = "";
            var vicinityIds = "";

            entity.Vicinities?.ForEach(v =>
            {
                vicinityStr += VicinityUtil.GetFullName(v);
                vicinityIds += VicinityUtil.GetFullIds(v);
            });

            return new RequestIE
            {
                ID = entity.ID.ToString(),
                CreationTime = entity.CreationTime.Ticks,
                PropertyTypes = entity.PropertyTypes?.ToList(),
                Mortgage = entity.Mortgage,
                Rent = entity.Rent,
                TotalPrice = entity.TotalPrice,
                EstateArea = entity.EstateArea,
                UnitArea = entity.UnitArea,
                Description = description,
                CreatedByID = entity.CreatedByID.ToString(),
                IsArchived = entity.IsArchived,
                IsPublic = entity.IsPublic,
                Vicinity = vicinityStr,
                VicinityIds = vicinityIds,
                ExpirationTime = entity.ExpirationTime?.Ticks,
            };
        }
    }
}