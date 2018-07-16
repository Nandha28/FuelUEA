using Android.App;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Widget;
using Com.Ngx.Mp100sdk;
using Com.Ngx.Mp100sdk.Intefaces;
using FuelUED.Modal;
using System;
using System.Linq;

namespace FuelUED
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class VehicleDetailActivity : AppCompatActivity, INGXCallback
    {
        private NGXPrinter nGXPrinter;
        private string billNumber;
        private string fuelStockType;
        private FuelEntryDetails enterdvalues;
        private TextView textField;

        public void OnRaiseException(int p0, string p1)
        {
            Console.WriteLine(p0 + p1);
        }

        public void OnReturnString(string p0)
        {
            Console.WriteLine(p0);
        }

        public void OnRunResult(bool p0)
        {
            System.Console.WriteLine(p0);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.content_main);

            try
            {
                nGXPrinter = NGXPrinter.NgxPrinterInstance;
                nGXPrinter.InitService(this, this);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            textField = FindViewById<TextView>(Resource.Id.txtField);
            var details = FuelDB.Singleton.GetFuelValues();
            var btnPrint = FindViewById<Button>(Resource.Id.btnPrint);
            try
            {
                enterdvalues = details?.Last();
            }
            catch (Exception em)
            {
                Console.WriteLine(em.Message);
            }
            //foreach (var enterdvalues in details)
            //{
            textField.Text = "BillNumber :\t\t" + enterdvalues?.BillNumber + "\n\nDate :\t" + enterdvalues?.CurrentDate +
                              "\n\nFuelType :\t" + enterdvalues?.FuelType + "\n\nFuelStockType :\t" + enterdvalues?.FuelStockType +
                              "\n\nVehicleNumber :\t" + enterdvalues?.VehicleNumber +
                              "\n\nVehicleType :\t" + enterdvalues?.VehicleType +
                              "\n\nDriverName :\t" + enterdvalues?.DriverName +
                              "\n\nFuelInLtrs :\t" + enterdvalues?.FuelInLtrs +
                              "\n\nOpeningKMS :\t" + enterdvalues?.OpeningKMS +
                              "\n\nKmpl :\t" + enterdvalues?.Kmpl +
                              "\n\nFilledBy :\t" + enterdvalues?.FilledBy +
                              "\n\nPaymentType    :\t" + enterdvalues?.PaymentType +
                              "\n\nRatePerLtr :\t" + enterdvalues?.RatePerLtr +
                              "\n\nPrice :\t" + enterdvalues?.Price +
                              "\n\nRemarks \t" + enterdvalues?.Remarks + "\n\n";
            billNumber = enterdvalues?.BillNumber;
            fuelStockType = enterdvalues?.FuelStockType;
            //}
            btnPrint.Click += (s, e) =>
            {
                //nGXPrinter.AddText(textField.Text);
                //nGXPrinter.LineFeed(2);
                //nGXPrinter.Print();
                if (fuelStockType.Equals("Bunk"))
                {
                    var alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle("Fuel is from petrol bunk");
                    alertDialog.SetMessage("Do you want to proceed ?");
                    alertDialog.SetPositiveButton("OK", (ss, se) =>
                    {
                        Print();

                    });
                    alertDialog.Show();
                }
                else
                {
                    Print();
                }
                var pref = PreferenceManager.GetDefaultSharedPreferences(this);
                pref.Edit().PutInt("billnumber", Convert.ToInt32(billNumber)).Apply();
            };
        }

        private void Print()
        {
            if (nGXPrinter != null)
            {
                nGXPrinter.PrintText(textField.Text);
                nGXPrinter.PrintText("\n");
            }
            else
            {
                Toast.MakeText(this, "Printer not connected", ToastLength.Short).Show();
            }
        }
    }
}