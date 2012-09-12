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
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Diagnostics;

namespace MonkeySpace
{
    /// <summary>
    /// Application Bar stuff...
    /// http://blog.galasoft.ch/archive/2010/06/08/two-small-issues-with-windows-phone-7-applicationbar-buttons-and.aspx
    /// </summary>
    public partial class SessionPage : PhoneApplicationPage
    {
        private ApplicationBarIconButton FavoriteBarIcon, VideoBarIcon, SlidesBarIcon, FeedbackBarIcon;

        const string addfavorite = "icons/heartbroken.png"; // shown when need to add as favorite
        const string unfavorite = "icons/heart.png"; // shown when IS a favorite

        const string addfavouritelabel = "my sched";
        const string unfavouritelabel = "remove";

        public SessionPage()
        {
            InitializeComponent();

            FavoriteBarIcon = new ApplicationBarIconButton();
            FavoriteBarIcon.IconUri = new Uri(addfavorite, UriKind.Relative);
            FavoriteBarIcon.Text = "add favorite";
            FavoriteBarIcon.Click += new EventHandler(AddFavoriteButton_Click);

            //FeedbackBarIcon = new ApplicationBarIconButton();
            //FeedbackBarIcon.IconUri = new Uri("icons/tick.png", UriKind.Relative);
            //FeedbackBarIcon.Text = "feedback";
            //FeedbackBarIcon.Click += new EventHandler(FeedbackButton_Click);

            VideoBarIcon = new ApplicationBarIconButton();
            VideoBarIcon.Text = "Video";
            VideoBarIcon.IconUri = new Uri("icons/appbar.feature.video.rest.png", UriKind.Relative);
            VideoBarIcon.Click += new EventHandler(ViewVideo_Click);

            SlidesBarIcon = new ApplicationBarIconButton();
            SlidesBarIcon.Text = "Slides";
            SlidesBarIcon.IconUri = new Uri("icons/ppt.png", UriKind.Relative);
            SlidesBarIcon.Click += new EventHandler(ViewSlides_Click);

            this.Loaded += new RoutedEventHandler(SessionPage_Loaded);
        }

        void SessionPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationBar.Buttons.Count == 0)
            {   // hitting 'back' from feedback page breaks otherwise
                var sess = from s in App.ViewModel.FavoriteSessions
                           where s.Code == session.Code
                           select s;
                foreach (var s in sess)
                {
                    isFavorite = true;
                }

                if (isFavorite)
                {
                    FavoriteBarIcon.IconUri = new Uri(unfavorite, UriKind.Relative);
                    FavoriteBarIcon.Text = unfavouritelabel;// "remove";
                }
                else
                {
                    FavoriteBarIcon.IconUri = new Uri(addfavorite, UriKind.Relative);
                    FavoriteBarIcon.Text = addfavouritelabel;// "add to sched";

                    //App.ViewModel.Items.Add(new ItemViewModel { LineOne = session.Title }); // add to the 'My Sessions' panel
                }

                ApplicationBar.Buttons.Add(FavoriteBarIcon);

                // Does this session have video or powerpoint?
                //if (!String.IsNullOrEmpty(session.WmvVideoUrl))
                //{
                //    ApplicationBar.Buttons.Add(VideoBarIcon);
                //}
                //if (!String.IsNullOrEmpty(session.PowerpointSlidesUrl))
                //{
                //    ApplicationBar.Buttons.Add(SlidesBarIcon);
                //}
            }
        }

        MonkeySpace.Core.Session session;
        bool isFavorite;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string code = "";

            if (NavigationContext.QueryString.TryGetValue("sessionCode", out code))
            {
                var sess = from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList() //App.ViewModel.ConfItem.Sessions
                           where s.Code == code
                           select s;

                session = sess.FirstOrDefault();
                //session.Room = session.Room;

                DataContext = session;
            }
        }
       
        private void AddFavoriteButton_Click(object sender, EventArgs e)
        {
            // Add to favorite
            Debug.WriteLine("Clicked add favourite " + session.Code);
            isFavorite = !isFavorite;

            ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];

            if (isFavorite)
            {
                b.IconUri = new Uri(unfavorite, UriKind.Relative);
                b.Text = unfavouritelabel;// "remove";
            }
            else 
            {
                b.IconUri = new Uri(addfavorite, UriKind.Relative);
                b.Text = addfavouritelabel;// "add to sched";
            }

            /*var fs = (from s in App.ViewModel.FavoriteSessions
                      where s.Code == session.Code
                      select s).ToList();
            
            if (fs.Count > 0)
            {
                // remove, as it might be old data persisted anyway (from previous conf.xml file)
                App.ViewModel.FavoriteSessions.Remove(fs[0]);
            }
            if (isFavorite)
            {   // add it again
                App.ViewModel.FavoriteSessions.Add(new Session2(session.Clone()));
            }*/
            App.ViewModel.UpdateFavorite(session, isFavorite); // HACK: testing
            /*
            if (IsolatedStorageSettings.ApplicationSettings.Contains("Favorite_" + session.Code))
            {
                IsolatedStorageSettings.ApplicationSettings["Favorite_" + session.Code] = isFavorite;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add("Favorite_" + session.Code, isFavorite);
            }
            IsolatedStorageSettings.ApplicationSettings.Save();
            */
        }

        private void ViewSlides_Click(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri(session.PowerpointSlidesUrl, UriKind.Absolute));
            //WebBrowserTask webBrowserTask = new WebBrowserTask();
            //webBrowserTask.URL = session.PowerpointSlidesUrl;
            //webBrowserTask.Show();

        }
        private void ViewVideo_Click(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri(session.WmvVideoUrl, UriKind.Absolute));
            //WebBrowserTask webBrowserTask = new WebBrowserTask();
            //webBrowserTask.URL = session.WmvVideoUrl;
            //webBrowserTask.Show();
        }
    }
}