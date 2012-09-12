using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;

using MonkeySpace.Core;
/*
 * http://channel9.msdn.com/Learn/Courses/WP7TrainingKit/WP7Silverlight/UsingBingMapsLab/Exercise-2-Handling-and-Customizing-Pushpins
 */

namespace MonkeySpace
{
    public partial class MapPage : PhoneApplicationPage
    {
        public MapPage()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(MapPage_Loaded);
        }

        void MapPage_Loaded(object sender, RoutedEventArgs e)
        {
            // This should really be more secure...
            LocationMap.CredentialsProvider = 
                new ApplicationIdCredentialsProvider("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        }

        MapLocation l;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                int index = int.Parse(selectedIndex);
                //DataContext = App.ViewModel.Items[index];
                //l = App.ViewModel.ConfItem.Locations[index];
                //DataContext = l;

                //PageTitle.Text = l.Title;
                //PageSubtitle.Text = l.Subtitle;
                //LocationMap.Center = new GeoCoordinate(l.Location.Y, l.Location.X);
                //LocationMap.ZoomLevel = 14;

                //Pushpin pin = new Pushpin();
                //pin.Content = l.Title;
                //pin.Location = new GeoCoordinate(l.Location.Y, l.Location.X);
                //LocationMapItems.ItemsSource = new List<Pushpin> { pin };

            }
        }
    }
}