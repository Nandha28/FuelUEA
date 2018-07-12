using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

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
                 if (email.Text == "admin" && password.Text == "admin")
                 {
                     StartActivity(typeof(MainScreenActivity));
                 }
                 else
                 {
                     Toast.MakeText(this,"Invalid Credentials", ToastLength.Short).Show();
                 }
             };
        }
    }
}