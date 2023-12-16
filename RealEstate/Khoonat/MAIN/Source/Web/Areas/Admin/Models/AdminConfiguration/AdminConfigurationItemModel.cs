using System.Web.Mvc;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminConfiguration
{
    public class AdminConfigurationItemModel
    {
        public string Key { get; set; }

		[AllowHtml]
        public string Value { get; set; }
    }
}