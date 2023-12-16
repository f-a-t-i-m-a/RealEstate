using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Application.Security;
using JahanJooy.RealEstate.Web.Models.OwnedProperties;
using JahanJooy.RealEstate.Web.Models.Shared.Profile;

namespace JahanJooy.RealEstate.Web.Controllers
{
	public class OwnedPropertiesController : CustomControllerBase
	{
		[ComponentPlug]
		public IPropertyService PropertyService { get; set; }

		[HttpGet]
		public ActionResult ViewList()
		{
			if (SessionInfo.OwnedProperties == null || SessionInfo.OwnedProperties.Count < 1)
				return Error(ErrorResult.EntityNotFound);

			var summaries = PropertyService.LoadSummaries(SessionInfo.OwnedProperties);
			return View(new OwnedPropertiesViewListModel { PropertyListingSummaries = summaries });
		}

		[HttpPost]
		[Authorize]
		public ActionResult Acquire()
		{
			if (SessionInfo.OwnedProperties != null && SessionInfo.OwnedProperties.Count > 0)
			{
				PropertyService.SetOwner(SessionInfo.OwnedProperties, User.CoreIdentity.UserId.Value);
				SessionInfo.OwnedProperties = null;
				OwnedPropertiesCookieUtil.Set(null);
			}

			return RedirectToAction("View", "MyProfile", new { activeTab = ProfileModel.ProfileActiveTab.AllPropertyListings });
		}
	}
}