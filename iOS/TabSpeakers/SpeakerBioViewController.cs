using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Twitter;

namespace Monospace11
{
	/// <remarks>
	/// Uses UIWebView since we want to format the text display (with HTML)
	/// </remarks>
	public class SpeakerBioViewController : WebViewControllerBase
	{
		MonkeySpace.Core.Speaker _speaker;

		public SpeakerBioViewController (MonkeySpace.Core.Speaker speaker) : base()
		{
			_speaker = speaker;
		}
		public void Update (MonkeySpace.Core.Speaker speaker)
		{
			_speaker = speaker;
			LoadHtmlString (FormatText ());
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			webView.Delegate = new WebViewDelegate (this);
		}
		class WebViewDelegate : UIWebViewDelegate
		{
			private SpeakerBioViewController _c;
			public WebViewDelegate (SpeakerBioViewController bc)
			{
				_c = bc;
			}
			private SessionViewController sessVC;

			public override bool ShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
			{
				if (navigationType == UIWebViewNavigationType.LinkClicked) {
					string path = request.Url.Path.Substring (1);
					var pathArray = path.Split ('/');
					if (pathArray[0] == "tweet") {
						string handle = pathArray[1];
						var tweet = new TWTweetComposeViewController();
						tweet.SetInitialText ("@" + handle + " #monkeyspace" );
						_c.PresentModalViewController(tweet, true);
					} else {
						if (sessVC == null)
							sessVC = new SessionViewController (path);
						else
							sessVC.Update (path);
						_c.NavigationController.PushViewController (sessVC, true);
					}
				}
				return true;
			}
			
		}
	
		protected override string FormatText ()
		{
			StringBuilder sb = new StringBuilder ();
			
			sb.Append (StyleHtmlSnippet);
			sb.Append ("<h2>" + _speaker.Name + "</h2>" + Environment.NewLine);

			if (TWTweetComposeViewController.CanSendTweet) {
				sb.Append ("<p><a href='http://MIX10.app/tweet/"+_speaker.TwitterHandle +"' style='font-weight:normal'>@" + _speaker.TwitterHandle + "</a></p>");
			}

			if (!string.IsNullOrEmpty (_speaker.HeadshotUrl)) {
				sb.Append ("<img height=160 width=160 align=right src='http://monkeyspace.org" + _speaker.HeadshotUrl + "'>" + Environment.NewLine);
			}
			if (!string.IsNullOrEmpty (_speaker.Bio)) {
				sb.Append ("<span class='body'>" + _speaker.Bio + "</span><br/>" + Environment.NewLine);
			}
			sb.Append ("<br />");
			foreach (var session in _speaker.Sessions) {
				sb.Append ("<div class='sessionspeaker'><a href='http://MIX10.app/" + session.Code + "' class='sessionspeaker'>" + session.Title + "</a></div><br />");
			}		
			return sb.ToString ();
		}
	}
	
}
