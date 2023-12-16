using AutoMapper;
using Compositional.Composer.Web;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Util;
using JahanJooy.RealEstate.Util.Presentation;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared
{
	public class ApiOutputVicinitySummaryModel
	{
		public static readonly IValueResolver VicinityIDValueResolver = new ValueResolverFromVicinityID();
		public static readonly IValueResolver NullableVicinityIDValueResolver = new ValueResolverFromNullableVicinityID();
		public static readonly IValueResolver VicinityValueResolver = new ValueResolverFromVicinity();

		public long ID { get; set; }
		public long? ParentID { get; set; }
		public string Label { get; set; }
		public string Text { get; set; }

		public static ApiOutputVicinitySummaryModel FromVicinityID(long? vicinityID, IVicinityCache vicinityCache)
		{
			if (!vicinityID.HasValue)
				return null;

			var vicinity = vicinityCache[vicinityID.Value];
			return FromVicinity(vicinity);
		}

		public static ApiOutputVicinitySummaryModel FromVicinity(Domain.Vicinity vicinity)
		{
			if (vicinity == null)
				return null;

			var result = new ApiOutputVicinitySummaryModel();
			result.PopulateFromVicinity(vicinity);
			return result;
		}

		public virtual void PopulateFromVicinity(Domain.Vicinity vicinity)
		{
			ID = vicinity.ID;
			ParentID = vicinity.ParentID;
			Label = VicinityPresentationHelper.BuildTitle(vicinity);
			Text = VicinityDisplayItem.ToString(VicinityPresentationHelper.BuildHierarchyString(vicinity, true, false, true, false));
		}

		private class ValueResolverFromVicinityID : ValueResolver<long, ApiOutputVicinitySummaryModel>
		{
			protected override ApiOutputVicinitySummaryModel ResolveCore(long vicinityId)
			{
				return FromVicinityID(vicinityId, ComposerWebUtil.ComponentContext.GetComponent<IVicinityCache>());
			}
		}

		private class ValueResolverFromNullableVicinityID : ValueResolver<long?, ApiOutputVicinitySummaryModel>
		{
			protected override ApiOutputVicinitySummaryModel ResolveCore(long? vicinityId)
			{
				return vicinityId.HasValue ? FromVicinityID(vicinityId.Value, ComposerWebUtil.ComponentContext.GetComponent<IVicinityCache>()) : null;
			}
		}

		private class ValueResolverFromVicinity : ValueResolver<Domain.Vicinity, ApiOutputVicinitySummaryModel>
		{
			protected override ApiOutputVicinitySummaryModel ResolveCore(Domain.Vicinity vicinity)
			{
				return FromVicinity(vicinity);
			}
		}
	}
}