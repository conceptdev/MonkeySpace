using System;
using System.Collections.Generic;
using System.Linq;
using Path = System.IO.Path;
using SQLite;

namespace MonkeySpace.Core
{
	public class FavoriteSession
	{
		[PrimaryKey]
		public string Code {get;set;}
	}

	/// <summary>
	/// Use to store 'favorite sessions'
	/// </summary>
	public class UserDatabase : SQLiteConnection
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

		public UserDatabase (string path) : base (path)
		{
			CreateTable<FavoriteSession>();
		}
		public IEnumerable<FavoriteSession> GetFavorites ()
		{
			return Query<FavoriteSession> ("SELECT Code FROM FavoriteSession ORDER BY Code");
		}
		public List<string> GetFavoriteCodes ()
		{
			IEnumerable<FavoriteSession> x = Query<FavoriteSession> ("SELECT Code FROM FavoriteSession ORDER BY Code");
			List<string> l = new List<string>();
			foreach (var s in x)
				l.Add(s.Code);
			return l;
		}
		public bool IsFavorite (string sessionCode)
		{
			IEnumerable<FavoriteSession> x = Query<FavoriteSession> ("SELECT Code FROM FavoriteSession WHERE Code = ?", sessionCode);
			return x.ToList().Count > 0;
		}
		public void AddFavoriteSession (string sessionCode) {
			Insert(new FavoriteSession() {
				Code = sessionCode
			});
		}
		public void RemoveFavoriteSession (string sessionCode) {
			Delete(new FavoriteSession() {
				Code = sessionCode
			});
		}
	}
}