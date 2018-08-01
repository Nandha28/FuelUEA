using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using UECrusher.Activity;

namespace UECrusher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
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
                     StartActivity(typeof(VehicleDetailActivity));
                 }
                 else if (email.Text.Equals(Utilities.CONFIG) && password.Text.Equals(Utilities.ADMIN))
                 {
                     StartActivity(typeof(ConfigActivity));
                 }
                 else if (email.Text.Equals(Utilities.DELIVERY) && password.Text.Equals(Utilities.ADMIN))
                 {
                     StartActivity(typeof(DeliveryActivity));
                 }
                 else
                 {
                     Toast.MakeText(this, "Invalid Credentials", ToastLength.Short).Show();
                 }
             };
        }
    }
}