using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MonkeyScan
{
	public class TabBarController : UITabBarController
	{
		UIViewController qrReader, history, attendees;
		//UINavigationController  ;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			qrReader = new QrScannerViewController();
			qrReader.Title = "Scan";

			attendees = new AttendeeController();
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

