using Android.App;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Widget;
using FuelUED.CommonFunctions;
using FuelUED.Service;

namespace FuelUED.Activity
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false, NoHistory = true)]
    public class ConfigActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.config);

            var txtIpAddress = FindViewById<EditText>(Resource.Id.txtIP);
            //txtIpAddress.TextChanged += (s, e) =>
            //{
            //};
            FindViewById<Button>(Resource.Id.btnConfig).Click += (s, e) =>
            {
                if (txtIpAddress.Text.Equals(string.Empty))
                {
                    Toast.MakeText(this, "Enter valid IPAdress..", ToastLength.Short).Show();
                    return;
                }
                Toast.MakeText(this, "Success", ToastLength.Short).Show();         
                AppPreferences.SaveString(this, Utilities.IPAddress, txtIpAddress.Text.Replace(" ", ""));
                StartActivity(typeof(LogInActivity));
            };
        }
    }
}