using System;

namespace JahanJooy.Common.Util.Configuration
{
    public static class ApplicationSettingsExtensions
    {
        public static int? GetInt(this IApplicationSettings settings, string key)
        {
            var value = settings[key];
            if (string.IsNullOrWhiteSpace(value))
                return null;

            int result;
            return int.TryParse(value, out result) ? (int?)result : null;
        }

        public static long? GetLong(this IApplicationSettings settings, string key)
        {
            var value = settings[key];
            if (string.IsNullOrWhiteSpace(value))
                return null;

            long result;
            return long.TryParse(value, out result) ? (long?)result : null;
        }

        public static bool? GetBool(this IApplicationSettings settings, string key)
        {
            var value = settings[key];
            if (string.IsNullOrWhiteSpace(value))
                return null;

            bool result;
            return bool.TryParse(value, out result) ? (bool?)result : null;
        }
    }
}