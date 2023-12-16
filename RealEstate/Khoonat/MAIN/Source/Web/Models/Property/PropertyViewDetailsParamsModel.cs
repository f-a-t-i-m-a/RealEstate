using JahanJooy.RealEstate.Web.Areas;

namespace JahanJooy.RealEstate.Web.Models.Property
{
	public class PropertyViewDetailsParamsModel
	{
		public long? ID { get; set; }
		public PropertyViewDetailsOrigin? Origin { get; set; }
		public string OriginQuery { get; set; }
		public long? OriginIndex { get; set; }
		public long? OriginCount { get; set; }

        public string Area { get { return AreaNames.Main; }}
	}

	public enum PropertyViewDetailsOrigin
	{
		Code,
		Search,
		Link
	}
}