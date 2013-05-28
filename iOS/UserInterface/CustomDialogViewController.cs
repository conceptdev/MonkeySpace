using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace Monospace11
{
	public class CustomDialogViewController : DialogViewController
	{
		public CustomDialogViewController (UITableViewStyle style, RootElement root, bool pushing) : base (style, root, pushing) 
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TableView.BackgroundView = new UIImageView (UIImage.FromBundle ("Images/Background"));
		}
	}
}

