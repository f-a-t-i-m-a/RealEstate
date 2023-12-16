using System.Collections.Generic;
using System.Linq;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Util.Resources;

namespace JahanJooy.RealEstate.Web.Models.VicinitySelector
{
    public class VicinitySelectorHierarchyItemModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string AlternativeNames { get; set; }
        public long? ParentID { get; set; }
        public bool CanContainPropertyRecords { get; set; }
        public string TypeLabel { get; set; }
        public bool ShowTypeInTitle { get; set; }


        public static List<VicinitySelectorHierarchyItemModel> Map(IEnumerable<Vicinity> vicinities)
        {
            return vicinities.Select(v => new VicinitySelectorHierarchyItemModel
            {
                ID = v.ID,
                Name = v.Name,
                AlternativeNames = v.AlternativeNames,
                ParentID = v.ParentID,
                CanContainPropertyRecords = v.CanContainPropertyRecords,
                TypeLabel = v.Type.Label(DomainEnumResources.ResourceManager),
                ShowTypeInTitle = v.ShowTypeInTitle

            }).ToList();
        }
    }
}