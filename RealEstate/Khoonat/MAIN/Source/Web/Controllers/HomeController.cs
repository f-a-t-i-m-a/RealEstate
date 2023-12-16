using System;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Web.Robots;
using JahanJooy.RealEstate.Core.DomainExtensions;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Models.Home;

namespace JahanJooy.RealEstate.Web.Controllers
{
	public class HomeController : CustomControllerBase
	{
		[ComponentPlug]
		public IAuthenticationService AuthenticationService { get; set; }

		[ComponentPlug]
		public DbManager DbManager { get; set; }

		public ActionResult Index()
		{
			var model = new HomeIndexModel();
			var listingsQuery = PropertyListingSummary.Summarize(DbManager.Db.PropertyListings.Where(PropertyListingExtensions.GetPublicListingExpression()));
			model.NewestListings = listingsQuery.OrderByDescending(l => l.PublishDate).Take(4).ToList();
			model.MostPopularListings = listingsQuery.OrderByDescending(l => l.Visits).Take(4).ToList();
			return View(model);
		}

		public ActionResult Page(string id)
		{
			ContentPage page;
			if (!Enum.TryParse(id, true, out page))
				return Error(ErrorResult.EntityNotFound);

			return View("Content/" + page);
		}

		public enum ContentPage
		{
			// General information about the application

			About,

			// Account, Profile and Verification

			WhyVerifyContactMethod
		}

		[HttpPost]
		public ActionResult AcknowledgeInteractiveSession(string token)
		{
			if (InteractiveSessionAckUtil.ValidateToken(token))
				AuthenticationService.AcknowledgeCurrentSessionAsInteractive();

			return Content("");
		}
	}
}
