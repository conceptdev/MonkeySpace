using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Monospace11
{
	public class WebViewController : UIViewController {
		UIWebView webView;
		UIToolbar navBar;
		NSUrlRequest url;
		UIBarButtonItem [] items;
		
		public WebViewController (NSUrlRequest url) : base ()
		{
			this.url = url;
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			navBar = new UIToolbar ();
			navBar.Frame = new RectangleF (0, this.View.Frame.Height-130, this.View.Frame.Width, 40);
			
			items = new UIBarButtonItem [] {
				new UIBarButtonItem ("Back", UIBarButtonItemStyle.Bordered, (o, e) => { webView.GoBack (); }),
				new UIBarButtonItem ("Forward", UIBarButtonItemStyle.Bordered, (o, e) => { webView.GoForward (); }),
				new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace, null),
				new UIBarButtonItem (UIBarButtonSystemItem.Refresh, (o, e) => { webView.Reload (); }),
				new UIBarButtonItem (UIBarButtonSystemItem.Stop, (o, e) => { webView.StopLoading (); })
			};
			navBar.Items = items;

			webView = new UIWebView ();
			webView.Frame = new RectangleF (0, 0, View.Frame.Width, View.Frame.Height - 130);
			
			webView.LoadStarted += delegate {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			};
			webView.LoadFinished += delegate {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			};
			
			webView.ScalesPageToFit = true;
			webView.SizeToFit ();
			webView.LoadRequest (url);
			
			View.AddSubview (webView);
			View.AddSubview (navBar);
		}
	}	
}

