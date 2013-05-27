using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace Monospace11
{
    public class TagsViewController : DialogViewController
    {
		public TagsViewController () : base (null)
		{
		}
		
		public override void LoadView ()
		{
			Root = GenerateRoot ();
			base.LoadView ();

			// SLIDEOUT BUTTON
			NavigationController.NavigationBar.SetTitleTextAttributes(AppDelegate.Current.FontTitleTextAttributes);
			var bbi = new UIBarButtonItem(UIImage.FromBundle ("Images/slideout"), UIBarButtonItemStyle.Plain, (sender, e) => {
				AppDelegate.Current.FlyoutNavigation.ToggleMenu();
			});
			NavigationItem.SetLeftBarButtonItem (bbi, false);
		}

		RootElement GenerateRoot ()
		{
			// The full list
			var allSessions = new RootElement ("All Sessions") {
				from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList () //AppDelegate.ConferenceData.Sessions
				orderby s.Start ascending
				group s by s.Start.Ticks into g
				select new Section (HomeViewController.MakeCaption ("", new DateTime (g.Key), true)) {
					from hs in g
					   select (Element) new SessionElement (hs)
			}};

			var root = new RootElement ("Sessions") { 
					allSessions
			};
			
			return root;
		}
	}
}