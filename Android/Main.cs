using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Android.App;
using MonkeySpace.Core;

namespace MonkeySpace
{
    [Application]
    public class Conf : Application
    {
        public static Conf Current { get; private set; }

		public static UserDatabase UserData {get; private set;} 
		/// <summary>userdata.db</summary>
		public static string SqliteDataFilename = "userdata.db";

		string docFolder;

        public Conf(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle,transfer)
        {
            Current = this;
        }

        public override void OnCreate()
        {
            base.OnCreate();

			docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var confFolder = System.IO.Path.Combine(docFolder, "monkeyspace12");
            if (!System.IO.Directory.Exists(confFolder))
                System.IO.Directory.CreateDirectory(confFolder);

			// setup SQLite for 'starred sessions' database
			UserData = new UserDatabase(UserDatabase.DatabaseFilePath);


			var confFile = Path.Combine(confFolder, MonkeySpace.Core.ConferenceManager.JsonDataFilename);

            if (!System.IO.File.Exists(confFile))
            {
                var s = Resources.OpenRawResource(Resource.Raw.sessions);  // RESOURCE NAME ###

                // create a write stream
                FileStream writeStream = new FileStream(confFile, FileMode.OpenOrCreate, FileAccess.Write);
                // write to the stream
                ReadWriteStream(s, writeStream);
            }

			var jsonString = System.IO.File.ReadAllText (confFile);

			MonkeySpace.Core.ConferenceManager.LoadFromString(jsonString);

            DeserializeConferenceFile("");

        }
        // readStream is the stream you need to read
        // writeStream is the stream you want to write to
        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }



        public string CurrentConferenceCode
        {
            get { return "monkeyspace12"; }
        }

        /// <summary>
        /// A collection of 'favorite' sessions
        /// </summary>
        private List<MonkeySpace.Core.Session> _FavoriteSessions;// { get; private set; }
		public List<MonkeySpace.Core.Session> FavoriteSessions // { get; set; }
        {
            get { return _FavoriteSessions; }
            set
            {
                _FavoriteSessions = value;
            }
        }
		public void UpdateFavorite (MonkeySpace.Core.Session session, bool isFavorite)
        {
			if (UserData.IsFavorite(session.Code))
				if (!isFavorite)
					UserData.RemoveFavoriteSession(session.Code);
			else // not Favorite
				if (isFavorite)
					UserData.AddFavoriteSession(session.Code);

			var favs = UserData.GetFavoriteCodes();
			_FavoriteSessions = (from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList () 
								 where favs.Contains(s.Code)
			                     select s).ToList();

//
//            var fs = (from s in this.FavoriteSessions
//                      where s.Code == session.Code
//                      select s).ToList();
//
//            if (fs.Count > 0)
//            {
//                // remove, as it might be old data persisted anyway (from previous conf.xml file)
//                this.FavoriteSessions.Remove(fs[0]);
//            }
//            if (isFavorite)
//            {   // add it again
//				//this.FavoriteSessions.Add(new MonkeySpace.Core.Session(session.Clone()));
//				this.FavoriteSessions.Add(session); // HACK
//            }
//
//            var temp = _FavoriteSessions.OrderBy(s => s.Start.Ticks);
//			_FavoriteSessions = new List<MonkeySpace.Core.Session>();
//            foreach (var t in temp)
//            {
//                _FavoriteSessions.Add(t);
//            }
//
//            SaveFavouritesFile();

            // This updates the 'whats on next' with favourites (if required)
            this.LoadWhatsOn(this.CurrentConferenceCode);
        }


//        public void SaveFavouritesFile()
//        {
//            var basedir = Path.Combine(docFolder, CurrentConferenceCode);
//            var xmlPath = Path.Combine(basedir, "Favourites.xml");
//            Console.WriteLine("[Main] SaveFeedbackFile");
//			ConceptDevelopment.Kelvin<List<MonkeySpace.Core.Session>>.ToXmlFile(Conf.Current.FavoriteSessions, xmlPath);
//        }

        private List<DayConferenceViewModel> _ScheduleItems;
        public List<DayConferenceViewModel> ScheduleItems
        {
            get { return _ScheduleItems; }
            set
            {
                _ScheduleItems = value;
            }
        }

		public ConferenceInfo ConfItem
		{ get; set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }


        void DeserializeConferenceFile(string xmlPath)
        {
			ConfItem = new ConferenceInfo () {
				StartDate = new DateTime(2012,10, 17),
				EndDate = new DateTime(2012,10,19), 
			};


            //
            // ######   Load 'favorites'   ####
            //
            var favPath = Path.Combine(docFolder, this.CurrentConferenceCode);
            favPath = Path.Combine(favPath, "Favorites.xml");

			FavoriteSessions = new List<MonkeySpace.Core.Session>();

            LoadWhatsOn(this.CurrentConferenceCode);

            this.IsDataLoaded = true;

        }

        
        public void LoadWhatsOn(string currentConf)
        {
            Console.WriteLine("[Conf] LoadWhatsOn");
            var temp = new List<DayConferenceViewModel>();

            var now = DateTime.Now;
#if DEBUG
            // TEST: this for testing only
            //now = new DateTime(2011, 3, 8, 15, 51, 0);
#endif
            //
            // #####  Find out what's up next  ######
            //
            if (now < this.ConfItem.EndDate.AddDays(1)) //new DateTime(2010,6,2))
            {   // During the conference

                //var nowStart = now.AddMinutes(now.Minute < 30 ? -now.Minute : -(now.Minute - 30)); // also helps (within 30 minutes bug)
                var nowStart = now.AddMinutes(-(now.Minute % 5)); // round to 5 minutes
                // The shortest session appears to be 30 minutes that I could see
                var nextStart = nowStart.AddMinutes(29); // possible fix to (within 30 minutes bug)
                nextStart = nextStart.AddSeconds(-nextStart.Second);

                // sometimes the data might not have an 'end time', so we'll handle that by assuming 1 hour long sessions
				var happeningNow = from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList ()
                                   where s.Start <= now
                                   && (now < (s.End == DateTime.MinValue ? s.Start.AddHours(1) : s.End))
                                   && (s.Start.Minute % 10 == 0 || s.Start.Minute % 15 == 0) // fix for short-sessions (which start at :05 after the hour)
                                   select s;

				var allUpcoming = from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList ()
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
                    foreach (var fs in this.FavoriteSessions)
                    {
                        if (fs.Code == s.Code)
                        {
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
                bool upcoming = (upcomingSessions.Sessions != null);
                int t1 = 0;
                fav = new List<DayConferenceViewModel>();
                foreach (var t in upcomingSessions.Sessions)
                {
                    upcoming = true; nowStartTime = t.Start; t1++;
                    foreach (var fs in this.FavoriteSessions)
                    {
                        if (fs.Code == t.Code)
                        {  //  fav.Add(t);
                            fav.Add(new DayConferenceViewModel
                            {
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
                if (upcoming)
                {
                    temp.Add(new DayConferenceViewModel
                    {
                        LineOne = (t1 == 1 ? "1 session" : t1 + " sessions"),
                        Section = MakeCaption("up next", nowStartTime),
                        Day = nowStartTime.ToString("yyyy-MM-dd hh:mm tt"),
                        SortOrder = 0
                    });
                    // Add favourites
                    temp.AddRange(fav);
                }
                if (!haveNow)
                {
                    upcomingSessions = allUpcoming.Skip(1).FirstOrDefault();    // the next timeslot after 'up next'

                    if (upcomingSessions == null)
                    {   // There are no more sessions after this

                    }
                    else
                    {
                        upcoming = (upcomingSessions.Sessions != null);
                        int u1 = 0;
                        fav = new List<DayConferenceViewModel>();
                        foreach (var u in upcomingSessions.Sessions)
                        {
                            upcoming = true; nowStartTime = u.Start; u1++;
                            foreach (var fs in this.FavoriteSessions)
                            {
                                if (fs.Code == u.Code)
                                {  //  fav.Add(t);
                                    fav.Add(new DayConferenceViewModel
                                    {
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
                        if (upcoming)
                        {
                            temp.Add(new DayConferenceViewModel
                            {
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

            //
            // ######   Load 'whats on' days  ####
            //
            var startDate = ConfItem.StartDate; // new DateTime(2010, 03, 15);
            TimeSpan ds = ConfItem.EndDate.Subtract(startDate);
            var days = ds.Days + 1;

            int NumberOfDays = days; // 3;

			var sections = from s in MonkeySpace.Core.ConferenceManager.Sessions.Values.ToList ()
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
            var now = DateTime.Now;

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
    }
}