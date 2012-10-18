using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace MonkeyScan
{
	public class HistoryController: DialogViewController
	{
		public HistoryController () : base (null)
		{
		}
		
		public override void ViewWillAppear (bool animated)
		{
			Root = GenerateRoot ();
			
			Console.WriteLine ("Summary " + Root.Summary() );
		}

		RootElement GenerateRoot ()
		{
			var root = 	new RootElement ("History") {
				new Section () {
					from s in AppDelegate.UserData.GetScans ()
					select (Element)new ScanElement(s)
				}
			};

			return root;
		}
	}
}

