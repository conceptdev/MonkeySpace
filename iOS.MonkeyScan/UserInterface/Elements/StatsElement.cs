using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MonkeyScan
{
	public class StatsElement : Element
	{
		static NSString cellId = new NSString ("stats");
		string title, stat;

		public StatsElement (string title, string stat) : base (title)
		{
			this.title = title;
			this.stat = stat;
		}

		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = tv.DequeueReusableCell (cellId);

			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Value1, cellId);
			
			cell.TextLabel.Text = title;
			cell.DetailTextLabel.Text = stat;

			return cell;
		}
	}
}

