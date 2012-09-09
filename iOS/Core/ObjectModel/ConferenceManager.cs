using System;
using System.Json;
using System.IO;
using System.Collections.Generic;

namespace MonkeySpace.Core
{
	public static class ConferenceManager
	{
		public static string JsonDataFilename = "sessions.json";

		public static Dictionary<int, Session> Sessions = new Dictionary<int, Session>();

		public static Dictionary<int, Speaker> Speakers = new Dictionary<int, Speaker>();


		public static void LoadFromString (string jsonString)
		{
			var jsonObject = JsonValue.Parse (jsonString);
			
			if (jsonObject != null)
			{
				for (var j = 0;j < jsonObject.Count; j++) {
					var jsonSession = jsonObject[j];// as JsonValue;
					var session = new Session(jsonSession);
					
					Sessions.Add(session.Id, session);
					Console.WriteLine ("Session: " + session.Title);
					
					var jsonSpeakers = jsonSession["speakers"];// as JsonValue;
					
					for (var k = 0; k < jsonSpeakers.Count; k++) {
						var jsonSpeaker = jsonSpeakers[k]; // as JsonValue;
						var speaker = new Speaker(jsonSpeaker);
						
						if (!Speakers.ContainsKey (speaker.Id)) {
							Speakers.Add (speaker.Id, speaker);
						} else {
							speaker = Speakers[speaker.Id];
						}
						speaker.Sessions.Add (session);
						session.Speakers.Add (speaker);
						
						Console.WriteLine ("Speaker: " + speaker.Name);
					}
				}
			}
			Console.WriteLine ("done");
		}
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
	}
}

