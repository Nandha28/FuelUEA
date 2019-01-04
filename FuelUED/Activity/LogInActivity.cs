using Android.App;
using Android.Net;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FuelUED.Activity;
using FuelUED.CommonFunctions;
using System;

namespace FuelUED
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, NoHistory = true)]
    public class LogInActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.LogIn);
            ExceptionLog.LogDetails(this, ConstantValues.NEWLINE + ConstantValues.NEWLINE + "App Log in at " + DateTime.Now);

            var ipAddress = AppPreferences.GetString(this, Utilities.IPAddress);
            var did = AppPreferences.GetString(this, Utilities.DEVICEID);
            var siteId = AppPreferences.GetString(this, Utilities.SITEID);            

            var email = FindViewById<EditText>(Resource.Id.txtEmail);
            var password = FindViewById<EditText>(Resource.Id.txtPassword);
            FindViewById<Button>(Resource.Id.btnLogin).Click += (s, e) =>
             {
                 if (email.Text.Equals(Utilities.ADMIN) && password.Text.Equals(Utilities.ADMIN))
                 {
                     if (ipAddress.Equals("") || did.Equals("") || siteId.Equals(""))
                     {
                         Toast.MakeText(this, "Please config device first", ToastLength.Short).Show();
                     }
                     else
                     {
                         ExceptionLog.LogDetails(this, "App entering History " + DateTime.Now);
                         StartActivity(typeof(HistoryActivity));
                     }
                 }
                 else if (email.Text.Equals(Utilities.CONFIG) && password.Text.Equals(Utilities.ADMIN))
                 {
                     ExceptionLog.LogDetails(this, "Appgone to Config " + DateTime.Now);
                     StartActivity(typeof(ConfigActivity));
                 }
                 else
                 {
                     Toast.MakeText(this, "Invalid Credentials", ToastLength.Short).Show();
                 }
             };
        }
    }
}