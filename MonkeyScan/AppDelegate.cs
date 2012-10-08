using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using com.google.zxing;
using com.google.zxing.common;

namespace QrSample.iOS
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


//			Console.WriteLine("Creating");
//			var path = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
//			var filename = System.IO.Path.Combine(path,"qrcode.png");
//			var data = "This is a test to see if I can encode and decode this data...";
//
//			ByteMatrix matrix = new MultiFormatWriter().encode(
//				data, BarcodeFormat.QR_CODE, 150, 150);
//
//			com.google.zxing.qrcode.MatrixToImageWriter.writeToFile (matrix, filename);
//		
//			//BinaryBitmap binaryBitmap = new BinaryBitmap(new HybridBinarizer(new BufferedImageLuminanceSource(ImageIO.read(new FileInputStream(filename)))))
//			//Result result = new MultiFormatReader().decode(binaryBitmap, hints)
//			
//			//result.getText() 
//
//			Console.WriteLine (filename);


			window.RootViewController = tabBarController; //viewController;
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

