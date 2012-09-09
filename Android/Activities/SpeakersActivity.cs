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
    [Activity(Label = "Speakers")]
    public class SpeakersActivity : BaseActivity
    {
        ListView list;
        List<MonkeySpace.Core.Speaker> speakers;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.CustomTitle); // BETTER: http://www.anddev.org/my_own_titlebar_backbutton_like_on_the_iphone-t4591.html
            SetContentView(Resource.Layout.Speakers);
            Window.SetFeatureInt(WindowFeatures.CustomTitle, Resource.Layout.WindowTitle); // http://www.londatiga.net/it/how-to-create-custom-window-title-in-android/

			speakers = MonkeySpace.Core.ConferenceManager.Speakers.Values.OrderBy ((s) => s.Name).ToList ();

            list = FindViewById<ListView>(Resource.Id.List);
			list.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(list_ItemClick);
        }
        protected override void OnResume()
        {
            base.OnResume();
            refreshSpeakers();
        }
        private void refreshSpeakers()
        {
            list.Adapter = new SpeakersAdapter(this, speakers);
        }

		private void list_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var speaker = ((SpeakersAdapter)list.Adapter).GetRow(e.Position);

            var intent = new Intent();
            intent.SetClass(this, typeof(SpeakerActivity));
            intent.PutExtra("Name", speaker.Name);
            
            StartActivity(intent);
        }
    }
}