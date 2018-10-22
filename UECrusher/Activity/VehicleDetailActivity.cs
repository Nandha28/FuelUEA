using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Ngx.Mp100sdk;
using Com.Ngx.Mp100sdk.Intefaces;
using Newtonsoft.Json;
using UECrusher.CommonFunctions;
using UECrusher.Model;
using Utilities;

namespace UECrusher.Activity
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    public class VehicleDetailActivity : AppCompatActivity
    {
        private TextView lblBillNumber;
        private TextView lblDate;
        private TextView lblEmptyWeight;
        private AutoCompleteTextView vehicleNumberAutoComplete;
        private RadioGroup radioGroup;
        private RadioButton cashRadioButton, creditRadioButton;
        private Spinner itemTypeSpinner;
        private ScrollView layScrollview;
        private TextView ownerName;
        private Spinner wMode;
        private Button btnPrint;
        private Button btnClear;
        private ScrollView layScroll;
        private LinearLayout layLinear;
        private ProgressBar progressLoader;
        private List<VehicleDetails> vehiclDetailList;
        private List<ItemDetails> itemDetails;
        private string did;
        private string siteId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.VehicleDetails);
            ExceptionLog.LogDetails(this, "Vehicle Entry Details...");    

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                ShowText("Something went wrong");
                ExceptionLog.LogDetails(this, "Exception in Vehicle Details Activity...");
            };
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

            layScrollview = FindViewById<ScrollView>(Resource.Id.layScrollview);
            layLinear = FindViewById<LinearLayout>(Resource.Id.layLinear);
            layScrollview.Visibility = ViewStates.Gone;

            ownerName = FindViewById<TextView>(Resource.Id.lblOwnerName);
            wMode = FindViewById<Spinner>(Resource.Id.vehicleModeSpinner);
            btnPrint = FindViewById<Button>(Resource.Id.btnPrint);
            btnClear = FindViewById<Button>(Resource.Id.btnClear);
            btnPrint.Click += BtnPrint_Click;
            btnClear.Click += BtnClear_Click;
            lblDate.Text = DateTime.Now.ToString(Utilities.DATE_MONTH_TIME_AMPM, CultureInfo.InvariantCulture);
            vehicleNumberAutoComplete.ItemClick += VehicleNumberAutoComplete_ItemClick;
            vehicleNumberAutoComplete.TextChanged += VehicleNumberAutoComplete_TextChanged;
            vehicleNumberAutoComplete.Threshold = 1;

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

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearAllFields();
            vehicleNumberAutoComplete.Text = string.Empty;
        }

        private void VehicleNumberAutoComplete_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                ExceptionLog.LogDetails(this, ex.Message + "\n In vehicle number autocomplete text..");
            }
        }

        private void VehicleNumberAutoComplete_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                var list = vehiclDetailList.Where(x => x.RegNo == vehicleNumberAutoComplete.Text).FirstOrDefault();
                // ownerNumber.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, list);
                if (list != null)
                {
                    ownerName.Text = list.OName;
                    lblEmptyWeight.Text = list.EmptyWeight;
                }
            }
            catch
            {
                ShowText("Something went wrong");
                ExceptionLog.LogDetails(this, "Exception in finding Owner Number or empty weight \n");
            }
            HideKeyboard();
            //ownerNumber.PerformClick();         
        }

        private void ShowText(string text)
        {
            Toast.MakeText(this, text, ToastLength.Short).Show();
        }

        private void ClearAllFields()
        {
            lblEmptyWeight.Text = string.Empty;
            ownerName.Text = string.Empty;
            itemTypeSpinner.SetSelection(0);
            cashRadioButton.Checked = true;
        }

        private void HideKeyboard()
        {
            InputMethodManager inputManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputManager.HideSoftInputFromWindow(Window.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
        }

        private async Task GetDetails()
        {
            WebService.IPADDRESS = AppPreferences.GetString(this, Utilities.IPAddress);
            //WebService.IPADDRESS = "49.207.180.49";
            //did = "FED11";
            did = AppPreferences.GetString(this, Utilities.DEVICEID);
            //siteId = "2";
            siteId = AppPreferences.GetString(this, Utilities.SITEID);

            if (!did.Equals(string.Empty) && !siteId.Equals(string.Empty) && !WebService.IPADDRESS.Equals(string.Empty))
            {
                try
                {
                    var result = await WebService.Singleton.PostDataToWebService(Utilities.GET_VEHICLE_DETAILS,
                                                                                 did, siteId, Utilities.GET_VEHICLE_RESULT);
                    vehiclDetailList = JsonConvert.DeserializeObject<List<VehicleDetails>>(result);
                    var itemType = await WebService.Singleton.PostDataToWebService(Utilities.GET_ITEM_DETAILS,
                                                                                   did, siteId, Utilities.GET_ITEM_RESULT);
                    itemDetails = JsonConvert.DeserializeObject<List<ItemDetails>>(itemType);

                    var response = await WebService.Singleton.PostDataToWebService(Utilities.GET_CURRENT_BILL,
                        did, siteId, Utilities.GET_CURRENT_BILL_RES);
                    var currentBill = JsonConvert.DeserializeObject<List<UploadFirstResult>>(response);
                    RunOnUiThread(() =>
                    {
                        lblBillNumber.Text = currentBill.First().CUNUM;
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
                    ExceptionLog.LogDetails(this, ex.Message + "\n\n In GetVE");

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
            //itemList.AddRange(itemDetails.Select(x => x.MaterialName).ToList());       
        }

        private void FillVehicleDetails()
        {
            //var billNum = AppPreferences.GetString(this, Utilities.BILLNUMBER);
            //lblBillNumber.Text = billNum == string.Empty ? "LB1" : billNum;
            if (vehiclDetailList != null)
            {
                try
                {
                    vehicleNumberAutoComplete.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material,
                                                       vehiclDetailList.Select(x => x.RegNo).Distinct().ToArray());

                    wMode.Adapter = new ArrayAdapter(this, Resource.Layout.spinner_item, new string[] { "Sales", "Purchase" });

                    var itemList = itemDetails.Select(x => x.MaterialName).ToList();
                    itemList.Insert(0, "Select");
                    itemTypeSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.spinner_item, itemList);
                }
                catch (Exception ex)
                {
                    ShowText("Something went wrong");
                    ExceptionLog.LogDetails(this, ex.Message + "\n\n In FillVehicleDetails");
                }
            }
        }

        private void ShowLoader(bool isToHide)
        {
            RunOnUiThread(() =>
            {
                if (isToHide)
                {
                    progressLoader.Visibility = ViewStates.Visible;
                    layScroll.Alpha = 0.5f;
                    Window.SetFlags(WindowManagerFlags.NotTouchable, WindowManagerFlags.NotTouchable);
                }
                else
                {
                    progressLoader.Visibility = ViewStates.Gone;
                    layScroll.Alpha = 1f;
                    Window.ClearFlags(WindowManagerFlags.NotTouchable);
                }
            });
            //thread.Start();
        }
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            //var response = UploadItemDetails();
            //if (response != null && !response.Equals(string.Empty))
            //{
            //    AppPreferences.SaveString(this, Utilities.BILLNUMBER, response);
            //    Toast.MakeText(this, "Sucess", ToastLength.Short).Show();
            //}
            if (itemTypeSpinner.SelectedItemPosition.Equals(0))
            {
                Toast.MakeText(this, "Select the particular item..", ToastLength.Short).Show();
                return;
            }
            RunOnUiThread(() =>
            {
                progressLoader.Visibility = ViewStates.Visible;
                layScroll.Alpha = 0.5f;
                Window.SetFlags(WindowManagerFlags.NotTouchable, WindowManagerFlags.NotTouchable);
            });
            var str = FindViewById<RadioButton>(radioGroup.CheckedRadioButtonId).Text;
            try
            {
                //Console.WriteLine(dataToUpload);
                var list = new List<UploadItemDetails>()
                {
                    new UploadItemDetails
                    {
                        // DID = AppPreferences.GetString(this, Utilities.DEVICEID),
                        DID = did,
                        EntryDate = DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                        EWeight = lblEmptyWeight.Text,
                        LBNo = lblBillNumber.Text,
                        OwnerName = ownerName.Text,
                        PayMode = str,
                        VehicleNo = vehicleNumberAutoComplete.Text,
                        //SiteID = AppPreferences.GetString(this, Utilities.SITEID),
                        SiteID = siteId,
                        WMode = wMode.SelectedItem.ToString(),
                        ItemName = itemTypeSpinner.SelectedItem.ToString(),
                        OwnerId = vehiclDetailList.Where(x => x.RegNo == vehicleNumberAutoComplete.Text).First().OID,
                        VehicleId = vehiclDetailList.Where(x => x.RegNo == vehicleNumberAutoComplete.Text).First().VID,
                        ItemId = itemDetails.Where(x => x.MaterialName == itemTypeSpinner.SelectedItem.ToString()).First().ItemID_PK
                    }
                };
                var serializedData = JsonConvert.SerializeObject(list);
                var result = WebService.Singleton.PostAllDataToWebService(Utilities.INVE, serializedData, "INVEResult");
                if (result == null)
                {
                    RunOnUiThread(() =>
                    {
                        progressLoader.Visibility = ViewStates.Gone;
                        layScroll.Alpha = 1f;
                        Window.ClearFlags(WindowManagerFlags.NotTouchable);
                        Toast.MakeText(this, "Error in upload..", ToastLength.Short).Show();
                        ExceptionLog.LogDetails(this, "Exception during upload..");
                    });
                    return;
                }
                var deserializeResult = JsonConvert.DeserializeObject<List<UploadFirstResult>>(result);
                //Save bill number
                AppPreferences.SaveString(this, Utilities.BILLNUMBER, deserializeResult.First().CUNUM);

                var intent = new Intent(this, typeof(PrintViewActivity));
                var newList = new List<UploadItemDetails>()
                {
                    new UploadItemDetails
                    {
                        LBNo = list.First().LBNo,
                        EntryDate = DateTime.Now.ToString(Utilities.DATE_MONTH_TIME_AMPM,CultureInfo.CurrentCulture),
                        VehicleNo = list.First().VehicleNo,
                        OwnerName = list.First().OwnerName,
                        ItemName = list.First().ItemName,
                        EWeight = list.First().EWeight,
                        PayMode = list.First().PayMode,
                        WMode = list.First().WMode
                    }
                };
                var lista = newList.Select(x => new { x.LBNo, x.EntryDate, x.VehicleNo, x.OwnerName, x.ItemName, x.EWeight, x.PayMode, x.WMode });

                var array = new string[] { "LB. No.", "Date", "Vehicle", "Customer", "Item", "Empty Weight", "Pay Mode", "W Mode" };
                var seralizedPrintData = JsonConvert.SerializeObject(lista);
                intent.PutExtra("data", seralizedPrintData);
                intent.PutStringArrayListExtra("array", array);
                intent.PutExtra("typeof", "UploadItemDetails");
                StartActivity(intent);
                Finish();
                RunOnUiThread(() =>
                {
                    progressLoader.Visibility = ViewStates.Gone;
                    layScroll.Alpha = 1f;
                    Window.ClearFlags(WindowManagerFlags.NotTouchable);
                });
            }
            catch (Exception ex)
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "Something went wrong", ToastLength.Short).Show();
                    progressLoader.Visibility = ViewStates.Gone;
                    layScroll.Alpha = 1f;
                    Window.ClearFlags(WindowManagerFlags.NotTouchable);
                });
                ExceptionLog.LogDetails(this, ex.Message + "\n\n Exception during button print");
            }

        }

        public void OnRaiseException(int p0, string p1)
        {

        }

        public void OnReturnString(string p0)
        {
        }

        public void OnRunResult(bool p0)
        {

        }
        //public override void OnBackPressed()
        //{
        //    if (IsExtraPrint)
        //    {
        //        base.OnBackPressed();
        //    }
        //    IsExtraPrint = true;
        //    Toast.MakeText(this, "Press agin to exit..", ToastLength.Short).Show();
        //}
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}