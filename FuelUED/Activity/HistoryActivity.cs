using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Ngx.Mp100sdk;
using Com.Ngx.Mp100sdk.Intefaces;
using FuelUED.Adapter;
using FuelUED.Modal;
using SQLite;
using System;
using System.Linq;

namespace FuelUED.Activity
{
    [Activity(Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class HistoryActivity : AppCompatActivity, INGXCallback
    {
        private ListView historyList;
        private NGXPrinter nGXPrinter;
        private TableQuery<BillHistory> billHistory;
        private Android.App.AlertDialog.Builder alertDialog;
        private BillHistoryListAdapter adapter;
        private bool IsExitApp;
        private Button btnclearHistory;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.History);

            try
            {
                nGXPrinter = NGXPrinter.NgxPrinterInstance;
                nGXPrinter.InitService(this, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            alertDialog = new Android.App.AlertDialog.Builder(this);
            alertDialog.SetTitle("Clear History");
            alertDialog.SetMessage("Do you want to clear history ?");
            alertDialog.SetPositiveButton("OK", (ss, se) =>
            {
                ClearHistory();
            });
            alertDialog.SetNegativeButton("Cancel", (ss, se) => { });

            var home = FindViewById<Button>(Resource.Id.btnHomeFromHistory);
            var btnPrint = FindViewById<Button>(Resource.Id.btnHistoryPrint);
            btnclearHistory = FindViewById<Button>(Resource.Id.btnClearHistory);
            var btnHome = FindViewById<Button>(Resource.Id.btnHomeFromHistory);

            btnclearHistory.Click += (s, e) =>
            {
                alertDialog.Show();
            };
            btnPrint.Click += BtnPrint_Click;

            btnHome.Click += BtnHome_Click;

            historyList = FindViewById<ListView>(Resource.Id.historylistView);
            billHistory = FuelDB.Singleton.GetBillHitory();
            try
            {
                if (billHistory?.Count() > 0)
                {
                    adapter = new BillHistoryListAdapter(this, billHistory.ToList());
                    historyList.Adapter = adapter;
                    DisableClearButton(true);
                }
                else
                {
                    Toast.MakeText(this, "There is no history to show..", ToastLength.Short).Show();
                    DisableClearButton(false);
                }
            }
            catch
            {
                Toast.MakeText(this, "There is no history to show..", ToastLength.Short).Show();
                DisableClearButton(false);
            }
        }

        private void DisableClearButton(bool disable)
        {
            btnclearHistory.Clickable = disable;
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(MainScreenActivity));
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (nGXPrinter != null)
            {
                PrintHistory();
                //if (listType.Equals("UploadItemDetails"))
                //{
                //    nGXPrinter.PrintText("\n\n\n\n\n");
                //}
                // layScrollview.Visibility = ViewStates.Gone;
            }
            else
            {
                Toast.MakeText(this, "Printer not connected", ToastLength.Short).Show();
            }
        }

        private void PrintAgain()
        {

            var alertDialog = new Android.App.AlertDialog.Builder(this);
            alertDialog.SetTitle("Additional Print");
            alertDialog.SetMessage("Do you want to print agin ?");
            alertDialog.SetCancelable(false);
            alertDialog.SetPositiveButton("Yes", (ss, se) =>
            {
                PrintHistory();
            });
            alertDialog.SetNegativeButton("No", (ss, ee) =>
            {
                //if (listType.Equals("UploadItemDetails"))
                //{
                //    var intent = new Intent(this, typeof(VehicleDetailActivity));
                //    intent.AddFlags(ActivityFlags.ClearTop);
                //    StartActivity(intent);
                //    Finish();
                //}
                //else
                //{
                //    var intent = new Intent(this, typeof(DeliveryActivity));
                //    intent.AddFlags(ActivityFlags.ClearTop);
                //    StartActivity(intent);
                //    Finish();
                //}
            });
            alertDialog.Show();
        }

        private void PrintHistory()
        {
            nGXPrinter.PrintText("\n");
            nGXPrinter.PrintImage(GetCanvas(historyList, historyList.GetChildAt(0).Height, historyList.GetChildAt(0).Width));
            nGXPrinter.PrintText("\n\n\n");
            PrintAgain();
        }

        public Bitmap GetCanvas(View view, int height, int width)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            Drawable bgDrawable = view.Background;
            if (bgDrawable != null)
                bgDrawable.Draw(canvas);
            else
                canvas.DrawColor(Color.White);
            view.Draw(canvas);
            return bitmap;
        }

        private void ClearHistory()
        {
            FuelDB.Singleton.DeleteTable<BillHistory>();
            historyList.Adapter = null;
            historyList.Invalidate();
            Toast.MakeText(this, "Successfully cleared history", ToastLength.Short).Show();
            DisableClearButton(false);
        }

        public void OnRaiseException(int p0, string p1)
        {
        }

        public void OnReturnString(string p0)
        {
        }

        public void OnRunResult(bool p0)
        {
        }
        protected override void OnStart()
        {
            base.OnStart();
            //Recreate();
        }
        protected override void OnResume()
        {
            IsExitApp = false;
            base.OnResume();
        }
        public override void OnBackPressed()
        {
            if (IsExitApp)
            {
                base.OnBackPressed();
            }
            IsExitApp = true;
            Toast.MakeText(this, "Press agin to exit app..", ToastLength.Short).Show();
        }
    }
}