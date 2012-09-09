using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;

namespace MonkeySpace
{
	/// <summary>
	/// Displays *only* the session title, used to bind to the list of sessions
	/// on the SpeakerActivity speaker details screen
	/// </summary>
    public class SessionsSimpleAdapter : BaseAdapter
    {
        private List<MonkeySpace.Core.Session> sessions;
        private Activity context;

        public SessionsSimpleAdapter(Activity context, List<MonkeySpace.Core.Session> sessions)
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

            view.FindViewById<TextView>(Resource.Id.Title).Text = row.Title;
            
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