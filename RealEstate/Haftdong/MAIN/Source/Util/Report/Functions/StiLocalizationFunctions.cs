using System;
using System.Linq;
using JahanJooy.Common.Util.Localization;
using JahanJooy.Common.Util.Text;
using JahanJooy.RealEstateAgency.Util.Resources;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Util.Report.Functions
{
    public static class StiLocalizationFunctions
    {
        public static string TranslateEnum(object value)
        {
            if (value == null)
                return string.Empty;

            var type = value.GetType();
            if (type.IsEnum)
                return EnumLocalizationUtils.ToLocalizedString(type, value, StaticEnumResources.ResourceManager);

            var stringValue = value as string;
            if (!stringValue.IsNullOrWhitespace())
            {
                if (stringValue != null)
                    return EnumLocalizationUtils.ToLocalizedString(stringValue.Replace('.', '_'), StaticEnumResources.ResourceManager);
            }

            return "N/A";
        }

        public static string TranslateEnum(string enumTypeName, object value)
        {
            if (value == null)
                return string.Empty;

            var stringValue = value as string;
            if (!stringValue.IsNullOrWhitespace())
                return EnumLocalizationUtils.ToLocalizedString(enumTypeName, stringValue, StaticEnumResources.ResourceManager);

            var enumType = ParseEnumTypeName(enumTypeName);
            if (enumType == null)
                return "Err";

            var type = value.GetType();
            if (type == enumType)
                return EnumLocalizationUtils.ToLocalizedString(type, value, StaticEnumResources.ResourceManager);

            if (type == typeof(byte) || type == typeof(sbyte) ||
                type == typeof(short) || type == typeof(ushort) ||
                type == typeof(int) || type == typeof(uint) ||
                type == typeof(long) || type == typeof(ulong))
            {
                var enumValue = Enum.ToObject(enumType, value);
                return EnumLocalizationUtils.ToLocalizedString(enumTypeName, enumValue.ToString(), StaticEnumResources.ResourceManager);
            }

            return "N/A";
        }

        private static Type ParseEnumTypeName(string enumTypeName)
        {
            return StaticEnumResourcesMetadata.TranslatedEnumTypes.FirstOrDefault(
                t => t.Name.EqualsIgnoreCase(enumTypeName));
        }
    }
}