using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace UECrusher.Activity
{
    [Activity(Label = "DeliveryActivity")]
    public class DeliveryActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.DeliveryDetail);
        }
    }
}