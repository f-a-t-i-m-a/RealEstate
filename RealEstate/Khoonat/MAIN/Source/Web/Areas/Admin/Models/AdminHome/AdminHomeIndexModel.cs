using TypeLite;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminHome
{
	[TsClass(Module = "AdminHome")]
	public class AdminHomeIndexModel
	{
		public int AbuseFlagsQueueSize { get; set; }
		public int PropertyListingQueueSize { get; set; }
		public int PropertyListingPhotosQueueSize { get; set; }

		public string[] IndexesWithErrors { get; set; }
		public string[] IndexesNotCommitting { get; set; }
	}
}