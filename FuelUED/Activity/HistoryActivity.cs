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
using System.Collections.Generic;
using System.Linq;
using static Android.Views.View;

namespace FuelUED.Activity
{
    [Activity(Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class HistoryActivity : AppCompatActivity, INGXCallback
    {
        private ListView historyList;
        private NGXPrinter nGXPrinter;
        private TableQuery<BillHistory> billHistory;
        private Android.App.AlertDialog.Builder alertDialog;
        private BillHistoryListAdapter adapter;
        private bool IsExitApp;
        private Button btnclearHistory;
        private LinearLayout linearLayout;

        public LinearLayout ParentLinearLayout { get; private set; }

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

            linearLayout = FindViewById<LinearLayout>(Resource.Id.baseLinearLayout);
            ParentLinearLayout = FindViewById<LinearLayout>(Resource.Id.parentLinearLayout);

            btnclearHistory.Click += (s, e) =>
            {
                alertDialog.Show();
            };
            // btnPrint.Click += BtnPrint_Click;

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
            //nGXPrinter.PrintImage(getWholeListViewItemsToBitmap());
            nGXPrinter.PrintImage(GetCanvas(historyList, ParentLinearLayout.GetChildAt(0).Height, ParentLinearLayout.GetChildAt(0).Width));
            // PrintListView();
            nGXPrinter.PrintText("\n\n\n");
            PrintAgain();
        }

        private void PrintListView()
        {
            foreach (var item in GetWholeListViewItemsToBitmap())
            {
                nGXPrinter.PrintImage(item);
            }
            //nGXPrinter.PrintImage(GetCanvas(historyList, historyList.GetChildAt(i).Height, historyList.GetChildAt(i).Width));
        }

        public Bitmap GetBitmapFromView(View view)
        {

            Bitmap returnedBitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(returnedBitmap);
            Drawable bgDrawable = view.Background;
            if (bgDrawable != null)
                bgDrawable.Draw(canvas);
            else
                canvas.DrawColor(Color.White);
            view.Draw(canvas);
            return returnedBitmap;
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


        public List<Bitmap> GetWholeListViewItemsToBitmap()
        {

            ListView listview = historyList;
            var adapter = listview.Adapter;
            int itemscount = adapter.Count;
            int allitemsheight = 0;
            List<Bitmap> bmps = new List<Bitmap>();

            for (int i = 0; i < itemscount; i++)
            {
                View childView = adapter.GetView(i, null, listview);
                childView.Measure(MeasureSpec.MakeMeasureSpec(listview.Width, MeasureSpecMode.Exactly),
                        MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));

                childView.Layout(0, 0, childView.MeasuredWidth, childView.MeasuredHeight);
                childView.DrawingCacheEnabled = true;
                childView.BuildDrawingCache();
                bmps.Add(childView.DrawingCache);
                allitemsheight += childView.MeasuredHeight;
            }

            Bitmap bigbitmap = Bitmap.CreateBitmap(listview.MeasuredWidth, allitemsheight, Bitmap.Config.Argb8888);
            Canvas bigcanvas = new Canvas(bigbitmap);

            Paint paint = new Paint();
            int iHeight = 0;

            for (int i = 0; i < bmps.Count; i++)
            {
                Bitmap bmp = bmps[i];
                bigcanvas.DrawBitmap(bmp, 0, iHeight, paint);
                iHeight += bmp.Height;

                if (bmp != null && !bmp.IsRecycled)
                {
                    bmp.Recycle();
                    bmp = null;
                }
            }
            return bmps;
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