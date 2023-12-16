using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.DomainExtensions;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Models.Sitemap;

namespace JahanJooy.RealEstate.Web.Controllers
{
	public class SitemapController : CustomControllerBase
	{
		#region Injected dependencies

		[ComponentPlug]
		public DbManager DbManager { get; set; }

		#endregion

		[HttpGet]
		public ActionResult Index()
		{
			// Sitemap index page
			// Contains addresses for the rest of the actions in this controller

			DateTime weekStart = GetWeekStartDate();

			var propertyTypeOptions = Enum.GetNames(typeof (PropertyType)).Select(s => s.ToLower()).ToList();
			var intentionOfOwnerOptions = Enum.GetNames(typeof (IntentionOfOwner)).Select(s => s.ToLower()).ToList();

			propertyTypeOptions.Insert(0, "all");
			intentionOfOwnerOptions.Insert(0, "all");

			var sitemaps = new List<SitemapIndexItemModel>(propertyTypeOptions.Count * intentionOfOwnerOptions.Count + 5);

			sitemaps.Add(new SitemapIndexItemModel { FileName = "sitemap.static.xml", LastModified = weekStart});
			sitemaps.Add(new SitemapIndexItemModel { FileName = "sitemap.properties.xml", LastModified = weekStart });

			// ReSharper disable LoopCanBeConvertedToQuery
			foreach (var propertyTypeOption in propertyTypeOptions)
			{
				foreach (var intentionOfOwnerOption in intentionOfOwnerOptions)
				{
					sitemaps.Add(new SitemapIndexItemModel
						                        {
							                        FileName = "sitemap.browsepropertylistings." + propertyTypeOption + "." + intentionOfOwnerOption + ".xml",
							                        LastModified = weekStart
						                        });
				}
			}
			// ReSharper restore LoopCanBeConvertedToQuery

			var model = new SitemapIndexModel {Sitemaps = sitemaps};
			return View("SitemapXmlIndex", model);
		}

		public ActionResult Static()
		{
			// Static pages, including:
			// - Index 
			// - Pages created by content action on HomeController
			// - Create property
			// - Login and Register

			DateTime weekStart = GetWeekStartDate();

			var items = new List<SitemapItemModel>();
			items.Add(new SitemapItemModel { Url = Url.Action("Index", "Home"), ChangeFrequency = SitemapItemChangeFrequency.Weekly, Priority = 1.0, LastModification = weekStart});
			items.Add(new SitemapItemModel { Url = Url.Action("QuickCreate", "Property"), ChangeFrequency = SitemapItemChangeFrequency.Weekly, Priority = 0.8, LastModification = weekStart });
			items.Add(new SitemapItemModel { Url = Url.Action("LogOn", "Account"), ChangeFrequency = SitemapItemChangeFrequency.Weekly, Priority = 0.1, LastModification = weekStart });
			items.Add(new SitemapItemModel { Url = Url.Action("Register", "Account"), ChangeFrequency = SitemapItemChangeFrequency.Weekly, Priority = 0.1, LastModification = weekStart });
			items.Add(new SitemapItemModel { Url = Url.Action("ContactUs", "UserFeedback"), ChangeFrequency = SitemapItemChangeFrequency.Weekly, Priority = 0.5, LastModification = weekStart });
			items.Add(new SitemapItemModel { Url = Url.Action("ReportIssue", "UserFeedback"), ChangeFrequency = SitemapItemChangeFrequency.Weekly, Priority = 0.5, LastModification = weekStart });
			items.Add(new SitemapItemModel { Url = Url.Action("ReportSuggestion", "UserFeedback"), ChangeFrequency = SitemapItemChangeFrequency.Weekly, Priority = 0.5, LastModification = weekStart });
			items.AddRange(
				Enum.GetNames(typeof (HomeController.ContentPage))
					.Select(contentPageId => new SitemapItemModel { Url = Url.Action("Page", "Home", new { id = contentPageId }), ChangeFrequency = SitemapItemChangeFrequency.Weekly, Priority = 0.5, LastModification = weekStart }));

			return View("SitemapXml", new SitemapModel {Items = items});
		}

		public ActionResult Properties()
		{
			// All of the published property listings

			var items = new List<SitemapItemModel>();

			var listings = DbManager.Db.PropertyListings.Where(PropertyListingExtensions.GetPublicListingExpression()).Select(l => new { l.ID, l.ModificationDate }).ToList();
			items.AddRange(listings.Select(l => new SitemapItemModel
				                                    {
					                                    Url = Url.Action("ViewDetails", "Property", new {id = l.ID}),
					                                    ChangeFrequency = SitemapItemChangeFrequency.Weekly,
					                                    LastModification = l.ModificationDate,
					                                    Priority = 0.8
				                                    }));

			return View("SitemapXml", new SitemapModel { Items = items });
		}

		public ActionResult BrowsePropertyListings(string param1, string param2)
		{
			// Property Listing search page for all various location filters (all, each province, each city, each city region, each neighborhood)
			// param1 -> property type (required, "all" means no filter on property type)
			// param2 -> intention of owner (required, "all" means no filter on intention of owner)

			if (string.IsNullOrWhiteSpace(param1) || string.IsNullOrWhiteSpace(param2))
				return Error(ErrorResult.EntityNotFound);

			DateTime weekStart = GetWeekStartDate();
			var search = new PropertySearch();

			PropertyType propertyType;
			if (Enum.TryParse(param1, true, out propertyType))
				search.PropertyType = propertyType;
			else if (param1.ToLower() != "all")
				return Error(ErrorResult.EntityNotFound);

			IntentionOfOwner intentionOfOwner;
			if (Enum.TryParse(param2, true, out intentionOfOwner))
				search.IntentionOfOwner = intentionOfOwner;
			else if (param2.ToLower() != "all")
				return Error(ErrorResult.EntityNotFound);

			var listingExpr = PropertyListingExtensions.GetPublicListingExpression();
            var vicinities = DbManager.Db.Vicinities.Select(v => new { v.ID, ListingCount = v.PropertyListings.AsQueryable().Count(listingExpr) }).ToList();

			var items = new List<SitemapItemModel>();

			items.Add(new SitemapItemModel
				          {
							  Url = Url.Action("Browse", "Properties", new { q = PropertySearchQueryUtil.GenerateQuery(search) }),
					          ChangeFrequency = SitemapItemChangeFrequency.Weekly,
							  Priority = 0.9,
							  LastModification = weekStart
				          });


            foreach (var v in vicinities)
            {
                if (v.ListingCount < 1)
                    continue;

                search.VicinityIDs = new[] { v.ID };
                items.Add(new SitemapItemModel
                {
                    Url = Url.Action("Browse", "Properties", new { q = PropertySearchQueryUtil.GenerateQuery(search) }),
                    ChangeFrequency = SitemapItemChangeFrequency.Weekly,
                    Priority = 0.9,
                    LastModification = weekStart
                });
            }
            search.VicinityIDs = null;

			return View("SitemapXml", new SitemapModel { Items = items });
		}

		private DateTime GetWeekStartDate()
		{
			DateTime today = DateTime.Now.Date;
			return today.AddDays(-((int) today.DayOfWeek));
		}
	}
}