using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using UECrusher.Activity;
using UECrusher.CommonFunctions;

namespace UECrusher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar",
       MainLauncher = true, NoHistory = true)]
    public class LogInActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.LogIn);
            ExceptionLog.LogDetails(this, "Login Activity...");

            AppDomain.CurrentDomain.UnhandledException += (s, e) => 
            {
                Toast.MakeText(this, "Something went wrong", ToastLength.Short).Show();
            };

            TaskScheduler.UnobservedTaskException += (s, e) => 
            {
                Toast.MakeText(this,"Something went wrong", ToastLength.Short).Show();
            };

            var email = FindViewById<EditText>(Resource.Id.txtEmail);
            var password = FindViewById<EditText>(Resource.Id.txtPassword);

            var ipAddress = AppPreferences.GetString(this, Utilities.IPAddress);
            var did = AppPreferences.GetString(this, Utilities.DEVICEID);
            var siteId = AppPreferences.GetString(this, Utilities.SITEID);



            FindViewById<Button>(Resource.Id.btnLogin).Click += (s, e) =>
             {
                 if (email.Text.Equals(Utilities.ADMIN) && password.Text.Equals(Utilities.ADMIN))
                 {
                     if (ipAddress.Equals("") || did.Equals("") || siteId.Equals(""))
                     {
                         ShowText("Please config device first");
                     }
                     else
                     {
                         StartActivity(typeof(VehicleDetailActivity));
                     }
                 }
                 else if (email.Text.Equals(Utilities.CONFIG) && password.Text.Equals(Utilities.ADMIN))
                 {
                     StartActivity(typeof(ConfigActivity));
                 }
                 else if (email.Text.Equals(Utilities.DELIVERY) && password.Text.Equals(Utilities.ADMIN))
                 {
                     if (ipAddress.Equals("") || did.Equals("") || siteId.Equals(""))
                     {
                         ShowText("Please config device first");
                     }
                     else
                     {
                         StartActivity(typeof(DeliveryActivity));
                     }
                 }
                 else
                 {
                     ShowText("Invalid Credentials");
                 }
             };
        }

        private void ShowText(string text)
        {
            Toast.MakeText(this, text, ToastLength.Short).Show();
        }
    }
}