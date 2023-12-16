using System;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;

namespace JahanJooy.RealEstateAgency.Util.Owin
{
    // ReSharper disable once InconsistentNaming
    public static class JJOwinRequestScopeContextExtensions
    {
        public static bool IsAdministrator()
        {
            try
            {
                return OwinRequestScopeContext.Current.GetUser().Identity.Name.Equals("administrator");
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
