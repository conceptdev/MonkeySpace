using System.Text;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Monospace11
{
	public class AboutViewController : WebViewControllerBase
	{
		UINavigationBar navBar;

		public AboutViewController () : base (44) 
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			webView.ShouldStartLoad = delegate(UIWebView webViewParam, NSUrlRequest request, UIWebViewNavigationType navigationType) {
				// view links in a new 'webbrowser' window like session & twitter
				if (navigationType == UIWebViewNavigationType.LinkClicked) {
					this.NavigationController.PushViewController (new WebViewController (request), true);
					return false;
				}
				return true;
			};

			navBar = new UINavigationBar (new RectangleF (0, 0, 320, 44));
			var bbi = new UIBarButtonItem(UIImage.FromBundle ("Images/slideout"), UIBarButtonItemStyle.Plain, (sender, e) => {
				AppDelegate.Current.FlyoutNavigation.ToggleMenu();
			});
			var item = new UINavigationItem ("About MonkeySpace");
			item.LeftBarButtonItem = bbi;
			var items = new UINavigationItem[] {
				item
			};
			navBar.SetItems (items, false);

			View.Add (navBar);
		}

		protected override void LoadHtmlString (string s)
		{
			string homePageUrl = NSBundle.MainBundle.BundlePath + "/About/Default.html";
			//webView.ScalesPageToFit = true;
			webView.LoadRequest (new NSUrlRequest (new NSUrl (homePageUrl, false)));
		}
	}
}