using System;
#if WINDOWS_PHONE
using Newtonsoft.Json;
#else
using System.Json;
#endif
using System.IO;
using System.Collections.Generic;
//using System.Runtime.Serialization.Json;
using System.Text;

namespace MonkeySpace.Core
{
	public static class ConferenceManager
	{
		public static string JsonDataFilename = "sessions.json";

        public static Dictionary<int, Session> Sessions = new Dictionary<int, Session>();

		public static Dictionary<int, Speaker> Speakers = new Dictionary<int, Speaker>();

		public static void LoadFromString (string jsonString)
        {
#if WINDOWS_PHONE
            jsonString = jsonString.Replace("0000-04:00", ""); // HACK: excuse this timezone hack
            var sessions = JsonConvert.DeserializeObject<List<Session>>(jsonString);
            Sessions = new Dictionary<int, Session>();
            Speakers = new Dictionary<int, Speaker>();
            foreach (var session in sessions) {
                Sessions.Add(session.Id, session);
                Console.WriteLine("Session: " + session.Title);
                foreach (var speaker in session.Speakers) {
					speaker.Sessions.Add(session);
					if (!Speakers.ContainsKey(speaker.Id)) {
                        Speakers.Add(speaker.Id, speaker);
                    }
                    Console.WriteLine("Speaker: " + speaker.Name);
                }
            }
#else
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
#endif
            Console.WriteLine ("done");
		}

#if !WINDOWS_PHONE
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
#endif
	}
}

