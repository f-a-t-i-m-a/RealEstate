using System.Collections.Generic;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Geo
{
	public class ApiGeoCodeOutputModel : ApiOutputModelBase
	{
		public List<Alternative> Alternatives { get; set; }

		public class Alternative
		{
			public ApiOutputVicinityDetailsModel Vicinity { get; set; }
			public string Address { get; set; }
		}
	}
}