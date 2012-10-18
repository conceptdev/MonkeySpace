using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using com.google.zxing;
using com.google.zxing.common;

namespace MonkeyScan
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		TabBarController tabBarController;

		public static SQLite.DataManager UserData {get; private set;} 

		// http://stackoverflow.com/questions/11332855/monotouch-zxing-camera-wont-open
		public override void OnResignActivation (UIApplication application)
		{
			UIApplication.SharedApplication.PerformSelector(new MonoTouch.ObjCRuntime.Selector("terminateWithSuccess"), null, 0f);
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			UserData = new SQLite.DataManager(SQLite.DataManager.DatabaseFilePath);

			tabBarController = new TabBarController ();

			if (UserData.CountAttendees() == 0)
				UserData.LoadSeedAttendeeData();

			window.RootViewController = tabBarController;
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

