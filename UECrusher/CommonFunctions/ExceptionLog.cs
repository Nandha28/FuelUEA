using Android.Content;
using Android.OS;
using Java.IO;
using System;

namespace UECrusher.CommonFunctions
{
    public class ExceptionLog
    {
        public static void LogDetails(Context context, string text)
        {
            File path = context.GetExternalFilesDir(null);
            File file = new File(path, "UECrusher.txt");
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
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

            //int length = (int)file.Length();
            //byte[] bytes = new byte[length];
            //if (file.Exists())
            //{
            //    FileInputStream input = new FileInputStream(file);
            //    try
            //    {
            //        input.Read(bytes);
            //    }
            //    finally
            //    {
            //        input.Close();
            //    }
            //}

            //String actual = Encoding.ASCII.GetString(bytes);

            //FileOutputStream stream = new FileOutputStream(file);
            //try
            //{
            //    stream.Write(Encoding.ASCII.GetBytes(actual + "\n" + message));
            //}
            //finally
            //{
            //    stream.Close();
            //}
        }
    }
}