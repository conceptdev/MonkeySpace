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
using System.Collections.ObjectModel;

using MonkeySpace.Core;

namespace MonkeySpace
{
    public partial class DayPage : PhoneApplicationPage
    {
        public DayPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string selectedDate = "", selectedDateTime = "";
            if (NavigationContext.QueryString.TryGetValue("selectedDate", out selectedDate))
            {
                var date = DateTime.Parse(selectedDate);

                var sess = from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList() //App.ViewModel.ConfItem.Sessions
                            where s.Start.Day == date.Day // > date && s.End < date.AddDays(1)
                            orderby s.Start
                            select s;

                //SessionsListBox.ItemsSource = sess.ToList();
                DataContext = sess.ToList();
                PageTitle.Text = date.ToString("dddd, dd MMMM");
            }
            else if (NavigationContext.QueryString.TryGetValue("selectedDateTime", out selectedDateTime))
            {
                var datetime = DateTime.Parse(selectedDateTime);

                var sess = from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList() //App.ViewModel.ConfItem.Sessions
                           where s.Start == datetime // > date && s.End < date.AddDays(1)
                           orderby s.Start
                           select s;

                //SessionsListBox.ItemsSource = sess.ToList();
                DataContext = sess.ToList();
                PageTitle.Text = datetime.ToString("dddd, dd MMMM");
            }
        }

        // Handle selection changed on ListBox
        private void Sessions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (SessionsListBox.SelectedIndex == -1)
                return;

            string selectedSessionCode = (SessionsListBox.SelectedItem as Session).Code; //((List<Conf.Session2>)DataContext)[SessionsListBox.SelectedIndex].Code;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/SessionPage.xaml?sessionCode=" + selectedSessionCode, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            SessionsListBox.SelectedIndex = -1;
        }
    }
}