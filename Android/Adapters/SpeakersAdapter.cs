using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Monospace;

namespace MonkeySpace
{
    public class SpeakersAdapter : BaseAdapter
    {
        private List<MonkeySpace.Core.Speaker> _speakers;
        private Activity _context;

        public SpeakersAdapter(Activity context, List<MonkeySpace.Core.Speaker> speakers)
        {
            _context = context;
            _speakers = speakers;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = (convertView
                            ?? _context.LayoutInflater.Inflate(
                                    Resource.Layout.SpeakersItem, parent, false)
                        ) as LinearLayout;
            var row = _speakers.ElementAt(position);

            view.FindViewById<TextView>(Resource.Id.Title).Text = row.Name;
            

            return view;
        }

        public override int Count
        {
            get { return _speakers.Count(); }
        }

        public MonkeySpace.Core.Speaker GetRow(int position)
        {
            return _speakers.ElementAt(position);
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }
    }
}