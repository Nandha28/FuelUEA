using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FuelApp.Modal;
using FuelUED.Modal;
using FuelUED.Service;
using Newtonsoft.Json;
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainScreen);

            mainLayout = FindViewById<LinearLayout>(Resource.Id.layHolder);
            loader = FindViewById<ProgressBar>(Resource.Id.loader);
            mainHolder = FindViewById<RelativeLayout>(Resource.Id.mainRelativeHolder);



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

        private async Task SyncButton_Click()
        {
            RunOnUiThread(() =>
            {
               //    //Toast.MakeText(this, "Please wait..", ToastLength.Short).Show();
               //    mainHolder.Alpha = 0.5f;
               //    loader.Visibility = Android.Views.ViewStates.Visible;
               //    syncButton.Clickable = false;

               //    pd.Show();
               pd = new ProgressDialog(this);
                pd.SetMessage("loading");
                pd.SetCanceledOnTouchOutside(false);
                pd.SetCancelable(false);
                pd.Show();
            });

            var thread = new Thread(new ThreadStart(delegate
            {
                var resposeString = WebService.GetDataFromWebService("LoadVD");
                var fuelStockRes = WebService.GetDataFromWebService("LoadFStock");
                var VehicleList = JsonConvert.DeserializeObject<List<VehicleDetails>>(resposeString);
                var fuelStoc = JsonConvert.DeserializeObject<List<Fuel>>(fuelStockRes);
                System.Console.WriteLine(fuelStoc);
                //if (FuelDB.Singleton.DBExists())
                //{
                //    //DeleteDatabase(FuelDB.Singleton.DBPath);                        
                //    FuelDB.Singleton.DeleteTable<>();
                //}
                FuelDB.Singleton.CreateDatabase<VehicleDetails>();
                FuelDB.Singleton.CreateDatabase<Fuel>();

                //var vehicleList = new VehicleDetails
                //{
                //    DriverName = driverNameSpinner.SelectedItem.ToString(),
                //    TypeName = vehicleTypeSpinner.SelectedItem.ToString(),
                //    RegNo = vehicleNumber.Text,
                //};
                FuelDB.Singleton.InsertValues(VehicleList);
                FuelDB.Singleton.InsertFuelValues(fuelStoc);
                RunOnUiThread(() =>
                {
                    pd.Hide();
                    syncButton.Clickable = true;
                    mainHolder.Alpha = 1f;
                });
                //Toast.MakeText(this, "Stored in Database", ToastLength.Short).Show();
            }));
            thread.Start();

            // loader.Visibility = Android.Views.ViewStates.Visible;
            // loader.Visibility = Android.Views.ViewStates.Gone;
        }
    }
}