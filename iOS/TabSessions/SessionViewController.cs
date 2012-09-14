using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Diagnostics;
using System.Linq;
using MonoTouch.Twitter;

namespace Monospace11
{
	public class SessionViewController : WebViewControllerBase
	{
		public MonkeySpace.Core.Session DisplaySession;
		public bool IsFromFavoritesView = false;
		public SessionViewController (MonkeySpace.Core.Session session) : base()
		{
			DisplaySession = session;
		}
		public SessionViewController (MonkeySpace.Core.Session session, bool isFromFavs) : this (session)
		{
			IsFromFavoritesView = isFromFavs;
			DisplaySession = session;
		}
		public SessionViewController (string sessionCode) : base()
		{
			foreach (var s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList () ) //AppDelegate.ConferenceData.Sessions)
			{
				if (s.Code == sessionCode)
				{	
					DisplaySession = s;
				}
			}
			
		}
		public void Update (string sessionCode) 
		{
			foreach (var s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList () ) //AppDelegate.ConferenceData.Sessions)
			{
				if (s.Code == sessionCode)
				{
					DisplaySession = s;
				}
			}
			if (DisplaySession != null) LoadHtmlString(FormatText()); 
		}

		public void Update (MonkeySpace.Core.Session session) 
		{
			DisplaySession = session;
			LoadHtmlString(FormatText());
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			webView.ShouldStartLoad = delegate (UIWebView webViewParam, NSUrlRequest request, UIWebViewNavigationType navigationType)
			{	// Catch the link click, and process the add/remove favorites
				if (navigationType == UIWebViewNavigationType.LinkClicked)
				{
					string path = request.Url.Path.Substring(1);
					string host = request.Url.Host.ToLower ();
					if (host == "tweet.mix10.app") {
						var tweet = new TWTweetComposeViewController();
						tweet.SetInitialText ("I'm in '" + DisplaySession.Title + "' at #monkeyspace" );
						PresentModalViewController(tweet, true);

					} else if (host == "add.mix10.app") {
						AppDelegate.UserData.AddFavoriteSession(path);
						Update(DisplaySession);
					}
					else if (host == "remove.mix10.app")
					{	// "remove.MIX10.app"
						AppDelegate.UserData.RemoveFavoriteSession(path);
						if (IsFromFavoritesView)
						{	// once unfavorited, hide and go back to list view
							NavigationController.PopViewControllerAnimated(true);
						}
						else
						{
							Update(DisplaySession);
						}
					}
					else
					{
						NavigationController.PushViewController (new WebViewController (request), true);
						return false;
					}
				}
				return true;
			};
		}

		protected override string FormatText()
		{
			string timeFormat = "H:mm";
			StringBuilder sb = new StringBuilder();
			sb.Append(StyleHtmlSnippet);
			sb.Append("<h2>"+DisplaySession.Title+"</h2>"+ Environment.NewLine);

			if (TWTweetComposeViewController.CanSendTweet) {
				sb.Append ("<p><a href='http://tweet.mix10.app/' style='font-weight:normal'>tweet</a></p>");
			}

			if (AppDelegate.UserData.IsFavorite(DisplaySession.Code))
			{	// okay this is a little bit of a HACK:
				sb.Append(@"<nobr><a href=""http://remove.mix10.app/"+DisplaySession.Code+@"""><img src='Images/favorited.png' align='right' border='0'/></a></nobr>");
			}
			else {
				sb.Append(@"<nobr><a href=""http://add.mix10.app/"+DisplaySession.Code+@"""><img src='Images/favorite.png' align='right' border='0'/></a></nobr>");
			}
			sb.Append("<br/>");
			if (DisplaySession.Speakers.Count > 0)
			{
				sb.Append("<span class='sessionspeaker'>"+DisplaySession.GetSpeakerList() +"</span> "+ Environment.NewLine);
				sb.Append("<br/>");
			}

			sb.Append("<span class='sessiontime'>"
					+ DisplaySession.Start.ToString("ddd MMM dd") + " " 
					+ DisplaySession.Start.ToString(timeFormat)+" - " 
					+ DisplaySession.End.ToString(timeFormat) +"</span><br />"+ Environment.NewLine);

			if (!String.IsNullOrEmpty (DisplaySession.Location))
			{
				sb.Append("<span class='sessionroom'>"+DisplaySession.LocationDisplay+"</span><br />"+ Environment.NewLine);
				sb.Append("<br />"+ Environment.NewLine);
			}
			sb.Append("<span class='body'>"+DisplaySession.Abstract+"</span>"+ Environment.NewLine);
				
			return sb.ToString();
		}
	}
}