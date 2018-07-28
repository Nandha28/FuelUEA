using System;
using System.Collections.Generic;
using System.Linq;
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
        private AutoCompleteTextView vehicleNumberAutoComplete;
        private RadioGroup radioGroup;
        private RadioButton cashRadioButton, creditRadioButton;
        private Spinner itemType;
        private Spinner ownerNumber;
        private Spinner wMode;
        private ScrollView layScroll;
        private ProgressBar progressLoader;
        private List<VehicleDetails> vehiclDetailList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.VehicleDetails);

            progressLoader = FindViewById<ProgressBar>(Resource.Id.loader);
            // progressLoader.Visibility = Android.Views.ViewStates.Visible;
            layScroll = FindViewById<ScrollView>(Resource.Id.layScroll);
            //layScroll.Alpha = 0.5f;
            var lblTittle = FindViewById<TextView>(Resource.Id.lblTittle);
            lblBillNumber = FindViewById<TextView>(Resource.Id.lblBillNumber);
            lblDate = FindViewById<TextView>(Resource.Id.lblDate);
            lblEmptyWeight = FindViewById<TextView>(Resource.Id.lblEmptyWeight);
            vehicleNumberAutoComplete = FindViewById<AutoCompleteTextView>(Resource.Id.vehicleNumber);
            radioGroup = FindViewById<RadioGroup>(Resource.Id.radioPaymentMode);
            cashRadioButton = FindViewById<RadioButton>(Resource.Id.cashRadioButton);
            creditRadioButton = FindViewById<RadioButton>(Resource.Id.creditRadioButton);
            itemType = FindViewById<Spinner>(Resource.Id.itemTypeSpinner);
            ownerNumber = FindViewById<Spinner>(Resource.Id.ownerNumberSpinner);
            wMode = FindViewById<Spinner>(Resource.Id.vehicleModeSpinner);

            //    var task = new Thread(() =>
            //      {                
            //          RunOnUiThread(() =>
            //          {
            //              progressLoader.Visibility = Android.Views.ViewStates.Visible;
            //              layScroll.Alpha = 0.5f;
            //              Window.SetFlags(Android.Views.WindowManagerFlags.NotTouchable, Android.Views.WindowManagerFlags.NotTouchable);
            //          });
            //      });
            //    task.Start();
            ShowLoader(true);
            Task.Run(() => GetDetails()
            );
        }

        private async Task GetDetails()
        {
            WebService.IPADDRESS = AppPreferences.GetString(this, Utilities.IPAddress);
            var did = AppPreferences.GetString(this, Utilities.DEVICEID);
            var siteId = AppPreferences.GetString(this, Utilities.SITEID);

            if (!did.Equals(string.Empty) && !siteId.Equals(string.Empty) && !WebService.IPADDRESS.Equals(string.Empty))
            {
                try
                {
                    var result = WebService.Singleton.PostDataToWebService("GetVE", did, siteId);
                    vehiclDetailList = JsonConvert.DeserializeObject<List<VehicleDetails>>(result);
                    FillVehicleDetails();
                }
                catch
                {
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "No Data to load..", ToastLength.Short).Show();
                    });
                }
            }
            else
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "Someting went wrong..", ToastLength.Short).Show();
                });
            }
            ShowLoader(false);
        }

        private void FillVehicleDetails()
        {
            var billNum = AppPreferences.GetString(this, Utilities.BILLNUMBER);
            lblBillNumber.Text = billNum == null ? "00" : billNum;
            if (vehiclDetailList != null)
            {
                vehicleNumberAutoComplete.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material,
                                                   vehiclDetailList.Select(x => x.RegNo).Distinct().ToArray());
                vehicleNumberAutoComplete.ItemSelected += VehicleNumberAutoComplete_ItemSelected;
            }
        }

        private void VehicleNumberAutoComplete_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            ownerNumber.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material,
                vehiclDetailList.Where(x => x.OName == vehicleNumberAutoComplete.Text).Select(x => x.OName).Distinct().ToArray());
        }

        private void ShowLoader(bool isToHide)
        {
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