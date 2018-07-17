using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Widget;
using FuelApp.Modal;
using FuelUED.Activity;
using FuelUED.CommonFunctions;
using FuelUED.Modal;
using FuelUED.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FuelUED
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class MainScreenActivity : AppCompatActivity
    {
        private LinearLayout mainLayout;
        private ProgressBar loader;
        private RelativeLayout mainHolder;
        private ProgressDialog pd;
        private Button syncButton;

        public string Ipadress { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainScreen);

            mainLayout = FindViewById<LinearLayout>(Resource.Id.layHolder);
            loader = FindViewById<ProgressBar>(Resource.Id.loader);
            mainHolder = FindViewById<RelativeLayout>(Resource.Id.mainRelativeHolder);

            //var pref = PreferenceManager.GetDefaultSharedPreferences(this);
            //Ipadress = pref.GetString(Utilities.IPAddress, string.Empty);

            //Get IPAdress for preference
            Ipadress = AppPreferences.GetString(this, Utilities.IPAddress);

            FindViewById<Button>(Resource.Id.btnFuelEntry).Click += (s, e) =>
             {
                 StartActivity(typeof(FuelActivity));
                 // SyncButton_Click();
             };

            syncButton = FindViewById<Button>(Resource.Id.btnSync);
            syncButton.Click += (s, e) =>
            {
                SyncButton_Click();
            };
        }

        private void SyncButton_Click()
        {
            if (Ipadress.Equals(string.Empty))
            {
                Toast.MakeText(this, "Please Configure IPAdress..", ToastLength.Short).Show();
                //StartActivity(typeof(ConfigActivity));
                return;
            }
            WebService.IPADDRESS = Ipadress;
            RunOnUiThread(() =>
            {
                //    //Toast.MakeText(this, "Please wait..", ToastLength.Short).Show();
                mainHolder.Alpha = 0.5f;
                loader.Visibility = Android.Views.ViewStates.Visible;
                Window.SetFlags(Android.Views.WindowManagerFlags.NotTouchable, Android.Views.WindowManagerFlags.NotTouchable);

                //    syncButton.Clickable = false;

                //    pd.Show();
                //pd = new ProgressDialog(this);
                //pd.SetMessage("loading");
                //pd.SetCanceledOnTouchOutside(false);
                //pd.SetCancelable(false);
                //pd.Show();
            });

            var thread = new Thread(new ThreadStart(delegate
            {
                var resposeString = WebService.GetDataFromWebService("LoadVD");
                var fuelStockRes = WebService.GetDataFromWebService("LoadFStock");
                try
                {
                    var VehicleList = JsonConvert.DeserializeObject<List<VehicleDetails>>(resposeString);
                    var fuelStoc = JsonConvert.DeserializeObject<List<Fuel>>(fuelStockRes);
                    Console.WriteLine(fuelStoc);

                    FuelDB.Singleton.CreateDatabase<VehicleDetails>();
                    FuelDB.Singleton.CreateDatabase<Fuel>();

                    FuelDB.Singleton.InsertValues(VehicleList);
                    FuelDB.Singleton.InsertFuelValues(fuelStoc);
                }
                catch (Exception ec)
                {
                    Console.WriteLine(ec.Message);
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Something wrong ...Check connectivity..", ToastLength.Short).Show();
                    });
                }
                RunOnUiThread(() =>
                {
                    loader.Visibility = Android.Views.ViewStates.Gone;
                    //pd.Hide();
                    //syncButton.Clickable = true;
                    mainHolder.Alpha = 1f;
                    Window.ClearFlags(Android.Views.WindowManagerFlags.NotTouchable);
                });
                //Toast.MakeText(this, "Stored in Database", ToastLength.Short).Show();
            }));
            thread.Start();
            // loader.Visibility = Android.Views.ViewStates.Visible;
            // loader.Visibility = Android.Views.ViewStates.Gone;
        }
    }
}