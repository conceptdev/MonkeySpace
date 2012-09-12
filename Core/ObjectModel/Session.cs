using System;
using System.Collections.Generic;
#if !WINDOWS_PHONE
using System.Json;
#endif
using System.Collections;

namespace MonkeySpace.Core
{
	/* Example Json
{"id":0
,"title":"Simple, Fast, Elastic NoSQL with Couchbase Server and Mono"
,"abstract":"Couchbase Server is a simple, fast, and elastic documented-oriented database. It is simple in its document-oriented approach to data modeling, where domain objects may be naturally mapped to their persistence layer. It is simple to monitor and manage in production, elastically allowing administrators to add and remove nodes to a cluster at any time, without downtime. Couchbase Server is fast thanks to its actively managed cache, compatible with (and built upon) memcached. Indexing, analytics and other advanced ways of managing data in a Couchbase cluster are easily available through the definition of incremental Map/Reduce views."
,"speakers":[{"id":9,"name":"John Zablocki","twitterHandle":"codevoyeur","bio":"John Zablocki is a Developer Advocate at Couchbase, where he is responsible for developing the Couchbase Client Library for .NET. He is the organizer of Beantown ALT.NET , a former adjunct at Fairfield University, and an author for O’Reilly. John holds an M.S. in Computer Science from Rensselaer Hartford. He has worked at startups throughout his career and is interested in the intersection of .NET and open source. Online, John can be found at http://johnzablocki.com. Offline, he can be found too infrequently around Boston, with his dog, daughter, and his Fender Telecaster.","headshotUrl":"/images/speakers/john_zablocki.jpg"}]
,"begins":"2012-10-17T10:15:00.0000000-04:00"
,"ends":"2012-10-17T11:15:00.0000000-04:00"
,"location":"Sampson"}
	 */
	public class Session
	{
		public Session ()
		{
			Speakers = new List<Speaker>();
		}

#if !WINDOWS_PHONE
		public Session (JsonValue json) : this()
		{
			Id = json["id"];
			Title = json["title"];
			Abstract = json["abstract"];
			Location = json["location"];

			var begins = json["begins"].ToString().Trim ('"').Substring (0, 19).Replace ("T", " ");
			Begins = DateTime.Parse (begins);
			var ends = json["ends"].ToString ().Trim ('"').Substring (0, 19).Replace ("T", " ");
			Ends = DateTime.Parse (ends, System.Globalization.CultureInfo.InvariantCulture);
		}
#endif
		public int Id { get; set; }
		public string Title {get;set;}
		public string Abstract {get;set;}
		public string Location {get;set;}
		public DateTime Begins {get;set;}
		public DateTime Ends {get;set;}

		public string LocationDisplay {
			get {
				if (Location.ToLower ().Contains ("tbd") 
				    || Location.ToLower ().Contains ("room")
				    || Location.ToLower ().Contains ("hall")
				    || Title.ToLower ().Contains ("party"))
					return Location;
				else
					return Location + " room";
			}
		}

		// compat
		public string Code { get { return Id.ToString (); } }
		public DateTime Start { get { return Begins; } }
		public DateTime End { get { return Ends; } }
		public string DateTimeDisplay {get{ return Begins.ToString("ddd MMM dd H:mm");}}
		public string DateTimeQuickJumpDisplay {
			get{return Begins.ToString("ddd MMM dd H:mm");}}
		public string DateTimeQuickJumpSubtitle {
			get{return Begins.ToString("ddd MMM dd H:mm");}}
        public string TimeQuickJumpDisplay{
            get{ return Start.ToString("ddd H:mm");}}
		public bool HasTag (string tag)
		{
			return false;
		}

		public List<Speaker> Speakers {get;set;}

        public string SpeakerList { get { return GetSpeakerList();  } }
		public string GetSpeakerList ()
		{
			var speakers = "";
			foreach (var s in Speakers) {
				speakers += s.Name + ", ";
			}
			return speakers.TrimEnd(new char[] {' ',','});
		}
	}
}