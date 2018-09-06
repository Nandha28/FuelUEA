using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FuelApp.Modal;
using FuelUED.CommonFunctions;
using FuelUED.Modal;
using FuelUED.Service;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FuelUED
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class MainScreenActivity : AppCompatActivity
    {
        private LinearLayout mainLayout;
        private ProgressBar loader;
        private RelativeLayout mainHolder;
        private Button btnDownloadData;

        public string IpAddress { get; private set; }
        public bool IsDeviceAvailable { get; private set; }

        private string siteId;
        private string deviceId;
        private Button btnUploadData;
        private TableQuery<FuelEntryDetails> fuelDetails;
        private bool IsExitApp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainScreen);

            mainLayout = FindViewById<LinearLayout>(Resource.Id.layHolder);
            loader = FindViewById<ProgressBar>(Resource.Id.loader);
            mainHolder = FindViewById<RelativeLayout>(Resource.Id.mainRelativeHolder);
            var alertDialog = new Android.App.AlertDialog.Builder(this);
            FindViewById<ImageButton>(Resource.Id.btnLogout).Click += (s, e) =>
            {
                alertDialog.SetTitle("Logout");
                alertDialog.SetMessage("Do you want to logout ?");
                alertDialog.SetPositiveButton("Yes", (se, ee) =>
                {
                    StartActivity(typeof(LogInActivity));
                    Finish();
                });
                alertDialog.SetNegativeButton("No", (se, ee) => { });
                alertDialog.Show();
            };            

            //Get IPAdress for preference
            IpAddress = AppPreferences.GetString(this, Utilities.IPAddress);
            siteId = AppPreferences.GetString(this, Utilities.SITEID);
            deviceId = AppPreferences.GetString(this, Utilities.DEVICEID);

            WebService.IPADDRESS = IpAddress;
            
            FindViewById<Button>(Resource.Id.btnFuelEntry).Click += (s, e) =>
             {
                 try
                 {
                     if (FuelDB.Singleton.DBExists() && FuelDB.Singleton.GetBillDetails() != null)
                     {
                         if (AppPreferences.GetString(this, Utilities.DEVICESTATUS).Equals("1"))
                         {
                             Toast.MakeText(this, "Please wait...", ToastLength.Short).Show();
                             StartActivity(typeof(FuelActivity));
                         }
                         else
                         {
                             Toast.MakeText(this, "Device not avalable", ToastLength.Short).Show();
                         }
                     }
                     else
                     {
                         var alertDialog1 = new Android.App.AlertDialog.Builder(this);
                         alertDialog1.SetTitle("you need to sync first");
                         alertDialog1.SetPositiveButton("OK", (ss, se) => { });
                         alertDialog1.Show();
                     }
                 }
                 catch
                 {
                     Toast.MakeText(this, "Something went wrong", ToastLength.Short).Show();
                 }
             };

            btnDownloadData = FindViewById<Button>(Resource.Id.btnDownloadData);
            btnDownloadData.Click += (s, e) =>
            {
                if (!AppPreferences.GetBool(this, Utilities.IsDownloaded))
                {
                    SyncButton_Click();
                }
                else
                {
                    Toast.MakeText(this, "Please Upload Data and Try again..", ToastLength.Short).Show();
                }
            };

            btnUploadData = FindViewById<Button>(Resource.Id.btnUploadData);
            btnUploadData.Click += (s, e) =>
            {
                UploadDetailsToServer();
            };
        }

        private void UploadDetailsToServer()
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, "Please wait..", ToastLength.Short).Show();
                mainHolder.Alpha = 0.5f;
                loader.Visibility = Android.Views.ViewStates.Visible;
                Window.SetFlags(Android.Views.WindowManagerFlags.NotTouchable, Android.Views.WindowManagerFlags.NotTouchable);
            });
            var thread = new Thread(new ThreadStart(delegate
            {
                try
                {
                    fuelDetails = FuelDB.Singleton.GetFuelValues();

                    if (fuelDetails != null && fuelDetails?.Count() > 0)
                    {
                        btnUploadData.Clickable = true;
                        btnDownloadData.Clickable = false;
                        AppPreferences.SaveBool(this, Utilities.IsDownloaded, true);
                        var result = UploadValues();
                        if (result)
                        {
                            RunOnUiThread(() =>
                            {
                                Toast.MakeText(this, "Upload Success", ToastLength.Short).Show();
                            });
                            btnDownloadData.Clickable = true;
                            AppPreferences.SaveBool(this, Utilities.IsDownloaded, false);
                        }
                        else
                        {
                            RunOnUiThread(() =>
                            {
                                Toast.MakeText(this, "Upload Failed", ToastLength.Short).Show();
                            });
                        }
                        RunOnUiThread(() =>
                        {
                            loader.Visibility = Android.Views.ViewStates.Gone;
                            mainHolder.Alpha = 1f;
                            Window.ClearFlags(Android.Views.WindowManagerFlags.NotTouchable);
                        });
                    }
                    else
                    {
                        RunOnUiThread(() =>
                        {
                            loader.Visibility = Android.Views.ViewStates.Gone;
                            mainHolder.Alpha = 1f;
                            Window.ClearFlags(Android.Views.WindowManagerFlags.NotTouchable);
                            Toast.MakeText(this, "No Data to upload", ToastLength.Short).Show();
                        });
                        btnUploadData.Clickable = true;
                        btnDownloadData.Clickable = true;
                        AppPreferences.SaveBool(this, Utilities.IsDownloaded, false);
                    }
                }
                catch
                {
                    RunOnUiThread(() =>
                    {
                        loader.Visibility = Android.Views.ViewStates.Gone;
                        mainHolder.Alpha = 1f;
                        Window.ClearFlags(Android.Views.WindowManagerFlags.NotTouchable);
                        Toast.MakeText(this, "No Data to Upload", ToastLength.Short).Show();
                    });
                    btnUploadData.Clickable = true;
                    btnDownloadData.Clickable = true;
                    AppPreferences.SaveBool(this, Utilities.IsDownloaded, false);
                    return;
                }
            }));
            thread.Start();
        }

        private bool UploadValues()
        {
            var billDetails = FuelDB.Singleton.GetBillDetails()?.First();
            if (billDetails != null)
            {
                var list = new List<UploadDetails>();
                foreach (var item in fuelDetails)
                {
                    list.Add(new UploadDetails
                    {
                        CID = billDetails.BillCurrentNumber == string.Empty ? "0" : billDetails.BillCurrentNumber,
                        DID = AppPreferences.GetString(this, Utilities.DEVICEID) == string.Empty ? "0" : AppPreferences.GetString(this, Utilities.DEVICEID),
                        SID = AppPreferences.GetString(this, Utilities.SITEID) == string.Empty ? "0" : AppPreferences.GetString(this, Utilities.SITEID),
                        CStock = billDetails.AvailableLiters == string.Empty ? "0" : billDetails.AvailableLiters,
                        ClosingKM = item.ClosingKMS == string.Empty ? "0" : item.ClosingKMS,
                        DriverID = item.DriverID_PK == string.Empty ? "0" : item.DriverID_PK,
                        DriverName = item.DriverName == string.Empty ? "0" : item.DriverName,
                        FilledBy = item.FilledBy == string.Empty ? "0" : item.FilledBy,
                        FuelDate = item.CurrentDate == string.Empty ? "0" : item.CurrentDate,
                        FuelLts = item.FuelInLtrs == string.Empty ? "0" : item.FuelInLtrs,
                        FuelNo = item.BillNumber == string.Empty ? "0" : item.BillNumber,
                        FuelSource = item.FuelStockType == string.Empty ? "0" : item.FuelStockType,
                        KMPL = item.Kmpl == "KMPL" ? "0" : item.Kmpl,
                        OpeningKM = item.OpeningKMS == string.Empty ? "0" : item.OpeningKMS,
                        RegNo = item.VehicleNumber == string.Empty ? "0" : item.VehicleNumber,
                        VType = item.VehicleType == string.Empty ? "0" : item.VehicleType,
                        Rate = item.RatePerLtr == string.Empty ? "0" : item.RatePerLtr,
                        TAmount = item.Price == string.Empty ? "0" : item.Price,
                        Remarks = item.Remarks == string.Empty ? "0" : item.Remarks,
                        TransType = item.FuelType == string.Empty ? "0" : item.FuelType,
                        Mode = item.PaymentType == string.Empty ? "0" : item.PaymentType,
                        VehicleID = item.VID == string.Empty ? "0" : item.VID,
                        MeterFault = item.MeterFault == string.Empty ? "0" : item.MeterFault,
                        TotalKM = item.TotalKM == string.Empty ? "0" : item.TotalKM,
                        IsExcess = item.IsExcess,
                        ExcessLtr = item.ExcessLtr,
                        IsShortage = item.IsShortage,
                        ShortageLtr = item.ShortageLtr
                    });
                }
                Console.WriteLine(list);
                var deserializedData = JsonConvert.SerializeObject(list);
                Console.WriteLine(deserializedData);
                var resposeAfterPost = WebService.PostAllDataToWebService("UPFStock", deserializedData);
                try
                {
                    var vehicleList = JsonConvert.DeserializeObject<List<VehicleDetails>>(resposeAfterPost);
                    if (vehicleList != null)
                    {
                        CreateDatabaseOrModifyDatabase(vehicleList);
                    }
                }
                catch
                {
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Error in Upload", ToastLength.Short).Show();
                    });
                    return false;
                }
                Console.WriteLine(resposeAfterPost);
            }
            return true;
        }


        private void SyncButton_Click()
        {
            if (IpAddress.Equals(string.Empty) || siteId.Equals(string.Empty) || deviceId.Equals(string.Empty))
            {
                Toast.MakeText(this, "Something went wrong..", ToastLength.Short).Show();
                return;
            }
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, "Please wait..", ToastLength.Short).Show();
                mainHolder.Alpha = 0.5f;
                loader.Visibility = Android.Views.ViewStates.Visible;
                Window.SetFlags(Android.Views.WindowManagerFlags.NotTouchable, Android.Views.WindowManagerFlags.NotTouchable);
            });

            var thread = new Thread(new ThreadStart(delegate
            {
                var resposeString = WebService.PostDeviceAndSiteIDToWebService("GetVD", deviceId, siteId);
                try
                {
                    var VehicleList = JsonConvert.DeserializeObject<List<VehicleDetails>>(resposeString);                    
                    CreateDatabaseOrModifyDatabase(VehicleList);
                }
                catch (Exception ec)
                {
                    Console.WriteLine(ec.Message);
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Something wrong ...Check connectivity..", ToastLength.Short).Show();
                        loader.Visibility = Android.Views.ViewStates.Gone;
                        mainHolder.Alpha = 1f;
                        Window.ClearFlags(Android.Views.WindowManagerFlags.NotTouchable);
                        AppPreferences.SaveBool(this, Utilities.IsDownloaded, false);
                    });
                    return;
                }
                RunOnUiThread(() =>
                {
                    loader.Visibility = Android.Views.ViewStates.Gone;
                    mainHolder.Alpha = 1f;
                    Window.ClearFlags(Android.Views.WindowManagerFlags.NotTouchable);
                    Toast.MakeText(this, "success..", ToastLength.Short).Show();
                    btnDownloadData.Clickable = false;
                    AppPreferences.SaveBool(this, Utilities.IsDownloaded, true);
                });
            }));
            thread.Start();
        }

        public void CreateDatabaseOrModifyDatabase(List<VehicleDetails> vehicleList)
        {
            DeleteDatabase(FuelDB.Singleton.DBPath);
            FuelDB.Singleton.CreateTable<VehicleDetails>();
            FuelDB.Singleton.CreateTable<BillDetails>();

            var details = vehicleList?.First();
            vehicleList.RemoveAt(0);
            var billDetails = new BillDetails
            {
                AvailableLiters = details.VID,
                BillCurrentNumber = details.DriverID_PK,
                BillPrefix = details.RegNo,
                DeviceStatus = details.DriverName
            };

            AppPreferences.SaveString(this, Utilities.DEVICESTATUS, billDetails.DeviceStatus);
            AppPreferences.SaveBool(this, Utilities.IsDownloaded, true);
            FuelDB.Singleton.InsertValues(vehicleList);
            btnDownloadData.Clickable = false;
            FuelDB.Singleton.InsertBillDetails(billDetails);
        }
        protected override void OnResume()
        {
            IsExitApp = false;
            base.OnResume();
        }
        public override void OnBackPressed()
        {
            if (IsExitApp)
            {
                base.OnBackPressed();
            }
            IsExitApp = true;
            Toast.MakeText(this, "Press agin to exit app..", ToastLength.Short).Show();
        }
    }
}