
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;

namespace Monospace11
{
	public class MapLocationViewController : TableViewControllerBase
	{
		private UINavigationBar navBar;
		private List<MonkeySpace.Core.MapLocation> _locations;
		public MapFlipViewController FlipController = null;

		public MapLocationViewController (MapFlipViewController mfvc) : base()
		{
			FlipController = mfvc;
			_locations = new List<MonkeySpace.Core.MapLocation> () {
				new MonkeySpace.Core.MapLocation{Title="MonkeySpace", Location=new MonkeySpace.Core.Point{X=-71.08363940740965,Y=42.36100515974955}}
			};
			_locations.Add(new MonkeySpace.Core.MapLocation{Title="My location", Location=new MonkeySpace.Core.Point{X=0,Y=0}});
		}
		
		public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
			// no XIB !
			navBar = new UINavigationBar();
			navBar.PushNavigationItem (new UINavigationItem("Choose Location"), false);
			navBar.BarStyle = UIBarStyle.Black;
			navBar.Frame = new RectangleF(0,0,this.View.Frame.Width,45);
			navBar.TopItem.RightBarButtonItem = new UIBarButtonItem("Done",UIBarButtonItemStyle.Bordered, delegate {FlipController.Flip();});
			tableView.TableHeaderView = navBar;
			tableView.Source = new TableViewSource(this, _locations);
		}
        private class TableViewSource : UITableViewSource
        {
			private MapLocationViewController _dvc;
			private List<MonkeySpace.Core.MapLocation> _locations;
			public TableViewSource(MapLocationViewController controller, List<MonkeySpace.Core.MapLocation> locations)
            {
				_dvc = controller;
				_locations = locations;
            }

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {
				var loc = _locations[indexPath.Row];
                Debug.WriteLine("RowSelected: Label=" + loc.Title);
				_dvc.FlipController.Flip(_locations[indexPath.Row]);
				tableView.DeselectRow(indexPath, true);
			}
            static NSString kCellIdentifier = new NSString ("MyLocationIdentifier");
            public override int RowsInSection (UITableView tableview, int section)
            {
                return _locations.Count;
            }
            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                UITableViewCell cell = tableView.DequeueReusableCell (kCellIdentifier);
                if (cell == null)
                {
                    cell = new UITableViewCell (UITableViewCellStyle.Default, kCellIdentifier);
                }
                cell.TextLabel.Text = _locations[indexPath.Row].Title;
                cell.Accessory = UITableViewCellAccessory.None;
                return cell;
            }
        }
	}
}