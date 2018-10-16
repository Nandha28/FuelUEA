using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using FuelUED.Adapter;
using FuelUED.Modal;
using SQLite;
using System;
using System.Linq;

namespace FuelUED.Activity
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    public class HistoryActivity : AppCompatActivity
    {
        private ListView historyList;
        private TableQuery<BillHistory> billHistory;
        private Android.App.AlertDialog.Builder alertDialog;
        private BillHistoryListAdapter adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.History);

            alertDialog = new Android.App.AlertDialog.Builder(this);
            alertDialog.SetTitle("Clear History");
            alertDialog.SetMessage("Do you want to clear history ?");
            alertDialog.SetPositiveButton("OK", (ss, se) =>
            {
                ClearHistory();
            });
            alertDialog.SetNegativeButton("Cancel", (ss, se) => { });

            var home = FindViewById<Button>(Resource.Id.btnHomeFromHistory);
            var btnclearHistory = FindViewById<Button>(Resource.Id.btnClearHistory);
            btnclearHistory.Click += (s, e) =>
            {
                alertDialog.Show();
            };

            historyList = FindViewById<ListView>(Resource.Id.historylistView);
            billHistory = FuelDB.Singleton.GetBillHitory();
            if (billHistory.Count() > 0)
            {
                adapter  = new BillHistoryListAdapter(this, billHistory.ToList());
                historyList.Adapter = adapter;
            }
            else
            {
                Toast.MakeText(this, "There is no history to show..", ToastLength.Short).Show();
            }
        }

        private void ClearHistory()
        {
            FuelDB.Singleton.DeleteTable<BillHistory>();
            historyList.Adapter = null;
            historyList.Invalidate();
            Toast.MakeText(this, "Successfully cleared history", ToastLength.Short).Show();
        }
    }
}