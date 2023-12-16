using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Permissions;
using JahanJooy.RealEstate.Web.Application.Authorization;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminConfiguration;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
    [AuthorizeGeneralPermission(GeneralPermission.ConfigureServer)]
    public class AdminConfigurationController : AdminControllerBase
    {
        [ComponentPlug]
        public IConfigurationService ConfigurationService { get; set; }

        [HttpGet]
        public ActionResult ViewAll()
        {
            // Load all configuration from the database
            var config = ConfigurationService.LoadAllItems().ToDictionary(i => i.Identifier, i => i.Value);

            // Add any item that is registered but not yet configured
            foreach (var key in ApplicationSettingKeys.RegisteredKeys)
                if (!config.ContainsKey(key))
                    config.Add(key, null);

            var model = new AdminConfigurationViewAllModel
            {
                Items = config
                    .Select(kvp => new AdminConfigurationItemModel {Key = kvp.Key, Value = kvp.Value})
                    .OrderBy(item => item.Key).ToList()
            };

            return View(model);
        }

        public ActionResult EditConfiguration(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return PartialView("_EmptyPartial");

            var item = ConfigurationService.LoadItem(key);
            var model = new AdminConfigurationItemModel {Key = key, Value = item.IfNotNull(i => i.Value)};

            return PartialView(model);
        }

        public ActionResult EditConfigurationConfirmed(AdminConfigurationItemModel model)
        {
            if (!ModelState.IsValid || model == null || string.IsNullOrWhiteSpace(model.Key))
                return PartialView("EditConfiguration", model);

            ConfigurationService.UpdateItem(model.Key, model.Value);

            return RedirectToAction("ViewAll");
        }

        public ActionResult DeleteConfiguration(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return PartialView("_EmptyPartial");

            var item = ConfigurationService.LoadItem(key);
            var model = new AdminConfigurationItemModel {Key = key, Value = item.IfNotNull(i => i.Value)};

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult DeleteConfigurationConfirmed(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return RedirectToAction("ViewAll");

            ConfigurationService.UpdateItem(key, null);

            return RedirectToAction("ViewAll");
        }
    }
}