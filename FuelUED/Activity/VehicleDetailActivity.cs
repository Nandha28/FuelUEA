using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Ngx.Mp100sdk;
using Com.Ngx.Mp100sdk.Intefaces;
using FuelUED.CommonFunctions;
using FuelUED.Modal;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;

namespace FuelUED
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class VehicleDetailActivity : AppCompatActivity, INGXCallback
    {
        private NGXPrinter nGXPrinter;
        private ScrollView mainScrollView;
        private string billNumber;
        private string fuelStockType;
        private FuelEntryDetails enterdvalues;
        private Button btnPrint;
        private LinearLayout layoutMain;
        static string[] InwardValues, OutwardBunk, OutwardStock;
        static string[] OutwardStockMeterFault, OutwardBunkMeterFault;
        private PrintDetails user;

        public bool IsExtraPrint { get; private set; }

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
            Console.WriteLine(p0);
        }

        public VehicleDetailActivity()
        {
            InwardValues = new string[] { "Bill No.", "Date", "Type","Fuel From", "Vehicle No.", "Vehicle Type", "Driver Name",
                "Ltrs.","Filled By","Payment Type","RatePerLtr","Price","Remarks"};

            OutwardStock = new string[] {"Bill No.", "Date", "Type","Fuel From", "Vehicle No.", "Vehicle Type", "Driver Name",
                "Ltrs.","Op. KMS","Cl. KMS","KMPL","Filled By","Remarks" };

            OutwardStockMeterFault = new string[] { "Bill No.", "Date", "Type","Fuel From", "Vehicle No.", "Vehicle Type",
                "Driver Name","Ltrs.","Meter fault","Filled By","Remarks"};

            OutwardBunk = new string[] {"Bill No.", "Date", "Type","Fuel From", "Vehicle No.", "Vehicle Type", "Driver Name",
                "Ltrs.","Op. KMS","Cl. KMS","KMPL","Filled By","Payment Type","RatePerLtr","Price","Remarks"};

            OutwardBunkMeterFault = new string[] { "Bill No.", "Date", "Type","Fuel From", "Vehicle No.", "Vehicle Type",
                "Driver Name", "Ltrs.","Meter fault","Filled By","Payment Type","RatePerLtr","Price","Remarks"};
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.content_main);
            user = JsonConvert.DeserializeObject<PrintDetails>(Intent.GetStringExtra("printDetails"));

            try
            {
                nGXPrinter = NGXPrinter.NgxPrinterInstance;
                nGXPrinter.InitService(this, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            var details = FuelDB.Singleton.GetFuelValues();
            btnPrint = FindViewById<Button>(Resource.Id.btnPrint);
            layoutMain = FindViewById<LinearLayout>(Resource.Id.layPrinterBaseLayout);
            try
            {
                if (details != null && details.Last().IsExcess == ConstantValues.ZERO)
                {
                    enterdvalues = details?.Last();
                }
                else
                {
                    enterdvalues = details?.ElementAt(details.Count() - 2);
                }
            }
            catch (Exception em)
            {
                Console.WriteLine(em.Message);
            }
            mainScrollView = FindViewById<ScrollView>(Resource.Id.mainScrollView);

            billNumber = enterdvalues?.BillNumber;
            fuelStockType = enterdvalues?.FuelStockType;

            if (enterdvalues.FuelType.Equals(ConstantValues.INWARD))
            {
                DrawPrintView(InwardValues);
            }
            else
            {
                if (enterdvalues.FuelStockType.Equals(ConstantValues.STOCK))
                {
                    if (enterdvalues.MeterFault.Equals(ConstantValues.ZERO))
                    {
                        DrawPrintView(OutwardStock);
                    }
                    else
                    {
                        DrawPrintView(OutwardStockMeterFault);
                    }
                }
                else
                {
                    if (enterdvalues.MeterFault.Equals(ConstantValues.ZERO))
                    {
                        DrawPrintView(OutwardBunk);
                    }
                    else
                    {
                        DrawPrintView(OutwardBunkMeterFault);
                    }
                }
            }

            btnPrint.Click += (s, e) =>
            {
                if (fuelStockType.Equals(ConstantValues.BUNK) && !enterdvalues.FuelType.Equals(ConstantValues.INWARD))
                {
                    var alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle("Fuel is from petrol bunk");
                    alertDialog.SetMessage(ConstantValues.PROCEED);
                    alertDialog.SetPositiveButton(ConstantValues.OK, (ss, se) =>
                    {
                        PrintFromPrinter();
                    });
                    alertDialog.Show();
                }
                else
                {
                    PrintFromPrinter();
                }
            };
        }

        private void DrawPrintView(string[] inwardValues)
        {
            var index = 0;
            foreach (var item in user.GetType().GetProperties())
            {
                try
                {
                    if (item.GetValue(user) != null)
                    {
                        var layoutInf = (LayoutInflater)GetSystemService(LayoutInflaterService);
                        View view = layoutInf.Inflate(Resource.Layout.PrintView, null);
                        view.FindViewById<TextView>(Resource.Id.txtName).Text = inwardValues[index];
                        view.FindViewById<TextView>(Resource.Id.txtValue).Text = item.GetValue(user, null).ToString();
                        layoutMain.AddView(view, index);
                        index++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    btnPrint.Visibility = ViewStates.Visible;
                }
            }
        }

        private void PrintFromPrinter()
        {
            var bill = FuelDB.Singleton.GetBillDetails();
            var billall = bill.First();
            var billDetails = new BillDetails
            {
                AvailableLiters = billall.AvailableLiters,
                BillCurrentNumber = (Convert.ToInt32(billall.BillCurrentNumber) + 1).ToString(),
                BillPrefix = billall.BillPrefix,
                DeviceStatus = billall.DeviceStatus
            };
            FuelDB.Singleton.DeleteTable<BillDetails>();
            FuelDB.Singleton.CreateTable<BillDetails>();
            FuelDB.Singleton.InsertBillDetails(billDetails);
            Print();
        }

        private void AgainPrint()
        {
            var alertDialog = new Android.App.AlertDialog.Builder(this);
            alertDialog.SetTitle("Print");
            alertDialog.SetMessage("Do you want to print ?");
            alertDialog.SetCancelable(false);
            alertDialog.SetPositiveButton(ConstantValues.YES, (ss, se) =>
            {
                Print();
            });
            alertDialog.SetNegativeButton(ConstantValues.NO, (s, e) =>
            {
                NavigateToMainActivity();
            });
            alertDialog.Show();
        }

        private void NavigateToMainActivity()
        {
            var intent = new Intent(this, typeof(MainScreenActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            Finish();
        }

        private void Print()
        {
            if (nGXPrinter != null)
            {
                nGXPrinter.PrintImage(GetCanvas(layoutMain, mainScrollView.GetChildAt(0).Height, mainScrollView.GetChildAt(0).Width));
                nGXPrinter.PrintText("\n");
                AgainPrint();
            }
            else
            {
                Toast.MakeText(this, "Printer not connected", ToastLength.Short).Show();
                NavigateToMainActivity();
            }
        }

        private Bitmap GetCanvas(View view, int height, int width)
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
        public override void OnBackPressed()
        {
            //if (IsExtraPrint)
            //{
            //  base.OnBackPressed();
            //}
            //IsExtraPrint = true;
        }
    }
}