namespace JahanJooy.RealEstate.Web.Areas
{
    public static class AreaRouteValue
    {
        private class AreaParam
        {
            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Area { get; private set; }

            public AreaParam(string area)
            {
                Area = area;
            }
        }

        public static readonly object Main =  new AreaParam(AreaNames.Main);
        public static readonly object Admin = new AreaParam(AreaNames.Admin);

        public static readonly object Ad = new AreaParam(AreaNames.Ad);
        public static readonly object Billing = new AreaParam(AreaNames.Billing);
        
        public static readonly object Cms = new AreaParam(AreaNames.Cms);
    }
}