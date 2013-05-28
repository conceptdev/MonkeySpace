using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.Dialog.Utilities;

namespace Monospace11
{
	public class SpeakerCollectionCell : UICollectionViewCell, IImageUpdated
	{
		UIImageView imageView;
		UITextView name;

		public static SizeF Size = new SizeF(100, 145);
		
		[Export ("initWithFrame:")]
		public SpeakerCollectionCell (System.Drawing.RectangleF frame) : base (frame)
		{
			BackgroundView = new UIView{BackgroundColor = UIColor.White};
			//SelectedBackgroundView = new UIView{BackgroundColor = UIColor.Green};

			ContentView.Frame = new RectangleF (0, 0, 100, 130);

			//ContentView.Layer.BorderColor = UIColor.LightGray.CGColor;
			//ContentView.Layer.BorderWidth = 2.0f;
			ContentView.BackgroundColor = UIColor.White;
			//ContentView.Transform = CGAffineTransform.MakeScale (0.8f, 0.8f);

			var image = UIImage.FromBundle ("/Images/avatar.png");  // default blank avatar image

			imageView = new UIImageView (image);
			imageView.Frame = new RectangleF (0,0,100,100);
		
			name = new UITextView(new RectangleF (0, 95, 100, 50));
			name.Font = AppDelegate.Current.FontCellSmall;
			name.BackgroundColor = UIColor.Clear;
			name.Editable = false;
			name.ScrollEnabled = false;


			ContentView.Add (imageView);
			ContentView.Add (name);
		}

		Uri uri;
		public string ImageUrl {
			set {
				var u = value;
				try {
					if (String.IsNullOrEmpty (u)) {
						imageView.Image = UIImage.FromBundle ("/Images/avatar.png");  // default blank avatar image
					} else {
						uri = new Uri(u);
						Console.WriteLine ("getting " + uri);
						var i = ImageLoader.DefaultRequestImage (uri, this);
						if (i != null)
							imageView.Image = i;
					}
				} catch (Exception e) {
					imageView.Image = UIImage.FromBundle ("/Images/avatar.png");  // default blank avatar image
					Console.WriteLine ("[IMAGE] " + e.Message + " ~ " + u);
				}
			}
		}

		public string Name {
			set {
				name.Text = value;
			}
		}

		public void UpdatedImage (Uri uri)
		{
			if (uri == this.uri)
				imageView.Image = ImageLoader.DefaultRequestImage (uri, null);
			else
				Console.WriteLine ("throw away " + uri);
		}
	}
}

