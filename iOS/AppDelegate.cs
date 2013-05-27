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
		static AppDelegate current;
		UIWindow window;
		public static AppDelegate Current {
			get { return current; }
		}

        UIImageView splashView;
		
		TabBarController tabBarController;
		public static UserDatabase UserData {get; private set;} 
		public static ConferenceManager Conference { get; private set; }
		string documentsPath;
		string libraryPath;
		string jsonPath;
		/// <summary>userdata.db</summary>
		static string SqliteDataFilename = "userdata.db";
		
		/// <summary>
		/// Loads the best sessions.json it can find - first look in SpecialFolder 
		/// (if not there, load the one that was included in the app download)
		/// </summary>
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {
			AppDelegate.current = this;

			documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
			var builtInJsonPath = Path.Combine (System.Environment.CurrentDirectory, ConferenceManager.JsonDataFilename);
			jsonPath = Path.Combine (libraryPath, ConferenceManager.JsonDataFilename); // 
			UserData = new UserDatabase(Path.Combine (libraryPath, SqliteDataFilename));

			Conference = new ConferenceManager();
			Conference.OnDownloadSucceeded += (jsonString) => {
				File.WriteAllText (jsonPath, jsonString);
				NSUserDefaults.StandardUserDefaults.SetString(ConferenceManager.LastUpdatedDisplay, "LastUpdated");

				Console.WriteLine("Local json file updated " + jsonPath);
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			};
			Conference.OnDownloadFailed += (error) => {
				Console.WriteLine("OnDownloadFailed:" + error);
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			};
			
			#region Get session data from json into memory...

			var json = "";
			if (!File.Exists(jsonPath)) {
				//jsonPath = builtInJsonPath; // use the bundled file
				NSUserDefaults.StandardUserDefaults.SetString("2012-09-15 15:15:15", "LastUpdated");

				File.Copy (builtInJsonPath, jsonPath); // so it is there for loading
			}
			json = File.ReadAllText(jsonPath);

			MonkeySpace.Core.ConferenceManager.LoadFromString (json);

			#endregion

			// Create the tab bar
			tabBarController = new TabBarController ();

			//#d4563e
			UINavigationBar.Appearance.TintColor = new UIColor(212/255f, 86/255f, 62/255f, 1f);

			// Create the main window and add the navigation controller as a subview
//			window = new UIWindow (UIScreen.MainScreen.Bounds);
//			window.RootViewController = tabBarController;
//			window.MakeKeyAndVisible ();
//			showSplashScreen();

			FlyoutNavigation = new CustomFlyoutNavigationController ();

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.RootViewController = FlyoutNavigation;
			window.MakeKeyAndVisible ();
			
            return true;
		}
		public CustomFlyoutNavigationController FlyoutNavigation;

		public void Refresh () 
		{
			AppDelegate.Conference.DownloadFromServer();
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
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







		#region iOS6 and iOS5 font support
		// seealso: WebViewControllerBase.cs for CSS specification
		static string FontName {
			get {
				if (UIDevice.CurrentDevice.CheckSystemVersion (6,0)) 
					return "Avenir";
				else
					return "HelveticaNeue";
			}
		}
		static string FontMediumName {
			get {
				if (UIDevice.CurrentDevice.CheckSystemVersion (6,0)) 
					return "Avenir-Medium";
				else
					return "HelveticaNeue-Medium";
			}
		}
		static string FontLightName {
			get {
				if (UIDevice.CurrentDevice.CheckSystemVersion (6,0)) 
					return "Avenir-Light";
				else
					return "HelveticaNeue-Light";
			}
		}
		#endregion
		public UIFont FontFlyoutMenu = UIFont.FromName (FontLightName, 18f); // Avenir-Heavy, Avenir-Black, Avenir-Medium, ...Oblique
		public UIFont FontFlyoutMenuSection = UIFont.FromName (FontName, 18f); // Avenir-Heavy, Avenir-Black, Avenir-Medium, ...Oblique
		public UIFont FontCell = UIFont.FromName (FontLightName, 18f); // Avenir-Heavy, Avenir-Black, Avenir-Medium, ...Oblique
		public UIFont FontCellMedium = UIFont.FromName (FontLightName, 16f); // Avenir-Heavy, Avenir-Black, Avenir-Medium, ...Oblique
		public UIFont FontCellSmall = UIFont.FromName (FontLightName, 14f); // Avenir-Heavy, Avenir-Black, Avenir-Medium, ...Oblique
		public UITextAttributes FontTitleTextAttributes {
			get {
				var fontTitleTextAttributes = new UITextAttributes();
				fontTitleTextAttributes.Font = UIFont.FromName(FontMediumName, 20f);
				return fontTitleTextAttributes;
			}
		}
		public UITextAttributes FontBackTextAttributes {
			get {
				var fontTitleTextAttributes = new UITextAttributes();
				fontTitleTextAttributes.Font = UIFont.FromName(FontLightName, 12f);
				return fontTitleTextAttributes;
			}
		}







        // This method is allegedly required in iPhoneOS 3.0
        public override void OnActivated (UIApplication application)
        {
        }
    }
}