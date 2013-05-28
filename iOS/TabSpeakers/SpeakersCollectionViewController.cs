using System;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

using MonkeySpace.Core;

namespace Monospace11
{
	public class SpeakersCollectionViewController : UICollectionViewController
	{
		static NSString speakerCellId = new NSString ("SpeakerCell");

		List<Speaker> speakerData = new List<Speaker>();

		public SpeakersCollectionViewController (UICollectionViewLayout layout) : base (layout)
		{
//			speakerData = (from s in Evolve.Core.ConferenceManager.Speakers.Values
//			               							orderby s.Name select s).ToList () ;

			CollectionView.BackgroundView = new UIImageView (UIImage.FromBundle("/Images/Background"));
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			CollectionView.RegisterClassForCell (typeof(SpeakerCollectionCell), speakerCellId);

			var bbi = new UIBarButtonItem(UIImage.FromBundle ("Images/slideout"), UIBarButtonItemStyle.Plain, (sender, e) => {
				AppDelegate.Current.FlyoutNavigation.ToggleMenu();
			});
			NavigationItem.SetLeftBarButtonItem (bbi, false);
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			speakerData = (from s in MonkeySpace.Core.ConferenceManager.Speakers.Values.ToList ()
			               orderby s.Name select s).ToList () ;
			CollectionView.ReloadData ();
		}

		public override int NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}
		
		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			return speakerData.Count;
		}
		 
		public override UICollectionViewCell GetCell (UICollectionView collectionView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var speakerCell = (SpeakerCollectionCell)collectionView.DequeueReusableCell (speakerCellId, indexPath);
			
			var speaker = speakerData [indexPath.Row];
			
			speakerCell.Name = speaker.Name;
			if (!string.IsNullOrEmpty(speaker.HeadshotUrl))
				speakerCell.ImageUrl = speaker.HeadshotUrl;
			else
				speakerCell.ImageUrl = ""; // use default
			
			return speakerCell;
		}
		
		public override bool ShouldHighlightItem (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;
		}

		SpeakerBioViewController bioVC;
		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			Speaker s = speakerData[indexPath.Row];
			if (bioVC == null)
				bioVC = new SpeakerBioViewController (s);
			else
				bioVC.Update (s);
			
			bioVC.Title = s.Name;
			NavigationController.PushViewController (bioVC, true);

			collectionView.DeselectItem (indexPath, false);
		}
	}
}

