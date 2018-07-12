using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace FuelUED
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class VehicleDetailActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.content_main);
            var textField = FindViewById<TextView>(Resource.Id.txtField);
            var details = FuelDB.Singleton.GetFuelValues();
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
                                  "\n\nPrice :\t" + item.FilledBy +
                                  "\n\nRemarks \t" + item.Remarks;
            }
        }
    }
}