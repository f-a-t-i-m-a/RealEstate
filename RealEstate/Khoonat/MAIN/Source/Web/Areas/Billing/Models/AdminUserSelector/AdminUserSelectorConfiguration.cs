namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.AdminUserSelector
{
    public class AdminUserSelectorConfiguration
    {
        public string Name { get; set; }
        public int MaxNumberOfSelections { get; set; }

        public string Area { get { return AreaNames.Billing; } }
    }
}