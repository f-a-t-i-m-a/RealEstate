using System.Collections.Generic;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Util.Resources;

namespace JahanJooy.RealEstate.Util.DomainUtil
{
    public static class VicinityExtensions
    {
        public static string GetDisplayName(this Vicinity vicinity)
        {
            return (vicinity.ShowTypeInTitle ? vicinity.Type.Label(DomainEnumResources.ResourceManager) + ' ' : "") +
                   vicinity.Name;
        }

        public static string GetDisplayFullName(this Vicinity vicinity)
        {
            return
                vicinity.GetDisplayName() +
                (string.IsNullOrWhiteSpace(vicinity.AlternativeNames)
                    ? ""
                    : ' ' + string.Format(GeneralResources.Paranthesis, vicinity.AlternativeNames));
        }
    }
}
