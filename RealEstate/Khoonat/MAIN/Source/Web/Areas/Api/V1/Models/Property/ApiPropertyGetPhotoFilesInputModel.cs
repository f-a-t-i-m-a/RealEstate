using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property
{
	public class ApiPropertyGetPhotoFilesInputModel : ApiInputModelBase
	{
		public long[] PhotoIDs { get; set; }
		public PhotoSizeSpec Size { get; set; }
		
		public enum PhotoSizeSpec : byte
		{
			Thumbnail = 0,
			Medium = 50,
			Full = 100
		}
	}
}