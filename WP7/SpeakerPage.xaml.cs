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

using MonkeySpace.Core;

namespace MonkeySpace
{
    public partial class SpeakerPage : PhoneApplicationPage
    {
        public SpeakerPage()
        {
            InitializeComponent();
        }

        MonkeySpace.Core.Speaker s;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string name = "";
            if (NavigationContext.QueryString.TryGetValue("name", out name))
            {
                var s = (from speaker in MonkeySpace.Core.ConferenceManager.Speakers.Values.ToList() //App.ViewModel.ConfItem.Speakers
                         where speaker.Name == name
                         select speaker).FirstOrDefault();
                DataContext = s;
            }
        }


        // Handle selection changed on ListBox
        private void Sessions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (SessionsListBox.SelectedIndex == -1)
                return;

            string selectedSessionCode = (DataContext as Speaker).Sessions[SessionsListBox.SelectedIndex].Code;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/SessionPage.xaml?sessionCode=" + selectedSessionCode, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            SessionsListBox.SelectedIndex = -1;
        }
    }
}