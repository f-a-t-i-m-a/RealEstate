using System.Collections.Generic;

namespace JahanJooy.Common.Util.Configuration
{
    public static class ApplicationSettingKeys
    {
        private static readonly List<string> Keys = new List<string>();

        public static void RegisterKey(string key)
        {
            if (!Keys.Contains(key))
                Keys.Add(key);
        }

        public static string[] RegisteredKeys
        {
            get
            {
                return Keys.ToArray();
            }
        }
    }
}