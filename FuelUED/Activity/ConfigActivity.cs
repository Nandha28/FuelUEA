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
            var txtDeviceID = FindViewById<EditText>(Resource.Id.txtDeviceId);
            var txtSiteID = FindViewById<EditText>(Resource.Id.txtSiteId);

            //txtIpAddress.TextChanged += (s, e) =>
            //{
            //};
            FindViewById<Button>(Resource.Id.btnConfig).Click += (s, e) =>
            {
                if (txtIpAddress.Text.Equals(string.Empty) || txtDeviceID.Text.Equals(string.Empty) || txtSiteID.Text.Equals(string.Empty))
                {
                    Toast.MakeText(this, "Enter all fields..", ToastLength.Short).Show();
                    return;
                }
                Toast.MakeText(this, "Success", ToastLength.Short).Show();
                AppPreferences.SaveString(this, Utilities.IPAddress, txtIpAddress.Text.Replace(" ", ""));
                AppPreferences.SaveString(this, Utilities.DEVICEID, txtDeviceID.Text.Replace(" ", ""));
                AppPreferences.SaveString(this, Utilities.SITEID, txtSiteID.Text.Replace(" ", ""));
                StartActivity(typeof(LogInActivity));
            };
        }
    }
}