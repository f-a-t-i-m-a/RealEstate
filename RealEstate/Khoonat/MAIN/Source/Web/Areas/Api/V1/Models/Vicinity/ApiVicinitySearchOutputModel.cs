using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Vicinity
{
	public class ApiVicinitySearchOutputModel : ApiOutputModelBase
	{
		public ApiOutputVicinityModel[] Vicinities { get; set; }
		public bool ThereIsMore { get; set; }
	}
}