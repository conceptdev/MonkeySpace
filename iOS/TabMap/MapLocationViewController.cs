using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;

using MonkeySpace.Core;

namespace Monospace11
{
	public class MapLocationViewController : TableViewControllerBase
	{
		private UINavigationBar navBar;
		private List<MonkeySpace.Core.MapLocation> locations;
		public MapFlipViewController FlipController = null;

		public MapLocationViewController (MapFlipViewController mfvc) : base()
		{
			FlipController = mfvc;
			locations = Constants.Locations;
		}
		
		public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
			navBar = new UINavigationBar();
			navBar.PushNavigationItem (new UINavigationItem("Choose Location"), false);
			navBar.BarStyle = UIBarStyle.Black;
			navBar.Frame = new RectangleF(0,0,this.View.Frame.Width,45);
			navBar.TopItem.RightBarButtonItem = new UIBarButtonItem("Done",UIBarButtonItemStyle.Bordered, delegate {FlipController.Flip();});
			tableView.TableHeaderView = navBar;
			tableView.Source = new TableViewSource(this, locations);
		}
        private class TableViewSource : UITableViewSource
        {
			private MapLocationViewController dvc;
			private List<MonkeySpace.Core.MapLocation> locations;
			public TableViewSource(MapLocationViewController controller, List<MonkeySpace.Core.MapLocation> locations)
            {
				dvc = controller;
				this.locations = locations;
            }

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {
				var loc = locations[indexPath.Row];
                Debug.WriteLine("RowSelected: Label=" + loc.Title);
				dvc.FlipController.Flip(locations[indexPath.Row]);
				tableView.DeselectRow(indexPath, true);
			}
            static NSString kCellIdentifier = new NSString ("MyLocationIdentifier");
            public override int RowsInSection (UITableView tableview, int section)
            {
                return locations.Count;
            }
            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                UITableViewCell cell = tableView.DequeueReusableCell (kCellIdentifier);
                if (cell == null)
                {
                    cell = new UITableViewCell (UITableViewCellStyle.Default, kCellIdentifier);
                }
                cell.TextLabel.Text = locations[indexPath.Row].Title;
                cell.Accessory = UITableViewCellAccessory.None;
                return cell;
            }
        }
	}
}