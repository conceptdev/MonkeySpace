using System;
using System.Collections.Generic;

namespace MonkeySpace.Core
{
    static class Constants
    {
        /// <summary>
        /// /data/data/com.confapp.monospace11/files
        /// </summary>
        public static string DocumentsFolder
        {
            get
            { 
                return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            }
        }

		/// <summary>
		/// Hash tag to append to tweets from the app
		/// </summary>
		/// <value>#monkeyspace</value>
		public static string TwitterHashTag {
			get { return "#monkeyspace"; }
		}

		public static class ConferenceInfo
		{	public static string DisplayName {get {return "MonkeySpace 2013"; }}
			public static DateTime StartDate {get {return new DateTime(2013, 07, 22); }}
			public static DateTime EndDate {get {return new DateTime(2013, 07, 25); }}
			public const int NumberOfDays = 4; 
		}

		/// <summary>
		/// "My location" requires special handling in the client!
		/// </summary>
		public static List<MapLocation> Locations {
			get {
				var locations = new List<MonkeySpace.Core.MapLocation> () {
					new MonkeySpace.Core.MapLocation{Title="MonkeySpace 2013", Subtitle="Columbia College", Location=new MonkeySpace.Core.Point{X=-87.624464,Y=41.874147}}
					,new MonkeySpace.Core.MapLocation{Title="Renaissance Hotel", Subtitle="636 South Michigan Ave",Location=new MonkeySpace.Core.Point{X=-87.624469,Y=41.873298}}
					,new MonkeySpace.Core.MapLocation{Title="My location", Location=new MonkeySpace.Core.Point{X=0,Y=0}}
				};
				return locations;
			}
		}
    }
}