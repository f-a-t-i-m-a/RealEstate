using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Application.Authorization;

namespace JahanJooy.RealEstate.Web.Application.Base
{
    [OperatorOnly]
	[RequireHttps]
    public class AdminControllerBase : CustomControllerBase
    {
    }
}