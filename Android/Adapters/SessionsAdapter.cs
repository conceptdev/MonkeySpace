using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;


namespace MonkeySpace
{
	/// <summary>
	/// Sessions adapter used in the Sessions list - 
	/// displays complete session details in each row View.
	/// </summary>
    public class SessionsAdapter : BaseAdapter
    {
        private List<MonkeySpace.Core.Session> sessions;
        private Activity context;

        public SessionsAdapter(Activity context, List<MonkeySpace.Core.Session> sessions)
        {
            this.context = context;
            this.sessions = sessions;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = (convertView
                            ?? context.LayoutInflater.Inflate(
                                    Resource.Layout.SessionsItem, parent, false)
                        ) as LinearLayout;
            var row = sessions.ElementAt(position);

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
            get { return sessions.Count(); }
        }

        public MonkeySpace.Core.Session GetRow(int position)
        {
            return sessions.ElementAt(position);
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