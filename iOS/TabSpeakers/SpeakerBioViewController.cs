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
		MonkeySpace.Core.Speaker speaker;

		public SpeakerBioViewController (MonkeySpace.Core.Speaker speaker) : base()
		{
			this.speaker = speaker;
		}
		public void Update (MonkeySpace.Core.Speaker speaker)
		{
			this.speaker = speaker;
			LoadHtmlString (FormatText ());
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			webView.Delegate = new WebViewDelegate (this);
		}
		class WebViewDelegate : UIWebViewDelegate
		{
			private SpeakerBioViewController viewController;
			public WebViewDelegate (SpeakerBioViewController speakerViewController)
			{
				viewController = speakerViewController;
			}
			private SessionViewController sessVC;

			public override bool ShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
			{
				if (navigationType == UIWebViewNavigationType.LinkClicked) {
					string path = request.Url.Path.Substring(1);
					string host = request.Url.Host.ToLower ();
					if (host == "tweet.monkeyspace.app") {
						var tweet = new TWTweetComposeViewController();
						tweet.SetInitialText ("@" + path + " " + MonkeySpace.Core.Constants.TwitterHashTag);
						viewController.PresentModalViewController(tweet, true);
					} else if (host == "twitter.monkeyspace.app") {
						var nsurl = new NSUrl("twitter://user?screen_name="+viewController.speaker.TwitterHandle);
						UIApplication.SharedApplication.OpenUrl (nsurl);
					} else if (host == "session.monkeyspace.app") {
						if (sessVC == null)
							sessVC = new SessionViewController (path);
						else
							sessVC.Update (path);
						viewController.NavigationController.PushViewController (sessVC, true);
					}
					else
					{
						viewController.NavigationController.PushViewController (new WebViewController (request), true);
						return false;
					}
				}
				return true;
			}
			
		}
	
		protected override string FormatText ()
		{
			StringBuilder sb = new StringBuilder ();
			
			sb.Append (StyleHtmlSnippet);
			sb.Append ("<h2>" + speaker.Name + "</h2>" + Environment.NewLine);

			if (!string.IsNullOrEmpty (speaker.HeadshotUrl)) {
				sb.Append ("<img height=160 width=160 align=right src='http://monkeyspace.org" + speaker.HeadshotUrl + "'>" + Environment.NewLine);
			}

			if (TWTweetComposeViewController.CanSendTweet) {
				var nsurl = new NSUrl("twitter://user?screen_name="+speaker.TwitterHandle);
				if (UIApplication.SharedApplication.CanOpenUrl(nsurl)){
					sb.Append ("<p><a href='http://twitter.monkeyspace.app/" + speaker.TwitterHandle + "' style='font-weight:normal'>@" + speaker.TwitterHandle + "</a>");
				} else {
					sb.Append ("<p><a href='http://tweet.monkeyspace.app/" + speaker.TwitterHandle + "' style='font-weight:normal'>@" + speaker.TwitterHandle + "</a>");
				}

				sb.Append ("<br /><a href='http://tweet.monkeyspace.app/" + speaker.TwitterHandle + "' style='font-weight:normal'><img height=22 width=58 src='Images/Tweet.png'></a></p>");
			} else {
				// can't send tweet, but maybe twitter is installed
				var nsurl = new NSUrl("twitter://user?screen_name="+speaker.TwitterHandle);
				if (UIApplication.SharedApplication.CanOpenUrl(nsurl)) {
					sb.Append ("<p><a href='http://twitter.monkeyspace.app/" + speaker.TwitterHandle + "' style='font-weight:normal'>@" + speaker.TwitterHandle + "</a>");
				}
			}


			if (!string.IsNullOrEmpty (speaker.Bio)) {
				sb.Append ("<span class='body'>" + speaker.Bio + "</span><br/>" + Environment.NewLine);
			}
			sb.Append ("<br />");
			foreach (var session in speaker.Sessions) {
				sb.Append ("<div class='sessionspeaker'><a href='http://session.monkeyspace.app/" + session.Code + "' class='sessionspeaker'>" + session.Title + "</a></div><br />");
			}		
			return sb.ToString ();
		}
	}
	
}
