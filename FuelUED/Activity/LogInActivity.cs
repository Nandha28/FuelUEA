using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FuelUED.Activity;
using FuelUED.CommonFunctions;

namespace FuelUED
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class LogInActivity : AppCompatActivity
    {      
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.LogIn);
            var email = FindViewById<EditText>(Resource.Id.txtEmail);
            var password = FindViewById<EditText>(Resource.Id.txtPassword);
            FindViewById<Button>(Resource.Id.btnLogin).Click += (s, e) =>
             {
                 if (email.Text.Equals(Utilities.ADMIN) && password.Text.Equals(Utilities.ADMIN))
                 {
                     StartActivity(typeof(MainScreenActivity));
                 }
                 else if (email.Text.Equals(Utilities.CONFIG) && password.Text.Equals(Utilities.ADMIN))
                 {
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