using System;
using FlyoutNavigation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace Monospace11
{
	public class CustomFlyoutNavigationController : FlyoutNavigationController
	{
		MonoTouch.UIKit.UINavigationController 
			navSessionController
				, navSpeakerController
				, navScheduleController
				, navFavoritesController;

		public CustomFlyoutNavigationController ()
		{
			var dvc = new HomeViewController ();

			navScheduleController = new MonoTouch.UIKit.UINavigationController();
			navScheduleController.PushViewController(dvc, false);
			navScheduleController.NavigationBar.BarStyle = UIBarStyle.Black;
			//			navScheduleController.TopViewController.Title ="What's on";
			//			navScheduleController.TabBarItem = new UITabBarItem("What's on", UIImage.FromFile("Images/83-calendar.png"), 0);

//			if (UIDevice.CurrentDevice.CheckSystemVersion (6,0)) {
//				// iOS 6 and above support CollectionView
//				var layout = new UICollectionViewFlowLayout (){
//					SectionInset = new UIEdgeInsets (0,0,0,0), 
////					ItemSize = SpeakerCollectionCell.Size,
//					MinimumInteritemSpacing = 10,
//					MinimumLineSpacing = 5
//				};
//				var svc = new SpeakersCollectionViewController (layout); // COLLECTION
//				navSpeakerController = new MonoTouch.UIKit.UINavigationController();
//				navSpeakerController.PushViewController(svc, false);
//				navSpeakerController.TopViewController.View.BackgroundColor = new UIColor(65.0f,169.0f,198.0f,255.0f);
//				navSpeakerController.NavigationBar.BarStyle = UIBarStyle.Black;
//				navSpeakerController.TopViewController.Title ="Speakers";
//			} else {
				// use a table
				var svc = new SpeakersViewController(); // TABLE
				navSpeakerController = new MonoTouch.UIKit.UINavigationController();
				navSpeakerController.PushViewController (svc, false);
				navSpeakerController.NavigationBar.BarStyle = UIBarStyle.Black;
//			}

			var ssvc = new TagsViewController(); 
			navSessionController = new MonoTouch.UIKit.UINavigationController();
			navSessionController.PushViewController(ssvc, false);
			navSessionController.NavigationBar.BarStyle = UIBarStyle.Black;
			//			navSessionController.TopViewController.Title ="Sessions";
			//			navSessionController.TabBarItem = new UITabBarItem("Sessions", UIImage.FromFile("Images/124-bullhorn.png"), 2);

			var mapViewController = new MapFlipViewController();
			mapViewController.View.BackgroundColor = UIColor.Black;

			var fvc = new FavoritesViewController();
			navFavoritesController = new MonoTouch.UIKit.UINavigationController();
			navFavoritesController.PushViewController(fvc, false);

			//var aboutViewController = new AboutViewController ();
			var passbookViewController = new PassKitViewController ();
			//var roomsViewController = new RoomsViewController ();

			// Create the navigation menu
			NavigationRoot = new RootElement ("Navigation") {
				new Section () {
					new StyledStringElement ("MonkeySpace"){BackgroundColor = UIColor.Clear, TextColor = UIColor.White, Font = AppDelegate.Current.FontFlyoutMenuSection},
					new StyledStringElement ("Sessions"){BackgroundColor = UIColor.Clear, TextColor = UIColor.LightGray, Font = AppDelegate.Current.FontFlyoutMenu},
					new StyledStringElement ("Speakers"){BackgroundColor = UIColor.Clear, TextColor = UIColor.LightGray, Font = AppDelegate.Current.FontFlyoutMenu},
					new StyledStringElement ("Favorites"){BackgroundColor = UIColor.Clear, TextColor = UIColor.LightGray, Font = AppDelegate.Current.FontFlyoutMenu},
					//new StyledStringElement ("Room Plan"){BackgroundColor = UIColor.Clear, TextColor = UIColor.LightGray, Font = AppDelegate.Current.FontFlyoutMenu},
					new StyledStringElement ("Location Map"){BackgroundColor = UIColor.Clear, TextColor = UIColor.LightGray, Font = AppDelegate.Current.FontFlyoutMenu},
					new StyledStringElement ("Passbook"){BackgroundColor = UIColor.Clear, TextColor = UIColor.LightGray, Font = AppDelegate.Current.FontFlyoutMenu},
					//new StyledStringElement ("About Evolve"){BackgroundColor = UIColor.Clear, TextColor = UIColor.LightGray, Font = AppDelegate.Current.FontFlyoutMenu},
				}
			};
			// Supply view controllers corresponding to menu items:
			ViewControllers = new UIViewController[] {
				navScheduleController
				, navSessionController
				, navSpeakerController
				, navFavoritesController
				//, roomsViewController
				, mapViewController
				, passbookViewController
				//, aboutViewController
			};

			View.BackgroundColor = UIColor.Blue;
		}
	}
}

