using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FuelApp.Modal;
using FuelUED.Modal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
        private TextView lblTitle;
        private AutoCompleteTextView vehicleNumber;
        private TextView billNumber;
        private string dateTimeNow;
        private Spinner fuelTypeSpinner;
        private Spinner fuelFormSpinner;
        private Spinner vehicleTypeSpinner;
        private FuelEntryDetails fuelDetails;
        private float availableFuel;
        private CheckBox checkBox;
        private PrintDetails printDetails;

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
            vehicleNumber = FindViewById<AutoCompleteTextView>(Resource.Id.vehicleNumber);
            if (myVehiclelist != null)
            {
                var adapter = new ArrayAdapter<String>(this, Resource.Layout.select_dialog_item_material, myVehiclelist);
                vehicleNumber.Adapter = adapter;
                vehicleNumber.ItemClick += VehicleNumber_ItemClick;
                vehicleNumber.TextChanged += VehicleNumber_TextChanged;
            }

            billNumber = FindViewById<TextView>(Resource.Id.txtBillNumber);
            billNumber.Text = billDetailsList?.BillPrefix + billDetailsList?.BillCurrentNumber;

            dateTimeNow = FindViewById<TextView>(Resource.Id.lbldateTime).Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

            fuelTypeSpinner = FindViewById<Spinner>(Resource.Id.fuelSpinner);
            fuelTypeSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, new string[] { "Outward", "Inwards" });

            fuelFormSpinner = FindViewById<Spinner>(Resource.Id.fuelFormSpinner);
            fuelFormSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, StockList);

            var bunkDetailsLayout = FindViewById<LinearLayout>(Resource.Id.layBunkDetails);

            cashModeSpinner = FindViewById<Spinner>(Resource.Id.paymentMode);
            cashModeSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, new string[] { "Cash", "Credit" });


            vehicleTypeSpinner = FindViewById<Spinner>(Resource.Id.vehicleType);
            vehicleTypeSpinner.Adapter = new ArrayAdapter(this,
                 Resource.Layout.select_dialog_item_material,
                 new string[] { "Line Vehicle", "InterCard", "Loader", "Genset 1", "Genset 2", "Genset 3" });

            vehicleTypeSpinner.ItemSelected += VehicleTypeSpinner_ItemSelected;

            //VehicleTypeSpinner_ItemClick;

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
            driverNameSpinner.ItemSelected += (s, ev) => { fuelToFill.RequestFocus(); };

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

            //var pref = PreferenceManager.GetDefaultSharedPreferences(this);
            //var billnumber = pref.GetInt(Utilities.BILLNUMBER, 0);

            //var billnumber = AppPreferences.GetInt(this, Utilities.BILLNUMBER);
            // var billnumber = FuelDB.Singleton.GetBillDetails().First()?.BillCurrentNumber;

            //Console.WriteLine(billnumber);

            //if (billnumber == 0)
            //{
            //    billNumber.Text = Utilities.BILL_NUMBER.ToString();
            //    //pref.Edit().PutInt("billnumber", Convert.ToInt32(billNumber.Text));
            //}
            //else
            //{
            //    ++billnumber;
            //    billNumber.Text = billnumber.ToString();
            //}


            var btnStore = FindViewById<LinearLayout>(Resource.Id.btnStore);
            btnStore.Click += (s, e) =>
            {
                //if (fuelSpinner.SelectedItem.ToString() != null && fuelFormSpinner.SelectedItem.ToString() != null
                //    && vehicleNumber.Text != string.Empty && vehicleTypeSpinner.SelectedItem.ToString() != null)
                //{

                //}
                //else
                //{
                //    Toast.MakeText(this, "Please enter all the values..", ToastLength.Short).Show();
                //}
                //if (!string.IsNullOrEmpty(txtOpeningKMS.Text) && !string.IsNullOrEmpty(txtClosingKMS.Text))
                //{
                //    if (Convert.ToDecimal(txtClosingKMS.Text) < Convert.ToDecimal(txtOpeningKMS.Text))
                //    {
                //        var alertDialog = new Android.App.AlertDialog.Builder(this);
                //        alertDialog.SetTitle("Enter valid destination KM");
                //        alertDialog.SetMessage("Please check starting KM and closing KM");
                //        alertDialog.SetPositiveButton("OK", (ss, se) => { });
                //        alertDialog.Show();
                //    }
                //}
                if (fuelFormSpinner.SelectedItem.Equals("Stock"))
                {
                    StoreDetils();
                }
                else
                {
                    alertDialog.Show();
                }
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
                    layMeterFault.Visibility = Android.Views.ViewStates.Gone;
                    checkBox.Visibility = Android.Views.ViewStates.Gone;
                    //StockList = new string[] { "Bunk" }; 
                }
                else
                {
                    fuelFormSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, new string[] { "Stock", "Bunk" });
                    layMeterFault.Visibility = Android.Views.ViewStates.Visible;
                    FindViewById<LinearLayout>(Resource.Id.layFuelEntry).SetBackgroundColor(Color.White);
                    lblTitle.SetBackgroundResource(Resource.Color.borderColor);
                    btnStore.SetBackgroundResource(Resource.Color.borderColor);
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
                        btnStore.SetBackgroundResource(Resource.Color.borderColor);
                        lblTitle.SetBackgroundResource(Resource.Color.borderColor);
                        FindViewById<LinearLayout>(Resource.Id.layFuelEntry).SetBackgroundColor(Color.White);
                        imgFuel.Visibility = Android.Views.ViewStates.Gone;
                    }
                    else
                    {
                        bunkDetailsLayout.Visibility = Android.Views.ViewStates.Visible;
                        if (fuelTypeSpinner.SelectedItem.Equals("Outward"))
                        {
                            FindViewById<LinearLayout>(Resource.Id.layFuelEntry).SetBackgroundResource(Resource.Color.backgroundBunk);
                            btnStore.SetBackgroundColor(Color.Brown);
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
        }

        private void VehicleTypeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (driverNameSpinner.Adapter != null && !vehicleTypeSpinner.SelectedItem.ToString().Equals(string.Empty))
            {
                driverNameSpinner.PerformClick();
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
            if (string.IsNullOrEmpty(fuelToFill.Text))
            {
                fuelAvailable.Text = $"{billDetailsList.AvailableLiters}" + " Ltrs.";
                return;
            }
            if (fuelTypeSpinner.SelectedItem.Equals("Outward"))
            {
                if (!fuelToFill.Text.Equals("."))
                {
                    if (float.Parse(fuelToFill.Text) <= float.Parse(billDetailsList.AvailableLiters))
                    {
                        availableFuel = float.Parse(billDetailsList?.AvailableLiters) - float.Parse(fuelToFill.Text);
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

        private void StoreDetils()
        {
            try
            {
                if (txtClosingKMS.Text != string.Empty && txtOpeningKMS.Text != string.Empty)
                {
                    fuelDetails = new FuelEntryDetails
                    {
                        BillNumber = billNumber.Text == string.Empty ? "0" : billNumber.Text,
                        CurrentDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
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
                        TotalKM = (Convert.ToDouble(txtClosingKMS.Text) - Convert.ToDouble(txtOpeningKMS.Text)).ToString()
                    };
                }
                else
                {
                    fuelDetails = new FuelEntryDetails
                    {
                        BillNumber = billNumber.Text == string.Empty ? "0" : billNumber.Text,
                        CurrentDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
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
                        TotalKM = "0"
                    };
                }
            }

            catch (Exception ec)
            {
                Console.WriteLine(ec.Message);
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
                    CurrentDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
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
                            CurrentDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
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
                            CurrentDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
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
                            CurrentDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
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
                            CurrentDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
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
            }
        }

        private void VehicleNumber_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (VehicleList != null)
            {
                DriverNames = VehicleList.Where(I => I.RegNo == vehicleNumber.Text).Select(I => I.DriverName).Distinct().ToArray();
                driverNameSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, DriverNames);

                txtOpeningKMS.Text = VehicleList.Where((a => a.DriverName == driverNameSpinner.SelectedItem.ToString()))
                    .Distinct().Select(i => i.OpeningKM).Distinct().First();

                vehicleTypeSpinner.PerformClick();
            }
        }
    }
}