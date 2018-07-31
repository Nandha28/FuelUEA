using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views.InputMethods;
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
        private Spinner itemTypeSpinner;
        private TextView ownerName;
        private Spinner wMode;
        private ScrollView layScroll;
        private ProgressBar progressLoader;
        private List<VehicleDetails> vehiclDetailList;
        private List<ItemDetails> itemDetails;

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
            lblDate = FindViewById<TextView>(Resource.Id.lbldateTime);
            lblEmptyWeight = FindViewById<TextView>(Resource.Id.lblEmptyWeight);
            vehicleNumberAutoComplete = FindViewById<AutoCompleteTextView>(Resource.Id.vehicleNumber);
            radioGroup = FindViewById<RadioGroup>(Resource.Id.radioPaymentMode);
            cashRadioButton = FindViewById<RadioButton>(Resource.Id.cashRadioButton);
            creditRadioButton = FindViewById<RadioButton>(Resource.Id.creditRadioButton);
            itemTypeSpinner = FindViewById<Spinner>(Resource.Id.itemTypeSpinner);
            // ownerNumber = FindViewById<Spinner>(Resource.Id.ownerNumberSpinner);
            ownerName = FindViewById<TextView>(Resource.Id.lblOwnerName);
            wMode = FindViewById<Spinner>(Resource.Id.vehicleModeSpinner);

            lblDate.Text = DateTime.Now.ToString(Utilities.DATE_MONTH_TIME, CultureInfo.InvariantCulture);
            vehicleNumberAutoComplete.ItemClick += VehicleNumberAutoComplete_ItemClick;
            vehicleNumberAutoComplete.TextChanged += VehicleNumberAutoComplete_TextChanged;
            vehicleNumberAutoComplete.Threshold = 1;

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

        private void VehicleNumberAutoComplete_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (vehicleNumberAutoComplete.Text == string.Empty)
            {
                ClearAllFields();
            }
            var isRegisterVehicle = vehiclDetailList.Any(x => x.RegNo.Contains(vehicleNumberAutoComplete.Text));
            if (!isRegisterVehicle)
            {
                ownerName.Text = Utilities.NEW_VEHICLE;
                lblEmptyWeight.Text = Utilities.EMPTY_WEIGHT;
            }
        }

        private void VehicleNumberAutoComplete_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var list = vehiclDetailList.Where(x => x.RegNo == vehicleNumberAutoComplete.Text).FirstOrDefault();
            // ownerNumber.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, list);
            if (list != null)
            {
                ownerName.Text = list.OName;
                lblEmptyWeight.Text = list.EmptyWeight;
            }
            itemTypeSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, itemDetails.Select(x => x.MaterialName).ToArray());

            //ownerNumber.PerformClick();         
        }

        private void ClearAllFields()
        {
            lblEmptyWeight.Text = string.Empty;
            ownerName.Text = string.Empty;
            itemTypeSpinner.Adapter = null;            
        }

        private void HideKeyboard()
        {
            InputMethodManager inputManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputManager.HideSoftInputFromWindow(Window.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
        }

        private async Task GetDetails()
        {
            //WebService.IPADDRESS = AppPreferences.GetString(this, Utilities.IPAddress);
            WebService.IPADDRESS = "49.207.180.49";
            var did = "FED11";
            //AppPreferences.GetString(this, Utilities.DEVICEID);
            var siteId = "2";
            //AppPreferences.GetString(this, Utilities.SITEID);

            if (!did.Equals(string.Empty) && !siteId.Equals(string.Empty) && !WebService.IPADDRESS.Equals(string.Empty))
            {
                try
                {
                    var result = await WebService.Singleton.PostDataToWebService(Utilities.GET_VEHICLE_DETAILS, did, siteId, Utilities.GET_VEHICLE_RESULT);
                    vehiclDetailList = JsonConvert.DeserializeObject<List<VehicleDetails>>(result);
                    var itemType = await WebService.Singleton.PostDataToWebService(Utilities.GET_ITEM_DETAILS, did, siteId, Utilities.GET_ITEM_RESULT);
                    itemDetails = JsonConvert.DeserializeObject<List<ItemDetails>>(itemType);
                    RunOnUiThread(() =>
                    {
                        FillVehicleDetails();
                    });
                }
                catch (Exception ex)
                {
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "No Data to load..", ToastLength.Short).Show();
                    });
                    Console.WriteLine(ex.Message);
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
            lblBillNumber.Text = "LB" + (billNum == string.Empty ? "00" : billNum);
            if (vehiclDetailList != null)
            {
                vehicleNumberAutoComplete.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material,
                                                   vehiclDetailList.Select(x => x.RegNo).Distinct().ToArray());

                wMode.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, new string[] { "Sales", "Purchase" });
            }
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