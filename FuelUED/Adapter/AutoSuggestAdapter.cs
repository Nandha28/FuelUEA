using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FuelUED.Adapter
{
    public class AutoSuggestAdapter : ArrayAdapter
    {
        private Context context;
        private int resource;
        private List<string> items;

        AutoTextFilter myfilter;
        public override Filter Filter
        {
            get
            {
                if (myfilter == null)
                {
                    myfilter = new AutoTextFilter(this);
                }
                return myfilter;
            }
        }

        public AutoSuggestAdapter(Context context, int resource, List<string> stringList) : base(context, resource, stringList)
        {
            this.context = context;
            this.resource = resource;
            this.items = stringList;
            ///nameFilter = new AutoTextFilter(this, suggestions, tempItems);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (convertView == null)
            {
                var layoutInflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                layoutInflater.Inflate(resource, parent, false);
            }
            var item = items[position];
            if (item != null)
            {
                ((TextView)view).Text = item;
            }
            return view;
        }
    }
    public class AutoTextFilter : Filter
    {
        private AutoSuggestAdapter adapter;

        public AutoTextFilter(AutoSuggestAdapter autoSuggestAdapter)
        {
            adapter = autoSuggestAdapter;
        }

        public override ICharSequence ConvertResultToStringFormatted(Object resultValue)
        {
            return (String)resultValue;

        }
        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            FilterResults results = new FilterResults();
            var originals = adapter.ToArray<String>();
            List<String> values = new List<String>();
            values.AddRange(originals);
            //int count = originals.Length;
            //for (int i = 0; i < count; ++i)
            //{
            //    values.Add(originals[i]);
            //}
            foreach (String value in values)
            {
                var valueText = value.ToString();
                if (null != valueText && null != constraint
                        && valueText.ToUpper().Contains(constraint.ToString().ToUpper()))
                {
                    values.Add(value);
                }
            }

            results.Values = values.ToArray<String>();
            results.Count = values.Count;
            return results;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            var list = results.Values.ToArray<string>();
            if (list != null && list.Length > 0)
            {
                adapter.Clear();
                foreach (var t in list)
                {
                    adapter.Add(t);
                }
            }
            if (results.Count > 0)
            {
                adapter.NotifyDataSetChanged();
            }
            else
            {
                adapter.NotifyDataSetInvalidated();
            }
        }
    }
}