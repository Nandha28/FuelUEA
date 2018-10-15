using Android.App;
using Android.OS;
using Android.Support.V7.App;
using FuelUED.Modal;
using SQLite;

namespace FuelUED.Activity
{
    [Activity(Label = "HistoryActivity")]
    public class HistoryActivity : AppCompatActivity
    {
        private TableQuery<FuelEntryDetails> fuelDetails;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.History);

            fuelDetails = FuelDB.Singleton.GetFuelValues();
            if (fuelDetails != null)
            {

            }

        }
    }
}