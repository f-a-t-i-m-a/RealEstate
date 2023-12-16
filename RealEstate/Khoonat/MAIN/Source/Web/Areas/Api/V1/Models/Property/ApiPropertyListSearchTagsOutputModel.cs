using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property
{
	public class ApiPropertyListSearchTagsOutputModel : ApiOutputModelBase
	{
		public TagDetail[] EstateTags { get; set; }
		public TagDetail[] BuildingTags { get; set; }
		public TagDetail[] UnitTags { get; set; }
		public TagDetail[] OtherTags { get; set; }

		public class TagDetail
		{
			public string Tag { get; set; }
			public string DisplayTextKey { get; set; }
			public string LocalizedDisplayText { get; set; }
		}
	}
}