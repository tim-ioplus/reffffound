using NuGet.Packaging.Signing;
using reffffound.Data;
using reffffound.Models;

namespace reffffound.Services
{
	public class BookmarkHelper
	{
		[Obsolete( "Use _bookmarkService provided by Constructor", true )]
		private BookmarkRepository _bookmarkRepository;
		private IBookmarkService _bookmarkService;
		private IUserService _userService;

		public BookmarkHelper(IBookmarkService bookmarkService, IUserService userService = null)
		{
			_bookmarkService = bookmarkService;
			_userService = userService;
		}
		public bool UpdateUsercounts()
		{
			bool success = false;

			var users = _userService.List( );
			if (users.Any( ))
			{
				foreach (var user in users)
				{
					int count = GetBookmarkCount( user.Name );
					user.Count = count;
					_userService.Update( user );
				}

				success = true;
			}

			return success;
		}

		public int GetPageCount(string user = "")
		{
			var pagesize = 10;
			var count = 0;

			var bookmarkcount = GetBookmarkCount( user );
			if (bookmarkcount > 0)
			{
				count = (int)Math.Ceiling( (decimal)bookmarkcount / pagesize );
			}

			return count;
		}
		public int GetBookmarkCount(string username = "")
		{
			var count = _bookmarkService.GetBookmarkCount( username, "" );
			return count;
		}


		internal bool UpdateUsernames()
		{
			var success = false;

			var bookmarks = _bookmarkService.ListAll( );
			var usernames = new List<string>( );
			if (bookmarks.Any( ))
			{
				foreach (var bookmark in bookmarks)
				{
					SetUsername( bookmark );
					_bookmarkService.Update( bookmark );

					if (!usernames.Contains( bookmark.Username ))
					{
						usernames.Add( bookmark.Username );
					}
				}

				UpdateUserCount( usernames );

				success = true;
			}

			return success;
		}

		/// <summary>
		/// Older or incomplete Data may not include explicit Username yet.
		/// Sets the  
		/// </summary>
		/// <param name="bookmark"></param>
		public void SetUsername(Bookmark bookmark)
		{
			if (string.IsNullOrWhiteSpace( bookmark.Username ) &&
				!string.IsNullOrWhiteSpace( bookmark.Usercontext ))
			{
				var userscontexts = bookmark.Usercontext.Replace( " ", "" ).Split( ',' );
				if (userscontexts.Any( ))
				{
					string username = userscontexts[0].ToString( );
					if (!string.IsNullOrWhiteSpace( username ))
					{
						bookmark.Username = username;
					}
				}
			}
		}

		public Bookmark CreateFrom(IFormCollection collection)
		{
			Bookmark bookmark = new Bookmark( );

			var url = collection["Url"][0] ?? "";
			var title = collection["Title"][0] ?? "";
			var image = collection["Image"][0] ?? "";
			var usercontext = collection["Usercontext"][0] ?? "";
			var username = usercontext;

			bookmark.Url = url;
			if (!string.IsNullOrWhiteSpace( bookmark.Url ) && string.IsNullOrWhiteSpace( title ))
			{
				title = GetTitleFromUrl( bookmark.Url );
			}

			bookmark.Title = title;
			bookmark.Image = image;
			bookmark.Usercontext = usercontext;
			bookmark.Username = username;
			bookmark.Savedby = 1;

			bookmark.Context1img = bookmark.Context1link =
			bookmark.Context2img = bookmark.Context2link =
			bookmark.Context3img = bookmark.Context3link =
			bookmark.FullUrl = "";

			return bookmark;
		}

		private string GetTitleFromUrl(string url)
		{
			string titlepart = "";
			var partsplits = url.Split( '/' );
			var lastPart = partsplits[partsplits.Length - 1];
			if (lastPart.Contains( "?" ))
			{
				var sitesplits = lastPart.Split( "?" );
				lastPart = sitesplits[0];
			}

			if (lastPart.Contains( "#" ))
			{
				var segmentsplits = lastPart.Split( "#" );
				lastPart = segmentsplits[0];
			}

			titlepart = lastPart;

			var site_urltext = titlepart.Replace( "_", " " ).Replace( "-", " " ).Trim( );
			var site_domaintext = "";

			var domainsplit = url.Split( "www." )?[1].Split( "." )?[0];
			if (!string.IsNullOrWhiteSpace( domainsplit ))
			{
				site_domaintext = domainsplit + " - ";
			}

			string titletext = site_domaintext + site_urltext;

			return titletext;
		}


		public List<Bookmark> ReadFromFile(IFormFile dataFile)
		{
			var bookmarks = new List<Bookmark>( );
			var hydrationType = HydrationType.None;

			using (var reader = new StreamReader( dataFile.OpenReadStream( ), System.Text.Encoding.UTF8, true ))
			{
				int currentLine = 0;
				char seperationChar = ',';
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine( );
					currentLine++;

					if (!string.IsNullOrWhiteSpace( line ))
					{
						if (currentLine == 1)
						{
							var columnHeaders = line.Split( seperationChar );
							if (columnHeaders.Length == 1)
							{
								char semicolonSeperationChar = ';';
								columnHeaders = line.Split( semicolonSeperationChar );

								if (columnHeaders.Length > 1)
								{
									seperationChar = semicolonSeperationChar;
								}
							}
							if (columnHeaders[0] == "Guid")
							{
								// Hydrate like Backup
								hydrationType = HydrationType.Backup;
							}
							else if (columnHeaders[0] == "User")
							{
								// Hydrate like Userposts
								hydrationType = HydrationType.Userpost;
							}
							else
							{
								// Dateifehler
								break;
							}
						}
						else if (currentLine > 1)
						{
							Bookmark? bookmark = null;
							if (hydrationType == HydrationType.Backup)
							{
								try
								{
									bookmark = ReadAsBackup( line, seperationChar );
								}
								catch (Exception ex)
								{
									throw ex;
								}
							}
							else if (hydrationType == HydrationType.Userpost)
							{
								try
								{
									bookmark = ReadAsPost( line, seperationChar );
								}
								catch (Exception ex)
								{
									throw ex;
								}
							}
							else
							{
								break;
							}

							if (bookmark != null)
							{
								bookmarks.Add( bookmark );
							}
							else
							{
								// Fehler 
								// Zeile konnte nicht geparsed werden
								// 
							}
						}
					}
				}
			}

			if (bookmarks.Any( ) && hydrationType == HydrationType.Userpost)
			{
				var bookmarksByUser = bookmarks.GroupBy( b => b.Username );
				foreach (var usersBookmarks in bookmarksByUser)
				{
					DateTime lastDatetime = DateTime.Now.Subtract( new TimeSpan( 1, 0, 0, 0, 0 ) );
					Bookmark lastPost = null;

					try
					{
						lastPost = _bookmarkService.GetLastPost( usersBookmarks.Key );
					}
					catch (Exception ex)
					{
						throw ex;
					}

					int intervals = usersBookmarks.Count( ) + 1;

					if (lastPost != null && lastPost.Timestamp != null)
					{
						lastDatetime = DateTime.Parse( lastPost.Timestamp );
					}

					var nextTimespan = DateTime.Now.Subtract( lastDatetime ).Divide( intervals );
					var nextPosttimestamp = lastDatetime.Add( nextTimespan );

					foreach (Bookmark bookmark in usersBookmarks)
					{
						bookmark.Timestamp = nextPosttimestamp.ToString( DatetimeFormat.Standard );
						nextPosttimestamp = nextPosttimestamp.Add( nextTimespan );
					}
				}
			}

			return bookmarks;
		}



		private Bookmark? ReadAsPost(string line, char seperationChar = ',')
		{
			var bookmarkValues = line.Split( seperationChar );
			if (bookmarkValues.Length < 4)
			{
				return null;
			}

			Bookmark? bookmark = new Bookmark( );
			string username = bookmarkValues[0];
			bookmark.Username = username;
			bookmark.Usercontext = username;
			bookmark.Image = bookmarkValues[1];
			bookmark.Url = bookmarkValues[2];
			bookmark.Title = bookmarkValues[3];

			if (bookmarkValues.Length > 4)
			{
				var extraText = "";
				for (int i = 4; i <= bookmarkValues.Length - 1; i++)
				{
					if (!string.IsNullOrWhiteSpace(bookmarkValues[i]))
					{
						extraText += " " + bookmarkValues[i]; 
					}
				}

				bookmark.Title += extraText;
			}

			return bookmark;
		}

		private Bookmark? ReadAsBackup(string line, char seperationChar = ',')
		{
			Bookmark bookmark = new Bookmark( );

			var bookmarkValues = line.Split( seperationChar );
			if (bookmarkValues.Length != 15) return null;

			bookmark.Guid = bookmarkValues[0];
			var savedBookmark = _bookmarkService.Read( bookmark.Guid );
			if (savedBookmark != null) return null;

			bookmark.Url = bookmarkValues[1];
			bookmark.Title = bookmarkValues[2];
			bookmark.Image = bookmarkValues[3];
			bookmark.Savedby = 1;
			if (int.TryParse( bookmarkValues[4], out int savedby ))
			{
				bookmark.Savedby = savedby;
			}
			bookmark.Timestamp = bookmarkValues[5];
			bookmark.Usercontext = bookmarkValues[6];
			bookmark.FullUrl = bookmarkValues[7];
			bookmark.Context1link = bookmarkValues[8];
			bookmark.Context1img = bookmarkValues[9];
			bookmark.Context2link = bookmarkValues[10];
			bookmark.Context2img = bookmarkValues[11];
			bookmark.Context3link = bookmarkValues[12];
			bookmark.Context3img = bookmarkValues[13];
			bookmark.Username = bookmarkValues[14];
			SetUsername( bookmark );

			return bookmark;
		}

		public enum HydrationType
		{
			None = 0,
			Backup = 1,
			Userpost = 2
		}

		internal void UpdateUserCount(List<string> usernames)
		{
			foreach (var username in usernames)
			{
				var userName = username.Trim( );
				var userRole = UserManagerHelper.GetRole( userName );
				var userCount = _bookmarkService.GetBookmarkCount( userName, "" );

				var storedUser = _userService.Read( userName );
				if (storedUser != null)
				{
					storedUser.Name = userName;
					storedUser.Role = userRole;
					storedUser.Count = userCount;

					_userService.Update( storedUser );
				}
				else
				{
					var user = new ContentUser( );
					user.Name = userName;
					user.Email = "";
					user.Role = userRole;
					user.Link = "";
					user.Count = userCount;

					_userService.Create( user );
				}
			}
		}
	}
}
