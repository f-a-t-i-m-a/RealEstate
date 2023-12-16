using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Web.Application.Authorization;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Billing.Models.AdminUserSelector;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Controllers
{
    [OperatorOnly]
    [RequireHttps]
    public class AdminUserSelectorController : CustomControllerBase
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [HttpGet]
        public ActionResult Search(string query, int? page, int? pageLength)
        {
            long numericInput;
            var hasNumericInput = long.TryParse(query, out numericInput);

            // Calculate pagination

            if (!pageLength.HasValue || pageLength.Value < 5)
                pageLength = 20;

            var skipCount = 0;
            var takeCount = pageLength.Value;

            if (page.HasValue && page.Value > 1)
                skipCount = (page.Value - 1)*pageLength.Value;

            var items = new List<AdminUserSelectorResultItemModel>();

            if (hasNumericInput)
            {
                // If the query is a number, include ID and Code matches (if any) at the beginning of the results

                var numericItems = AdminUserSelectorResultItemModel.Map(DbManager.Db.Users.Where(u => u.ID == numericInput || u.Code == numericInput).OrderByDescending(u => u.ID)).ToList();

                if (skipCount < numericItems.Count)
                {
                    items.AddRange(numericItems);
                    takeCount -= numericItems.Count;
                }
                else
                {
                    skipCount -= numericItems.Count;
                }
            }

            // If the query is a number, match partial ID and Code values too

            IQueryable<User> dbQuery = hasNumericInput
                ? DbManager.Db.Users.Where(
                    u => u.ID != numericInput && u.Code != numericInput && (
                        SqlFunctions.StringConvert((double?) u.ID).Contains(query) ||
                        SqlFunctions.StringConvert((double?) u.Code).Contains(query) ||
                        u.LoginName.Contains(query) ||
                        u.DisplayName.Contains(query) ||
                        u.FullName.Contains(query)))
                : DbManager.Db.Users.Where(
                    u => u.LoginName.Contains(query) ||
                         u.DisplayName.Contains(query) ||
                         u.FullName.Contains(query));

            dbQuery = dbQuery.OrderByDescending(u => u.LastLogin);

            if (skipCount > 0)
                dbQuery = dbQuery.Skip(skipCount);

            // Query for one more item than required, to see if there is more for the next page

            items.AddRange(AdminUserSelectorResultItemModel.Map(dbQuery.Take(takeCount + 1)));
            var result = new AdminUserSelectorResultModel {Items = items, More = false};

            if (items.Count > pageLength.Value)
            {
                // If there's more, remove the last (additionally queried) item

                result.More = true;
                result.Items.RemoveAt(result.Items.Count - 1);
            }

            return Json(result);
        }
    }
}