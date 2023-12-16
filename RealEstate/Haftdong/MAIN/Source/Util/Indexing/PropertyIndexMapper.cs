using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Property;
using Nest;

namespace JahanJooy.RealEstateAgency.Util.Indexing
{
    [Component]
    public class PropertyIndexMapper : IIndexMapper<Property, PropertyIE>
    {
        public PropertyIE Map(Property entity)
        {
            var description = entity.UsageType + " " + entity.PropertyType
                              + " " + entity.State
                              + " " + (entity.Owner != null ? entity.Owner.DisplayName : "");

            LatLon location = null;
            if (entity.GeographicLocation != null)
            {
                location = new LatLon
                {
                    Lat = entity.GeographicLocation.Y,
                    Lon = entity.GeographicLocation.X
                };
            }

            return new PropertyIE
            {
                ID = entity.ID.ToString(),
                Address = entity.Address,
                Description = description,
                IsArchived = entity.IsArchived,
                EstateArea = entity.EstateArea,
                UnitArea = entity.UnitArea,
                NumberOfRooms = entity.NumberOfRooms,
                CreationTime = entity.CreationTime.Ticks,
                CreatedByID = entity.CreatedByID.ToString(),
                GeographicLocation = location
            };
        }
    }
}