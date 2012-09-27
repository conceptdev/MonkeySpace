using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Resources;
using System.Linq;
using System.Net;
using System.Xml;
using System.IO.IsolatedStorage;
using MonkeySpace.Core;

namespace MonkeySpace
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
			MapLocations = new List<MapLocation>();
        }


        /// <summary>
        /// A collection of 'favorite' sessions
        /// </summary>
        private ObservableCollection<MonkeySpace.Core.Session> favoriteSessions;
        public ObservableCollection<MonkeySpace.Core.Session> FavoriteSessions
        {
            get { return favoriteSessions; }
            set
            {
                favoriteSessions = value;
                NotifyPropertyChanged("FavoriteSessions");
            }
        }
        public void UpdateFavorite(MonkeySpace.Core.Session session, bool isFavorite)
        {
            var fs = (from s in App.ViewModel.FavoriteSessions
                      where s.Code == session.Code
                      select s).ToList();

            if (fs.Count > 0)
            {
                // remove, as it might be old data persisted anyway (from previous conf.xml file)
                App.ViewModel.FavoriteSessions.Remove(fs[0]);
            }
            if (isFavorite)
            {   // add it again
                App.ViewModel.FavoriteSessions.Add(session);
            }

            var temp = favoriteSessions.OrderBy(s => s.Start.Ticks);
            favoriteSessions = new ObservableCollection<MonkeySpace.Core.Session>();
            foreach (var t in temp)
            {
                favoriteSessions.Add(t);
            }
            NotifyPropertyChanged("FavoriteSessions");
            string currentConf = (string)IsolatedStorageSettings.ApplicationSettings["LastConferenceCode"];
            App.ViewModel.LoadWhatsOn(currentConf);
        }

        private List<DayConferenceViewModel> scheduleItems;
        public List<DayConferenceViewModel> ScheduleItems
        { 
            get { return scheduleItems; }
            set 
            {
                scheduleItems = value;
                NotifyPropertyChanged("ScheduleItems");
            }
        }

		private List<MapLocation> mapLocations;
		public List<MapLocation> MapLocations
		{
			get { return mapLocations; }
			set
			{
				mapLocations = value;
				NotifyPropertyChanged("MapLocations");
			}
		}

        private ConferenceInfo confItem;
        public ConferenceInfo ConfItem 
        {
            get { return confItem; }
            set 
            {
                confItem = value;
                NotifyPropertyChanged("ConfItem");
            }
        }

        public bool IsDataLoaded { get; private set; }
        public IEnumerable<MonkeySpace.Core.Speaker> Speakers { get; set; }
        public IEnumerable<MonkeySpace.Core.Session> Sessions { get; set; }



        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            #region load data

            var ci = new ConferenceInfo {
                Code = "monkeyspace12",
                DisplayLocation = "Boston",
                DisplayName = "MonkeySpace"
                                                        ,
                StartDate = new DateTime(2012, 10, 17),
                EndDate = new DateTime(2012, 10, 19)
            };
            App.ViewModel.ConfItem = ci;

			// HACK: would prefer to get this data from the server
			var ml = new List<MapLocation>();
			ml.Add(new MapLocation() { 
						Title = "MonkeySpace", 
						Subtitle = "NERD Center", 
						Location = new Core.Point(-71.08363940740965, 42.36100515974955) 
			});
			MapLocations = ml;

            var json = App.LoadText(@"monkeyspace12\sessions.json");
            MonkeySpace.Core.ConferenceManager.LoadFromString(json);

            Speakers = MonkeySpace.Core.ConferenceManager.Speakers.Values.ToList();
            NotifyPropertyChanged("Speakers");

            Sessions = MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList();
            NotifyPropertyChanged("Sessions");

            string currentConf="";
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LastConferenceCode"))
                currentConf = (string)IsolatedStorageSettings.ApplicationSettings["LastConferenceCode"];
                //
                // ######   Load 'favorites'   ####
                //
                FavoriteSessions = App.Load<ObservableCollection<MonkeySpace.Core.Session>>(currentConf + "\\Favorites.xml");

                LoadWhatsOn(currentConf);
            #endregion

            this.IsDataLoaded = true;
        }

        public void LoadWhatsOn(string currentConf)
        {
            var temp = new List<DayConferenceViewModel>();

            var now = DateTime.Now;
#if DEBUG
            // TEST: this for testing only
            //now = new DateTime(2012, 10, 18, 15, 51, 0);
#endif            
            //
            // #####  Find out what's up next  ######
            //
            if (now < App.ViewModel.ConfItem.EndDate.AddDays(1)) //new DateTime(2010,6,2))
            {   // During the conference
               
                //var nowStart = now.AddMinutes(now.Minute < 30 ? -now.Minute : -(now.Minute - 30)); // also helps (within 30 minutes bug)
                var nowStart = now.AddMinutes( - (now.Minute % 5) ); // round to 5 minutes
                // The shortest session appears to be 30 minutes that I could see
                var nextStart = nowStart.AddMinutes(29); // possible fix to (within 30 minutes bug)
                nextStart = nextStart.AddSeconds(-nextStart.Second);

                // sometimes the data might not have an 'end time', so we'll handle that by assuming 1 hour long sessions
                var happeningNow = from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList() //App.ViewModel.ConfItem.Sessions
                                   where s.Start <= now 
                                   && (now < (s.End == DateTime.MinValue?s.Start.AddHours(1):s.End))
                                   && (s.Start.Minute % 10 == 0 || s.Start.Minute % 15 == 0) // fix for short-sessions (which start at :05 after the hour)
                                   select s;

                var allUpcoming = from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList() //App.ViewModel.ConfItem.Sessions
                                  where s.Start >= nextStart && (s.Start.Minute % 10 == 0 || s.Start.Minute % 15 == 0) // (s.Start.Minute == 0 || s.Start.Minute == 30)
                                  orderby s.Start.Ticks
                                  group s by s.Start.Ticks into g
                                  select new { Start = g.Key, Sessions = g };

                var fav = new List<DayConferenceViewModel>();

                bool haveNow = false; DateTime nowStartTime = new DateTime();
                int s1 = 0;

                foreach (var s in happeningNow)
                {
                    haveNow = true; nowStartTime = s.Start; s1++;
                    foreach (var fs in App.ViewModel.FavoriteSessions)
                    {
                        if (fs.Code == s.Code)
                        {  //  fav.Add(t);
                            fav.Add(new DayConferenceViewModel
                            {
                                LineOne = s.Title,
                                LineTwo = s.DateTimeQuickJumpSubtitle,
                                Section = MakeCaption("on now", nowStartTime),
                                Day = "",
                                SessCode = s.Code,
                                SortOrder = 0
                            });
                        }
                    }
                }
                if (haveNow)
                {
                    temp.Add(new DayConferenceViewModel
                    {
                        LineOne = (s1 == 1 ? "1 session" : s1 + " sessions"),
                        Section = MakeCaption("on now", nowStartTime),
                        Day = nowStartTime.ToString("yyyy-MM-dd hh:mm tt"),
                        SortOrder = 0
                    });
                    // Add favourites
                    temp.AddRange(fav);
                }
                
                var upcomingSessions = allUpcoming.FirstOrDefault();
                if (upcomingSessions != null) {
                    bool upcoming = (upcomingSessions.Sessions != null);
                    int t1 = 0;
                    fav = new List<DayConferenceViewModel>();
                    foreach (var t in upcomingSessions.Sessions) {
                        upcoming = true; nowStartTime = t.Start; t1++;
                        foreach (var fs in App.ViewModel.FavoriteSessions) {
                            if (fs.Code == t.Code) {  //  fav.Add(t);
                                fav.Add(new DayConferenceViewModel {
                                    LineOne = t.Title,
                                    LineTwo = t.DateTimeQuickJumpSubtitle,
                                    Section = MakeCaption("up next", nowStartTime),
                                    Day = "",
                                    SessCode = t.Code,
                                    SortOrder = 0
                                });
                            }
                        }
                    }
                    if (upcoming) {
                        temp.Add(new DayConferenceViewModel {
                            LineOne = (t1 == 1 ? "1 session" : t1 + " sessions"),
                            Section = MakeCaption("up next", nowStartTime),
                            Day = nowStartTime.ToString("yyyy-MM-dd hh:mm tt"),
                            SortOrder = 0
                        });
                        // Add favourites
                        temp.AddRange(fav);
                    }

                    if (!haveNow) {
                        upcomingSessions = allUpcoming.Skip(1).FirstOrDefault();    // the next timeslot after 'up next'

                        if (upcomingSessions == null) {   // There are no more sessions after this

                        } else {
                            upcoming = (upcomingSessions.Sessions != null);
                            int u1 = 0;
                            fav = new List<DayConferenceViewModel>();
                            foreach (var u in upcomingSessions.Sessions) {
                                upcoming = true; nowStartTime = u.Start; u1++;
                                foreach (var fs in App.ViewModel.FavoriteSessions) {
                                    if (fs.Code == u.Code) {  //  fav.Add(t);
                                        fav.Add(new DayConferenceViewModel {
                                            LineOne = u.Title,
                                            LineTwo = u.DateTimeQuickJumpSubtitle,
                                            Section = "afterwards",
                                            Day = "",
                                            SessCode = u.Code,
                                            SortOrder = 0
                                        });
                                    }
                                }
                            }
                            if (upcoming) {
                                temp.Add(new DayConferenceViewModel {
                                    LineOne = (u1 == 1 ? "1 session" : u1 + " sessions"),
                                    Section = "afterwards",
                                    Day = nowStartTime.ToString("yyyy-MM-dd hh:mm tt"),
                                    SortOrder = 0
                                });
                                // Add favorites
                                temp.AddRange(fav);
                            }
                        }
                    }
                }
            }

            //
            // ######   Load 'whats on' days  ####
            //
            var startDate = ConfItem.StartDate; 
            TimeSpan ds = ConfItem.EndDate.Subtract(startDate);
            var days = ds.Days + 1;

            int NumberOfDays = days; // 3;

            var sections = from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList() //ConfItem.Sessions
                           where s.Start.Day == startDate.Day
                           orderby s.Start ascending
                           group s by s.Start.ToString() into g
                           select g;


            
            for (int i = 0; i < NumberOfDays; i++)
            {
                temp.Add(
                new DayConferenceViewModel
                {
                    LineOne = FormatDate(startDate.AddDays(i), now) //startDate.AddDays(i).ToString("dddd, dd MMM")
                    ,
                    Section = "full schedule"
                    ,
                    Day = startDate.AddDays(i).ToString("yyyy-MM-dd")
                    ,
                    SortOrder = 1
                });
            }

          
            ScheduleItems = temp;
        }






        // Used to format a date
        static string FormatDate(DateTime date, DateTime now)
        {
            //[]date = date.ToUniversalTime();
            //[]now = now.ToUniversalTime();
            if (date.Year == now.Year && date.Month == now.Month)
            {
                if (date.Day == now.Day)
                    return "Today";
                if (date.AddDays(1).Day == now.Day)
                    return "Yesterday";
                if (date.AddDays(-1).Day == now.Day)
                    return "Tomorrow";
            }
            return date.ToString("dddd");
        }

        // Pretifies a caption to show a nice time relative to the current time.
        public string MakeCaption(string caption, DateTime start)
        {
            string date;
            var now = DateTime.Now;//[].ToUniversalTime();

            if (start.Year == now.Year && start.Month == now.Month)
            {
                if (start.Day == now.Day)
                    date = "";
                else if (start.Day == now.Day + 1)
                    date = "tom. at";
                else
                    date = start.ToString("MMM dd");
            }
            else
                date = start.ToString("MMM dd");

            return String.Format("{0}{1}{2} {3}", caption, caption != "" ? " " : "", date, start.ToString("H:mm"));
        }








        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}