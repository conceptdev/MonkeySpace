using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.ObjCRuntime;
using System.Threading;
using System.Diagnostics;
using MonkeySpace.Core;

namespace Monospace11
{
	/// <summary>
	/// Starting point for our MonoTouch application. Specifies the AppDelegate to load to kick things off
	/// </summary>
	public class Application
	{
		static void Main (string[] args)
		{
			try
			{
				UIApplication.Main (args, null, "AppDelegate");
			}
			catch (Exception ex)
			{	// HACK: this is just here for debugging
				Debug.WriteLine(ex);
			}
		}
	}

	/// <summary>
	/// ROOT of this application; referenced in "Main.cs"
	/// </summary>
	[Register ("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        UIWindow window;
		UIImageView splashView;
		
		TabBarController tabBarController;
		public static UserDatabase UserData {get; private set;} 

		/// <summary>userdata.db</summary>
		public static string SqliteDataFilename = "userdata.db";
		
		/// <summary>
		/// Loads the best conf.xml it can find - first look in SpecialFolder 
		/// (if not there, load the one that was included in the app download)
		/// </summary>
		/// <remarks>
		/// I wonder if there could be a problem with newer app code trying to 
		/// open an older Xml after an upgrade is installed? 
		/// I guess newer apps that aren't backward compatible
		/// should use a different filename eg. conf2.xml...
		/// </remarks>
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {
			// setup SQLite for 'starred sessions' database
			var basedir = Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			UserData = new UserDatabase(Path.Combine (basedir, SqliteDataFilename));

			#region Get All Session data...

			MonkeySpace.Core.ConferenceManager.LoadFromFile();

			#endregion

			UIApplication.SharedApplication.SetStatusBarStyle (UIStatusBarStyle.Default, false);
			UINavigationBar.Appearance.TintColor = new UIColor(188/255f, 23/255f, 24/255f, 1f);

			// Create the tab bar
			tabBarController = new TabBarController ();
			// Create the main window and add the navigation controller as a subview
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.AddSubview(tabBarController.View);
			window.MakeKeyAndVisible ();
			showSplashScreen();
			
            return true;
		}
		
		void showSplashScreen ()
		{
			splashView = new UIImageView(new RectangleF(0f, 0f, 320f, 480f));
			splashView.Image = UIImage.FromFile("Default.png");
			window.AddSubview(splashView);
			window.BringSubviewToFront(splashView);
			UIView.BeginAnimations("SplashScreen");
			UIView.SetAnimationDuration(0.5f);
			UIView.SetAnimationDelegate(this);
			UIView.SetAnimationTransition(UIViewAnimationTransition.None, window, true);
			UIView.SetAnimationDidStopSelector(new Selector("completedAnimation"));
		    splashView.Alpha = 0f;
		    splashView.Frame = new RectangleF(-60f, -60f, 440f, 600f);
		    UIView.CommitAnimations();
		}

		[Export("completedAnimation")]
		void StartupAnimationDone()
		{
			Debug.WriteLine ("Done");
			splashView.RemoveFromSuperview();
			splashView.Dispose();
		}

		
		public static void GetCellSelectedColor(UITableViewCell cell)
		{
			using (var v = new UIView(cell.Frame))
			{
				var LightBlue = new UIColor(0.29f, 0.50f, 0.53f, 255.0f);
				v.BackgroundColor = LightBlue;
				cell.SelectedBackgroundView = v;
			}
		}
		
        // This method is allegedly required in iPhoneOS 3.0
        public override void OnActivated (UIApplication application)
        {
        }
    }
}