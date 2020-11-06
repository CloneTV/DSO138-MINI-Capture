namespace DSO138_Capture
{
    public static class SettingsHelper
    {
        public static void Set<T>(string key, T val)
        {
            var ls = Windows.Storage.ApplicationData.Current.LocalSettings;
            ls.Values[key] = val;
        }
        public static T Get<T>(string key) where T : class
        {
            var ls = Windows.Storage.ApplicationData.Current.LocalSettings;
            return ls.Values[key] as T;
        }
        public static bool Found(string key)
        {
            var ls = Windows.Storage.ApplicationData.Current.LocalSettings;
            return (ls.Values[key] != null);
        }
    }
}
