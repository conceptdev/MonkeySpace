using System;
#if !WINDOWS_PHONE
using System.Json;
#endif
using System.Collections.Generic;

namespace MonkeySpace.Core
{
	/* Example Json
{"id":0,"name":"Chris Hardy","twitterHandle":"chrisntr","bio":"Chris Hardy, a Microsoft ASPInsider, is an .NET consultant focusing on MonoTouch and Mono for Android development.\n\nEver since MonoTouch was in beta, Chris has been developing and evangelising MonoTouch and Mono for Android and was one of the first users to get a MonoTouch application on to the App Store. Speaking at conferences around the world on the subject, Chris has been a key part of the community and extended this by contributing to MonoTouch and Mono for Android books.","headshotUrl":"/images/speakers/chris.jpg"}
	 */
	public class Speaker
	{
		public Speaker ()
		{
			Sessions = new List<Session>();
		}
#if !WINDOWS_PHONE
		public Speaker (JsonValue json) : this()
		{
			Id = json["id"];
			Name = json["name"];
			TwitterHandle = json["twitterHandle"];
			Bio = json["bio"];
			HeadshotUrl = json["headshotUrl"];
		}
#endif
		public int Id { get; set; }
		public string Name {get;set;}
		public string TwitterHandle {get;set;}
		public string Bio {get;set;}
		public string HeadshotUrl {get;set;}

		#if WINDOWS_PHONE
		// bit of a HACK: because we're serializing and storing as XML, 
		// this prevents a circular reference in the object graph
		// TODO: store favorites in Sqlite like the other platforms
		[System.Xml.Serialization.XmlIgnore]
		#endif
		public List<Session> Sessions {get;set;}
	}
}

