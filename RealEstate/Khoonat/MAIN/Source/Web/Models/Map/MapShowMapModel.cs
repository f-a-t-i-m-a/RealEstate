using System.Collections.Generic;
using System.Data.Entity.Spatial;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Web.Models.Properties;
using JahanJooy.RealEstate.Web.Resources.Views.Properties;

namespace JahanJooy.RealEstate.Web.Models.Map
{
    public class MapShowMapModel
    {
        public long? ID { get; set; }
        public DbGeography CenterPoint { get; set; }
        public PropertySearch SearchQuery  { get; set; }

        public Vicinity Vicinity { get; set; }
        public IEnumerable<PropertiesMapSearchMenuModel> PropertyTypeMenuItems { get; set; }
        public IEnumerable<PropertiesMapSearchMenuModel> IntentionMenuItems { get; set; }
        public bool ShouldHaveElevator { get; set; }
        public bool ShouldHaveParking { get; set; }
        public bool ShouldHaveStorageRoom { get; set; }
        public bool ShouldHasPhotos { get; set; }

    }
}