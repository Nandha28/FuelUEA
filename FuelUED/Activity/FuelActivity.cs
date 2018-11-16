using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views.InputMethods;
using Android.Widget;
using FuelApp.Modal;
using FuelUED.CommonFunctions;
using FuelUED.Modal;
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
            StockList = new string[] { ConstantValues.STOCK, ConstantValues.BUNK };
        }

        public string[] VehicleNumber;

        public string[] StockList;

        private List<VehicleDetails> VehicleList;
        private List<FuelEntryDetails> billEntryList;
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
        private bool isDriverNameSpinnerSelected;
        private float ExcessLiter;
        private bool isExcess;
        private bool isInward;
        private bool isVehicleTypeSpinnerSelected;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.FuelEntry);

            alertDialog = new Android.App.AlertDialog.Builder(this);
            alertDialog.SetTitle("Fuel is from petrol bunk");
            alertDialog.SetMessage(ConstantValues.PROCEED);
            alertDialog.SetPositiveButton(ConstantValues.OK, (s, e) =>
             {
                 StoreDetils();
             });
            alertDialog.SetNegativeButton(ConstantValues.CANCEL, (s, e) => { });
            loader = FindViewById<ProgressBar>(Resource.Id.loader);
            layFuelEntry = FindViewById<LinearLayout>(Resource.Id.layFuelEntry);
            try
            {
                VehicleList = FuelDB.Singleton.GetValue().ToList();
                var allvAl = FuelDB.Singleton.GetBillDetails().ToList();
                billDetailsList = allvAl.FirstOrDefault();
                billEntryList = FuelDB.Singleton.GetFuelValues()?.ToList();
            }
            catch (Exception w)
            {
                //Console.WriteLine(w.Message);
            }
            if (VehicleList != null)
            {
                myVehiclelist = VehicleList.Select(I => I.RegNo).Distinct().ToArray();
            }

            lblTitle = FindViewById<TextView>(Resource.Id.lblTittle);
            btnClear = FindViewById<Button>(Resource.Id.btnClear);
            vehicleNumber = FindViewById<AutoCompleteTextView>(Resource.Id.vehicleNumber);
            if (myVehiclelist != null)
            {
                var adapter = new ArrayAdapter<string>(this, Resource.Layout.select_dialog_item_material, myVehiclelist);
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
                new string[]
                {
                    ConstantValues.OUTWARD, ConstantValues.INWARD,
                    ConstantValues.EXCESS, ConstantValues.SHORTAGE
                });

            fuelFormSpinner = FindViewById<Spinner>(Resource.Id.fuelFormSpinner);
            fuelFormSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, StockList);

            var bunkDetailsLayout = FindViewById<LinearLayout>(Resource.Id.layBunkDetails);

            cashModeSpinner = FindViewById<Spinner>(Resource.Id.paymentMode);
            cashModeSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material,
                new string[]
                {
                    ConstantValues.CASH, ConstantValues.CREDIT
                });

            vehicleTypeSpinner = FindViewById<Spinner>(Resource.Id.vehicleType);

            vehicleTypeSpinner.ItemSelected += VehicleTypeSpinner_ItemSelected;

            btnClear.Click += BtnClear_Click;

            var layMeterFault = FindViewById<LinearLayout>(Resource.Id.layMeterFault);

            checkBox = FindViewById<CheckBox>(Resource.Id.chckMeterFault);
            checkBox.CheckedChange += (s, e) =>
            {
                if (fuelTypeSpinner.SelectedItem.ToString() == ConstantValues.OUTWARD)
                {
                    layMeterFault.Visibility = checkBox.Checked ? Android.Views.ViewStates.Gone : Android.Views.ViewStates.Visible;
                }
                txtClosingKMS.Text = string.Empty;
                lblkmpl.Text = ConstantValues.KMPL;
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
                if (txtOpeningKMS.Text.Equals(ConstantValues.ZERO))
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
                        lblkmpl.Text = ConstantValues.KMPL;
                    }
                }
            };

            var btnStore = FindViewById<LinearLayout>(Resource.Id.btnStore);
            btnStore.Click += (s, e) =>
            {
                try
                {
                    btnStore.Clickable = false;
                    ShowLoader(true);
                    if (fuelTypeSpinner.SelectedItem.Equals(ConstantValues.OUTWARD) &&
                    !fuelToFill.Text.Equals(".") && fuelFormSpinner.SelectedItem.Equals(ConstantValues.STOCK))
                    {
                        if (float.Parse(fuelToFill.Text) > float.Parse(billDetailsList.AvailableLiters))
                        {
                            var alertDialog = new Android.App.AlertDialog.Builder(this);
                            alertDialog.SetTitle("Fuel exceeds stock");
                            alertDialog.SetMessage($"Do you want add " +
                                $"{float.Parse(fuelToFill.Text) - float.Parse(billDetailsList.AvailableLiters)} ltrs. to excess fuel");
                            alertDialog.SetCancelable(false);
                            alertDialog.SetPositiveButton(ConstantValues.OK, (ss, es) =>
                            {
                                ExcessLiter = float.Parse(fuelToFill.Text) - float.Parse(billDetailsList.AvailableLiters);
                                isExcess = true;
                                StoreDetils();
                            });
                            alertDialog.SetNegativeButton(ConstantValues.CANCEL, (ss, se) =>
                            {
                                fuelToFill.Text = string.Empty;
                                ExcessLiter = 0f;
                                btnStore.Clickable = true;
                                isExcess = false;
                                alertDialog.Dispose();
                                ShowLoader(false);
                            });
                            alertDialog.Show();
                            var totalLtrs = (float.Parse(billDetailsList?.AvailableLiters) - float.Parse(fuelToFill.Text));
                            availableFuel = totalLtrs < 1 ? 0f : totalLtrs;
                            return;
                        }
                        else
                        {
                            var totalLtrs = (float.Parse(billDetailsList?.AvailableLiters) - float.Parse(fuelToFill.Text));
                            availableFuel = totalLtrs < 1 ? 0f : totalLtrs;
                        }
                    }

                    if (fuelTypeSpinner.SelectedItem.Equals(ConstantValues.SHORTAGE))
                    {
                        var alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle($"Shortage {availableFuel} ltrs.");
                        alertDialog.SetMessage(ConstantValues.PROCEED);
                        alertDialog.SetCancelable(false);
                        alertDialog.SetPositiveButton(ConstantValues.YES, (ss, se) =>
                        {
                            UpdateShortageAndRefreshCurrentActivity();
                            fuelAvailable.Text = string.Empty;
                            availableFuel = 0;

                        });
                        alertDialog.SetNegativeButton(ConstantValues.NO, (ss, se) =>
                        {
                            alertDialog.Dispose();
                            btnStore.Clickable = true;
                            ShowLoader(false);
                        });
                        alertDialog.Show();
                        return;
                    }
                    if (fuelTypeSpinner.SelectedItem.Equals(ConstantValues.INWARD) || fuelTypeSpinner.SelectedItem.Equals(ConstantValues.EXCESS))
                    {
                        if (!fuelAvailable.Text.Equals(ConstantValues.ZERO))
                        {
                            var alertDialog = new Android.App.AlertDialog.Builder(this);
                            alertDialog.SetTitle("Adding fuel");
                            alertDialog.SetMessage("Do you need to add fuel to existing");
                            alertDialog.SetCancelable(false);
                            alertDialog.SetPositiveButton(ConstantValues.YES, (ss, se) =>
                            {
                                fuelAvailable.Text = (Convert.ToInt32(fuelAvailable.Text) + Convert.ToInt32(fuelToFill.Text)).ToString();
                                availableFuel = float.Parse(billDetailsList?.AvailableLiters) + float.Parse(fuelToFill.Text);
                                StoreDetils();
                            });
                            alertDialog.SetNegativeButton(ConstantValues.NO, (ss, se) =>
                            {
                                alertDialog.Dispose();
                                ShowLoader(false);
                                btnStore.Clickable = true;
                            });
                            alertDialog.Show();
                            return;
                        }
                        else
                        {
                            fuelAvailable.Text = (Convert.ToInt32(fuelAvailable.Text) + Convert.ToInt32(fuelToFill.Text)).ToString();
                            availableFuel = float.Parse(billDetailsList?.AvailableLiters) + float.Parse(fuelToFill.Text);
                        }

                    }              
                    if (fuelFormSpinner.SelectedItem.Equals(ConstantValues.BUNK) && !fuelTypeSpinner.SelectedItem.Equals(ConstantValues.INWARD))
                    {
                        alertDialog.Show();
                    }
                    else
                    {
                        StoreDetils();
                    }
                    ShowLoader(false);
                }
                catch
                {
                    Toast.MakeText(this, "Check all fields are filled...", ToastLength.Short).Show();
                    ShowLoader(false);
                }
            };

            fuelTypeSpinner.ItemSelected += (s, e) =>
            {
                ClearAllFields();
                fuelFormSpinner.Adapter = null;
                fuelToFill.Enabled = true;
                vehicleNumber.Enabled = true;
                vehicleNumber.Focusable = true;
                fuelToFill.Focusable = true;
                fuelToFill.FocusableInTouchMode = true;
                if (fuelTypeSpinner.SelectedItem.Equals(ConstantValues.INWARD) || fuelTypeSpinner.SelectedItem.Equals(ConstantValues.EXCESS))
                {
                    vehicleNumber.Text = string.Empty;
                    if (fuelTypeSpinner.SelectedItem.Equals(ConstantValues.INWARD))
                    {
                        fuelFormSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, new string[] { ConstantValues.BUNK });
                    }
                    else
                    {
                        fuelFormSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, new string[] { ConstantValues.STOCK });
                    }
                    layMeterFault.Visibility = Android.Views.ViewStates.Gone;
                    FindViewById<LinearLayout>(Resource.Id.layFuelEntry).SetBackgroundResource(Resource.Color.backgroundInward);
                    lblTitle.SetBackgroundResource(Resource.Color.btnAndTitleBackgroundGreen);
                    btnStore.SetBackgroundResource(Resource.Color.btnAndTitleBackgroundGreen);
                    btnClear.SetBackgroundResource(Resource.Color.btnAndTitleBackgroundGreen);
                    layMeterFault.Visibility = Android.Views.ViewStates.Gone;
                    checkBox.Visibility = Android.Views.ViewStates.Gone;
                    lblButtonStore.Text = ConstantValues.STORE;
                    isInward = true;
                }
                else if (fuelTypeSpinner.SelectedItem.Equals(ConstantValues.SHORTAGE))
                {
                    lblButtonStore.Text = ConstantValues.UPDATE;
                    fuelFormSpinner.Adapter = null;
                    vehicleTypeSpinner.Adapter = null;
                    driverNameSpinner.Adapter = null;
                    vehicleNumber.Enabled = false;
                    fuelToFill.Focusable = false;
                    fuelToFill.Text = availableFuel.ToString();
                    vehicleNumber.Text = ConstantValues.ZERO;
                    lblTitle.SetBackgroundResource(Resource.Color.borderColor);
                    FindViewById<LinearLayout>(Resource.Id.layFuelEntry).SetBackgroundResource(Resource.Color.borderColor);
                    btnStore.SetBackgroundColor(Color.White);
                    btnClear.SetBackgroundColor(Color.White);
                    layMeterFault.Visibility = Android.Views.ViewStates.Gone;
                    checkBox.Visibility = Android.Views.ViewStates.Gone;
                    imgFuel.Visibility = Android.Views.ViewStates.Gone;
                    isInward = false;
                }
                else
                {
                    fuelFormSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material,
                        new string[] {
                            ConstantValues.STOCK, ConstantValues.BUNK
                        });
                    layMeterFault.Visibility = Android.Views.ViewStates.Visible;
                    lblButtonStore.Text = ConstantValues.STORE;
                    vehicleNumber.Text = string.Empty;
                    FindViewById<LinearLayout>(Resource.Id.layFuelEntry).SetBackgroundResource(Resource.Color.borderColor);
                    lblTitle.SetBackgroundResource(Resource.Color.borderColor);
                    lblTitle.SetTextColor(Color.Black);
                    btnStore.SetBackgroundColor(Color.White);
                    btnClear.SetBackgroundColor(Color.White);
                    layMeterFault.Visibility = Android.Views.ViewStates.Visible;
                    checkBox.Visibility = Android.Views.ViewStates.Visible;
                    isInward = false;
                }
            };


            fuelFormSpinner.ItemSelected += (s, e) =>
                {
                    if (fuelFormSpinner.SelectedItem.Equals(ConstantValues.STOCK))
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
                        if (fuelTypeSpinner.SelectedItem.Equals(ConstantValues.OUTWARD))
                        {
                            FindViewById<LinearLayout>(Resource.Id.layFuelEntry).SetBackgroundResource(Resource.Color.backgroundBunk);
                            btnStore.SetBackgroundColor(Color.Brown);
                            btnClear.SetBackgroundColor(Color.Brown);
                            lblTitle.SetBackgroundResource(Resource.Color.btnAndTitleBackgroundRed);
                        }
                        imgFuel.Visibility = Android.Views.ViewStates.Visible;
                    }
                };
            ShowLoader(false);
        }

        private void UpdateShortageAndRefreshCurrentActivity()
        {
            fuelDetails = new FuelEntryDetails
            {
                BillNumber = billNumber.Text == string.Empty ? ConstantValues.ZERO : billNumber.Text,
                CurrentDate = DateTime.Now.ToString(Utilities.MONTH_DATE_TIME, CultureInfo.InvariantCulture),
                FuelType = ConstantValues.SHORTAGE,
                FuelStockType = ConstantValues.ZERO,
                VehicleNumber = ConstantValues.ZERO,
                VehicleType = ConstantValues.ZERO,
                DriverName = ConstantValues.ZERO,
                FuelInLtrs = ConstantValues.ZERO,
                FilledBy = ConstantValues.ZERO,
                ClosingKMS = ConstantValues.ZERO,
                Kmpl = ConstantValues.ZERO,
                OpeningKMS = ConstantValues.ZERO,
                PaymentType = ConstantValues.ZERO,
                Price = ConstantValues.ZERO,
                RatePerLtr = ConstantValues.ZERO,
                Remarks = ConstantValues.ZERO,
                VID = ConstantValues.ZERO,
                DriverID_PK = ConstantValues.ZERO,
                MeterFault = ConstantValues.ZERO,
                TotalKM = ConstantValues.ZERO,
                IsExcess = ConstantValues.ZERO,
                ExcessLtr = 0,
                IsShortage = ConstantValues.ONE,
                ShortageLtr = Convert.ToDecimal(availableFuel)
            };
            SaveDetailsToDb();
            InsertUpadatedBillDetails();
            RefreshCurrent();
        }

        private void InsertUpadatedBillDetails()
        {
            var bill = FuelDB.Singleton.GetBillDetails();
            var billall = bill.First();
            var billDetails = new BillDetails
            {
                AvailableLiters = ConstantValues.ZERO,
                BillCurrentNumber = (Convert.ToInt32(billall.BillCurrentNumber) + 1).ToString(),
                BillPrefix = billall.BillPrefix,
                DeviceStatus = billall.DeviceStatus
            };
            FuelDB.Singleton.DeleteTable<BillDetails>();
            FuelDB.Singleton.CreateTable<BillDetails>();
            FuelDB.Singleton.InsertBillDetails(billDetails);
            availableFuel = 0f;
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

        private void VehicleTypeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {            
            DriverNames = VehicleList.Where(I => I.RegNo == vehicleNumber.Text).Select(I => I.DriverName).Distinct().ToArray();
            try
            {
                txtOpeningKMS.Text = billEntryList?.Where(I => I.VehicleNumber == vehicleNumber.Text).Last().ClosingKMS;
            }
            catch
            {
                txtOpeningKMS.Text = ConstantValues.ZERO;
            }
            driverNameSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, DriverNames);
            driverNameSpinner.PerformClick();

            isDriverNameSpinnerSelected = false;
        }

        private void ClearAllFields()
        {
            fuelToFill.Text = string.Empty;
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
                    if (!fuelTypeSpinner.SelectedItem.Equals(ConstantValues.SHORTAGE))
                    {
                        fuelAvailable.Text = $"{billDetailsList.AvailableLiters}";
                    }
                    return;
                }
                if (!string.IsNullOrEmpty(txtOpeningKMS.Text) && !string.IsNullOrEmpty(txtClosingKMS.Text)
                    && string.IsNullOrEmpty(fuelToFill.Text))
                {
                    GetKMPL();
                }
            }
            catch
            {
                Toast.MakeText(this, "Could not load data please sync again", ToastLength.Short).Show();
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
                        BillNumber = billNumber.Text == string.Empty ? ConstantValues.ZERO : billNumber.Text,
                        CurrentDate = DateTime.Now.ToString(Utilities.MONTH_DATE_TIME, CultureInfo.InvariantCulture),
                        FuelType = fuelTypeSpinner.SelectedItem.ToString(),
                        FuelStockType = fuelFormSpinner.SelectedItem.ToString(),
                        VehicleNumber = vehicleNumber.Text,
                        VehicleType = vehicleTypeSpinner.SelectedItem.ToString(),
                        DriverName = driverNameSpinner.SelectedItem.ToString(),
                        FuelInLtrs = fuelToFill.Text == string.Empty ? ConstantValues.ZERO : fuelToFill.Text,
                        FilledBy = txtFilledBy.Text,
                        ClosingKMS = txtClosingKMS.Text == string.Empty ? ConstantValues.ZERO : txtClosingKMS.Text,
                        Kmpl = lblkmpl.Text == ConstantValues.KMPL ? ConstantValues.ZERO : lblkmpl.Text,
                        OpeningKMS = txtOpeningKMS.Text == string.Empty ? ConstantValues.ZERO : txtOpeningKMS.Text,
                        PaymentType = cashModeSpinner.SelectedItem?.ToString(),
                        Price = lblTotalPrice.Text == string.Empty ? ConstantValues.ZERO : lblTotalPrice.Text,
                        RatePerLtr = txtRate.Text == string.Empty ? ConstantValues.ZERO : txtRate.Text,
                        Remarks = txtRemarks.Text,
                        VID = VehicleList.Where(I => I.RegNo == vehicleNumber.Text).Select(i => i.VID).First(),
                        DriverID_PK = VehicleList.Where(I => I.RegNo == vehicleNumber.Text).First().DriverID_PK,
                        MeterFault = checkBox.Checked == true ? ConstantValues.ONE : ConstantValues.ZERO,
                        TotalKM = (Convert.ToDouble(txtClosingKMS.Text) - Convert.ToDouble(txtOpeningKMS.Text)).ToString(),
                        IsExcess = isExcess ? ConstantValues.ONE : ConstantValues.ZERO,
                        ExcessLtr = isExcess ? Convert.ToDecimal(ExcessLiter) : 0m
                    };
                }
                else
                {
                    fuelDetails = new FuelEntryDetails
                    {
                        BillNumber = billNumber.Text == string.Empty ? ConstantValues.ZERO : billNumber.Text,
                        CurrentDate = DateTime.Now.ToString(Utilities.MONTH_DATE_TIME, CultureInfo.InvariantCulture),
                        FuelType = fuelTypeSpinner.SelectedItem.ToString(),
                        FuelStockType = fuelFormSpinner.SelectedItem.ToString(),
                        VehicleNumber = vehicleNumber.Text,
                        VehicleType = vehicleTypeSpinner.SelectedItem.ToString(),
                        DriverName = driverNameSpinner.SelectedItem.ToString(),
                        FuelInLtrs = fuelToFill.Text == string.Empty ? ConstantValues.ZERO : fuelToFill.Text,
                        FilledBy = txtFilledBy.Text,
                        ClosingKMS = txtClosingKMS.Text == string.Empty ? ConstantValues.ZERO : txtClosingKMS.Text,
                        Kmpl = lblkmpl.Text == ConstantValues.KMPL ? ConstantValues.ZERO : lblkmpl.Text,
                        OpeningKMS = txtOpeningKMS.Text == string.Empty ? ConstantValues.ZERO : txtOpeningKMS.Text,
                        PaymentType = cashModeSpinner.SelectedItem?.ToString(),
                        Price = lblTotalPrice.Text == string.Empty ? ConstantValues.ZERO : lblTotalPrice.Text,
                        RatePerLtr = txtRate.Text == string.Empty ? ConstantValues.ZERO : txtRate.Text,
                        Remarks = txtRemarks.Text,
                        VID = VehicleList.Where(I => I.RegNo == vehicleNumber.Text).Select(i => i.VID).First(),
                        DriverID_PK = VehicleList.Where(I => I.RegNo == vehicleNumber.Text).First().DriverID_PK,
                        MeterFault = checkBox.Checked == true ? ConstantValues.ONE : ConstantValues.ZERO,
                        TotalKM = ConstantValues.ZERO,
                        IsExcess = isExcess ? ConstantValues.ONE : ConstantValues.ZERO,
                        ExcessLtr = isExcess ? Convert.ToDecimal(ExcessLiter) : 0m
                    };
                }
            }

            catch (Exception ec)
            {
                //Console.WriteLine(ec.Message);
                Toast.MakeText(this, "Error in storing the values", ToastLength.Short).Show();
                return;
            }
            SaveDetailsToDb();
            NavigateToPrintScreen();
        }

        private void RefreshCurrent()
        {
            Finish();
            StartActivity(Intent);
            Toast.MakeText(this, "Shortage uploaded successfully", ToastLength.Short).Show();
        }

        private void NavigateToPrintScreen()
        {
            var printValues = StorePrintDetails(fuelDetails);
            Intent intent = new Intent(this, typeof(VehicleDetailActivity));
            intent.PutExtra("printDetails", JsonConvert.SerializeObject(printValues));
            StartActivity(intent);
        }

        private void SaveDetailsToDb()
        {
            FuelDB.Singleton.CreateTable<FuelEntryDetails>();
            FuelDB.Singleton.CreateTable<BillHistory>();
            var billhistory = new BillHistory
            {
                CurrentDate = fuelDetails.CurrentDate,
                FuelInLtrs = fuelDetails.FuelInLtrs,
                VehicleNumber = fuelDetails.VehicleNumber,
                IsInward = isInward
            };
            FuelDB.Singleton.InsertFuelEntryValues(fuelDetails);
            FuelDB.Singleton.InsertBillHistoryValues(billhistory);
            FuelDB.Singleton.UpdateFuel(availableFuel.ToString());
        }

        private PrintDetails StorePrintDetails(FuelEntryDetails fuelDetails)
        {
            if (fuelDetails.FuelType.Equals(ConstantValues.INWARD))
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
                if (fuelDetails.FuelStockType.Equals(ConstantValues.STOCK))
                {
                    if (fuelDetails.MeterFault.Equals(ConstantValues.ZERO))
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
                    if (fuelDetails.MeterFault.Equals(ConstantValues.ZERO))
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
            }
            return printDetails;
        }

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
            if (VehicleList != null && !fuelTypeSpinner.SelectedItem.Equals(ConstantValues.SHORTAGE))
            {
                var vehicleTypes = VehicleList.Where(q => q.RegNo == vehicleNumber.Text).Select(t => t.TypeName);
                vehicleTypeAdapter =
                    new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, vehicleTypes.ToArray());
                //new string[] { "Select", "Line Vehicle", "InterCard", "Loader", "Genset 1", "Genset 2", "Genset 3" });
                vehicleTypeSpinner.Adapter = vehicleTypeAdapter;
                //vehicleTypeSpinner.PerformClick();
                isVehicleTypeSpinnerSelected = false;

                InputMethodManager inputManager = (InputMethodManager)GetSystemService(InputMethodService);
                inputManager.HideSoftInputFromWindow(Window.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
            }
        }
    }
}