using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Compositional.Composer.Web;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Util;
using JahanJooy.RealEstate.Util.Presentation;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared
{
	public class ApiOutputVicinityModel
	{
		public static readonly IValueResolver VicinityIDValueResolver = new ValueResolverFromVicinityID();
		public static readonly IValueResolver NullableVicinityIDValueResolver = new ValueResolverFromNullableVicinityID();
		public static readonly IValueResolver VicinityValueResolver = new ValueResolverFromVicinity();

		public long ID { get; set; }
		public string Label { get; set; }
		public string Text { get; set; }

		public HierarchyItem[] Hierarchy { get; set; }

		public class HierarchyItem
		{
			public long ID { get; set; }
			public long? ParentID { get; set; }
			public string Label { get; set; }
			public VicinityType Type { get; set; }
			public bool ShowTypeInTitle { get; set; }

			public static HierarchyItem FromDomain(Domain.Vicinity vicinity)
			{
				return new HierarchyItem
				       {
					       ID = vicinity.ID,
					       ParentID = vicinity.ParentID,
					       Label = vicinity.Name,
					       Type = vicinity.Type,
					       ShowTypeInTitle = vicinity.ShowTypeInTitle
				       };
			}
		}

		public static ApiOutputVicinityModel FromVicinityID(long? vicinityID, IVicinityCache vicinityCache, bool includeHierarchy = true)
		{
            if (!vicinityID.HasValue)
                return null;
			
			var vicinity = vicinityCache[vicinityID.Value];
			return FromVicinity(vicinity, includeHierarchy);
		}

		public static ApiOutputVicinityModel FromVicinity(Domain.Vicinity vicinity, bool includeHierarchy)
		{
			if (vicinity == null)
				return null;

			var result = new ApiOutputVicinityModel();
			result.PopulateFromVicinity(vicinity, includeHierarchy);
			return result;
		}

        public virtual void PopulateFromVicinity(Domain.Vicinity vicinity, bool includeHierarchy)
		{
            ID = vicinity.ID;
            Label = VicinityPresentationHelper.BuildTitle(vicinity);
            Text = VicinityDisplayItem.ToString(VicinityPresentationHelper.BuildHierarchyString(vicinity, true, false, true, false));

            if (includeHierarchy)
            {
                var resultHierarchy = new List<HierarchyItem>();
                resultHierarchy.AddRange(vicinity.GetParentsInclusive().Select(HierarchyItem.FromDomain));
                Hierarchy = resultHierarchy.ToArray();
            }
        }

		private class ValueResolverFromVicinityID : ValueResolver<long, ApiOutputVicinityModel>
		{
			protected override ApiOutputVicinityModel ResolveCore(long vicinityId)
			{
				return FromVicinityID(vicinityId, ComposerWebUtil.ComponentContext.GetComponent<IVicinityCache>());
			}
		}

		private class ValueResolverFromNullableVicinityID : ValueResolver<long?, ApiOutputVicinityModel>
		{
			protected override ApiOutputVicinityModel ResolveCore(long? vicinityId)
			{
				return vicinityId.HasValue ? FromVicinityID(vicinityId.Value, ComposerWebUtil.ComponentContext.GetComponent<IVicinityCache>()) : null;
			}
		}

		private class ValueResolverFromVicinity : ValueResolver<Domain.Vicinity, ApiOutputVicinityModel>
		{
			protected override ApiOutputVicinityModel ResolveCore(Domain.Vicinity vicinity)
			{
				return FromVicinity(vicinity, true);
			}
		}
	}
}