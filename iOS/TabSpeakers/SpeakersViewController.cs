using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Linq;

namespace Monospace11
{
	/// <summary>
	/// Lists Speakers
	/// </summary>
	/// <remarks>
	/// LOADS data from the json file
	/// </remarks>
	[Register]
	public class SpeakersViewController : UIViewController
	{
		private UITableView tableView;
	
		private List<MonkeySpace.Core.Speaker> speakerData;

		/// <remarks>
		/// Background image idea from 
		/// http://mikebluestein.wordpress.com/2009/10/05/setting-an-image-background-on-a-uitableview-using-monotouch/
		/// </remarks>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//_speakerData = AppDelegate.ConferenceData.Speakers;
			speakerData = MonkeySpace.Core.ConferenceManager.Speakers.Values.ToList ();
			
			UIImageView imageView = new UIImageView (UIImage.FromFile ("Background.png"));
			imageView.Frame = new RectangleF (0, 0, View.Frame.Width, View.Frame.Height);
			imageView.UserInteractionEnabled = true;

			tableView = new UITableView { Source = new TableViewSource (this, speakerData)
				, AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth
				, BackgroundColor = UIColor.Clear
				, Frame = new RectangleF (0, 0, View.Frame.Width, View.Frame.Height - 93) 
				, ShowsVerticalScrollIndicator = true};
			
			imageView.AddSubview (tableView);
			View.AddSubview (imageView);
		}

		private class TableViewSource : UITableViewSource
		{
			private SpeakersViewController svc;
			List<string> sectionTitles;
			// for index
			private SortedDictionary<int, List<MonkeySpace.Core.Speaker>> sectionElements = new SortedDictionary<int, List<MonkeySpace.Core.Speaker>> ();
			// for index
			public TableViewSource (SpeakersViewController controller, List<MonkeySpace.Core.Speaker> speakers)
			{
				svc = controller;

				var newSpeakers = from nonempty in speakers
					where !(String.IsNullOrEmpty (nonempty.Bio) && nonempty.Sessions.Count == 0)
					select nonempty;
				speakers = newSpeakers.ToList ();

				// apologies in advance for this dodgy linq/loop - could do better...[CD]
				sectionTitles = (from c in speakers
					select c.Name.Substring (0, 1)).Distinct ().ToList ();
				// sort the list alphabetically
				sectionTitles.Sort ();
				// add each element to the List<Element> according to the letter it starts with
				// in the SortedDictionary<int, List<Element>>
				foreach (var element in speakers) {
					// sectionNum a=1, b=2, c=3, etc
					int sectionNum = sectionTitles.IndexOf (element.Name.Substring (0, 1));
					if (sectionElements.ContainsKey (sectionNum)) {
						// SortedDictionary already contains a List<Element> for that letter
						sectionElements[sectionNum].Add (element);
					} else {
						// first time that letter has appeared, create new List<Element> in the SortedDictionary
						sectionElements.Add (sectionNum, new List<MonkeySpace.Core.Speaker> { element });
					}
				}
			}

			public override NSIndexPath WillSelectRow (UITableView tableView, NSIndexPath indexPath)
			{
				var s = sectionElements[indexPath.Section][indexPath.Row];
				if (String.IsNullOrEmpty (s.Bio) && s.Sessions.Count == 0)
					return null;
				else
					return indexPath;
			}

			private SpeakerBioViewController bioVC;

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				MonkeySpace.Core.Speaker s = sectionElements[indexPath.Section][indexPath.Row];
				if (bioVC == null)
					bioVC = new SpeakerBioViewController (s);
				else
					bioVC.Update (s);
				
				bioVC.Title = s.Name;
				svc.NavigationController.PushViewController (bioVC, true);
				tableView.DeselectRow (indexPath, true);
			}

			public override int NumberOfSections (UITableView tableView)
			{
				return sectionTitles.Count;
			}
			public override int RowsInSection (UITableView tableview, int section)
			{
				return sectionElements[section].Count;
			}
			public override string[] SectionIndexTitles (UITableView tableView)
			{
				return sectionTitles.ToArray ();
			}

			static NSString kCellIdentifier = new NSString ("MySpeakerIdentifier");
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell (kCellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, kCellIdentifier);
					AppDelegate.GetCellSelectedColor (cell);
				}
				var s = sectionElements[indexPath.Section][indexPath.Row];
				cell.TextLabel.Text = s.Name;
				return cell;
			}
		}
	}
}
