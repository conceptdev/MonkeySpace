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
using System.Xml;
using Clarity.Phone.Controls;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Diagnostics;

using MonkeySpace.Core;

namespace MonkeySpace
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor of the main page -- also kicks off the Timer event that downloads the conference update.!
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            // http://www.peterfoot.net/
            // Determine light or dark theme
            Visibility v = (Visibility)Resources["PhoneLightThemeVisibility"];
            if (v == System.Windows.Visibility.Visible)
            {   // LIGHT
                ImageBrush ib = new ImageBrush();
                Uri u = new Uri("PanoramaBackground_light.png", UriKind.Relative);
                ib.ImageSource = new System.Windows.Media.Imaging.BitmapImage(u);
                ConfPanorama.Background = ib;
            }
            else
            {   // DARK
                ImageBrush ib = new ImageBrush();
                Uri u = new Uri("PanoramaBackground_dark.png", UriKind.Relative);
                ib.ImageSource = new System.Windows.Media.Imaging.BitmapImage(u);
                ConfPanorama.Background = ib;
            }
        }
        
        // Handle selection changed on ListBox
        private void Days_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (DaysListBox.SelectedIndex == -1)
                return;

            if (!(DaysListBox.SelectedItem is DayConferenceViewModel)) return;
            var clickedItem = (DayConferenceViewModel)DaysListBox.SelectedItem;

            if (clickedItem.SortOrder == 0)
            {   // Navigation to page showing sessions for that hour
                var d = (DaysListBox.SelectedItem as DayConferenceViewModel);
                if (d.Day == "")
                {   // go to single session
                    NavigationService.Navigate(new Uri("/SessionPage.xaml?sessionCode=" + d.SessCode, UriKind.Relative));
                }
                else
                {   // go to list of sessions
                    NavigationService.Navigate(new Uri("/DayPage.xaml?selectedDateTime=" + d.Day
                        , UriKind.Relative));
                }
            }
            else if (clickedItem.SortOrder == 1)
            {   // Navigate to the new page showing sessions for that day
                NavigationService.Navigate(new Uri("/DayPage.xaml?selectedDate=" + (DaysListBox.SelectedItem as DayConferenceViewModel).Day
                    , UriKind.Relative));
            }
            else if (clickedItem.SortOrder == 2)
            {   // Re-set the data by loading a different conf.xml file BUT FIRST save the favorites for the 'old' displayed conference
                ((MonkeySpace.App)App.Current).SaveFavorites();

                IsolatedStorageSettings.ApplicationSettings["LastConferenceCode"] = clickedItem.ConfCode;
                App.ViewModel.LoadData();
                ConfPanorama.Title = "MonkeySpace"; // App.ViewModel.ConfItem.Name.ToLower();
                MessageBox.Show("Now viewing data for MonkeySpace##", "Switched conferences", MessageBoxButton.OK);
            }
            else
            { 
                // Get list from the web
                // Navigate to the new page showing sessions for that day
                NavigationService.Navigate(new Uri("/DownloadConferences.xaml"
                    , UriKind.Relative));
            }

            // Reset selected index to -1 (no selection)
            DaysListBox.SelectedIndex = -1;
        }


        // Handle selection changed on ListBox
        private void Speakers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (SpeakersListBox.SelectedIndex == -1)
                return;

            if (!(SpeakersListBox.SelectedItem is Speaker)) return;
            var speaker = (Speaker)SpeakersListBox.SelectedItem;
            // Navigate to the new page
            NavigationService.Navigate(new Uri("/SpeakerPage.xaml?name=" + speaker.Name, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            SpeakersListBox.SelectedIndex = -1;
        }

        // Handle selection changed on ListBox
        private void Sessions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (SessionsListBox.SelectedIndex == -1)
                return;

            if (!(SessionsListBox.SelectedItem is Session)) return;

            //string selectedSessionCode = (SessionsListBox.ItemsSource as ObservableCollection<Session2>)[SessionsListBox.SelectedIndex].Code;
            string selectedSessionCode = (SessionsListBox.SelectedItem as Session).Code;//((ObservableCollection<Conf.Session2>)DataContext)[SessionsListBox.SelectedIndex].Code;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/SessionPage.xaml?sessionCode=" + selectedSessionCode, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            SessionsListBox.SelectedIndex = -1;
        }

        // Handle selection changed on ListBox
        private void Maps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (MapsListBox.SelectedIndex == -1)
                return;
            // Navigate to the new page
            NavigationService.Navigate(new Uri("/MapPage.xaml?selectedItem=" + (MapsListBox.SelectedIndex), UriKind.Relative));

            // Reset selected index to -1 (no selection)
            MapsListBox.SelectedIndex = -1;
        }

        private void Info_Clicked(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/InfoPage.xaml", UriKind.Relative));
        }
        private void About_Clicked(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void Favorites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (FavoritesListBox.SelectedIndex == -1)
                return;

            //string selectedSessionCode = (SessionsListBox.ItemsSource as ObservableCollection<Session2>)[SessionsListBox.SelectedIndex].Code;
            string selectedSessionCode = (FavoritesListBox.SelectedItem as Session).Code;//((ObservableCollection<Conf.Session2>)DataContext)[SessionsListBox.SelectedIndex].Code;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/SessionPage.xaml?sessionCode=" + selectedSessionCode, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            FavoritesListBox.SelectedIndex = -1;
        }
    }
}