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
    [Activity(Label = "Speaker")]
    public class SpeakerActivity : BaseActivity
    {
        string name;
        MonkeySpace.Core.Speaker currentSpeaker;
        List<MonkeySpace.Core.Session> sessions;
        ListView list;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.CustomTitle); // BETTER: http://www.anddev.org/my_own_titlebar_backbutton_like_on_the_iphone-t4591.html
            SetContentView(Resource.Layout.Speaker);
            Window.SetFeatureInt(WindowFeatures.CustomTitle, Resource.Layout.WindowTitle); // http://www.londatiga.net/it/how-to-create-custom-window-title-in-android/

            name = Intent.GetStringExtra("Name");
            
			currentSpeaker = (from speaker in MonkeySpace.Core.ConferenceManager.Speakers.Values.ToList ()
                    where speaker.Name == name
                    select speaker).FirstOrDefault();

			if (currentSpeaker.Name != "")
            {
				FindViewById<TextView>(Resource.Id.Name).Text = currentSpeaker.Name;

				if (!String.IsNullOrEmpty(currentSpeaker.Bio))
					FindViewById<TextView>(Resource.Id.Bio).Text = currentSpeaker.Bio;
                else
                {
                    var tv = FindViewById<TextView>(Resource.Id.Bio);
                    tv.Text = "no speaker bio available";
                }
				sessions = currentSpeaker.Sessions;

                list = FindViewById<ListView>(Resource.Id.SessionList);
				list.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(_list_ItemClick);
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            refreshSpeaker();
        }
        private void refreshSpeaker()
        {
            list.Adapter = new SessionsSimpleAdapter(this, sessions);
        }

		private void _list_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var session = ((SessionsSimpleAdapter)list.Adapter).GetRow(e.Position);

            var intent = new Intent();
            intent.SetClass(this, typeof(SessionActivity));
            intent.PutExtra("Code", session.Code);

            StartActivity(intent);
        }
    }
}