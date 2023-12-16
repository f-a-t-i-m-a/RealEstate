using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.SavedSearch;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminSavedPropertySearches;
using JahanJooy.RealEstate.Web.Helpers.Properties;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
	public class AdminSavedPropertySearchesController : AdminControllerBase
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public ISavedSearchService SavedSearchService { get; set; }

		[ComponentPlug]
		public PropertySearchHelper PropertySearchHelper { get; set; }

		public static string DefaultList(UrlHelper url)
		{
			return url.Action("List", "AdminSavedPropertySearches", new AdminSavedPropertySearchesListModel { DeletedFilter = false });
		}

		[HttpGet]
		public ActionResult List(AdminSavedPropertySearchesListModel model)
		{
			if (model == null)
				model = new AdminSavedPropertySearchesListModel();

			int pageNum = 1;
			if (!string.IsNullOrWhiteSpace(model.Page))
				int.TryParse(model.Page, out pageNum);

			// Use DbSet to include deleted items for admin access
			IQueryable<SavedPropertySearch> searchesQuery = DbManager.Db.SavedPropertySearchesDbSet.Include(sps => sps.EmailNotificationTarget).Include(sps => sps.SmsNotificationTarget);

			searchesQuery = ApplyFilterQuery(model, searchesQuery);

			model.SavedSearchInfos = PagedList<AdminSavedPropertySearchesListModel.SavedPropertySearchAdminInfo>.BuildUsingPageNumber(searchesQuery.Count(), 20, pageNum);
			model.SavedSearchInfos.FillFrom(searchesQuery.OrderByDescending(s => s.CreationTime).ToList()
				.Select(s => new AdminSavedPropertySearchesListModel.SavedPropertySearchAdminInfo
				             {
					             SavedSearch = s,
								 CriteriaTextParts = PropertySearchHelper.GetFullTextParts(PropertySearchUtil.BuildPropertySearch(s)),
								 HasNotificationTargetError = PropertySavedSearchHelper.NotificationErrorDelegate(s)
				             }));

			return View(model);
		}

		[HttpGet]
		public ActionResult Details(long? id)
		{
			if (!id.HasValue)
				return Error(ErrorResult.EntityNotFound);

			// Use DbSet to include deleted items for admin access
			var savedSearch = DbManager.Db.SavedPropertySearchesDbSet
				.Include(sps => sps.EmailNotificationTarget)
				.Include(sps => sps.SmsNotificationTarget)
				.Include(sps => sps.User)
				.Include(sps => sps.GeographicRegions)
				.SingleOrDefault(sps => sps.ID == id.Value);

			if (savedSearch == null)
				return Error(ErrorResult.EntityNotFound);

			var model = new AdminSavedPropertySearchesDetailsModel
			            {
				            SavedSearch = savedSearch,
				            CriteriaTextParts = PropertySearchHelper.GetFullTextParts(PropertySearchUtil.BuildPropertySearch(savedSearch)),
				            HasNotificationTargetError = PropertySavedSearchHelper.NotificationErrorDelegate(savedSearch)
			            };

			return View(model);
		}

		private static IQueryable<SavedPropertySearch> ApplyFilterQuery(AdminSavedPropertySearchesListModel model, IQueryable<SavedPropertySearch> query)
		{
			if (model.IdFilter.HasValue)
				query = query.Where(s => s.ID == model.IdFilter.Value);

			if (model.UserIdFilter.HasValue)
				query = query.Where(s => s.UserID == model.UserIdFilter.Value);

			if (model.CreatorSessionIdFilter.HasValue)
				query = query.Where(s => s.CreatorSessionID == model.CreatorSessionIdFilter.Value);

			if (model.DeletedFilter.HasValue)
				query = model.DeletedFilter.Value ? query.Where(s => s.DeleteTime.HasValue) : query.Where(s => !s.DeleteTime.HasValue);

			if (model.NotificationFilter.HasValue)
			{
				if (model.NotificationFilter.Value == SavedPropertySearchNotificationFilterType.None)
					query = query.Where(s => !s.SendNotificationEmails && !s.SendPaidSmsMessages && !s.SendPromotionalSmsMessages);
				else if (model.NotificationFilter.Value == SavedPropertySearchNotificationFilterType.Email)
					query = query.Where(s => s.SendNotificationEmails);
				else if (model.NotificationFilter.Value == SavedPropertySearchNotificationFilterType.PromotionalSms)
					query = query.Where(s => s.SendPromotionalSmsMessages);
				else if (model.NotificationFilter.Value == SavedPropertySearchNotificationFilterType.PaidSms)
					query = query.Where(s => s.SendPaidSmsMessages);
				else if (model.NotificationFilter.Value == SavedPropertySearchNotificationFilterType.AnySms)
					query = query.Where(s => s.SendPromotionalSmsMessages || s.SendPaidSmsMessages);
				else if (model.NotificationFilter.Value == SavedPropertySearchNotificationFilterType.Any)
					query = query.Where(s => s.SendNotificationEmails || s.SendPaidSmsMessages || s.SendPromotionalSmsMessages);
				else if (model.NotificationFilter.Value == SavedPropertySearchNotificationFilterType.All)
					query = query.Where(s => s.SendNotificationEmails && s.SendPaidSmsMessages && s.SendPromotionalSmsMessages);
			}

			if (model.NotificationTargetErrorFilter)
				query = query.Where(PropertySavedSearchHelper.NotificationErrorExpression);

			return query;
		}
	}
}