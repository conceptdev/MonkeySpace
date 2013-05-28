using System;
using MonoTouch.Dialog;

namespace Monospace11
{
	public class CustomRootElement : RootElement
	{
		public CustomRootElement (string caption) : base(caption, (root) => {
			return new CustomDialogViewController(MonoTouch.UIKit.UITableViewStyle.Plain, root, true);
		})
		{
		}

		public override MonoTouch.UIKit.UITableViewCell GetCell (MonoTouch.UIKit.UITableView tv)
		{
			var cell = base.GetCell (tv);

			cell.TextLabel.Font = AppDelegate.Current.FontCell;

			return cell;
		}
	}
}

