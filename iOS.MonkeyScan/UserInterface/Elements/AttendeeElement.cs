using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


namespace MonkeyScan {
	/// <summary>
	/// Speaker element.
	/// on iPhone, pushes via MT.D
	/// on iPad, sends view to SplitViewController
	/// </summary>
	public class AttendeeElement : Element  {

		static UIImage blue, orange, green;

		static NSString cellId = new NSString ("confattendee");
		ConfAttendee attendee;

		public AttendeeElement (ConfAttendee ca) : base (ca.Name)
		{
			attendee = ca;
		}

		static int count;
		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = tv.DequeueReusableCell (cellId);
			count++;
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellId);

			cell.TextLabel.Text = attendee.Name;
			cell.DetailTextLabel.Text = attendee.Barcode + " (" + attendee.ScanCount + ")";
			if (attendee.ScanCount <= 0) {
				cell.ImageView.Image = UIImage.FromFile ("Images/Yellow.png");
			} else if (attendee.ScanCount == 1) {
				cell.ImageView.Image = UIImage.FromFile ("Images/Green.png");
			} else {
				cell.ImageView.Image = UIImage.FromFile ("Images/Orange.png");
			}

			return cell;
		}

		/// <summary>Implement MT.D search on name and company properties</summary>
		public override bool Matches (string text)
		{
			return (attendee.Name + " " + attendee.Barcode).ToLower ().IndexOf (text.ToLower ()) >= 0;
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