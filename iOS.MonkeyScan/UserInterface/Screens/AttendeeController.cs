using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace MonkeyScan
{
	public class AttendeeController: DialogViewController
	{
		public AttendeeController () : base (null)
		{
		}
		
		public override void ViewWillAppear (bool animated)
		{
			Root = GenerateRoot ();
			//RefreshAsync(); // maybe we don't need to do this, manual refresh instead :)
		}

		RootElement GenerateRoot ()
		{
			var root = 	new RootElement ("Attendees") {
				new Section () {
					from a in AppDelegate.UserData.GetAttendees()
						select (Element)new AttendeeElement(a)
				}
			};

			return root;
		}
	}
}