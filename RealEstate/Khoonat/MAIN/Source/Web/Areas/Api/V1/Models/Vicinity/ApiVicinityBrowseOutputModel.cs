using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Vicinity
{
	public class ApiVicinityBrowseOutputModel : ApiOutputModelBase
	{
		public ApiOutputVicinityDetailsModel[] Vicinities { get; set; }
	}
}