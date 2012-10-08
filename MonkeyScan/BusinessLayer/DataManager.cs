using System;
using System.Collections.Generic;
using System.Linq;
using Path = System.IO.Path;
using SQLite;
using QrSample.iOS; // Business Objects

namespace SQLite
{
	/// <summary>
	/// Use to store data
	/// </summary>
	public class DataManager : SQLiteConnection
	{
		public static string DatabaseFilePath {
			get { 
				var path = "userdata.db3";
				#if SILVERLIGHT
				//path = "userdata.db3";
				#else
				
				#if __ANDROID__
				string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				#else
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				string libraryPath = Path.Combine (documentsPath, "..", "Library");
				#endif
				path = Path.Combine (libraryPath, path);
				#endif		
				return path;	
			}
		}

		public DataManager (string path) : base (path)
		{
			CreateTable<ConfScan>();
			CreateTable<ConfAttendee>();
		}

		public IEnumerable<ConfScan> GetScans ()
		{
			return Query<ConfScan> ("SELECT Barcode, ScannedAt FROM ConfScan ORDER BY ScannedAt DESC");
		}

		public bool HasScanned (string barcode)
		{
			IEnumerable<ConfScan> x = Query<ConfScan> ("SELECT Barcode FROM ConfScan WHERE Barcode = ?", barcode);
			return x.ToList().Count > 0;
		}

		public void AddScan (ConfScan scan) {
			Insert(scan);
		}
	}
}