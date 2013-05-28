using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.PassKit;
using System.Drawing;

namespace Monospace11
{
	public class PassKitViewController : UIViewController
	{
		PKPassLibrary library;
		NSObject noteCenter;
		UIImageView passImage;
		UILabel passHeading;
		UITextView passAvailable;
		UIButton showPass;
		PKPass[] passes;
		PKPass currentPass;

		UINavigationBar navBar;
		int top = 44;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.DarkGray;

			passAvailable = new UITextView (new RectangleF (20, top + 100, 300, 200));
			passAvailable.Font = UIFont.SystemFontOfSize(14f);
			passAvailable.BackgroundColor = UIColor.Clear;
			passAvailable.TextColor = UIColor.White;
			passAvailable.Editable = false;
			Add (passAvailable);

			var topOfButton = View.Frame.Height - 160 + top;
			showPass = UIButton.FromType (UIButtonType.Custom);
			showPass.Frame = new RectangleF(60, topOfButton, 200, 40);
			showPass.Layer.CornerRadius = 7f;
			showPass.Layer.MasksToBounds = true;
			showPass.Layer.BorderColor = new MonoTouch.CoreGraphics.CGColor(0.8f, 1.0f);	
			showPass.Layer.BorderWidth = 1;
			showPass.SetTitle ("Open in Passbook", UIControlState.Normal);
			showPass.BackgroundColor = UIColor.DarkGray;
			showPass.SetTitleColor(UIColor.White, UIControlState.Normal);
			showPass.SetTitleShadowColor (UIColor.Black, UIControlState.Normal);
			showPass.TitleShadowOffset = new SizeF(1,1);
			showPass.SetTitleColor(UIColor.LightGray, UIControlState.Highlighted);

			showPass.TouchUpInside += (sender, e) => {
				UIApplication.SharedApplication.OpenUrl (currentPass.PassUrl);
			};
			Add (showPass);

			passHeading = new UILabel (new RectangleF (10, top + 40, 310, 80));
			passHeading.Text = "No Event Ticket\nin Passbook";
			passHeading.Lines = 2;
			passHeading.TextAlignment = UITextAlignment.Center;
			passHeading.Font = UIFont.SystemFontOfSize(24f);
			passHeading.BackgroundColor = UIColor.Clear;
			passHeading.TextColor = UIColor.White;
			passHeading.ShadowColor= UIColor.Black;
			passHeading.ShadowOffset = new SizeF(1,1);
			Add (passHeading);

			passImage = new UIImageView (new RectangleF(90,top + 120,147,186));
			passImage.Image = UIImage.FromBundle("Images/NoTicketSlash");
			Add (passImage);




			navBar = new UINavigationBar (new RectangleF (0, 0, 320, 44));
			var bbi = new UIBarButtonItem(UIImage.FromBundle ("Images/slideout"), UIBarButtonItemStyle.Plain, (sender, e) => {
				AppDelegate.Current.FlyoutNavigation.ToggleMenu();
			});
			var item = new UINavigationItem ("Passbook");
			item.LeftBarButtonItem = bbi;
			var items = new UINavigationItem[] {
				item
			};
			navBar.SetItems (items, false);

			View.Add (navBar);



			library = new PKPassLibrary ();
			noteCenter = NSNotificationCenter.DefaultCenter.AddObserver (PKPassLibrary.DidChangeNotification, (not) => {
				BeginInvokeOnMainThread (() => {
					// refresh the pass
					passes = library.GetPasses ();
					RenderPass(passes);
				});
			}, library);  // IMPORTANT: must pass the library in
			
			passes = library.GetPasses ();
			RenderPass(passes);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			passes = library.GetPasses ();
			RenderPass(passes);
		}

		void RenderPass(PKPass[] passes) {
			if (passes.Length == 0) { 
				// NO PASS
				showPass.Alpha = 0f;
				passHeading.Text = "No Event Ticket\nin Passbook";
				passImage.Image = UIImage.FromBundle("Images/NoTicketSlash");
			} else {
				currentPass = passes[0];
				showPass.Alpha = 1f;
				passHeading.Text = "MonkeySpace Ticket Available";
				passImage.Image = UIImage.FromBundle("Images/Monkey");
			}
		}
	}
}