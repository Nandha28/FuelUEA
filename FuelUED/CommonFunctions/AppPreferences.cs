using Android.Content;
using Android.Preferences;

namespace FuelUED.CommonFunctions
{
    public static class AppPreferences
    {
        public static void SaveString(Context context, string key, string value)
        {
            var preference = PreferenceManager.GetDefaultSharedPreferences(context);
            preference.Edit().PutString(key, value).Apply();
        }
        public static void SaveInt(Context context, string key, int value)
        {
            var preference = PreferenceManager.GetDefaultSharedPreferences(context);
            preference.Edit().PutInt(key, value).Apply();
        }
        public static string GetString(Context context, string key)
        {
            var preference = PreferenceManager.GetDefaultSharedPreferences(context);
            return preference.GetString(key, string.Empty);
        }
        public static int GetInt(Context context, string key)
        {
            var preference = PreferenceManager.GetDefaultSharedPreferences(context);
            return preference.GetInt(key,0);
        }
    }
}