using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Widget;
using FuelApp.Modal;
using FuelUED.CommonFunctions;
using FuelUED.Modal;
using FuelUED.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FuelUED
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]

    public class FuelActivity : AppCompatActivity
    {
        public FuelActivity()
        {
            VehicleNumber = new string[50];
        }

        public string[] VehicleNumber;
        private List<VehicleDetails> VehicleList;

        public List<Fuel> FuelLiters;
        Android.App.AlertDialog.Builder alertDialog;

        private string[] myVehiclelist;
        private string[] DriverNames;
        private Spinner cashModeSpinner;
        private Spinner driverNameSpinner;
        private EditText fuelToFill;
        private TextView fuelAvailable;
        private EditText txtOpeningKMS;
        private EditText txtClosingKMS;
        private TextView lblkmpl;
        private EditText txtFilledBy;
        private EditText txtRate;
        private TextView lblTotalPrice;
        private EditText txtRemarks;
        private ImageView imgFuel;
        private TextView lblTitle;
        private AutoCompleteTextView vehicleNumber;
        private EditText billNumber;
        private string dateTimeNow;
        private Spinner fuelSpinner;
        private Spinner fuelFormSpinner;
        private Spinner vehicleTypeSpinner;
        private FuelEntryDetails fuelDetails;

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
                FuelLiters = FuelDB.Singleton.GetFuel().ToList();
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

            billNumber = FindViewById<EditText>(Resource.Id.txtBillNumber);

            dateTimeNow = FindViewById<TextView>(Resource.Id.lbldateTime).Text = DateTime.Now.ToString();

            fuelSpinner = FindViewById<Spinner>(Resource.Id.fuelSpinner);
            fuelSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, new string[] { "Outward", "Inwards" });

            fuelFormSpinner = FindViewById<Spinner>(Resource.Id.fuelFormSpinner);
            fuelFormSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, new string[] { "Stock", "Bunk" });

            var bunkDetailsLayout = FindViewById<LinearLayout>(Resource.Id.layBunkDetails);

            cashModeSpinner = FindViewById<Spinner>(Resource.Id.paymentMode);
            cashModeSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, new string[] { "Cash", "Credit" });


            vehicleTypeSpinner = FindViewById<Spinner>(Resource.Id.vehicleType);

            var layMeterFault = FindViewById<LinearLayout>(Resource.Id.layMeterFault);

            var checkBox = FindViewById<CheckBox>(Resource.Id.chckMeterFault);
            checkBox.CheckedChange += (s, e) =>
            {
                layMeterFault.Visibility = checkBox.Checked ? Android.Views.ViewStates.Gone : Android.Views.ViewStates.Visible;
            };

            driverNameSpinner = FindViewById<Spinner>(Resource.Id.driverName);
            fuelToFill = FindViewById<EditText>(Resource.Id.fuelToFill);
            fuelAvailable = FindViewById<TextView>(Resource.Id.fuelAvailable);
            txtOpeningKMS = FindViewById<EditText>(Resource.Id.txtOpeningKMS);
            txtClosingKMS = FindViewById<EditText>(Resource.Id.txtClosingKMS);
            lblkmpl = FindViewById<TextView>(Resource.Id.lblkmpl);
            txtFilledBy = FindViewById<EditText>(Resource.Id.txtFilledBy);
            txtRate = FindViewById<EditText>(Resource.Id.txtRate);
            lblTotalPrice = FindViewById<TextView>(Resource.Id.lblTotalPrice);
            txtRemarks = FindViewById<EditText>(Resource.Id.txtRemarks);
            imgFuel = FindViewById<ImageView>(Resource.Id.imgFuel);

            fuelToFill.TextChanged += (s, e) => CheckFuelAvailbility();
            txtRate.TextChanged += (s, e) => CalculateFuelTotalAmount();
            txtOpeningKMS.TextChanged += (s, e) =>
            {
                if (!string.IsNullOrEmpty(txtOpeningKMS.Text))
                {

                }
            };

            txtClosingKMS.TextChanged += (s, e) =>
            {
                if (string.IsNullOrEmpty(txtOpeningKMS.Text))
                {
                    var alertDialog1 = new Android.App.AlertDialog.Builder(this);
                    alertDialog1.SetTitle("Enter start KM first");
                    alertDialog1.SetPositiveButton("OK", (ss, se) =>
                    {
                        txtOpeningKMS.RequestFocus();
                    });
                    alertDialog1.Show();
                }
                else if (!string.IsNullOrEmpty(txtOpeningKMS.Text) && !string.IsNullOrEmpty(txtClosingKMS.Text) && !string.IsNullOrEmpty(fuelToFill.Text))
                {
                    if (Convert.ToDecimal(txtClosingKMS.Text) > Convert.ToDecimal(txtOpeningKMS.Text))
                    {
                        var start = Convert.ToDecimal(txtOpeningKMS?.Text);
                        var end = Convert.ToDecimal(txtClosingKMS?.Text);
                        lblkmpl.Text = ((end - start) / Convert.ToDecimal(fuelToFill?.Text)).ToString();
                    }
                }
            };

            if (FuelLiters != null)
            {
                fuelAvailable.Text = $"({FuelLiters.FirstOrDefault().FuelLtts})";
            }

            var pref = PreferenceManager.GetDefaultSharedPreferences(this);
            var billnumber = pref.GetInt("billnumber", 0);
            if (billnumber == 0)
            {
                billNumber.Text = Utilities.BILL_NUMBER.ToString();
                //pref.Edit().PutInt("billnumber", Convert.ToInt32(billNumber.Text));
            }
            else
            {
                ++billnumber;
                billNumber.Text = billnumber.ToString();
            }


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

            fuelFormSpinner.ItemSelected += (s, e) =>
                {
                    if (fuelFormSpinner.SelectedItem.Equals("Stock"))
                    {
                        bunkDetailsLayout.Visibility = Android.Views.ViewStates.Gone;
                        btnStore.SetBackgroundResource(Resource.Color.borderColor);
                        lblTitle.SetBackgroundResource(Resource.Color.borderColor);
                        imgFuel.Visibility = Android.Views.ViewStates.Gone;
                    }
                    else
                    {
                        bunkDetailsLayout.Visibility = Android.Views.ViewStates.Visible;
                        btnStore.SetBackgroundColor(Color.Red);
                        lblTitle.SetBackgroundColor(Color.Red);
                        imgFuel.Visibility = Android.Views.ViewStates.Visible;
                        //btnStore.SetCompoundDrawables(Resources.GetDrawable(Resource.Drawable.ic_launcher), null, null, null);
                    }
                };
        }


        private void CheckFuelAvailbility()
        {
            if (string.IsNullOrEmpty(fuelToFill.Text))
            {
                return;
            }
            if (Convert.ToDecimal(fuelToFill.Text) <= Convert.ToDecimal(FuelLiters?.FirstOrDefault().FuelLtts))
            {
                CalculateFuelTotalAmount();
            }
            else
            {
                //Toast.MakeText(this, "No stock available..", ToastLength.Short).Show();
                var alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle("Fuel exceeds stock");
                alertDialog.SetMessage("Please note fuel availability");
                alertDialog.SetPositiveButton("OK", (s, e) =>
                {
                    fuelToFill.Text = string.Empty;
                });
                //alertDialog.SetNegativeButton("Cancel", (s, e) => { });
                alertDialog.Show();
            }
        }

        private void StoreDetils()
        {
            try
            {
                fuelDetails = new FuelEntryDetails
                {
                    BillNumber = billNumber.Text,
                    CurrentDate = dateTimeNow,
                    FuelType = fuelSpinner.SelectedItem.ToString(),
                    FuelStockType = fuelFormSpinner.SelectedItem.ToString(),
                    VehicleNumber = vehicleNumber.Text,
                    VehicleType = vehicleTypeSpinner.SelectedItem.ToString(),
                    DriverName = driverNameSpinner.SelectedItem.ToString(),
                    FuelInLtrs = fuelToFill.Text,
                    FilledBy = txtFilledBy.Text,
                    ClosingKMS = txtClosingKMS.Text,
                    Kmpl = lblkmpl.Text,
                    OpeningKMS = txtOpeningKMS.Text,
                    PaymentType = cashModeSpinner.SelectedItem?.ToString(),
                    Price = lblTotalPrice.Text,
                    RatePerLtr = txtRate.Text,
                    Remarks = txtRemarks.Text
                };
            }
            catch (Exception ec)
            {
                Console.WriteLine(ec.Message);
            }
            FuelDB.Singleton.CreateTable<FuelEntryDetails>();
            FuelDB.Singleton.InsertFuelEntryValues(fuelDetails);

            StartActivity(typeof(VehicleDetailActivity));
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
            }
        }

        private void VehicleNumber_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (VehicleList != null)
            {
                DriverNames = VehicleList.Where(I => I.RegNo == vehicleNumber.Text).Select(I => I.DriverName).Distinct().ToArray();
                driverNameSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, DriverNames);

                //VehicleType = VehicleList.Select(I => I.TypeName).Distinct().ToArray();
                vehicleTypeSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, new string[] { "Line Vehicle", "InterCard", "Loader" });
            }
        }
    }
}