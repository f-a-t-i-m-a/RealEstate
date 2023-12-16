using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Application.Authorization;
using JahanJooy.RealEstate.Web.Application.Base;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Controllers
{
    [OperatorOnly]
    [RequireHttps]
    public class PromotionalBonusCouponAdminController : CustomControllerBase
    {
         
    }
}