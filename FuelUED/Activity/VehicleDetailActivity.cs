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
                "Ltrs.","Filled By","RatePerLtr","Price","Payment Type","Remarks"};

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
                enterdvalues = details?.Last();
            }
            catch (Exception em)
            {
                Console.WriteLine(em.Message);
            }
            mainScrollView = FindViewById<ScrollView>(Resource.Id.mainScrollView);
            // textName.Text = GetTextHeads().ToString();
            //textValue.Text = GetTextValues().ToString();
            //if (enterdvalues.FuelType.Equals("Inwards"))


            //var s = Array.Sort(OutwardValues,);

            //{
            //    textField.Text = "Bill No. \t\t:\t" + enterdvalues?.BillNumber +
            //                    "\n\nDate \t:\t" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) +
            //                    "\n\nType \t:\t" + enterdvalues?.FuelType +
            //                    "\n\nFuel From \t:\t" + enterdvalues?.FuelStockType +
            //                    "\n\nVehicle No \t:\t" + enterdvalues?.VehicleNumber +
            //                    "\n\nVehicle \t:\t" + enterdvalues?.VehicleType +
            //                    "\n\nDriver Name \t:\t" + enterdvalues?.DriverName +
            //                    "\n\nLtrs. \t:\t" + enterdvalues?.FuelInLtrs +
            //                     "\n\nFilled By :\t" + enterdvalues?.FilledBy +
            //                     "\n\nPayment \t:\t" + enterdvalues?.PaymentType +
            //                     "\n\nRatePerLtr\t :\t" + enterdvalues?.RatePerLtr +
            //                     "\n\nPrice \t:\t" + enterdvalues?.Price +
            //                     "\n\nRemarks \t:\t" + enterdvalues?.Remarks + "\n\n";
            //}
            //else
            //{
            //    textField.Text = "Bill No: \t:\t\t" + enterdvalues?.BillNumber +
            //                     "\n\nDate \t:\t" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) +
            //                     "\n\nType \t:\t" + enterdvalues?.FuelType +
            //                     "\n\nFuel From \t:\t" + enterdvalues?.FuelStockType +
            //                     "\n\nVehicle No \t:\t" + enterdvalues?.VehicleNumber +
            //                     "\n\nVehicle \t:\t" + enterdvalues?.VehicleType +
            //                     "\n\nDriver Name \t:\t" + enterdvalues?.DriverName +
            //                     "\n\nLtrs. \t:\t" + enterdvalues?.FuelInLtrs +
            //                      "\n\nOp. KMS \t:\t" + enterdvalues?.OpeningKMS +
            //                      "\n\nCl. KMS \t:\t" + enterdvalues?.ClosingKMS +
            //                      "\n\nKmpl \t:\t" + enterdvalues?.Kmpl +
            //                      "\n\nFilled By \t:\t" + enterdvalues?.FilledBy +
            //                      "\n\nPayment \t:\t" + enterdvalues?.PaymentType +
            //                      "\n\nRatePerLtr \t:\t" + enterdvalues?.RatePerLtr +
            //                      "\n\nPrice \t:\t" + enterdvalues?.Price +
            //                      "\n\nRemarks \t:\t" + enterdvalues?.Remarks + "\n\n";
            //}
            billNumber = enterdvalues?.BillNumber;
            fuelStockType = enterdvalues?.FuelStockType;
            //enterdvalues.Find();
            //var index = 0;
            //foreach (var item in enterdvalues.GetType().GetProperties())
            //{
            //    //Console.WriteLine(item.Name+" : "+item.GetValue(enterdvalues,null).ToString());
            //    if (!item.GetValue(enterdvalues, null).ToString().Equals("0")
            //        && !item.GetValue(enterdvalues, null).ToString().Equals(string.Empty))
            //    {
            //        var layoutInf = (LayoutInflater)GetSystemService(LayoutInflaterService);
            //        View view = layoutInf.Inflate(Resource.Layout.PrintView, null);
            //        view.FindViewById<TextView>(Resource.Id.txtName).Text = item.Name;
            //        view.FindViewById<TextView>(Resource.Id.txtValue).Text = item.GetValue(enterdvalues, null).ToString();       
            //        layoutMain.AddView(view, index);
            //        index++;
            //    }
            //}
            if (enterdvalues.FuelType.Equals("Inwards"))
            {
                DrawPrintView(InwardValues);
            }
            else
            {
                if (enterdvalues.FuelStockType.Equals("Stock"))
                {
                    if (enterdvalues.MeterFault.Equals("0"))
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
                    if (enterdvalues.MeterFault.Equals("0"))
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
                //var pref = PreferenceManager.GetDefaultSharedPreferences(this);
                //pref.Edit().PutInt("billnumber", Convert.ToInt32(billNumber)).Apply();

                //AppPreferences.SaveInt(this,Utilities.BILLNUMBER, Convert.ToInt32(billNumber));
            };
        }

        private void DrawPrintView(string[] inwardValues)
        {
            var index = 0;
            //foreach (var item in enterdvalues.GetType().GetProperties())
            //{
            //    //Console.WriteLine(item.Name+" : "+item.GetValue(enterdvalues,null).ToString());
            //    if (!item.GetValue(enterdvalues, null).ToString().Equals("0")
            //        && !item.GetValue(enterdvalues, null).ToString().Equals(string.Empty) &&
            //        !item.Name.Equals("VID") && !item.Name.Equals("DriverID_PK"))
            //    {
            //        var layoutInf = (LayoutInflater)GetSystemService(LayoutInflaterService);
            //        View view = layoutInf.Inflate(Resource.Layout.PrintView, null);
            //        view.FindViewById<TextView>(Resource.Id.txtName).Text = inwardValues[index];
            //        view.FindViewById<TextView>(Resource.Id.txtValue).Text = item.GetValue(enterdvalues, null).ToString();
            //        layoutMain.AddView(view, index);
            //        index++;
            //    }
            //}
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

        //private StringBuilder GetTextValues()
        //{
        //    var textheads = new StringBuilder();
        //    foreach (var item in enterdvalues.GetType().GetProperties())
        //    {
        //        //Console.WriteLine(item.Name+" : "+item.GetValue(enterdvalues,null).ToString());
        //        if (!item.GetValue(enterdvalues, null).ToString().Equals("0")
        //            && !item.GetValue(enterdvalues, null).ToString().Equals(string.Empty))
        //        {
        //            textheads.Append(item.GetValue(enterdvalues, null).ToString()+"\n");
        //            //textValue.Text = item.GetValue(enterdvalues, null).ToString();
        //        }
        //    }
        //    return textheads;
        //}

        private void Print()
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
            if (nGXPrinter != null)
            {
                //nGXPrinter.PrintText(textName.Text);              
                nGXPrinter.PrintImage(GetCanvas(layoutMain, mainScrollView.GetChildAt(0).Height, mainScrollView.GetChildAt(0).Width));
                nGXPrinter.PrintText("\n");
            }
            else
            {
                Toast.MakeText(this, "Printer not connected", ToastLength.Short).Show();
            }
            StartActivity(typeof(MainScreenActivity));
            Finish();
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
    }
}