using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using Nest;

namespace JahanJooy.RealEstateAgency.Util.Indexing
{
    [Component]
    public class SupplyIndexMapper : IIndexMapper<Supply, SupplyIE>
    {
        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [ComponentPlug]
        public VicinityUtil VicinityUtil { get; set; }

        public SupplyIE Map(Supply entity)
        {
            var description = entity.Property?.UsageType + " " + entity.Property?.PropertyType
                              + " " + entity.IntentionOfOwner + " " + entity.State
                              + " " + entity.Property?.SourceType;

            var vicinityStr = entity.Property?.Address ?? "";
            var vicinityIds = "";

            if (entity.Property?.Vicinity != null)
            {
                vicinityStr += VicinityUtil.GetFullName(entity.Property.Vicinity.ID);
                vicinityIds += VicinityUtil.GetFullIds(entity.Property.Vicinity.ID);
            }

            LatLon location = null;
            if (entity.Property?.GeographicLocation != null)
            {
                location = new LatLon
                {
                    Lat = entity.Property.GeographicLocation.Y,
                    Lon = entity.Property.GeographicLocation.X
                };
            }

            return new SupplyIE
            {
                ID = entity.ID.ToString(),
                Description = description,
                IsArchived = entity.IsArchived,
                HasPhoto =
                    entity.Property?.CoverImageID.HasValue != null && (bool) entity.Property?.CoverImageID.HasValue,
                HasWarning =
                    entity.Property?.ConversionWarning.HasValue != null &&
                    (bool) entity.Property?.ConversionWarning.HasValue
                        ? entity.Property?.ConversionWarning.Value
                        : null,
                IsHidden =
                    entity.Property?.IsHidden.HasValue != null && (bool) entity.Property?.IsHidden.HasValue
                        ? entity.Property?.IsHidden.Value
                        : null,
                IsPublic = entity.IsPublic,
                EstateArea = entity.Property?.EstateArea,
                UnitArea = entity.Property?.UnitArea,
                NumberOfRooms = entity.Property?.NumberOfRooms,
                Mortgage = entity.Mortgage,
                Rent = entity.Rent,
                TotalPrice = entity.TotalPrice,
                PricePerEstateArea = entity.PricePerEstateArea,
                PricePerUnitArea = entity.PricePerUnitArea,
                CreationTime = entity.CreationTime.Ticks,
                ExpirationTime = entity.ExpirationTime?.Ticks,
                CreatedByID = entity.CreatedByID.ToString(),
                Vicinity = vicinityStr,
                VicinityIds = vicinityIds,
                GeographicLocation = location
            };
        }
    }
}