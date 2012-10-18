using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MonkeyScan
{
	public class TabBarController : UITabBarController
	{
		UIViewController qrReader, history, attendees, stats;
		UINavigationController statsNav;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			stats = new StatsController();
			statsNav = new UINavigationController();
			statsNav.PushViewController (stats, false);
			statsNav.TabBarItem = new UITabBarItem("Stats", UIImage.FromFile("Images/77-ekg.png"), 0);

			qrReader = new QRScannerViewController();
			qrReader.TabBarItem = new UITabBarItem("Scan", UIImage.FromFile("Images/64-zap.png"), 1);

			attendees = new AttendeeController();
			attendees.TabBarItem = new UITabBarItem("Attendees", UIImage.FromFile("Images/TabMonkey.png"), 2);

			history = new HistoryController();
			history.TabBarItem = new UITabBarItem("History", UIImage.FromFile("Images/112-group.png"), 3);

			var u = new UIViewController[]
			{
				statsNav,
				qrReader,
				attendees,
				history
			};
			
			SelectedIndex = 0;
			ViewControllers = u;

			CustomizableViewControllers = new UIViewController[]{};

			//#d4563e
			UINavigationBar.Appearance.TintColor = new UIColor(212/255f, 86/255f, 62/255f, 1f);
		}
	}
}

