using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FuelUED.Modal;
using System.Collections.Generic;

namespace FuelUED.Adapter
{
    public class BillHistoryListAdapter : BaseAdapter
    {

        Context context;
        private List<BillHistory> billHistory;

        public BillHistoryListAdapter(Context context, List<BillHistory> billHistory)
        {
            this.context = context;
            this.billHistory = billHistory;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            BillHistoryListAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as BillHistoryListAdapterViewHolder;

            if (holder == null)
            {
                holder = new BillHistoryListAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.history_items, parent, false);
                holder.Date = view.FindViewById<TextView>(Resource.Id.date);
                holder.VehicleNumber = view.FindViewById<TextView>(Resource.Id.vehNumber);
                holder.Litres = view.FindViewById<TextView>(Resource.Id.litre);
                holder.InwardImage = view.FindViewById<ImageView>(Resource.Id.inwardsarrow);
                view.Tag = holder;
            }

            //fill in your items
            if (billHistory != null)
            {
                holder.Date.Text = billHistory?[position].CurrentDate;
                holder.VehicleNumber.Text = billHistory?[position].VehicleNumber;
                holder.Litres.Text = billHistory?[position].FuelInLtrs;
                holder.InwardImage.Visibility = billHistory[position].IsInward ? ViewStates.Visible : ViewStates.Invisible;
            }

            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return billHistory.Count;
            }
        }

    }

    class BillHistoryListAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        public TextView Date { get; set; }
        public TextView VehicleNumber { get; set; }
        public TextView Litres { get; set; }
        public ImageView InwardImage { get; set; }
    }
}