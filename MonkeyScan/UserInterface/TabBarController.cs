using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace QrSample.iOS
{
	public class TabBarController : UITabBarController
	{
		UIViewController qrReader, history;
		UINavigationController attendees ;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			qrReader = new QrScannerViewController();
			qrReader.Title = "Scan";

			attendees = new UINavigationController();
			attendees.Title = "Attendees";

			history = new HistoryController();
			history.Title = "History";

			var u = new UIViewController[]
			{
				attendees,
				qrReader,
				history
			};
			
			SelectedIndex = 0;
			ViewControllers = u;

			CustomizableViewControllers = new UIViewController[]{};
		}
	}
}

