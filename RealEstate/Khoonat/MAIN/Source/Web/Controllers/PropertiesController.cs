using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Impl.Data.CommonQueries;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Helpers.Properties;
using JahanJooy.RealEstate.Web.Models.Properties;
using JahanJooy.RealEstate.Web.Models.Property;
using JahanJooy.RealEstate.Web.Resources.Controllers.Properties;
using JahanJooy.RealEstate.Web.Resources.Helpers.Properties;
using JahanJooy.RealEstate.Web.Resources.Views.Properties;

namespace JahanJooy.RealEstate.Web.Controllers
{
	public class PropertiesController : CustomControllerBase
	{
		private const int SearchPageSize = 20;
		private const int SidebarSavedSearchListSize = 5;

		#region Injected dependencies
		
		[ComponentPlug]
		public IPropertyService PropertyService { get; set; }

		[ComponentPlug]
		public ISavedSearchService SavedSearchService { get; set; }

		[ComponentPlug]
		public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

		[ComponentPlug]
		public IComposer Composer { get; set; }

		[ComponentPlug]
		public PropertySearchHelper PropertySearchHelper { get; set; }

		#endregion

		#region Action methods

		public ActionResult SearchPartial(string q)
		{
			var currentSearch = PropertySearchQueryUtil.ParseQuery(q);

			var model = PropertiesSearchPartialModel.FromPropertySearch(currentSearch);

			return PartialView("SearchPartial", model);
		}

		[HttpPost]
        [ActionName("SearchPostback")]
        [SubmitButton("btnSearch")]
		public ActionResult Search(PropertiesSearchPartialModel model)
		{
			if (!ModelState.IsValid)
				return RedirectToAction("Browse");

			var search = model.ToPropertySearch();
			search.FixLogicalErrors();

			return RedirectToAction("Browse", new { q = PropertySearchQueryUtil.GenerateQuery(search) });
		}

        [HttpPost]
        [ActionName("SearchPostback")]
        [SubmitButton("btnSearchOnMap")]
        public ActionResult SearchOnMap(PropertiesSearchPartialModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Browse");

            var search = model.ToPropertySearch();
            search.FixLogicalErrors();

            return RedirectToAction("ShowMap", "Map", new { q = PropertySearchQueryUtil.GenerateQuery(search) });
        }

		[HttpGet]
		public ActionResult Browse(int? page, string q, int? index)
		{
			if (index.HasValue)
				page = (index + SearchPageSize - 1) / SearchPageSize;

			if (page == null || page < 1)
				page = 1;

			var model = new PropertiesBrowseModel();
			model.Search = PropertySearchQueryUtil.ParseQuery(q);
			model.RequestedPageNumber = page.Value;

			if (model.Search == null)
				return Error(ErrorResult.EntityNotFound);

			model.SeoCanonicalSearch = BuildSeoCanonicalSearch(model.Search);

			model.SearchResult = PropertyService.RunSearch(model.Search, (page.Value - 1) * SearchPageSize, SearchPageSize, true, true);
			model.TotalNumberOfPages = (model.SearchResult.TotalNumberOfRecords + SearchPageSize - 1) / SearchPageSize;

			FillBreadcrumbItems(model);
			FillSortMenuItems(model);
			FillPropertyTypeMenuItems(model);
			FillIntentionMenuItems(model);
			FillLocationMenuItems(model);

			if (User.Identity.IsAuthenticated)
			{
				FillSidebarSavedSearchItems(model);
			}

			model.SelectedOptions = PropertySearchHelper.ExtractSelectedOptions(model.Search);

			model.LocationText = Composer.GetComponent<PropertySearchHelper>().BuildLocationText(model.Search);

			return View(model);
		}

		public ActionResult BrowseNext(int? originIndex, int? originCount, string originQuery)
		{
			if (!originIndex.HasValue || !originCount.HasValue)
				return Error(ErrorResult.EntityNotFound);

			return BrowseByIndex(originIndex.Value, originCount.Value, originQuery, 1);
		}

		public ActionResult BrowsePrev(int? originIndex, int? originCount, string originQuery)
		{
			if (!originIndex.HasValue || !originCount.HasValue)
				return Error(ErrorResult.EntityNotFound);

			return BrowseByIndex(originIndex.Value, originCount.Value, originQuery, -1);
		}

		[HttpPost]
		public ActionResult SelectLocation(string q)
		{
			var search = PropertySearchQueryUtil.ParseQuery(q) ?? new PropertySearch();

			var model = new PropertiesSelectLocationModel();
			model.Search = search;
			model.Query = PropertySearchQueryUtil.GenerateQuery(search);
            model.VicinityIDs = search.VicinityIDs == null ? null : (long[])search.VicinityIDs.Clone();

			return PartialView(model);
		}

		public ActionResult SelectLocationDone()
		{
			var model = new PropertiesSelectLocationModel();
			if (!TryUpdateModel(model))
				return RedirectToAction("Browse");

			var search = PropertySearchQueryUtil.ParseQuery(model.Query);
			if (search == null)
				return Error(ErrorResult.EntityNotFound);

		    if (model.VicinityIDs != null && model.VicinityIDs.Length > 0)
		        search.VicinityIDs = model.VicinityIDs;
			var newQuery = PropertySearchQueryUtil.GenerateQuery(search);
			return RedirectToAction("Browse", new {q = newQuery});
		}

		[HttpPost]
		public ActionResult SelectOption(string q)
		{
			var model = new PropertiesSelectOptionModel();
			var search = PropertySearchQueryUtil.ParseQuery(q) ?? new PropertySearch();
			q = search.Query; // To prevent attacks

			Func<KeyValuePair<string, PropertySearchOption>, bool> optionFilter = o => (!o.Value.DependsOnUserAccount || User.IsAuthenticated) && !search.Options.Contains(o.Key);
			Func<KeyValuePair<string, PropertySearchOption>, PropertiesSelectOptionItemModel> optionToModelFunc = o => new PropertiesSelectOptionItemModel
								 {
									 Label = "option_" + o.Value.Label.ToLower(),
									 Query = q + "-" + o.Key.ToLower()
								 };

			model.UnitOptions = PropertySearchOptions.UnitOptions.Where(optionFilter).Select(optionToModelFunc);
			model.BuildingOptions = PropertySearchOptions.BuildingOptions.Where(optionFilter).Select(optionToModelFunc);
			model.EstateOptions = PropertySearchOptions.EstateOptions.Where(optionFilter).Select(optionToModelFunc);
			model.OtherOptions = PropertySearchOptions.OtherOptions.Where(optionFilter).Select(optionToModelFunc);
			model.Query = q;

			return PartialView("SelectOption", model);
		}

		[HttpGet]
		[Authorize]
		public ActionResult SaveSearch(string searchString)
		{
			var model = new PropertiesSaveSearchModel {SearchString = searchString};
			model.Search = PropertySearchQueryUtil.ParseQuery(model.SearchString);

			FillSavedPropertySearchItems(model);

			model.SendNotificationEmails = true;
			model.SendPromotionalSmsMessages = true;
			model.SendPaidSmsMessages = true;

			model.SmsPartListingCode = true;
			model.SmsPartNumberOfRooms = true;
			model.SmsPartArea = true;
			model.SmsPartPriceOrRent = true;
			model.SmsPartContactPhone = true;

			model.Title = PropertySearchHelper.GetShortTitle(model.Search);

			return View(model);
		}

		[HttpPost]
		[Authorize]
		[ActionName("SaveSearch")]
		public ActionResult SaveSearchPostback(PropertiesSaveSearchModel model)
		{
			model.Search = PropertySearchQueryUtil.ParseQuery(model.SearchString);
			FillSavedPropertySearchItems(model);

			if (!ModelState.IsValid)
				return View(model);

			var savedPropertySearch = model.ToDomain(VicinityCache);
			var validationResult = SavedSearchService.SavePropertySearch(savedPropertySearch);

			if (!validationResult.IsValid)
			{
				foreach (var error in validationResult.Errors)
					ModelState.AddModelError("", PropertiesControllerResources.ResourceManager.GetString(error.FullResourceKey));

				return View(model);
			}

			return RedirectToAction("SavedSearches");
		}

		[HttpGet]
		[Authorize]
		public ActionResult SavedSearches()
		{
			var model = new PropertiesSavedSearchesModel();
			var savedPropertySearches = DbManager.Db.SavedPropertySearches
				.Where(sps => sps.UserID == User.CoreIdentity.UserId)
				.Include(sps => sps.GeographicRegions)
				.Include(sps => sps.SmsNotificationTarget)
				.Include(sps => sps.EmailNotificationTarget)
				.ToList();

			model.SavedSearches = savedPropertySearches.Select(sps => new PropertiesSavedSearchesModel.SavedPropertySearchModel
																	  {
																		  SavedPropertySearch = sps,
																		  PropertySearch = PropertySearchUtil.BuildPropertySearch(sps),
																	  }).ToList();

			foreach (var modelSavedSearch in model.SavedSearches)
				modelSavedSearch.FullSearchTitleParts = PropertySearchHelper.GetFullTextParts(modelSavedSearch.PropertySearch);

			var userEmailContactMethods = User.LoadVerifiedContactMethods(DbManager, ContactMethodType.Email);
			var userSmsContactMethods = User.LoadVerifiedContactMethods(DbManager, ContactMethodType.Phone);
			var notExpiredSearches = savedPropertySearches.Where(sps => !sps.SendNotificationsUntil.HasValue || sps.SendNotificationsUntil.Value < DateTime.Now).ToArray();

			if (notExpiredSearches.Any(sps => sps.SendNotificationEmails) && !userEmailContactMethods.Any())
				model.ShowNoEmailInProfileWarning = true;

			if (notExpiredSearches.Any(sps => sps.SendPromotionalSmsMessages || sps.SendPaidSmsMessages) && !userSmsContactMethods.Any())
				model.ShowNoPhoneNumberInProfileWarning = true;

			if (notExpiredSearches.Any(sps => sps.SendNotificationEmails && sps.EmailNotificationTarget == null) ||
				notExpiredSearches.Any(sps => (sps.SendPaidSmsMessages || sps.SendPromotionalSmsMessages) && sps.SmsNotificationTarget == null))
			{
				model.ShowSavedSearchWithoutContactWarning = true;
			}

			return View(model);
		}

		[HttpGet]
		[Authorize]
		public ActionResult SavedSearchDetails(long id)
		{
			var savedSearch = DbManager.Db.SavedPropertySearches.SingleOrDefault(sps => sps.ID == id);
			if (savedSearch == null)
				return Error(ErrorResult.EntityNotFound);

			if (savedSearch.UserID != User.CoreIdentity.UserId)
				return Error(ErrorResult.AccessDenied);

			var model = PropertiesSaveSearchModel.FromDomain(savedSearch);
			FillSavedPropertySearchItems(model);

			// Set DaysToKeepSendingNotificatons to null, so that the original value 
			// is not modified by default, unless the user explicitly changes it.
			model.DaysToKeepSendingNotificatons = null;

			return View(model);
		}

		[HttpPost]
		[Authorize]
		[ActionName("SavedSearchDetails")]
		public ActionResult SavedSearchDetailsPostback(long id)
		{
			var savedSearch = DbManager.Db.SavedPropertySearches.SingleOrDefault(sps => sps.ID == id);
			if (savedSearch == null)
				return Error(ErrorResult.EntityNotFound);

			if (savedSearch.UserID != User.CoreIdentity.UserId)
				return Error(ErrorResult.AccessDenied);

			var model = PropertiesSaveSearchModel.FromDomain(savedSearch);
			FillSavedPropertySearchItems(model);

			if (!TryUpdateModel(model))
				return View(model);

			model.UpdateDomain(savedSearch, VicinityCache);
			var validationResult = SavedSearchService.Update(savedSearch);

			if (!validationResult.IsValid)
			{
				foreach (var error in validationResult.Errors)
					ModelState.AddModelError("", PropertiesControllerResources.ResourceManager.GetString(error.FullResourceKey));

				return View(model);
			}

			return RedirectToAction("SavedSearches");
		}

		[HttpPost]
		[Authorize]
		public ActionResult DeleteSavedSearch(long id)
		{
			var savedSearch = DbManager.Db.SavedPropertySearches.SingleOrDefault(sps => sps.ID == id);
			if (savedSearch == null)
				return Error(ErrorResult.EntityNotFound);

			if (savedSearch.UserID != User.CoreIdentity.UserId)
				return Error(ErrorResult.AccessDenied);

			return PartialView(savedSearch);
		}

		public ActionResult DeleteSavedSearchConfirmed(long id)
		{
			var savedSearch = DbManager.Db.SavedPropertySearches.SingleOrDefault(sps => sps.ID == id);
			if (savedSearch == null)
				return Error(ErrorResult.EntityNotFound);

			if (savedSearch.UserID != User.CoreIdentity.UserId)
				return Error(ErrorResult.AccessDenied);

			SavedSearchService.DeletePropertySearch(id);
			return RedirectToAction("SavedSearches");
		}

		#endregion

		#region Private helper methods

		private PropertySearch BuildSeoCanonicalSearch(PropertySearch search)
		{
			var result = new PropertySearch
							 {
								 PropertyType = search.PropertyType,
								 IntentionOfOwner = search.IntentionOfOwner,
								 VicinityIDs = (search.VicinityIDs == null || search.VicinityIDs.Length < 1) ? null : new [] { search.VicinityIDs[0]}
							 };

			result.Query = PropertySearchQueryUtil.GenerateQuery(result);

			return result;
		}

		private void FillBreadcrumbItems(PropertiesBrowseModel model)
		{
			var breadcrumbItems = new List<PropertiesSearchMenuModel>();

			var tempSearch = new PropertySearch
								 {
									 SortOrder = model.Search.SortOrder,
									 PropertyType = model.Search.PropertyType,
									 IntentionOfOwner = model.Search.IntentionOfOwner,
									 VicinityIDs = model.Search.VicinityIDs
								 };

			if (PropertySearchQueryUtil.GenerateQuery(tempSearch) != PropertySearchQueryUtil.GenerateQuery(model.Search))
				breadcrumbItems.Add(new PropertiesSearchMenuModel {Label = PropertiesBrowseResources.OtherFilters, Query = model.Search.Query});

            if (tempSearch.VicinityIDs != null && tempSearch.VicinityIDs.Length > 0)
            {
                if (tempSearch.VicinityIDs.Length == 1 && VicinityCache[tempSearch.VicinityIDs[0]] != null)
                    breadcrumbItems.Add(new PropertiesSearchMenuModel { Label = VicinityCache[tempSearch.VicinityIDs[0]].Name, Query = PropertySearchQueryUtil.GenerateQuery(tempSearch) });
                else
                    breadcrumbItems.Add(new PropertiesSearchMenuModel
                    {
                        Label = string.Format(PropertySearchTitleResources.MultipleNeighborhoods, tempSearch.VicinityIDs.Length),
                        Query = PropertySearchQueryUtil.GenerateQuery(tempSearch)
                    });
            }


			tempSearch.IntentionOfOwner = null;
			if (tempSearch.PropertyType.HasValue)
				breadcrumbItems.Add(new PropertiesSearchMenuModel
										{
											Label = PropertiesBrowseResources.ResourceManager.GetString(tempSearch.PropertyType.Value.ToString()),
											Query = PropertySearchQueryUtil.GenerateQuery(tempSearch)
										});

			breadcrumbItems.Add(new PropertiesSearchMenuModel { Label = PropertiesBrowseResources.AllProperties, Query = ""});

			breadcrumbItems.Reverse();
			model.BreadcrumbItems = breadcrumbItems;
		}

		private void FillSortMenuItems(PropertiesBrowseModel model)
		{
			var sortMenuItems = new List<PropertiesSearchMenuModel>();

			var tempSearch = PropertySearch.Copy(model.Search);
			tempSearch.SortOrder = null;

			foreach (PropertySearchSortOrder order in Enum.GetValues(typeof (PropertySearchSortOrder)))
			{
				tempSearch.SortOrder = order;
				sortMenuItems.Add(new PropertiesSearchMenuModel { Label = order.ToString(), Query = PropertySearchQueryUtil.GenerateQuery(tempSearch) });
			}

			model.SortMenuItems = sortMenuItems;
		}

		private void FillPropertyTypeMenuItems(PropertiesBrowseModel model)
		{
			var propertyTypeMenuItems = new List<PropertiesSearchMenuModel>();

			var tempSearch = PropertySearch.Copy(model.Search);
			tempSearch.PropertyType = null;
			propertyTypeMenuItems.Add(new PropertiesSearchMenuModel { Label = "All", Query = PropertySearchQueryUtil.GenerateQuery(tempSearch) });
			foreach (PropertyType propertyType in Enum.GetValues(typeof (PropertyType)))
			{
				tempSearch.PropertyType = propertyType;
				propertyTypeMenuItems.Add(new PropertiesSearchMenuModel { Label = propertyType.ToString(), Query = PropertySearchQueryUtil.GenerateQuery(tempSearch) });
			}

			model.PropertyTypeMenuItems = propertyTypeMenuItems;
		}

		private void FillIntentionMenuItems(PropertiesBrowseModel model)
		{
			var intentionMenuItems = new List<PropertiesSearchMenuModel>();

			var tempSearch = PropertySearch.Copy(model.Search);
			tempSearch.IntentionOfOwner = null;
			intentionMenuItems.Add(new PropertiesSearchMenuModel { Label = "All", Query = PropertySearchQueryUtil.GenerateQuery(tempSearch) });
			foreach (IntentionOfOwner intention in Enum.GetValues(typeof (IntentionOfOwner)))
			{
				tempSearch.IntentionOfOwner = intention;
				intentionMenuItems.Add(new PropertiesSearchMenuModel { Label = intention.ToString(), Query = PropertySearchQueryUtil.GenerateQuery(tempSearch) });
			}

			model.IntentionMenuItems = intentionMenuItems;
		}

		private void FillSidebarSavedSearchItems(PropertiesBrowseModel model)
		{
			var savedPropertySearches = DbManager.Db.SavedPropertySearches
				.Where(sps => sps.UserID == User.CoreIdentity.UserId)
				.OrderByDescending(sps => sps.CreationTime)
				.Take(SidebarSavedSearchListSize)
				.ToList();

			model.SavedSearches = savedPropertySearches.Select(sps => new PropertiesBrowseModel.SavedPropertySearchModel
			{
				SavedPropertySearch = sps,
				PropertySearch = PropertySearchUtil.BuildPropertySearch(sps),
			}).ToList();
		}

		private void FillLocationMenuItems(PropertiesBrowseModel model)
		{
			var locationMenuItems = new List<PropertiesSearchMenuModel>();
			var tempSearch = PropertySearch.Copy(model.Search);

            if (tempSearch.VicinityIDs != null && tempSearch.VicinityIDs.Length > 1)
            {
                var vids = tempSearch.VicinityIDs;
                foreach (var vid in vids)
                {
                    if (locationMenuItems.Count >= 8)
                        break;

                    tempSearch.VicinityIDs = new[] { vid };
                    if (VicinityCache[vid] != null)
                        locationMenuItems.Add(new PropertiesSearchMenuModel { Label = PropertySearchHelper.BuildLocationText(tempSearch), Query = PropertySearchQueryUtil.GenerateQuery(tempSearch) });
                }
            }

			model.LocationMenuItems = locationMenuItems;
		}


		private ActionResult BrowseByIndex(int index, int count, string query, int delta)
		{
			var search = PropertySearchQueryUtil.ParseQuery(query);
			if (search == null)
				return Error(ErrorResult.EntityNotFound);

			index += delta;
			long? listingID = PropertyService.FindSearchIndex(search, index);
			if (!listingID.HasValue)
			{
				index -= delta;
				listingID = PropertyService.FindSearchIndex(search, index);
			}

			if (!listingID.HasValue)
				return Error(ErrorResult.EntityNotFound);

			return RedirectToAction("ViewDetails", "Property",
									new PropertyViewDetailsParamsModel
										{
											ID = listingID.Value,
											Origin = PropertyViewDetailsOrigin.Search,
											OriginQuery = PropertySearchQueryUtil.GenerateQuery(search),
											OriginIndex = index,
											OriginCount = count
										});
		}

		private void FillSavedPropertySearchItems(PropertiesSaveSearchModel model)
		{
			model.SelectedOptions = PropertySearchHelper.ExtractSelectedOptions(model.Search);
			model.UserEmailContactMethods = User.LoadVerifiedContactMethods(DbManager, ContactMethodType.Email);
			model.UserSmsContactMethods = User.LoadVerifiedContactMethods(DbManager, ContactMethodType.Phone);
		}

		#endregion
	}
}