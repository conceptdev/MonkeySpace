using System;
using System.Json;
using System.IO;
using System.Collections.Generic;

using System.Net;

namespace MonkeySpace.Core
{
	public class ConferenceManager
	{
		public Action<string> OnDownloadSucceeded;
		public Action<string> OnDownloadFailed;

		/// <summary>"sessions.json"</summary>
		public static string JsonDataFilename = "sessions.json";

		public static string SessionDataUrl = "http://monkeyspace.org/data/sessions.json";

		public static Dictionary<int, Session> Sessions = new Dictionary<int, Session>();

		public static Dictionary<int, Speaker> Speakers = new Dictionary<int, Speaker>();

		public static DateTime LastUpdated { get; private set; }

		public static String LastUpdatedDisplay {get{return LastUpdated.ToString("yyyy-MM-dd HH:mm:ss");}}

		public static bool LoadFromString (string jsonString)
		{
			Dictionary<int, Session> localSessions = null;
			Dictionary<int, Speaker> localSpeakers = null;

			// if parsing succeeds, use the data
			if (ParseJson (jsonString, ref localSessions, ref localSpeakers)) {
				Sessions = localSessions;
				Speakers = localSpeakers;
				Console.WriteLine ("sessions.json has been loaded");
				return true;
			}
			Console.WriteLine ("Parsing sessions.json failed");
			return false;

//			var jsonObject = JsonValue.Parse (jsonString);
//			
//			if (jsonObject != null)
//			{
//				for (var j = 0;j < jsonObject.Count; j++) {
//					var jsonSession = jsonObject[j];// as JsonValue;
//					var session = new Session(jsonSession);
//					
//					Sessions.Add(session.Id, session);
//					Console.WriteLine ("Session: " + session.Title);
//					
//					var jsonSpeakers = jsonSession["speakers"];// as JsonValue;
//					
//					for (var k = 0; k < jsonSpeakers.Count; k++) {
//						var jsonSpeaker = jsonSpeakers[k]; // as JsonValue;
//						var speaker = new Speaker(jsonSpeaker);
//						
//						if (!Speakers.ContainsKey (speaker.Id)) {
//							Speakers.Add (speaker.Id, speaker);
//						} else {
//							speaker = Speakers[speaker.Id];
//						}
//						speaker.Sessions.Add (session);
//						session.Speakers.Add (speaker);
//						
//						Console.WriteLine ("Speaker: " + speaker.Name);
//					}
//				}
//			}

		}

		/// <summary>
		/// Parses the json passed in and ONLY if the parsing succeeded does it populate the ref params
		/// </summary>
		static bool ParseJson (string jsonString, ref Dictionary<int, Session> sessions, ref Dictionary<int, Speaker> speakers)
		{
			Dictionary<int, Session> localSessions = new Dictionary<int, Session>();
			Dictionary<int, Speaker> localSpeakers = new Dictionary<int, Speaker>();

			try {
				var jsonObject = JsonValue.Parse (jsonString);
			
				if (jsonObject != null) {
					for (var j = 0; j < jsonObject.Count; j++) {
						var jsonSession = jsonObject [j];// as JsonValue;
						var session = new Session (jsonSession);
					
						localSessions.Add (session.Id, session);
						Console.WriteLine ("Session: " + session.Title);
					
						var jsonSpeakers = jsonSession ["speakers"];// as JsonValue;
					
						for (var k = 0; k < jsonSpeakers.Count; k++) {
							var jsonSpeaker = jsonSpeakers [k]; // as JsonValue;
							var speaker = new Speaker (jsonSpeaker);
						
							if (!localSpeakers.ContainsKey (speaker.Id)) {
								localSpeakers.Add (speaker.Id, speaker);
							} else {
								speaker = localSpeakers [speaker.Id];
							}
							speaker.Sessions.Add (session);
							session.Speakers.Add (speaker);
						
							Console.WriteLine ("Speaker: " + speaker.Name);
						}
					}
				}
			} catch (Exception ex) {
				// something in the parsing failed
				Console.WriteLine ("Parsing failed " + ex);
				return false;
			}
			speakers = localSpeakers;
			sessions = localSessions;
			return true;
		}

		[Obsolete("System.IO dependency breaks WP7")]
		public static void LoadFromFile ()
		{
			string xmlPath = JsonDataFilename;
			var basedir = Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);

			if (File.Exists (Path.Combine (basedir, JsonDataFilename))) {	// load a downloaded copy
				xmlPath = Path.Combine (basedir, JsonDataFilename);
			}
		
			var jsonString = File.ReadAllText(xmlPath);

			LoadFromString(jsonString);
		}

		public void DownloadFromServer() 
		{
			var client = new WebClient();
			
			client.DownloadStringCompleted += DownloadCompleted;

			client.DownloadStringAsync(new Uri(SessionDataUrl));

		}
		private void DownloadCompleted (object sender, DownloadStringCompletedEventArgs e)
		{
			if (e.Error != null || e.Result == null) {
				Console.WriteLine("Nothing downloaded from " + SessionDataUrl); 
				if (e.Error != null)
					Console.WriteLine("Error was: " + e.Error.Message); 
				if (OnDownloadFailed != null)
					OnDownloadFailed("Download error.");
			} else {
				string jsonString = e.Result;

				if (LoadFromString(jsonString)) {
					LastUpdated = DateTime.Now;

					if (OnDownloadSucceeded != null)
						OnDownloadSucceeded(jsonString);
				} else {
					if (OnDownloadFailed != null)
						OnDownloadFailed("Parsing error.");
				}
			}
		}

	}
}

