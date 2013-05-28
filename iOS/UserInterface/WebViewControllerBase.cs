using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
namespace Monospace11
{
	public class WebViewControllerBase : UIViewController
	{
		protected string basedir;
		protected UIWebView webView;

		public WebViewControllerBase () : base()
		{
		}

		int topPosition = 0;
		public WebViewControllerBase (int top) : base()
		{
			topPosition = top;
		}

		/// <summary>
		/// Shared Css styles
		/// </summary>
		public string StyleHtmlSnippet
		{
			get 
			{  // http://jonraasch.com/blog/css-rounded-corners-in-all-browsers
				return "<style>" +
				"body {background-image:url('Images/Background.png'); background-color:#F0F0F0; }"+
				"body,b,i,p,h1,h2{font-family:Avenir,HelveticaNeue,Helvetica;}" +
				"h1,h2{color:#D4563E;}" + //BC1718
				"h1,h2{margin-bottom:0px;}" +
				".footnote{font-size:small;}" +
				".sessionspeaker{color:#444444;font-weight:bold;}" +
				".sessionroom{color:#666666;}" +
				".sessiontime{color:#666666;}" +
				".sessiontag{color:#800020;}" +
				"div.sessionspeaker { -webkit-border-radius:0px; background:white; width:285; color:black; padding:8 10 10 8;  }" +
				"a.sessionspeaker {color:black; text-decoration:none;}"+
				"</style>";
			}
		}
		public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
			basedir = Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			basedir = System.IO.Path.Combine(basedir, "..", "iOS.app"); // HACK: bad bad bad
			Console.WriteLine ("~~~"+System.Environment.CurrentDirectory);
			// no XIB !
			webView = new UIWebView()
			{
				ScalesPageToFit = false,
			};
			LoadHtmlString(FormatText());
            webView.SizeToFit();
			webView.Frame = new RectangleF (0, topPosition, this.View.Frame.Width, this.View.Frame.Height-44);
            // Add the table view as a subview
            this.View.AddSubview(webView);
		}
		/// <summary>
		/// Override this in subclass
		/// </summary>
		protected virtual string FormatText()
		{ return ""; }

		protected virtual void LoadHtmlString (string s)
		{
			webView.LoadHtmlString(s, new NSUrl(basedir, true));
		}
	}
}