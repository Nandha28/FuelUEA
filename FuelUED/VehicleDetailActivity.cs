using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Com.Ngx.Mp100sdk;
using Com.Ngx.Mp100sdk.Intefaces;

namespace FuelUED
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class VehicleDetailActivity : AppCompatActivity, INGXCallback
    {
        private NGXPrinter nGXPrinter;

        public void OnRaiseException(int p0, string p1)
        {
            System.Console.WriteLine(p0 + p1);
        }

        public void OnReturnString(string p0)
        {
            System.Console.WriteLine(p0);
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
            var textField = FindViewById<TextView>(Resource.Id.txtField);
            var details = FuelDB.Singleton.GetFuelValues();
            var btnPrint = FindViewById<Button>(Resource.Id.btnPrint);
            foreach (var item in details)
            {
                textField.Text = "BillNumber :\t\t" + item.BillNumber + "\n\nDate :\t" + item.CurrentDate +
                                  "\n\nFuelType :\t" + item.FuelType + "\n\nFuelStockType :\t" + item.FuelStockType +
                                  "\n\nVehicleNumber :\t" + item.VehicleNumber +
                                  "\n\nVehicleType :\t" + item.VehicleType +
                                  "\n\nDriverName :\t" + item.DriverName +
                                  "\n\nFuelInLtrs :\t" + item.FuelInLtrs +
                                  "\n\nOpeningKMS :\t" + item.OpeningKMS +
                                  "\n\nKmpl :\t" + item.Kmpl +
                                  "\n\nFilledBy :\t" + item.FilledBy +
                                  "\n\nPaymentType    :\t" + item.PaymentType +
                                  "\n\nRatePerLtr :\t" + item.RatePerLtr +
                                  "\n\nPrice :\t" + item.Price +
                                  "\n\nRemarks \t" + item.Remarks+"\n\n";
            }
            btnPrint.Click += (s, e) =>
            {
                //nGXPrinter.AddText(textField.Text);
                //nGXPrinter.LineFeed(2);
                //nGXPrinter.Print();
                nGXPrinter.PrintText(textField.Text);
                nGXPrinter.PrintText("\n");
            };
        }
    }
}