using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Newtonsoft.Json;
using UECrusher.CommonFunctions;
using UECrusher.Model;
using Utilities;

namespace UECrusher.Activity
{
    [Activity(Label = "VehicleDetailActivity", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class VehicleDetailActivity : AppCompatActivity
    {
        private TextView lblBillNumber;
        private TextView lblDate;
        private TextView lblEmptyWeight;
        private AutoCompleteTextView autoVehicleNumber;
        private RadioGroup radioGroup;
        private RadioButton cashRadioButton, creditRadioButton;
        private Spinner itemType;
        private Spinner wMode;
        private ScrollView layScroll;
        private ProgressBar progressLoader;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.VehicleDetails);
            layScroll = FindViewById<ScrollView>(Resource.Id.layScroll);
            progressLoader = FindViewById<ProgressBar>(Resource.Id.loader);
            var lblTittle = FindViewById<TextView>(Resource.Id.lblTittle);
            lblBillNumber = FindViewById<TextView>(Resource.Id.lblBillNumber);
            lblDate = FindViewById<TextView>(Resource.Id.lblDate);
            lblEmptyWeight = FindViewById<TextView>(Resource.Id.lblEmptyWeight);
            autoVehicleNumber = FindViewById<AutoCompleteTextView>(Resource.Id.vehicleNumber);
            radioGroup = FindViewById<RadioGroup>(Resource.Id.radioPaymentMode);
            cashRadioButton = FindViewById<RadioButton>(Resource.Id.cashRadioButton);
            creditRadioButton = FindViewById<RadioButton>(Resource.Id.creditRadioButton);
            itemType = FindViewById<Spinner>(Resource.Id.itemTypeSpinner);
            wMode = FindViewById<Spinner>(Resource.Id.vehicleModeSpinner);

            var task = new Thread(() =>
              {                
                  RunOnUiThread(() =>
                  {
                      progressLoader.Visibility = Android.Views.ViewStates.Visible;
                      layScroll.Alpha = 0.5f;
                      Window.SetFlags(Android.Views.WindowManagerFlags.NotTouchable, Android.Views.WindowManagerFlags.NotTouchable);
                  });
              });
            task.Start();
            ShowLoader(true);

            WebService.IPADDRESS = AppPreferences.GetString(this, Utilities.IPAddress);
            var did = AppPreferences.GetString(this, Utilities.DEVICEID);
            var siteId = AppPreferences.GetString(this, Utilities.SITEID);

            if (!did.Equals(string.Empty) && !siteId.Equals(string.Empty) && !WebService.IPADDRESS.Equals(string.Empty))
            {
                try
                {
                    var result = WebService.Singleton.PostDataToWebService("GetVE", did, siteId);
                    var deserialize = JsonConvert.DeserializeObject<List<VehicleDetails>>(result);
                    var taskRemoveLoader = new Thread(() =>
                    {
                        RunOnUiThread(() =>
                        {
                            progressLoader.Visibility = Android.Views.ViewStates.Gone;
                            layScroll.Alpha = 1f;
                            Window.ClearFlags(Android.Views.WindowManagerFlags.NotTouchable);
                        });
                    });
                    taskRemoveLoader.Start();                   
                }
                catch
                {
                    Toast.MakeText(this, "No Data to load..", ToastLength.Short).Show();
                }
            }
            else
            {
                Toast.MakeText(this, "Someting went wrong..", ToastLength.Short).Show();
            }
            ShowLoader(false);
        }

        private void ShowLoader(bool isToHide)
        {
            //var thread = new Thread(new ThreadStart(delegate
            //{
            RunOnUiThread(() =>
            {
                if (isToHide)
                {
                    progressLoader.Visibility = Android.Views.ViewStates.Visible;
                    layScroll.Alpha = 0.5f;
                    Window.SetFlags(Android.Views.WindowManagerFlags.NotTouchable, Android.Views.WindowManagerFlags.NotTouchable);
                }
                else
                {
                    progressLoader.Visibility = Android.Views.ViewStates.Gone;
                    layScroll.Alpha = 1f;
                    Window.ClearFlags(Android.Views.WindowManagerFlags.NotTouchable);
                }
            });
            //thread.Start();
        }
    }
}