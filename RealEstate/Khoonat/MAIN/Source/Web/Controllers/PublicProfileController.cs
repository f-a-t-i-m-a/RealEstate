using System;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Application.Security;
using JahanJooy.RealEstate.Web.Models.Shared.Profile;

namespace JahanJooy.RealEstate.Web.Controllers
{
	public class PublicProfileController : CustomControllerBase
	{
		#region Injected dependencies

		[ComponentPlug]
		public IAuthenticationService AuthenticationService { get; set; }

		[ComponentPlug]
		public IPropertyService PropertyService { get; set; }

		[ComponentPlug]
		public IPrincipalCache PrincipalCache { get; set; }

		#endregion

		[HttpGet]
		[ActionName("View")]
		public ActionResult ViewAction(string loginName, string activeTab, string page)
		{
			var user = AuthenticationService.LoadUserInfo(loginName);
			if (user == null)
				return Error(ErrorResult.EntityNotFound);

			var activeTabEnum = ProfileModel.ProfileActiveTab.General;
			if (!string.IsNullOrWhiteSpace(activeTab))
				Enum.TryParse(activeTab, out activeTabEnum);

			int pageNum = 1;
			if (!string.IsNullOrWhiteSpace(page))
				int.TryParse(page, out pageNum);

			var model = new ProfileModel
			{
				ActiveTab = activeTabEnum,
				User = user,
				PaginationUrlTemplate = Url.Action("View", new { activeTab = activeTabEnum.ToString() }) + "&page={0}",
				EnableAdmin = false,
				EnableEdit = false
			};

			switch (activeTabEnum)
			{
				case ProfileModel.ProfileActiveTab.General:
					break;

				case ProfileModel.ProfileActiveTab.PublishedPropertyListings:
					model.PropertyListings = PropertyService.ListingsOfUser(user.ID, true, false, pageNum, 20);
					break;

				default:
					return Error(ErrorResult.EntityNotFound);
			}

			return View(model);
		}

	}
}