using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


namespace MonkeyScan {
	/// <summary>
	/// Speaker element.
	/// on iPhone, pushes via MT.D
	/// on iPad, sends view to SplitViewController
	/// </summary>
	public class ScanElement : Element  {
		static NSString cellId = new NSString ("confattendee");
		ConfScan scan;

		public ScanElement (ConfScan sc) : base (sc.Barcode)
		{
			scan = sc;
		}

		static int count;
		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = tv.DequeueReusableCell (cellId);
			count++;
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellId);

			cell.TextLabel.Text = scan.AttendeeName;
			cell.DetailTextLabel.Text = scan.Barcode + " (" + scan.ScannedAt + ")";

			if (scan.IsValid) {
				cell.TextLabel.TextColor = UIColor.Black;
			} else {
				cell.TextLabel.TextColor = UIColor.Red;
			}

			return cell;
		}

		/// <summary>Implement MT.D search on name and company properties</summary>
		public override bool Matches (string text)
		{
			return (scan.Barcode).ToLower ().IndexOf (text.ToLower ()) >= 0;
		}

//		public override void Selected (DialogViewController dvc, UITableView tableView, MonoTouch.Foundation.NSIndexPath path)
//		{
//			if (splitView != null)
//				splitView.ShowSpeaker (speaker.ID);
//			else {
//				var sds = new MWC.iOS.Screens.iPhone.Speakers.SpeakerDetailsScreen (speaker.ID);
//				sds.Title = "Speaker";
//				dvc.ActivateController (sds);
//			}
//		}
	}
}