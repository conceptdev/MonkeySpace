using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Monospace;

namespace MonkeySpace
{
    public class Sessions2Adapter : BaseAdapter
    {
        private List<MonkeySpace.Core.Session> _sessions;
        private Activity _context;

        public Sessions2Adapter(Activity context, List<MonkeySpace.Core.Session> sessions)
        {
            _context = context;
            _sessions = sessions;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = (convertView
                            ?? _context.LayoutInflater.Inflate(
                                    Resource.Layout.SessionsItem, parent, false)
                        ) as LinearLayout;
            var row = _sessions.ElementAt(position);

            view.FindViewById<TextView>(Resource.Id.Time).Text = row.DateTimeDisplay;

            view.FindViewById<TextView>(Resource.Id.Title).Text = row.Title;

            if (row.Location == "")
                view.FindViewById<TextView>(Resource.Id.Room).Text = row.GetSpeakerList ();
            else
				view.FindViewById<TextView>(Resource.Id.Room).Text = row.LocationDisplay + "; " + row.GetSpeakerList();
            

            return view;
        }

        public override int Count
        {
            get { return _sessions.Count(); }
        }

        public MonkeySpace.Core.Session GetRow(int position)
        {
            return _sessions.ElementAt(position);
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