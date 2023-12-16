using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property
{
	public class ApiPropertySearchOutputModel : ApiOutputModelBase
	{
		public long TotalNumberOfRecords { get; set; }
		public int IndexOfFirstResult { get; set; }
		public int IndexOfLastResult { get; set; }

		public ApiPropertySummaryModel[] PropertyListings { get; set; }
		public ApiSponsoredPropertyListingSummaryModel[] SponsoredPropertyListingSummaryModel { get; set; }
	}
}