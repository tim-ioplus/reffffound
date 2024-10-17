using reffffound.Data;
using reffffound.Models;

namespace reffffound.Services
{
	public class BookmarkHelper
{
	private string _connectionString;
	private BookmarkRepository _bookmarkRepository;

	public BookmarkHelper(string connectionString)
	{
			_connectionString = connectionString;
			_bookmarkRepository = new BookmarkRepository(_connectionString);
	}
		public bool UpdateUsercounts()
		{
			bool success = false;

			var userRepository = new UserRepository(_connectionString);

			var users = userRepository.List( );
			if (users.Any( ))
			{
				foreach (var user in users)
				{
					int count = GetBookmarkCount( user.Name );
					user.Count = count;
					userRepository.Update( user );
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
			var count = _bookmarkRepository.GetCount( username );
			return count;
		}

		public Bookmark GetFeedNullFour(string username = "", string filter = "", int page = 0)
		{
			var bookmark = new Bookmark( );
			bookmark.Guid = "";
			bookmark.Savedby = 0;
			bookmark.Usercontext = "";
			bookmark.Timestamp = new List<string>( ) { "Eternity", "Someday", "Soon", "In a while", "Aeons ago", "In the past", "In the future" }.ElementAt( new Random( ).Next( 0, 7 ) );
			bookmark.Title = "This is the end..";
			bookmark.Url = "NoUrl";
			bookmark.Image = new List<string>( )
				{
				"https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fp1.pxfuel.com%2Fpreview%2F528%2F171%2F802%2Fthe-end-sand-end-beach.jpg&f=1&nofb=1&ipt=c02ebd78bbd57fa507556d98bf73fa9f5f5e0ccd71f26767d9e248250ba4ed2c&ipo=images",
				"https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fimages.pexels.com%2Fphotos%2F3991792%2Fpexels-photo-3991792.jpeg%3Fauto%3Dcompress%26cs%3Dtinysrgb%26h%3D750%26w%3D1260&f=1&nofb=1&ipt=ea924ac3b14d8f3d424975543d9165a19c0f58f739cea23d44cb110ccedfe06c&ipo=images",
				"https://external-content.duckduckgo.com/iu/?u=http%3A%2F%2Fwww.publicdomainpictures.net%2Fpictures%2F190000%2Fvelka%2Fthe-end-title.jpg&f=1&nofb=1&ipt=54b622a92e13962a19ec827c67b50d8e5738ef2b3f91dd2316054a22abd6a34d&ipo=images",
				"https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fimages.pexels.com%2Fphotos%2F1888004%2Fpexels-photo-1888004.jpeg%3Fcs%3Dsrgb%26dl%3Dletters-on-yellow-tiles-forming-the-end-text-1888004.jpg%26fm%3Djpg&f=1&nofb=1&ipt=871760771f4feffe7295a577a694c6c64d73b401127fa43da6c7ee5eaea7039c&ipo=images"
				}.ElementAt( new Random( ).Next( 0, 4 ) );

			return bookmark;
		}
		internal bool UpdateUsernames()
		{
			var success = false;

			var bookmarks = _bookmarkRepository.List("", "", 0, true);
			var usernames = new List<string>( );
			if (bookmarks.Any( ))
			{
				foreach (var bookmark in bookmarks)
				{
					bookmark.SetUsername( );
					_bookmarkRepository.Update( bookmark );

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

		public bool Hydrate(IFormFile dataFile)
		{
			bool success = false;
			var userService = new UserService( _connectionString );

			using (var reader = new StreamReader( dataFile.OpenReadStream( ), System.Text.Encoding.UTF8, true ))
			{
				int currentLine = 0;
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine( );
					currentLine++;

					if (currentLine > 1 && !string.IsNullOrWhiteSpace( line ))
					{
						var bookmarkValues = line.Split( ';' );
						Bookmark bookmark = new Bookmark( );
						bookmark.Guid = bookmarkValues[1];

						var savedBookmark = _bookmarkRepository.Read( bookmark.Guid );
						if (savedBookmark != null) continue;

						bookmark.Url = bookmarkValues[2];
						bookmark.Title = bookmarkValues[3];
						bookmark.Image = bookmarkValues[4];
						bookmark.Savedby = 0;
						if (int.TryParse( bookmarkValues[5], out int savedby ))
						{
							bookmark.Savedby = savedby;
						}
						bookmark.Timestamp = bookmarkValues[6];
						bookmark.Usercontext = bookmarkValues[7];
						bookmark.FullUrl = bookmarkValues[8];
						bookmark.Context1link = bookmarkValues[9];
						bookmark.Context1img = bookmarkValues[10];
						bookmark.Context2link = bookmarkValues[11];
						bookmark.Context2img = bookmarkValues[12];
						bookmark.Context3link = bookmarkValues[13];
						bookmark.Context3img = bookmarkValues[14];
						bookmark.SetUsername( );

						_bookmarkRepository.Create( bookmark );
						userService.IncreaseBookmarkCount( bookmark.Username );
					}
				}
				success = true;
			}

			return success;
		}

		public bool HydrateMockData()
		{
			bool success = false;

			success = _bookmarkRepository.Hydrate( );

			return success;
		}

		internal void UpdateUserCount(List<string> usernames)
		{
			var _userRepository = new UserRepository( _connectionString );
			foreach (var username in usernames)
			{
				var userName = username.Trim( );
				var userRole = UserHelper.GetRole( userName );
				var userCount = _bookmarkRepository.GetCount( userName );

				var storedUser = _userRepository.Read( userName );
				if (storedUser != null)
				{
					storedUser.Name = userName;
					storedUser.Role = userRole;
					storedUser.Count = userCount;

					_userRepository.Update( storedUser );
				}
				else
				{
					var user = new ContentUser( );
					user.Name = userName;
					user.Email = "";
					user.Role = userRole;
					user.Link = "";
					user.Count = userCount;

					_userRepository.Create( user );
				}
			}
		}

		/// <summary>
		/// Loads the 10 Related 
		/// </summary>
		/// <param name="username"></param>
		/// <param name="guid"></param>
		/// <returns></returns>
		public List<Bookmark> GetUsersRelatedBookmarks(string username, string guid)
		{
			var model = new List<Bookmark>();

			model = _bookmarkRepository.GetUsersRelatedBookmarks(username, guid);

			return model;
		}
	}
}
