using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Util.Resources;

namespace JahanJooy.RealEstate.Util.Presentation
{
    [Contract]
    [Component]
    public class VicinityPresentationHelper
    {
        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

		public List<VicinityDisplayItem> BuildHierarchyString(long vicinityID, bool includeType = true,
            bool includeAlternativeNames = true, bool useWellknownScope = true, bool summary = false)
        {
			return BuildHierarchyString(VicinityCache, vicinityID, includeType, includeAlternativeNames,
                useWellknownScope, summary);
        }

		public static List<VicinityDisplayItem> BuildHierarchyString(IVicinityCache vicinityCache, long vicinityID, bool includeType = true, bool includeAlternativeNames = true, bool useWellknownScope = true, bool summary = false)
        {
            var vicinity = vicinityCache[vicinityID];
	        return BuildHierarchyString(vicinity, includeType, includeAlternativeNames, useWellknownScope, summary);
        }

	    public static List<VicinityDisplayItem> BuildHierarchyString(Vicinity vicinity, bool includeType = true, bool includeAlternativeNames = true,
		    bool useWellknownScope = true, bool summary = false)
	    {
		    if (vicinity == null)
			    return null;

			var result = new List<VicinityDisplayItem>();
			VicinityType? scopeToInclude = null;

			foreach (var hierarchy in vicinity.GetParentsInclusive())
			{
				if (useWellknownScope && scopeToInclude.HasValue)
				{
					if (hierarchy.Type != scopeToInclude.Value)
						continue;
				}

				scopeToInclude = hierarchy.WellKnownScope;

				if (summary && !hierarchy.ShowInSummary && hierarchy.ID != vicinity.ID)
					continue;

				var vicinityDisplayItem = new VicinityDisplayItem
				                          {
					                          Name = hierarchy.Name,
					                          VicinityType = hierarchy.ShowTypeInTitle && includeType
						                          ? hierarchy.Type.Label(DomainEnumResources.ResourceManager)
						                          : string.Empty,
					                          AlternativeNames = hierarchy.AlternativeNames,
				                          };

				result.Add(vicinityDisplayItem);
			}
			result.Reverse();
			return result;
		}

		public static string BuildTitle(Vicinity vicinity, bool includeType = true, bool includeAlternativeNames = false)
        {
            return includeType && vicinity.ShowTypeInTitle
                ? vicinity.Type.Label(DomainEnumResources.ResourceManager) + " " + vicinity.Name
                : vicinity.Name;
        }
    }
}