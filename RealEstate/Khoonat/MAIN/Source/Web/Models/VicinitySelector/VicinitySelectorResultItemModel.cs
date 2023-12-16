using System.Collections.Generic;
using System.Linq;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Util.DomainUtil;
using JahanJooy.RealEstate.Util.Resources;

namespace JahanJooy.RealEstate.Web.Models.VicinitySelector
{
    public class VicinitySelectorResultItemModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string AlternativeNames { get; set; }
        public bool Enabled { get; set; }
        public List<VicinitySelectorHierarchyItemModel> Hierarchy { get; set; }
        public string HierarchyString { get; set; }
        public VicinityType Type { get; set; }
        public string TypeLabel { get; set; }
        public string FullName { get; set; }
        public long? ParentID { get; set; }
        public bool HasChildren { get; set; }
        public bool CanContainPropertyRecords { get; set; }
        public bool ShowTypeInTitle { get; set; }


        public static IEnumerable<VicinitySelectorResultItemModel> Map(IEnumerable<Vicinity> vicinities)
        {
            return vicinities.Select(v => new VicinitySelectorResultItemModel
            {
                ID = v.ID,
                Name = v.Name,
                AlternativeNames = v.AlternativeNames,
                Enabled = v.Enabled,
                HierarchyString = string.Join(GeneralResources.Comma, v.GetParents()
                    .Select(h => h.GetDisplayName())
                    .Reverse()),
                Type = v.Type,
                TypeLabel = v.Type.Label(DomainEnumResources.ResourceManager),
                FullName = v.GetDisplayFullName(),
                ParentID = v.ParentID,
                HasChildren = (v.Children != null && v.Children.Any()),
                Hierarchy = VicinitySelectorHierarchyItemModel.Map(v.GetParents()),
                CanContainPropertyRecords = v.CanContainPropertyRecords,
                ShowTypeInTitle = v.ShowTypeInTitle
            });
        }

       
        // ReSharper restore ReturnTypeCanBeEnumerable.Global
    }
}