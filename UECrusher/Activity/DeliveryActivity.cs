using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
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
        private List<string> itemList;
        private string did;
        private string siteId;
        private List<VehicleDetailsGETVE> itemDetailsGetVE;
        private bool isBillEntry;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.DeliveryDetail);
            ExceptionLog.LogDetails(this, "\n\n In Vehicle Delivery...");

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Toast.MakeText(this, "Something went wrong", ToastLength.Short).Show();
            };


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

            itemTypeSpinner.ItemSelected += (s, e) =>
            {

            };

            Task.Run(() => GetDetails());


            FindViewById<ImageButton>(Resource.Id.btnLogout).Click += (s, e) =>
            {
                var alertDialog = new Android.App.AlertDialog.Builder(this);
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

                itemList = itemDetails.Select(x => x.MaterialName).ToList();
                //itemList.Insert(0, "Select");                
                //itemTypeSpinner.Adapter = new BasicAdapter(this, itemDetails.Select(x => x.MaterialName).ToArray());
                ShowLoader(false);
            }
            catch (Exception ex)
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "No Data to load..", ToastLength.Short).Show();
                    ShowLoader(false);
                });
                ExceptionLog.LogDetails(this, ex.Message + "\n\n Exception in GetItem");
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
            //if (itemTypeSpinner.SelectedItemPosition.Equals(0))
            //{
            //    Toast.MakeText(this, "Select the particular item..", ToastLength.Short).Show();
            //    return;
            //}
            try
            {
                //  var itemId = itemDetailsGetVE.First().ItemID_FK;
                var id = itemDetails.Where(x => x.MaterialName == itemTypeSpinner.SelectedItem.ToString()).Select(x => x.ItemID_PK).First();
                var response = WebService.Singleton.UVEDS(lblBillNumber.Text, id,
                    itemTypeSpinner.Selected.ToString(), DateTime.Now.ToString(Utilities.MONTH_DATE_TIME, CultureInfo.InvariantCulture));
                if (!response.Equals(string.Empty))
                {
                    layDeliveryDetails.Visibility = Android.Views.ViewStates.Gone;
                    layBillEntry.Visibility = Android.Views.ViewStates.Visible;
                    isBillEntry = true;

                    var array = new string[] { "LB. No.", "Out Date and Time", "Item", "Vehicle No." };
                    var list = new VehicleDetailsGETVE
                    {
                        LoadBillNo = itemDetailsGetVE.First().LoadBillNo,
                        EntryDate = DateTime.Now.ToString(Utilities.DATE_MONTH_TIME_AMPM, CultureInfo.InvariantCulture),
                        ItemName = itemTypeSpinner.SelectedItem.ToString(),
                        VehicleNo = itemDetailsGetVE.First().VehicleNo
                    };
                    var serilizedData = JsonConvert.SerializeObject(list);

                    var intent = new Intent(this, typeof(PrintViewActivity));
                    intent.PutExtra("data", serilizedData);
                    intent.PutStringArrayListExtra("array", array);
                    intent.PutExtra("typeof", "VehicleDetailsGETVE");
                    StartActivity(intent);
                    //Toast.MakeText(this, "Successfully updated..", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "Error in update..", ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.LogDetails(this, ex.Message + "\n\n Exception in UVEDS...");
            }
        }

        private void BtnCheck_Click(object sender, EventArgs e)
        {
            if (!txtBillNumber.Text.Equals(string.Empty))
            {
                loaderLayout.Visibility = Android.Views.ViewStates.Visible;
                try
                {
                    CheckBillNumberAndGetDetails();
                }
                catch (Exception ex)
                {
                    ExceptionLog.LogDetails(this, ex.Message + "\n\n Exception in CheckBillNumberAndGetDetails");
                }
            }
            else
            {
                Toast.MakeText(this, "Enter the Bill Number..", ToastLength.Short).Show();
            }
        }

        private void CheckBillNumberAndGetDetails()
        {
            // WebService.IPADDRESS = "49.207.180.49";
            // did = "FED11";
            //AppPreferences.GetString(this, Utilities.DEVICEID);
            // siteId = "2";
            WebService.IPADDRESS = AppPreferences.GetString(this, Utilities.IPAddress);
            did = AppPreferences.GetString(this, Utilities.DEVICEID);
            siteId = AppPreferences.GetString(this, Utilities.SITEID);

            if (!did.Equals(string.Empty) && !siteId.Equals(string.Empty) && !WebService.IPADDRESS.Equals(string.Empty))
            {
                try
                {
                    var result = WebService.Singleton.GetBillDetails("CheckVE", txtBillNumber.Text, siteId);
                    itemDetailsGetVE = JsonConvert.DeserializeObject<List<VehicleDetailsGETVE>>(result);
                    if (!itemDetailsGetVE.Equals(string.Empty))
                    {
                        if (itemDetailsGetVE.First().IsDS.ToLower().Equals("false"))
                        {
                            layDeliveryDetails.Visibility = Android.Views.ViewStates.Visible;
                            layBillEntry.Visibility = Android.Views.ViewStates.Gone;
                            isBillEntry = false;
                            lblBillNumber.Text = itemDetailsGetVE.First().LoadBillNo;
                            var name = itemDetailsGetVE.First().ItemName;
                            var index = itemList.FindIndex(x => x.StartsWith(name));
                            var selectedItem = itemList[index];
                            itemList.RemoveAt(index);
                            itemList.Insert(0, selectedItem);
                            itemTypeSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.spinner_item, itemList);
                        }
                        else
                        {
                            ShowWrongText("Already Updated...");
                        }
                    }
                    else
                    {
                        ShowWrongText();
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "No Data to load..", ToastLength.Short).Show();
                    ExceptionLog.LogDetails(this, ex.Message + "\n\n Exception in CheckBillNumberAndGetDetails");
                }
            }
            else
            {
                ShowWrongText();
            }
        }

        private void ShowWrongText(string text = "")
        {
            if (text != "")
            {
                Toast.MakeText(this, text, ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Something went wrong", ToastLength.Short).Show();
            }
        }
        public override void OnBackPressed()
        {
            if (isBillEntry)
            {
                base.OnBackPressed();
            }
            else
            {
                layDeliveryDetails.Visibility = Android.Views.ViewStates.Gone;
                layBillEntry.Visibility = Android.Views.ViewStates.Visible;
                txtBillNumber.Text = string.Empty;
                isBillEntry = true;
            }
        }
    }
}