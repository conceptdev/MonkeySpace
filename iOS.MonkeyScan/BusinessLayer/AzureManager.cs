using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.CoreFoundation;
using MonoTouch.AVFoundation;
using MonoTouch.CoreVideo;
using MonoTouch.CoreMedia;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
namespace MonkeyScan
{
	public class AzureManager
	{
		// azure
		public static readonly MobileServiceClient MobileService = new MobileServiceClient (Constants.AzureUrl, Constants.AzureKey);
		public static readonly IMobileServiceTable<ConfScan> scanTable = MobileService.GetTable<ConfScan>();
		public static readonly IMobileServiceTable<ConfAttendee> attendeeTable = MobileService.GetTable<ConfAttendee>();
		// --

		public static Action<string> OnDownloadSucceeded;
		public static Action<string> OnDownloadFailed;

		public AzureManager ()
		{
		}

		public static void DownloadAttendees (TaskScheduler scheduler) {

			AzureManager.attendeeTable.ToListAsync()
					.ContinueWith (t =>
					{
						if (t.Status == TaskStatus.RanToCompletion) {
							var rows = t.Result;
							AppDelegate.UserData.UpdateAllAttendees(rows);
							if (OnDownloadSucceeded != null)
								OnDownloadSucceeded("");
					} else {
						
						if (OnDownloadFailed != null)
							OnDownloadFailed("Error " + t.Status);
					}
				}, scheduler);
		}
	}
}

