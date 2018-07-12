using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FuelApp.Modal;
using FuelUED.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FuelUED
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class MainScreenActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainScreen);

            FindViewById<Button>(Resource.Id.btnFuelEntry).Click += (s, e) =>
             {
                 StartActivity(typeof(FuelActivity));
             };
            var syncButton = FindViewById<Button>(Resource.Id.btnSync);
            syncButton.Click += (s, e) =>
             {
                 Toast.MakeText(this, "Please wait..", ToastLength.Short).Show();
                 syncButton.Alpha = 0f;
                 syncButton.Clickable = false;
                 var resposeString = WebService.GetDataFromWebService("LoadVD");
                 var VehicleList = JsonConvert.DeserializeObject<List<VehicleDetails>>(resposeString);
                 //if (FuelDB.Singleton.DBExists())
                 //{
                 //    //DeleteDatabase(FuelDB.Singleton.DBPath);                        
                 //    FuelDB.Singleton.DeleteTable<>();
                 //}
                 FuelDB.Singleton.CreateDatabase<VehicleDetails>();
                 //var vehicleList = new VehicleDetails
                 //{
                 //    DriverName = driverNameSpinner.SelectedItem.ToString(),
                 //    TypeName = vehicleTypeSpinner.SelectedItem.ToString(),
                 //    RegNo = vehicleNumber.Text,
                 //};
                 FuelDB.Singleton.InsertValues(VehicleList);
                 syncButton.Clickable = true;
                 syncButton.Alpha = 1f;
                 Toast.MakeText(this,"Stored in Database", ToastLength.Short).Show();
             };
        }
    }
}