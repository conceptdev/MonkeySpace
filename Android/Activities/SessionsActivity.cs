using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MonkeySpace
{
    [Activity(Label = "Sessions")]
    public class SessionsActivity : BaseActivity
    {
        List<MonkeySpace.Core.Session> sessions;
        ListView list;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			RequestWindowFeature (WindowFeatures.CustomTitle); // BETTER: http://www.anddev.org/my_own_titlebar_backbutton_like_on_the_iphone-t4591.html
			SetContentView (Resource.Layout.Sessions);
			Window.SetFeatureInt (WindowFeatures.CustomTitle, Resource.Layout.WindowTitle); // http://www.londatiga.net/it/how-to-create-custom-window-title-in-android/

			list = FindViewById<ListView> (Resource.Id.List);
			list.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs> (list_ItemClick);

			var d = Intent.GetStringExtra ("SelectedDate");
			var dt = Intent.GetStringExtra ("SelectedDateTime");
            
			if (!String.IsNullOrEmpty (d)) {
				var date = DateTime.Parse (d);

				FindViewById<TextView> (Resource.Id.Title).Text = date.ToString ("dddd, dd MMMM");

				var sess = from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList ()
                            where s.Start.Day == date.Day
                            orderby s.Start
                            select s;

				sessions = sess.ToList ();
			} else if (!String.IsNullOrEmpty (dt)) {
				var datetime = DateTime.Parse (dt);

				FindViewById<TextView> (Resource.Id.Title).Text = datetime.ToString ("dddd, dd MMMM");

				var sess = from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList ()
                           where s.Start == datetime
                           orderby s.Start
                           select s;

				sessions = sess.ToList ();
			} else {
				// no parameters
				FindViewById<TextView> (Resource.Id.Title).Text = "Sessions";
				var sess = from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList ()
						orderby s.Start
						select s;
				
				sessions = sess.ToList ();
			}
        }
        protected override void OnResume()
        {
            base.OnResume();
            refreshSessions();
        }
        private void refreshSessions()
        {
            list.Adapter = new SessionsAdapter(this, sessions);
        }

		private void list_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var session = ((SessionsAdapter)list.Adapter).GetRow(e.Position);

            var intent = new Intent();
            intent.SetClass(this, typeof(SessionActivity));
            intent.PutExtra("Code", session.Code);

            StartActivity(intent);
        }
    }
}