using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Ngx.Mp100sdk;
using Com.Ngx.Mp100sdk.Intefaces;
using Newtonsoft.Json;
using UECrusher.Model;
using Utilities;

namespace UECrusher.Activity
{
    [Activity(Label = "PrintViewActivity", Theme = "@style/AppTheme.NoActionBar")]
    public class PrintViewActivity : AppCompatActivity, INGXCallback
    {
        private LinearLayout layMainLinear;
        private ScrollView layMainScroll;
        private NGXPrinter nGXPrinter;

        public void OnRaiseException(int p0, string p1)
        {

        }

        public void OnReturnString(string p0)
        {

        }

        public void OnRunResult(bool p0)
        {

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                nGXPrinter = NGXPrinter.NgxPrinterInstance;
                nGXPrinter.InitService(this, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            // Create your application here
            SetContentView(Resource.Layout.activity_main);
            layMainLinear = FindViewById<LinearLayout>(Resource.Id.layMainLinear);
            layMainScroll = FindViewById<ScrollView>(Resource.Id.layMainScroll);        
            var data = Intent.GetStringExtra("data");
            var deserializeResult = JsonConvert.DeserializeObject<List<UploadItemDetails>>(data);
            FindViewById<Button>(Resource.Id.btn).Click += (s,e) =>
            {
                if (nGXPrinter != null)
                {
                    nGXPrinter.PrintImage(GetCanvas(layMainLinear, layMainScroll.GetChildAt(0).Height, layMainScroll.GetChildAt(0).Width));
                    nGXPrinter.PrintText("\n");
                    // layScrollview.Visibility = ViewStates.Gone;
                }
            };
            Print(deserializeResult.First());
        }
        private void Print(UploadItemDetails uploadItemDetails)
        {
            var array = new string[] { "LB. No.","Date", "Vehicle", "Customer","Item","Empty Weight",
                                        "Pay Mode","W Mode","DID"};
            var index = 0;
            foreach (var item in uploadItemDetails.GetType().GetProperties())
            {
                try
                {
                    var layoutInf = (LayoutInflater)GetSystemService(LayoutInflaterService);
                    View view = layoutInf.Inflate(Resource.Layout.PrintView, null);
                    view.FindViewById<TextView>(Resource.Id.txtName).Text = "dummy";
                    view.FindViewById<TextView>(Resource.Id.txtValue).Text = item.GetValue(uploadItemDetails, null).ToString();
                    layMainLinear.AddView(view, index);
                    index++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }      
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
    }
}