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
		// azure
		private static readonly MobileServiceClient MobileService = new MobileServiceClient (Constants.AzureUrl, Constants.AzureKey);
		//private readonly IMobileServiceTable<ConfAttendee> attendeeTable = MobileService.GetTable<ConfAttendee>();
		private readonly IMobileServiceTable<ConfScan> attendeeTable = MobileService.GetTable<ConfScan>();
		private TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
		// --

		private List<ConfScan> items = new List<ConfScan>();

		public AttendeeController () : base (null)
		{
		}
		
		public override void ViewWillAppear (bool animated)
		{
			Root = GenerateRoot ();
			RefreshAsync(); // maybe we don't need to do this, manual refresh instead :)
		}

		RootElement GenerateRoot ()
		{
			var root = 	new RootElement ("Attendees") {
				new Section () {
					from a in items
						select (Element)new StringElement(a.Barcode)
				}
			};

			return root;
		}

		public void RefreshAsync()
		{
			//IsUpdating = true;
			this.attendeeTable.ToListAsync()
				.ContinueWith (t =>
				               {
					this.items = t.Result;
					Root = GenerateRoot ();
					//IsUpdating = false;
				}, scheduler);
		}

	}
}

