using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MapKit;  // required
using MonoTouch.CoreLocation;  // required
using System.Diagnostics;

using MonkeySpace.Core;

namespace Monospace11
{
	public class MapViewController : UIViewController
	{
		private MKMapView mapView;
		public UILabel labelDistance;
		
		public MapLocation ConferenceLocation;
		private CLLocationManager locationManager;

		private MapFlipViewController mapFlipViewController;

		public MapViewController (MapFlipViewController mfvc):base()
		{
			mapFlipViewController = mfvc;
			ConferenceLocation = Constants.Locations [0]; 
		}
		
		public void SetLocation (MonkeySpace.Core.MapLocation toLocation)
		{
			var targetLocation = toLocation.Location.To2D();
			if (toLocation.Location.X == 0 && toLocation.Location.Y == 0)
			{
				// use the 'location manager' current coordinate
				if (locationManager.Location == null) {
					return; // catch a possible null reference that i saw once [CD]
				} else {	
					targetLocation = locationManager.Location.Coordinate;
					ConferenceAnnotation a = new ConferenceAnnotation(targetLocation, "My location", "");
					mapView.AddAnnotationObject(a); 
				}
			}
			else if (toLocation.Title == "MonkeySpace")
			{
				// no need to drop anything
			}
			else
			{
				// drop a new pin
				ConferenceAnnotation a = new ConferenceAnnotation(toLocation.Location.To2D(), toLocation.Title,toLocation.Subtitle);
				mapView.AddAnnotationObject(a); 
			}
			mapView.CenterCoordinate = targetLocation;
		}

		UINavigationBar navBar;
		public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();






			navBar = new UINavigationBar (new RectangleF (0, 0, 320, 44));
			var bbi = new UIBarButtonItem(UIImage.FromBundle ("Images/slideout"), UIBarButtonItemStyle.Plain, (sender, e) => {
				AppDelegate.Current.FlyoutNavigation.ToggleMenu();
			});
			var rbi = new UIBarButtonItem (UIImage.FromBundle ("Images/113-navigation"), UIBarButtonItemStyle.Plain, (sender,e) => {
				mapFlipViewController.Flip();
			});

			var item = new UINavigationItem ("Location Map");
			item.LeftBarButtonItem = bbi;
			item.RightBarButtonItem = rbi;
			var items = new UINavigationItem[] {
				item
			};
			navBar.SetItems (items, false);





			mapView = new MKMapView()
			{
				ShowsUserLocation = true
			};
			
			labelDistance = new UILabel()
			{
				Frame = new RectangleF (0, 44, 320, 49),
				Lines = 2,
				BackgroundColor = UIColor.Black,
				TextColor = UIColor.White
			};

			var segmentedControl = new UISegmentedControl();
			var topOfSegement = View.Frame.Height - 60;
			segmentedControl.Frame = new RectangleF(20, topOfSegement, 282, 30);
			segmentedControl.InsertSegment("Map", 0, false);
			segmentedControl.InsertSegment("Satellite", 1, false);
			segmentedControl.InsertSegment("Hybrid", 2, false);
			segmentedControl.SelectedSegment = 0;
			segmentedControl.ControlStyle = UISegmentedControlStyle.Bar;
			segmentedControl.TintColor = UIColor.DarkGray;
			
			segmentedControl.ValueChanged += delegate {
				if (segmentedControl.SelectedSegment == 0)
					mapView.MapType = MonoTouch.MapKit.MKMapType.Standard;
				else if (segmentedControl.SelectedSegment == 1)
					mapView.MapType = MonoTouch.MapKit.MKMapType.Satellite;
				else if (segmentedControl.SelectedSegment == 2)
					mapView.MapType = MonoTouch.MapKit.MKMapType.Hybrid;
			};
			
			mapView.Delegate = new MapViewDelegate(this); 

			// Set the web view to fit the width of the app.
            mapView.SizeToFit();

            // Reposition and resize the receiver
            mapView.Frame = new RectangleF (0, 44 + 50, View.Frame.Width, View.Frame.Height - 93);

			MKCoordinateSpan span = new MKCoordinateSpan(0.01,0.01);
			MKCoordinateRegion region = new MKCoordinateRegion(ConferenceLocation.Location.To2D(),span);
			mapView.SetRegion(region, true);
			
			ConferenceAnnotation a = new ConferenceAnnotation(ConferenceLocation.Location.To2D()
			                                                  , ConferenceLocation.Title
			                                                  , ConferenceLocation.Subtitle
                              );
			mapView.AddAnnotationObject(a); 
			
			
			locationManager = new CLLocationManager();
			locationManager.Delegate = new LocationManagerDelegate(mapView, this);
			locationManager.Purpose = "Show distance on map"; // also Info.plist
			locationManager.StartUpdatingLocation();
			
            // Add the table view as a subview
            View.AddSubview(mapView);
			View.AddSubview(labelDistance);
			View.AddSubview(segmentedControl);
			
			// Add the 'info' button to flip
			var flipButton = UIButton.FromType(UIButtonType.InfoLight);
			flipButton.Frame = new RectangleF(290,17,20,20);
			flipButton.Title (UIControlState.Normal);
			flipButton.TouchDown += delegate {
				mapFlipViewController.Flip();
			};
			View.AddSubview(flipButton);


			View.Add (navBar);
		}	
		
		public class MapViewDelegate : MKMapViewDelegate
		{
			public MapViewDelegate (MapViewController controller):base()
			{
			}

			public override MKAnnotationView GetViewForAnnotation (MKMapView mapView, NSObject annotation)
			{
				try {
					var ca = (ConferenceAnnotation)annotation;
					var aview = (MKPinAnnotationView)mapView.DequeueReusableAnnotation("pin");
					if (aview == null)
					{
						aview = new MKPinAnnotationView(ca, "pin");
					}
					else 
					{
						aview.Annotation = ca;
					}
					aview.AnimatesDrop = true;
					aview.PinColor = MKPinAnnotationColor.Purple;
					aview.CanShowCallout = true;

					return aview;
				} catch (Exception) {
					return null;
				}
			}
		}
		/// <summary>
		/// MonoTouch definition seemed to work without too much trouble
		/// </summary>
		private class LocationManagerDelegate: CLLocationManagerDelegate
		{
			private MapViewController mapVC;

			public LocationManagerDelegate(MKMapView mapview, MapViewController mapvc):base()
			{
				mapVC = mapvc;
			}
			/// <summary>
			/// Whenever the GPS sends a new location, update text in label
			/// and increment the 'count' of updates AND reset the map to that location 
			/// </summary>
			public override void UpdatedLocation (CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
			{
				double distanceToConference = MapHelper.Distance (new Coordinate(mapVC.ConferenceLocation.Location.To2D()), new Coordinate(newLocation.Coordinate), UnitsOfLength.Miles);
				mapVC.labelDistance.TextAlignment = UITextAlignment.Center;
				mapVC.labelDistance.Text = String.Format("{0} miles from MonkeySpace!", Math.Round(distanceToConference,0));
				Debug.WriteLine("Distance: {0}", distanceToConference);
				
				// only use the first result
				manager.StopUpdatingLocation();
			}
			public override void Failed (CLLocationManager manager, NSError error)
			{
				Debug.WriteLine("Failed to find location");
			}
		}
	}
	
	
	/// <summary>
	/// MKAnnotation is an abstract class (in Objective C I think it's a protocol).
	/// Therefore we must create our own implementation of it. Since all the properties
	/// are read-only, we have to pass them in via a constructor.
	/// </summary>
	public class ConferenceAnnotation : MKAnnotation
	{
		private CLLocationCoordinate2D coordinate;
		private string title, subtitle;
		public override CLLocationCoordinate2D Coordinate {
			get {
				return coordinate;
			}
			set {
				coordinate = value;
			}
		}
		public override string Title {
			get {
				return title;
			}
		}
		public override string Subtitle {
			get {
				return subtitle;
			}
		}
		/// <summary>
		/// custom constructor
		/// </summary>
		public ConferenceAnnotation (CLLocationCoordinate2D coord, string t, string s) : base()
		{
			coordinate=coord;
		 	title=t; 
			subtitle=s;
		}
	}
}
