using System;
using System.Reflection;

namespace JahanJooy.RealEstateAgency.HaftDong.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}