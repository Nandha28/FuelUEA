using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FuelApp.Modal;
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

        private string[] myVehiclelist;

        public string[] VehicleType;
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

            fuelToFill.TextChanged += (s, e) => CalculateFuelTotalAmount();
            txtRate.TextChanged += (s, e) => CalculateFuelTotalAmount();

            if (FuelLiters != null)
            {
                fuelAvailable.Text = $"({FuelLiters.FirstOrDefault().FuelLtts})";
            }

            var btnStore = FindViewById<Button>(Resource.Id.btnStore);
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
            };

            fuelFormSpinner.ItemSelected += (s, e) =>
                {
                    if (fuelFormSpinner.SelectedItem.Equals("Stock"))
                    {
                        bunkDetailsLayout.Visibility = Android.Views.ViewStates.Gone;
                    }
                    else
                    {
                        bunkDetailsLayout.Visibility = Android.Views.ViewStates.Visible;
                    }
                };
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

                VehicleType = VehicleList.Select(I => I.TypeName).Distinct().ToArray();
                vehicleTypeSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.select_dialog_item_material, VehicleType);
            }
        }
    }
}