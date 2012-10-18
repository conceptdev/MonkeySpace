using System;
using System.Collections;
using System.Drawing;
using System.Text;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Threading.Tasks;

namespace MonkeyScan
{
	public class StatsController : DialogViewController
	{
		public TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();

		public StatsController () : base (null)
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (6,0)) {
				// UIRefreshControl iOS6
				RefreshControl = new UIRefreshControl();
				RefreshControl.ValueChanged += HandleValueChanged;
				AzureManager.OnDownloadSucceeded += (jsonString) => {
					Console.WriteLine ("OnDownloadSucceeded");
					InvokeOnMainThread (() => {
						UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
						RefreshControl.EndRefreshing ();
					});
				};
				AzureManager.OnDownloadFailed += (err) => {
					Console.WriteLine ("OnDownloadFailed");
					InvokeOnMainThread (() => {
						UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
						RefreshControl.EndRefreshing ();
					});
				};

			} else {
				// old style refresh button and no PassKit for older iOS
				NavigationItem.SetRightBarButtonItem (new UIBarButtonItem (UIBarButtonSystemItem.Refresh), false);
				NavigationItem.RightBarButtonItem.Clicked += (sender, e) => { Refresh(); };
			}
		}
		// UIRefreshControl iOS6
		void HandleValueChanged (object sender, EventArgs e)
		{
			Refresh();
		}
		void Refresh() 
		{
			Console.WriteLine ("Refresh data from server");
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			AzureManager.DownloadAttendees (scheduler);
		}


		public override void ViewWillAppear (bool animated)
		{
			Root = GenerateRoot ();
		}

		RootElement GenerateRoot ()
		{
			int total, present, absent;
			total = AppDelegate.UserData.CountAttendees();
			present = AppDelegate.UserData.CountPresent();
			absent = total - present;

			return new RootElement ("MonkeyScan") {
				new Section ("Attendees", null){
					new StatsElement ("Total",total.ToString ()),
					new StatsElement ("Present",present.ToString ()),
					new StatsElement ("Absent",absent.ToString ())
				},
				new Section ("Data", null) {
					new StringElement ("Export to Documents", () => {
						Console.WriteLine("Exporting Attendees");
						var all = AppDelegate.UserData.GetAttendees();
						var sb = new StringBuilder();
						foreach (var a in all) {
							sb.Append(String.Format("{0},{1},{2},{3},{4}\n",a.Barcode, a.Name.Replace(",",""), a.ScanCount, a.Id, a.SqlId));
						}
						var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
						var filename = System.IO.Path.Combine (documents, "Attendees.csv");
						if (System.IO.File.Exists(filename))
							System.IO.File.Delete (filename);
						System.IO.File.WriteAllText(filename, sb.ToString ());
						Console.WriteLine("Exported to " + filename);

						Console.WriteLine("Exporting Scans");
						var allscans = AppDelegate.UserData.GetScans();
						sb = new StringBuilder();
						foreach (var b in allscans) {
							sb.Append(String.Format("{0},{1},{2},{3},{4}\n", b.ScannedAt.ToString ("yyyy-MM-dd HH:mm:ss"), b.Barcode, b.AttendeeName, b.Id, b.SqlId));
						}
						filename = System.IO.Path.Combine (documents, "Scans.csv");
						if (System.IO.File.Exists(filename))
							System.IO.File.Delete (filename);
						System.IO.File.WriteAllText(filename, sb.ToString ());
						Console.WriteLine("Exported to " + filename);
					})
				}
			};		
		}
	}
}

