using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace UECrusher.Activity
{
    [Activity(Label = "VehicleDetailActivity", MainLauncher = true)]
    public class VehicleDetailActivity : AppCompatActivity
    {
        private TextView lblBillNumber;
        private TextView lblDate;
        private TextView lblEmptyWeight;
        private AutoCompleteTextView autoVehicleNumber;
        private RadioGroup radioGroup;
        private RadioButton cashRadioButton, creditRadioButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.VehicleDetails);
            var lblTittle = FindViewById<TextView>(Resource.Id.lblTittle);
            lblBillNumber = FindViewById<TextView>(Resource.Id.lblBillNumber);
            lblDate = FindViewById<TextView>(Resource.Id.lblDate);
            lblEmptyWeight = FindViewById<TextView>(Resource.Id.lblEmptyWeight);
            autoVehicleNumber = FindViewById<AutoCompleteTextView>(Resource.Id.vehicleNumber);
            radioGroup = FindViewById<RadioGroup>(Resource.Id.radioPaymentMode);
            cashRadioButton = FindViewById<RadioButton>(Resource.Id.cashRadioButton);
            creditRadioButton = FindViewById<RadioButton>(Resource.Id.creditRadioButton);
        }
    }
}