using System;
// ReSharper disable PossibleNullReferenceException

namespace JahanJooy.RealEstateAgency.Util.Resources
{
    public static class EnumTranslatorUtil
    {
        public static byte GetByteOfEnum(Type enumType, string enumTranslate)
        {
            var vals = Enum.GetValues(enumType);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var val in vals)
            {
                if (StaticEnumResources.ResourceManager.GetString(enumType.Name + "_" + val)
                    .Equals(enumTranslate))
                    return (byte) val;
            }

            return 0;
        }
    }
}