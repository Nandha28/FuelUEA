using Android.Content;
using Android.Net;
using Java.IO;
using System;

namespace FuelUED.CommonFunctions
{
    public class ExceptionLog
    {
        public static void LogDetails(Context context, string text)
        {
            File path = context.GetExternalFilesDir(null);
            File file = new File(path, "UEFuel.txt");
            if (!file.Exists())
            {
                try
                {
                    file.CreateNewFile();
                }
                catch { }
            }
            try
            {
                BufferedWriter buf = new BufferedWriter(new FileWriter(file, true));
                buf.Append(text);
                buf.NewLine();
                buf.Close();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
        public static bool IsNetworkConected(Context context)
        {
            var cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);

            NetworkInfo activeNetwork = cm.ActiveNetworkInfo;
            var isConnected = activeNetwork != null && activeNetwork.IsConnectedOrConnecting;
            return isConnected;
        }
    }
}