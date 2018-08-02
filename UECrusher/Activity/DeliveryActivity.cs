using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    [Activity(Label = "DeliveryActivity", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class DeliveryActivity : AppCompatActivity
    {
        private TextView lblBillNumber;
        private Spinner itemTypeSpinner;
        private LinearLayout loaderLayout;
        private Button btnUpdate;
        private Button btnCheck;
        private LinearLayout layBillEntry;
        private LinearLayout layDeliveryDetails;
        private EditText txtBillNumber;
        private List<ItemDetails> itemDetails;
        private string did;
        private string siteId;
        private List<VehicleDetailsGETVE> itemDetailsGetVE;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.DeliveryDetail);
            lblBillNumber = FindViewById<TextView>(Resource.Id.lblBillNumber);
            itemTypeSpinner = FindViewById<Spinner>(Resource.Id.itemTypeSpinner);
            //  lblDate = FindViewById<TextView>(Resource.Id.lblDate);
            loaderLayout = FindViewById<LinearLayout>(Resource.Id.layProgressLoader);
            btnUpdate = FindViewById<Button>(Resource.Id.btnUpdate);
            btnCheck = FindViewById<Button>(Resource.Id.btnCheck);
            layBillEntry = FindViewById<LinearLayout>(Resource.Id.layBillEntry);
            layDeliveryDetails = FindViewById<LinearLayout>(Resource.Id.layDeliveryDetails);

            //bill number text
            txtBillNumber = FindViewById<EditText>(Resource.Id.txtBillNumber);

            btnCheck.Click += BtnCheck_Click;
            btnUpdate.Click += BtnUpdate_Click;

            ShowLoader(true);
            //loaderLayout.Visibility = Android.Views.ViewStates.Visible;

            lblBillNumber.Text = AppPreferences.GetString(this, Utilities.BILLNUMBER);
            //  lblDate.Text = DateTime.Now.ToString(Utilities.DATE_MONTH_TIME, CultureInfo.InvariantCulture);

            Task.Run(() => GetDetails());
        }


        private async void GetDetails()
        {
            WebService.IPADDRESS = AppPreferences.GetString(this, Utilities.IPAddress);
            //WebService.IPADDRESS = "49.207.180.49";
            //did = "FED11";
            did = AppPreferences.GetString(this, Utilities.DEVICEID);
            //siteId = "2";
            siteId = AppPreferences.GetString(this, Utilities.SITEID);
            try
            {
                //var result = await WebService.Singleton.PostDataToWebService(Utilities.GET_VEHICLE_DETAILS, did, siteId, Utilities.GET_VEHICLE_RESULT);
                //vehiclDetailList = JsonConvert.DeserializeObject<List<VehicleDetails>>(result);
                var itemType = await WebService.Singleton.PostDataToWebService(Utilities.GET_ITEM_DETAILS, did, siteId, Utilities.GET_ITEM_RESULT);
                itemDetails = JsonConvert.DeserializeObject<List<ItemDetails>>(itemType);
                itemTypeSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, itemDetails.Select(x => x.MaterialName).ToArray());
                ShowLoader(false);
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
        private void ShowLoader(bool value)
        {
            RunOnUiThread(() =>
            {
                if (value)
                {
                    loaderLayout.Visibility = Android.Views.ViewStates.Visible;
                    //  layScroll.Alpha = 0.5f;
                    Window.SetFlags(Android.Views.WindowManagerFlags.NotTouchable, Android.Views.WindowManagerFlags.NotTouchable);
                }
                else
                {
                    loaderLayout.Visibility = Android.Views.ViewStates.Gone;
                    // layScroll.Alpha = 1f;
                    Window.ClearFlags(Android.Views.WindowManagerFlags.NotTouchable);
                }
            });
            //thr
        }
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                //  var itemId = itemDetailsGetVE.First().ItemID_FK;
                var response = WebService.Singleton.UVEDS(lblBillNumber.Text, itemDetailsGetVE.First().ItemID_FK,
                    itemDetailsGetVE.First().ItemName, DateTime.Now.ToString(Utilities.MONTH_DATE_TIME, CultureInfo.InvariantCulture));
                if (!response.Equals(string.Empty))
                {
                    layDeliveryDetails.Visibility = Android.Views.ViewStates.Gone;
                    layBillEntry.Visibility = Android.Views.ViewStates.Visible;
                    Toast.MakeText(this, "Successfully updated..", ToastLength.Short).Show();
                }
            }
            catch { }
        }

        private void BtnCheck_Click(object sender, EventArgs e)
        {
            if (!txtBillNumber.Text.Equals(string.Empty))
            {
                loaderLayout.Visibility = Android.Views.ViewStates.Visible;
                CheckBillNumberAndGetDetails();
            }
            else
            {
                Toast.MakeText(this, "Enter the Bill Number..", ToastLength.Short).Show();
            }
        }

        private void CheckBillNumberAndGetDetails()
        {
            WebService.IPADDRESS = "49.207.180.49";
            did = "FED11";
            //AppPreferences.GetString(this, Utilities.DEVICEID);
            siteId = "2";
            //AppPreferences.GetString(this, Utilities.SITEID);

            if (!did.Equals(string.Empty) && !siteId.Equals(string.Empty) && !WebService.IPADDRESS.Equals(string.Empty))
            {
                try
                {
                    var result = WebService.Singleton.GetBillDetails("CheckVE", txtBillNumber.Text, siteId);
                    itemDetailsGetVE = JsonConvert.DeserializeObject<List<VehicleDetailsGETVE>>(result);
                    if (!itemDetailsGetVE.Equals(string.Empty))
                    {
                        layDeliveryDetails.Visibility = Android.Views.ViewStates.Visible;
                        layBillEntry.Visibility = Android.Views.ViewStates.Gone;
                    }
                    else
                    {
                        Toast.MakeText(this, "Something went wrong", ToastLength.Short).Show();
                    }
                    //vehiclDetailList = JsonConvert.DeserializeObject<List<VehicleDetails>>(result);
                    //var itemType = await WebService.Singleton.PostDataToWebService(Utilities.GET_ITEM_DETAILS, did, siteId, Utilities.GET_ITEM_RESULT);
                }
                catch (Exception ex)
                {

                    Toast.MakeText(this, "No Data to load..", ToastLength.Short).Show();
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}