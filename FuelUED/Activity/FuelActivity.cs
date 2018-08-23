using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views.InputMethods;
using Android.Widget;
using FuelApp.Modal;
using FuelUED.Adapter;
using FuelUED.CommonFunctions;
using FuelUED.Modal;
using FuelUED.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FuelUED
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]

    public class FuelActivity : AppCompatActivity
    {
        public FuelActivity()
        {
            VehicleNumber = new string[50];
            StockList = new string[] { "Stock", "Bunk" };
        }

        public string[] VehicleNumber;

        public string[] StockList;

        private List<VehicleDetails> VehicleList;
        private BillDetails billDetailsList;
        Android.App.AlertDialog.Builder alertDialog;

        private string[] myVehiclelist;
        private string[] DriverNames;
        private Spinner cashModeSpinner;
        private Spinner driverNameSpinner;
        private EditText fuelToFill;
        private TextView fuelAvailable;
        private TextView txtOpeningKMS;
        private EditText txtClosingKMS;
        private TextView lblkmpl;
        private EditText txtFilledBy;
        private EditText txtRate;
        private TextView lblTotalPrice;
        private EditText txtRemarks;
        private ImageView imgFuel;
        private TextView lblButtonStore;
        private TextView lblTitle;
        private ProgressBar loader;
        private Button btnClear;
        private LinearLayout layFuelEntry;
        private AutoCompleteTextView vehicleNumber;
        private TextView billNumber;
        private string dateTimeNow;
        private Spinner fuelTypeSpinner;
        private Spinner fuelFormSpinner;
        private Spinner vehicleTypeSpinner;
        private ArrayAdapter vehicleTypeAdapter;
        private FuelEntryDetails fuelDetails;
        private float availableFuel;
        private CheckBox checkBox;
        private PrintDetails printDetails;
        private bool isVehicleTypeSpinnerSelected;
        private bool isDriverNameSpinnerSelected;
        private float ExcessLiter;
        private bool isExcess;
        private bool isAddedAlready;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.FuelEntry);

            alertDialog = new Android.App.AlertDialog.Builder(this);
            alertDialog.SetTitle("Fuel is from petrol bunk");
            alertDialog.SetMessage("Do you want to proceed ?");
            alertDialog.SetPositiveButton("OK", (s, e) =>
             {
                 StoreDetils();
             });
            alertDialog.SetNegativeButton("Cancel", (s, e) => { });
            // var resposeString = WebService.GetDataFromWebService("LoadVD");
            loader = FindViewById<ProgressBar>(Resource.Id.loader);
            layFuelEntry = FindViewById<LinearLayout>(Resource.Id.layFuelEntry);
            //ShowLoader(true);
            try
            {
                VehicleList = FuelDB.Singleton.GetValue().ToList();
                billDetailsList = FuelDB.Singleton.GetBillDetails().ToList().FirstOrDefault();
                // FuelLiters = FuelDB.Singleton.GetFuel().ToList();
            }
            catch (Exception w)
            {
                Console.WriteLine(w.Message);
            }
            if (VehicleList != null)
            {
                myVehiclelist = VehicleList.Select(I => I.RegNo).Distinct().ToArray();
            }

            lblTitle = FindViewById<TextView>(Resource.Id.lblTittle);
            // layLoader = FindViewById<LinearLayout>(Resource.Id.layLoader);
            btnClear = FindViewById<Button>(Resource.Id.btnClear);
            vehicleNumber = FindViewById<AutoCompleteTextView>(Resource.Id.vehicleNumber);
            if (myVehiclelist != null)
            {
                var adapter = new ArrayAdapter<String>(this, Resource.Layout.select_dialog_item_material, myVehiclelist);
                //var adapter = new AutoSuggestAdapter(this, Resource.Layout.select_dialog_item_material, myVehiclelist.ToList());
                vehicleNumber.Adapter = adapter;
                vehicleNumber.Threshold = 1;
                vehicleNumber.ItemClick += VehicleNumber_ItemClick;
                vehicleNumber.TextChanged += VehicleNumber_TextChanged;
            }

            billNumber = FindViewById<TextView>(Resource.Id.txtBillNumber);
            billNumber.Text = billDetailsList?.BillPrefix + billDetailsList?.BillCurrentNumber;

            dateTimeNow = FindViewById<TextView>(Resource.Id.lbldateTime).Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

            fuelTypeSpinner = FindViewById<Spinner>(Resource.Id.fuelSpinner);
            fuelTypeSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material,
                new string[] { "Outward", "Inwards", "Shortage" });

            fuelFormSpinner = FindViewById<Spinner>(Resource.Id.fuelFormSpinner);
            fuelFormSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, StockList);

            var bunkDetailsLayout = FindViewById<LinearLayout>(Resource.Id.layBunkDetails);

            cashModeSpinner = FindViewById<Spinner>(Resource.Id.paymentMode);
            cashModeSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, new string[] { "Cash", "Credit" });

            vehicleTypeSpinner = FindViewById<Spinner>(Resource.Id.vehicleType);
            vehicleTypeAdapter =
                new ArrayAdapter(this, Resource.Layout.select_dialog_item_material,
                new string[] { "Select", "Line Vehicle", "InterCard", "Loader", "Genset 1", "Genset 2", "Genset 3" });

            vehicleTypeSpinner.ItemSelected += VehicleTypeSpinner_ItemSelected;

            //VehicleTypeSpinner_ItemClick;
            btnClear.Click += BtnClear_Click;

            var layMeterFault = FindViewById<LinearLayout>(Resource.Id.layMeterFault);

            checkBox = FindViewById<CheckBox>(Resource.Id.chckMeterFault);
            checkBox.CheckedChange += (s, e) =>
            {
                if (fuelTypeSpinner.SelectedItem.ToString() == "Outward")
                {
                    layMeterFault.Visibility = checkBox.Checked ? Android.Views.ViewStates.Gone : Android.Views.ViewStates.Visible;
                }
                txtClosingKMS.Text = string.Empty;
                lblkmpl.Text = "KMPL";
            };

            driverNameSpinner = FindViewById<Spinner>(Resource.Id.driverName);
            fuelToFill = FindViewById<EditText>(Resource.Id.fuelToFill);
            fuelAvailable = FindViewById<TextView>(Resource.Id.fuelAvailable);
            txtOpeningKMS = FindViewById<TextView>(Resource.Id.txtOpeningKMS);
            txtClosingKMS = FindViewById<EditText>(Resource.Id.txtClosingKMS);
            lblkmpl = FindViewById<TextView>(Resource.Id.lblkmpl);
            txtFilledBy = FindViewById<EditText>(Resource.Id.txtFilledBy);
            txtRate = FindViewById<EditText>(Resource.Id.txtRate);
            lblTotalPrice = FindViewById<TextView>(Resource.Id.lblTotalPrice);
            txtRemarks = FindViewById<EditText>(Resource.Id.txtRemarks);
            imgFuel = FindViewById<ImageView>(Resource.Id.imgFuel);
            lblButtonStore = FindViewById<TextView>(Resource.Id.lblButtonStore);

            fuelToFill.TextChanged += (s, e) => CheckFuelAvailbility();
            txtRate.TextChanged += (s, e) => CalculateFuelTotalAmount();
            //txtOpeningKMS.TextChanged += (s, e) =>
            //{
            //    if (!string.IsNullOrEmpty(txtOpeningKMS.Text) && !string.IsNullOrEmpty(txtClosingKMS.Text)
            //    && string.IsNullOrEmpty(fuelToFill.Text))
            //    {
            //        GetKMPL();
            //    }
            //};
            driverNameSpinner.ItemSelected += (s, ev) =>
            {
                if (!isDriverNameSpinnerSelected)
                {
                    isDriverNameSpinnerSelected = true;
                    driverNameSpinner.PerformClick();
                }
                else
                {
                    txtOpeningKMS.Text = VehicleList.Where((a => a.DriverName == driverNameSpinner.SelectedItem.ToString()))
                    .Distinct().Select(i => i.OpeningKM).Distinct().First();

                    fuelToFill.RequestFocus();
                }
            };

            if (billDetailsList != null)
            {
                fuelAvailable.Text = $"{billDetailsList.AvailableLiters}";
            }
            txtClosingKMS.TextChanged += (s, e) =>
            {
                if (txtOpeningKMS.Text.Equals("0"))
                {
                    return;
                }
                if (!string.IsNullOrEmpty(txtOpeningKMS.Text) && !string.IsNullOrEmpty(txtClosingKMS.Text) && !string.IsNullOrEmpty(fuelToFill.Text))
                {
                    if (Convert.ToDecimal(txtClosingKMS.Text) > Convert.ToDecimal(txtOpeningKMS.Text) &&
                    Convert.ToDecimal(fuelToFill.Text) > 0)
                    {
                        GetKMPL();
                    }
                    else
                    {
                        lblkmpl.Text = "KMPL";
                    }
                }
            };           

            var btnStore = FindViewById<LinearLayout>(Resource.Id.btnStore);
            btnStore.Click += (s, e) =>
            {                
                if (fuelTypeSpinner.SelectedItem.Equals("Shortage"))
                {                    
                    ShowLoader(true);
                    Task.Run(async () =>
                    {
                        await SentAndStoreShortage();
                        ShowLoader(false);
                    });
                    //}
                    return;
                }
                if (fuelTypeSpinner.SelectedItem.Equals("Inwards") && !isAddedAlready)
                {
                    var alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle("Adding fuel");
                    alertDialog.SetMessage("Do you need to add fuel to existing");
                    alertDialog.SetCancelable(false);
                    alertDialog.SetPositiveButton("Yes", (ss, se) =>
                    {
                        fuelAvailable.Text = (Convert.ToInt32(fuelAvailable.Text) + Convert.ToInt32(fuelToFill.Text)).ToString();
                        isAddedAlready = true;
                        StoreDetils();
                    });
                    alertDialog.SetNegativeButton("No", (ss, se) =>
                    {
                        isAddedAlready = true;
                        StoreDetils();
                    });
                    alertDialog.Show();                    
                    return;
                }
                if (vehicleTypeSpinner.SelectedItemPosition.Equals(0))
                {
                    ShowLoader(false);
                    Toast.MakeText(this, "Select Vehicle Type...", ToastLength.Short).Show();
                    return;
                }
                //try
                //{
                //    if (Convert.ToInt32(fuelAvailable.Text) < 1 && !fuelTypeSpinner.SelectedItem.Equals("Inwards"))
                //    {
                //        Toast.MakeText(this, "No stock availabe", ToastLength.Short).Show();
                //        ShowLoader(false);
                //        return;
                //    }
                //}
                //catch
                //{
                //    ShowLoader(false);
                //    return;
                //}
                if (fuelFormSpinner.SelectedItem.Equals("Bunk") && !fuelTypeSpinner.SelectedItem.Equals("Inwards"))
                {
                    alertDialog.Show();
                }
                else
                {
                    StoreDetils();
                }
                ShowLoader(false);
            };

            fuelTypeSpinner.ItemSelected += (s, e) =>
            {
                fuelFormSpinner.Adapter = null;
                //StockList = null;
                if (fuelTypeSpinner.SelectedItem.Equals("Inwards"))
                {
                    fuelFormSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, new string[] { "Bunk" });
                    layMeterFault.Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<LinearLayout>(Resource.Id.layFuelEntry).SetBackgroundResource(Resource.Color.backgroundInward);
                    lblTitle.SetBackgroundResource(Resource.Color.btnAndTitleBackgroundGreen);
                    btnStore.SetBackgroundResource(Resource.Color.btnAndTitleBackgroundGreen);
                    btnClear.SetBackgroundResource(Resource.Color.btnAndTitleBackgroundGreen);
                    layMeterFault.Visibility = Android.Views.ViewStates.Gone;
                    checkBox.Visibility = Android.Views.ViewStates.Gone;
                    lblButtonStore.Text = "store";
                    //StockList = new string[] { "Bunk" }; 
                }
                else if (fuelTypeSpinner.SelectedItem.Equals("Shortage"))
                {
                    lblButtonStore.Text = "Update";
                    fuelFormSpinner.Adapter = null;
                    vehicleNumber.Text = string.Empty;
                    driverNameSpinner.Adapter = null;
                    lblTitle.SetBackgroundResource(Resource.Color.borderColor);
                    FindViewById<LinearLayout>(Resource.Id.layFuelEntry).SetBackgroundResource(Resource.Color.borderColor);
                    btnStore.SetBackgroundColor(Color.White);
                    btnClear.SetBackgroundColor(Color.White);
                    layMeterFault.Visibility = Android.Views.ViewStates.Gone;
                    checkBox.Visibility = Android.Views.ViewStates.Gone;
                    imgFuel.Visibility = Android.Views.ViewStates.Gone;
                }
                else
                {
                    fuelFormSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, new string[] { "Stock", "Bunk" });
                    layMeterFault.Visibility = Android.Views.ViewStates.Visible;
                    lblButtonStore.Text = "store";
                    FindViewById<LinearLayout>(Resource.Id.layFuelEntry).SetBackgroundResource(Resource.Color.borderColor);
                    lblTitle.SetBackgroundResource(Resource.Color.borderColor);
                    lblTitle.SetTextColor(Color.Black);
                    btnStore.SetBackgroundColor(Color.White);
                    btnClear.SetBackgroundColor(Color.White);
                    layMeterFault.Visibility = Android.Views.ViewStates.Visible;
                    checkBox.Visibility = Android.Views.ViewStates.Visible;
                    // StockList = new string[] { "Stock", "Bunk" };
                }
                ClearAllFields();
                //fuelFormSpinner.PerformClick();
                //fuelFormSpinnerAdapter.NotifyDataSetChanged();
            };


            fuelFormSpinner.ItemSelected += (s, e) =>
                {
                    if (fuelFormSpinner.SelectedItem.Equals("Stock"))
                    {
                        bunkDetailsLayout.Visibility = Android.Views.ViewStates.Gone;
                        lblTitle.SetTextColor(Color.Black);
                        btnStore.SetBackgroundColor(Color.White);
                        btnClear.SetBackgroundColor(Color.White);
                        lblTitle.SetBackgroundResource(Resource.Color.borderColor);
                        FindViewById<LinearLayout>(Resource.Id.layFuelEntry).SetBackgroundResource(Resource.Color.borderColor);
                        imgFuel.Visibility = Android.Views.ViewStates.Gone;
                    }
                    else
                    {
                        bunkDetailsLayout.Visibility = Android.Views.ViewStates.Visible;
                        if (fuelTypeSpinner.SelectedItem.Equals("Outward"))
                        {
                            FindViewById<LinearLayout>(Resource.Id.layFuelEntry).SetBackgroundResource(Resource.Color.backgroundBunk);
                            btnStore.SetBackgroundColor(Color.Brown);
                            btnClear.SetBackgroundColor(Color.Brown);
                            lblTitle.SetBackgroundResource(Resource.Color.btnAndTitleBackgroundRed);
                        }
                        //else
                        //{
                        //    btnStore.SetBackgroundColor(Color.Red);
                        //    lblTitle.SetBackgroundColor(Color.Red);
                        //    FindViewById<LinearLayout>(Resource.Id.layFuelEntry).SetBackgroundColor(Color.White);
                        //}
                        imgFuel.Visibility = Android.Views.ViewStates.Visible;
                        //btnStore.SetCompoundDrawables(Resources.GetDrawable(Resource.Drawable.ic_launcher), null, null, null);
                    }
                };
            ShowLoader(false);
        }

        private async Task SentAndStoreShortage()
        {
            var list = new List<UploadDetails>
            {
                new UploadDetails
                {
                    DID = AppPreferences.GetString(this, Utilities.DEVICEID) == string.Empty ? "0" : AppPreferences.GetString(this, Utilities.DEVICEID),
                    SID = AppPreferences.GetString(this, Utilities.SITEID) == string.Empty ? "0" : AppPreferences.GetString(this, Utilities.SITEID),
                    IsShortage = "1",
                    ShortageLtr = Convert.ToDecimal(availableFuel),
                    FuelDate = DateTime.Now.ToString(Utilities.MONTH_DATE_TIME,CultureInfo.InvariantCulture)
                }
            };
            var deserializedData = JsonConvert.SerializeObject(list);
            //Console.WriteLine(deserializedData);
            var resposeString = WebService.PostAllDataToWebService("UPFStock", deserializedData);
            if (resposeString != null)
            {
                try
                {
                    var VehicleList = JsonConvert.DeserializeObject<List<VehicleDetails>>(resposeString);
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Please wait", ToastLength.Short).Show();
                    });
                    CreateDatabaseOrModifyDatabase(VehicleList);
                }
                catch
                {
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Something went wrong...", ToastLength.Short).Show();
                    });
                }
            }
            else
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "Error in shortage update", ToastLength.Short).Show();
                });
            }
            RunOnUiThread(() => ShowLoader(false));
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            vehicleNumber.Text = string.Empty;
            driverNameSpinner.Adapter = null;
            vehicleTypeSpinner.Adapter = null;
            fuelToFill.Text = string.Empty;
            txtClosingKMS.Text = string.Empty;
            txtRemarks.Text = string.Empty;
        }

        private async Task ShowLoader(bool isToShow)
        {
            if (isToShow)
            {
                loader.Visibility = Android.Views.ViewStates.Visible;
                Window.SetFlags(Android.Views.WindowManagerFlags.NotTouchable, Android.Views.WindowManagerFlags.NotTouchable);
                layFuelEntry.Alpha = 0.5f;
            }
            else
            {
                loader.Visibility = Android.Views.ViewStates.Gone;
                Window.ClearFlags(Android.Views.WindowManagerFlags.NotTouchable);
                layFuelEntry.Alpha = 1f;
            }
        }

        public async Task CreateDatabaseOrModifyDatabase(List<VehicleDetails> vehicleList)
        {
            DeleteDatabase(FuelDB.Singleton.DBPath);
            FuelDB.Singleton.CreateTable<VehicleDetails>();
            FuelDB.Singleton.CreateTable<BillDetails>();

            //FuelDB.Singleton.CreateDatabase<Fuel>();

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
            //btnDownloadData.Clickable = false;
            FuelDB.Singleton.InsertBillDetails(billDetails);
            //loader.Visibility = Android.Views.ViewStates.Gone;
            layFuelEntry.Alpha = 1f;
            //RunOnUiThread(() =>
            //{
            Finish();
            StartActivity(Intent);
            Toast.MakeText(this, "Shortage uploaded successfully", ToastLength.Short).Show();
            //});
        }
        private void VehicleTypeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (!isVehicleTypeSpinnerSelected)
            {
                isVehicleTypeSpinnerSelected = true;
                vehicleTypeSpinner.PerformClick();
            }
            else
            {
                DriverNames = VehicleList.Where(I => I.RegNo == vehicleNumber.Text).Select(I => I.DriverName).Distinct().ToArray();
                driverNameSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, DriverNames);
                driverNameSpinner.PerformClick();
                //if (driverNameSpinner.Adapter != null && !vehicleTypeSpinner.SelectedItem.ToString().Equals(string.Empty))
                //{
                //}
                isDriverNameSpinnerSelected = false;
            }
        }

        private void ClearAllFields()
        {
            fuelToFill.Text = string.Empty;
            // fuelAvailable.Text = string.Empty;
            //txtOpeningKMS.Text = string.Empty;
            txtClosingKMS.Text = string.Empty;
            txtFilledBy.Text = string.Empty;
            txtRate.Text = string.Empty;
            lblTotalPrice.Text = string.Empty;
            txtRemarks.Text = string.Empty;
        }

        private void GetKMPL()
        {
            var start = Convert.ToDecimal(txtOpeningKMS?.Text);
            var end = Convert.ToDecimal(txtClosingKMS?.Text);
            var result = ((end - start) / Convert.ToDecimal(fuelToFill?.Text));
            lblkmpl.Text = Math.Round(result, 2).ToString();
        }

        private void CheckFuelAvailbility()
        {
            try
            {
                availableFuel = float.Parse(billDetailsList?.AvailableLiters);
                if (string.IsNullOrEmpty(fuelToFill.Text))
                {
                    if (!fuelTypeSpinner.SelectedItem.Equals("Shortage"))
                    {
                        fuelAvailable.Text = $"{billDetailsList.AvailableLiters}";
                    }
                    return;
                }
                if (fuelTypeSpinner.SelectedItem.Equals("Outward"))
                {
                    if (!fuelToFill.Text.Equals(".") && fuelFormSpinner.SelectedItem.Equals("Stock"))
                    {
                        if (float.Parse(fuelToFill.Text) <= float.Parse(billDetailsList.AvailableLiters) + 50)
                        {
                            if (float.Parse(fuelToFill.Text) > float.Parse(billDetailsList.AvailableLiters))
                            {
                                ExcessLiter = float.Parse(fuelToFill.Text) - float.Parse(billDetailsList.AvailableLiters);
                                isExcess = true;
                            }
                            else
                            {
                                ExcessLiter = 0f;
                                isExcess = false;
                            }
                            var totalLtrs = (float.Parse(billDetailsList?.AvailableLiters) - float.Parse(fuelToFill.Text));
                            availableFuel = totalLtrs < 1 ? 0f : availableFuel;
                        }
                        else
                        {
                            var alertDialog = new Android.App.AlertDialog.Builder(this);
                            alertDialog.SetTitle("Fuel exceeds stock");
                            alertDialog.SetMessage("Please note fuel availability");
                            alertDialog.SetCancelable(false);
                            alertDialog.SetPositiveButton("OK", (s, e) =>
                            {
                                fuelToFill.Text = string.Empty;
                            });
                            //alertDialog.SetNegativeButton("Cancel", (s, e) => { });
                            alertDialog.Show();
                        }
                    }
                }
                else
                {
                    //Toast.MakeText(this, "No stock available..", ToastLength.Short).Show();
                    availableFuel = float.Parse(billDetailsList?.AvailableLiters) + float.Parse(fuelToFill.Text);
                }
                if (!string.IsNullOrEmpty(txtOpeningKMS.Text) && !string.IsNullOrEmpty(txtClosingKMS.Text)
                    && string.IsNullOrEmpty(fuelToFill.Text))
                {
                    GetKMPL();
                }
                //fuelAvailable.Text = $"{availableFuel.ToString()}" + "ltrs";
            }
            catch
            {
                Toast.MakeText(this, "could not load data please sync again", ToastLength.Short).Show();
            }
        }

        private void StoreDetils()
        {
            try
            {
                if (txtClosingKMS.Text != string.Empty && txtOpeningKMS.Text != string.Empty)
                {
                    fuelDetails = new FuelEntryDetails
                    {
                        BillNumber = billNumber.Text == string.Empty ? "0" : billNumber.Text,
                        CurrentDate = DateTime.Now.ToString(Utilities.MONTH_DATE_TIME, CultureInfo.InvariantCulture),
                        FuelType = fuelTypeSpinner.SelectedItem.ToString(),
                        FuelStockType = fuelFormSpinner.SelectedItem.ToString(),
                        VehicleNumber = vehicleNumber.Text,
                        VehicleType = vehicleTypeSpinner.SelectedItem.ToString(),
                        DriverName = driverNameSpinner.SelectedItem.ToString(),
                        FuelInLtrs = fuelToFill.Text == string.Empty ? "0" : fuelToFill.Text,
                        FilledBy = txtFilledBy.Text,
                        ClosingKMS = txtClosingKMS.Text == string.Empty ? "0" : txtClosingKMS.Text,
                        Kmpl = lblkmpl.Text == "KMPL" ? "0" : lblkmpl.Text,
                        OpeningKMS = txtOpeningKMS.Text == string.Empty ? "0" : txtOpeningKMS.Text,
                        PaymentType = cashModeSpinner.SelectedItem?.ToString(),
                        Price = lblTotalPrice.Text == string.Empty ? "0" : lblTotalPrice.Text,
                        RatePerLtr = txtRate.Text == string.Empty ? "0" : txtRate.Text,
                        Remarks = txtRemarks.Text,
                        VID = VehicleList.Where(I => I.RegNo == vehicleNumber.Text).Select(i => i.VID).First(),
                        DriverID_PK = VehicleList.Where(I => I.RegNo == vehicleNumber.Text).First().DriverID_PK,
                        MeterFault = checkBox.Checked == true ? "1" : "0",
                        TotalKM = (Convert.ToDouble(txtClosingKMS.Text) - Convert.ToDouble(txtOpeningKMS.Text)).ToString(),
                        IsExcess = isExcess ? "1" : "0",
                        ExcessLtr = Convert.ToDecimal(ExcessLiter)
                    };
                }
                else
                {
                    fuelDetails = new FuelEntryDetails
                    {
                        BillNumber = billNumber.Text == string.Empty ? "0" : billNumber.Text,
                        CurrentDate = DateTime.Now.ToString(Utilities.MONTH_DATE_TIME, CultureInfo.InvariantCulture),
                        FuelType = fuelTypeSpinner.SelectedItem.ToString(),
                        FuelStockType = fuelFormSpinner.SelectedItem.ToString(),
                        VehicleNumber = vehicleNumber.Text,
                        VehicleType = vehicleTypeSpinner.SelectedItem.ToString(),
                        DriverName = driverNameSpinner.SelectedItem.ToString(),
                        FuelInLtrs = fuelToFill.Text == string.Empty ? "0" : fuelToFill.Text,
                        FilledBy = txtFilledBy.Text,
                        ClosingKMS = txtClosingKMS.Text == string.Empty ? "0" : txtClosingKMS.Text,
                        Kmpl = lblkmpl.Text == "KMPL" ? "0" : lblkmpl.Text,
                        OpeningKMS = txtOpeningKMS.Text == string.Empty ? "0" : txtOpeningKMS.Text,
                        PaymentType = cashModeSpinner.SelectedItem?.ToString(),
                        Price = lblTotalPrice.Text == string.Empty ? "0" : lblTotalPrice.Text,
                        RatePerLtr = txtRate.Text == string.Empty ? "0" : txtRate.Text,
                        Remarks = txtRemarks.Text,
                        VID = VehicleList.Where(I => I.RegNo == vehicleNumber.Text).Select(i => i.VID).First(),
                        DriverID_PK = VehicleList.Where(I => I.RegNo == vehicleNumber.Text).First().DriverID_PK,
                        MeterFault = checkBox.Checked == true ? "1" : "0",
                        TotalKM = "0",
                        IsExcess = isExcess ? "1" : "0",
                        ExcessLtr = Convert.ToDecimal(ExcessLiter)
                    };
                }
            }

            catch (Exception ec)
            {
                Console.WriteLine(ec.Message);
                Toast.MakeText(this,"Error in storing the values", ToastLength.Short).Show();
                return;
            }
            //var fuelBalance = new BillDetails
            //{
            //    AvailableLiters = availableFuel.ToString()
            //};         
            FuelDB.Singleton.CreateTable<FuelEntryDetails>();
            FuelDB.Singleton.InsertFuelEntryValues(fuelDetails);
            FuelDB.Singleton.UpdateFuel(availableFuel.ToString());
            var printValues = StorePrintDetails(fuelDetails);

            Intent intent = new Intent(this, typeof(VehicleDetailActivity));
            intent.PutExtra("printDetails", JsonConvert.SerializeObject(printValues));
            StartActivity(intent);
        }

        private PrintDetails StorePrintDetails(FuelEntryDetails fuelDetails)
        {
            if (fuelDetails.FuelType.Equals("Inwards"))
            {
                printDetails = new PrintDetails
                {
                    BillNumber = fuelDetails.BillNumber,
                    CurrentDate = DateTime.Now.ToString(Utilities.MONTH_DATE_TIME, CultureInfo.InvariantCulture),
                    FuelType = fuelDetails.FuelType,
                    FuelStockType = fuelDetails.FuelStockType,
                    VehicleNumber = fuelDetails.VehicleNumber,
                    VehicleType = fuelDetails.VehicleType,
                    DriverName = fuelDetails.DriverName,
                    FuelInLtrs = fuelDetails.FuelInLtrs,
                    FilledBy = fuelDetails.FilledBy,
                    PaymentType = fuelDetails.PaymentType,
                    RatePerLtr = fuelDetails.RatePerLtr,
                    Price = fuelDetails.Price,
                    Remarks = fuelDetails.Remarks
                };
            }
            else
            {
                if (fuelDetails.FuelStockType.Equals("Stock"))
                {
                    if (fuelDetails.MeterFault.Equals("0"))
                    {
                        printDetails = new PrintDetails
                        {
                            BillNumber = fuelDetails.BillNumber,
                            CurrentDate = DateTime.Now.ToString(Utilities.MONTH_DATE_TIME, CultureInfo.InvariantCulture),
                            FuelType = fuelDetails.FuelType,
                            FuelStockType = fuelDetails.FuelStockType,
                            VehicleNumber = fuelDetails.VehicleNumber,
                            VehicleType = fuelDetails.VehicleType,
                            DriverName = fuelDetails.DriverName,
                            FuelInLtrs = fuelDetails.FuelInLtrs,
                            FilledBy = fuelDetails.FilledBy,
                            OpeningKMS = fuelDetails.OpeningKMS,
                            ClosingKMS = fuelDetails.ClosingKMS,
                            Kmpl = fuelDetails.Kmpl,
                            Remarks = fuelDetails.Remarks
                        };
                    }
                    else
                    {
                        printDetails = new PrintDetails
                        {
                            BillNumber = fuelDetails.BillNumber,
                            CurrentDate = DateTime.Now.ToString(Utilities.MONTH_DATE_TIME, CultureInfo.InvariantCulture),
                            FuelType = fuelDetails.FuelType,
                            FuelStockType = fuelDetails.FuelStockType,
                            VehicleNumber = fuelDetails.VehicleNumber,
                            VehicleType = fuelDetails.VehicleType,
                            DriverName = fuelDetails.DriverName,
                            FuelInLtrs = fuelDetails.FuelInLtrs,
                            FilledBy = fuelDetails.FilledBy,
                            MeterFault = fuelDetails.MeterFault,
                            Remarks = fuelDetails.Remarks
                        };
                    }
                }
                else
                {
                    if (fuelDetails.MeterFault.Equals("0"))
                    {
                        printDetails = new PrintDetails
                        {
                            BillNumber = fuelDetails.BillNumber,
                            CurrentDate = DateTime.Now.ToString(Utilities.MONTH_DATE_TIME, CultureInfo.InvariantCulture),
                            FuelType = fuelDetails.FuelType,
                            FuelStockType = fuelDetails.FuelStockType,
                            VehicleNumber = fuelDetails.VehicleNumber,
                            VehicleType = fuelDetails.VehicleType,
                            DriverName = fuelDetails.DriverName,
                            FuelInLtrs = fuelDetails.FuelInLtrs,
                            FilledBy = fuelDetails.FilledBy,
                            OpeningKMS = fuelDetails.OpeningKMS,
                            ClosingKMS = fuelDetails.ClosingKMS,
                            Kmpl = fuelDetails.Kmpl,
                            PaymentType = fuelDetails.PaymentType,
                            RatePerLtr = fuelDetails.RatePerLtr,
                            Price = fuelDetails.Price,
                            Remarks = fuelDetails.Remarks
                        };
                    }
                    else
                    {
                        printDetails = new PrintDetails
                        {
                            BillNumber = fuelDetails.BillNumber,
                            CurrentDate = DateTime.Now.ToString(Utilities.MONTH_DATE_TIME, CultureInfo.InvariantCulture),
                            FuelType = fuelDetails.FuelType,
                            FuelStockType = fuelDetails.FuelStockType,
                            VehicleNumber = fuelDetails.VehicleNumber,
                            VehicleType = fuelDetails.VehicleType,
                            DriverName = fuelDetails.DriverName,
                            FuelInLtrs = fuelDetails.FuelInLtrs,
                            FilledBy = fuelDetails.FilledBy,
                            PaymentType = fuelDetails.PaymentType,
                            RatePerLtr = fuelDetails.RatePerLtr,
                            Price = fuelDetails.Price,
                            MeterFault = fuelDetails.MeterFault,
                            Remarks = fuelDetails.Remarks
                        };
                    }
                }
                //var json = 
            }
            return printDetails;
        }
        //FuelType = new FuelType
        //{
        //    IsInward = false,
        //    Outward = new Outward
        //    {
        //        MeterDetails = new MeterDetails
        //        {
        //            IsMeterFault = fuelDetails.MeterFault == "0" ? false : true,
        //            OpeningKM = fuelDetails.OpeningKMS,
        //            ClosingKM = fuelDetails.ClosingKMS,
        //            KMPL = fuelDetails.Kmpl
        //        },
        //        StockType = new StockType
        //        {
        //            IsStock = true
        //        }
        //    }
        //},
        private void CalculateFuelTotalAmount()
        {
            if (!fuelToFill.Text.Equals(string.Empty) && !txtRate.Text.Equals(string.Empty))
            {
                lblTotalPrice.Text = (float.Parse(fuelToFill.Text) * float.Parse(txtRate.Text)).ToString();
            }
        }

        private void VehicleNumber_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (vehicleNumber.Text.Equals(string.Empty))
            {
                driverNameSpinner.Adapter = null;
                vehicleTypeSpinner.Adapter = null;
                ClearAllFields();
            }
        }

        private void VehicleNumber_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (VehicleList != null)
            {
                vehicleTypeSpinner.Adapter = vehicleTypeAdapter;
                vehicleTypeSpinner.PerformClick();
                isVehicleTypeSpinnerSelected = false;

                InputMethodManager inputManager = (InputMethodManager)GetSystemService(InputMethodService);
                inputManager.HideSoftInputFromWindow(Window.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
            }
        }
    }
}