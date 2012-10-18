using System;
using System.Collections.Generic;
using System.Linq;
using Path = System.IO.Path;
using SQLite;
using MonkeyScan; // Business Objects

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
			SQLite3.Config (SQLite3.ConfigOption.Serialized);

			CreateTable<ConfScan>();
			CreateTable<ConfAttendee>();
		}

		public IEnumerable<ConfAttendee> GetAttendees ()
		{
			return Query<ConfAttendee> ("SELECT * FROM ConfAttendee ORDER BY Name ASC");
		}

		public IEnumerable<ConfScan> GetScans ()
		{
			return Query<ConfScan> ("SELECT * FROM ConfScan ORDER BY ScannedAt DESC");
		}

		public bool HasScanned (string barcode)
		{
			IEnumerable<ConfScan> x = Query<ConfScan> ("SELECT Barcode FROM ConfScan WHERE Barcode = ?", barcode);
			return x.ToList().Count > 0;
		}

		public void AddScan (ConfScan scan) {
			Insert (scan);
		}

		public ConfAttendee CheckBarcode (ConfScan scan)
		{
			var attendee = Find<ConfAttendee> (a => a.Barcode == scan.Barcode);
			//var attendees = Query<ConfAttendee> ("SELECT * FROM ConfAttendee WHERE Barcode = ?", scan.Barcode);

			if (attendee != null)
				scan.AttendeeName = attendee.Name;
			return attendee; // could be null
		}


		// helper for checking if database has been populated
		public int CountAttendees()
		{
			string sql = string.Format ("select count (*) from \"{0}\"", typeof (ConfAttendee).Name);
			var c = CreateCommand (sql, new object[0]);
			return c.ExecuteScalar<int>();
		}
		public int CountPresent()
		{
			string sql = string.Format ("select count (*) from \"{0}\" where ScanCount > 0", typeof (ConfAttendee).Name);
			var c = CreateCommand (sql, new object[0]);
			return c.ExecuteScalar<int>();
		}

		/// <summary>
		/// Gets all the attendees from Azure and updates the local database
		/// </summary>
		public void UpdateAllAttendees (List<ConfAttendee> latestAttendeeList)
		{
			foreach (var latest in latestAttendeeList) {
				var local = Find<ConfAttendee> (ca => ca.Barcode == latest.Barcode);
				if (local == null) {
					Console.WriteLine ("Inserting " + latest.Name);
					Insert (local);
				} else {
					Console.WriteLine ("Updating " + latest.Name);
					if (latest.ScanCount > local.ScanCount) {
						local.ScanCount = latest.ScanCount;
						Update (local);
					}
				}
			}
		}
		/// <summary>"Attendees.csv" static seed data to load for first use</summary>
		public static string CsvDataFilename = "Attendees.csv";

		/// <summary>
		/// Load the embedded attendee data, which allows the app
		/// to work offline from installation. Hardcoding is bad :)
		/// </summary>
		public void LoadSeedAttendeeData ()
		{
			var csv = System.IO.File.ReadAllText (CsvDataFilename);

			var lines = csv.Split ('\n');
			foreach (var line in lines) {
				var fields = line.Trim().Split (',');
				var attendee = new ConfAttendee() {
					SqlId=Convert.ToInt32(fields[0]), 
					Barcode=fields[1], 
					Name=fields[2]};
				Insert (attendee);
				Console.WriteLine("Added " + attendee.Name);
			}
		}
	}
}