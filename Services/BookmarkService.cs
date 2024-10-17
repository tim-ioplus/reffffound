using Microsoft.Data.SqlClient;
using NuGet.Versioning;
using reffffound.Data;
using reffffound.Models;
using System.Drawing.Imaging;

namespace reffffound.Services
{
	public class BookmarkService : IBookmarkService
	{
		private string _connectionString;
		private BookmarkRepository _bookmarkRepository;

		[Obsolete]
		public BookmarkService(IConfiguration configuration)
		{
			_connectionString = configuration["ConnectionStrings:AzureSqlConnection"] ?? configuration["ConnectionStrings:DataConnection"] ?? "";
			_bookmarkRepository = new BookmarkRepository( _connectionString );
		}
		public BookmarkService(string connectionString)
		{
			_connectionString = connectionString;
			_bookmarkRepository = new BookmarkRepository( _connectionString );
		}

		#region CRUD Methods
		public void Create(Bookmark bookmark)
		{
			_Create( bookmark );
			new UserService( _connectionString ).IncreaseBookmarkCount( bookmark.Username );
		}

		private void _Create(Bookmark bookmark)
		{
			try
			{
				_bookmarkRepository.Create( bookmark );
			}
			catch (Exception)
			{
				throw;
			}
		}
		public Bookmark? Read(string guid)
		{
			try
			{
				var bookmark = _bookmarkRepository.Read( guid );
				return bookmark;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void Update(Bookmark bookmark)
		{
			try
			{
				_bookmarkRepository.Update( bookmark );
			}
			catch (Exception)
			{
				throw;
			}
		}
		public void Delete(string guid, string username = "")
		{
			try
			{
				Delete( guid );
				new UserService( _connectionString ).DecreaseBookmarkCount( username );
			}
			catch (Exception)
			{
				throw;
			}
		}
		public void Delete(string guid)
		{
			try
			{
				_bookmarkRepository.Delete( guid );
			}
			catch (Exception)
			{
				throw;
			}
		}

		#endregion

		#region List Methods 

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public List<Bookmark> ListAll()
		{
			var bookmarks = _bookmarkRepository.List( "", "", 0, true );
			return bookmarks;
		}

		/// <summary>
		/// Listing the 10 Bookmarks on the given page
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		public List<Bookmark> List(int page)
		{
			return List( "", "", page );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		/// <param name="filter"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		public List<Bookmark> List(string username = "", string filter = "post", int page = 1)
		{
			var bookmarks = _bookmarkRepository.List( username, filter, page );
			AddContext( bookmarks );

			return bookmarks;
		}

		#endregion

		public List<Bookmark> AddContext(List<Bookmark> bookmarks)
		{
			var usedGuidsByUser = new Dictionary<string, List<string>>( );

			foreach (var bookmark in bookmarks)
			{
				var usedGuidsForUser = usedGuidsByUser.GetValueOrDefault( bookmark.Username );

				usedGuidsForUser ??= [];
				AddContext( bookmark, usedGuidsForUser );

				if (!string.IsNullOrWhiteSpace( bookmark.Context1link )) usedGuidsForUser.Add( bookmark.Context1link );
				if (!string.IsNullOrWhiteSpace( bookmark.Context2link )) usedGuidsForUser.Add( bookmark.Context2link );
				if (!string.IsNullOrWhiteSpace( bookmark.Context3link )) usedGuidsForUser.Add( bookmark.Context3link );

				usedGuidsByUser[bookmark.Username] = usedGuidsForUser;
			}

			return bookmarks;
		}

		public Bookmark AddContext(Bookmark bookmark, List<string>? usedGuids = null)
		{
			var user = bookmark.Usercontext.Replace( " ", "" ).Split( "," ).First( );
			var contextBookmarks = _bookmarkRepository.ReadThreeContextBookmarks( user, bookmark.Timestamp, usedGuids );

			if (contextBookmarks.Count >= 1)
			{
				bookmark.Context1img = contextBookmarks.ElementAt( 0 ).Image;
				bookmark.Context1link = contextBookmarks.ElementAt( 0 ).Guid ?? "";
			}

			if (contextBookmarks.Count >= 2)
			{
				bookmark.Context2img = contextBookmarks.ElementAt( 1 ).Image ?? "";
				bookmark.Context2link = contextBookmarks.ElementAt( 1 ).Guid ?? "";
			}

			if (contextBookmarks.Count == 3)
			{
				bookmark.Context3img = contextBookmarks.ElementAt( 2 ).Image ?? "";
				bookmark.Context3link = contextBookmarks.ElementAt( 2 ).Guid ?? "";
			}

			return bookmark;
		}

		public int GetPageCount(string username)
		{
			return GetBookmarkCount( username ) / 10;
		}

		public int GetBookmarkCount(string username)
		{
			return _bookmarkRepository.GetCount( username );
		}

		public List<Bookmark> GetUsersRelatedBookmarks(string username, string guid)
		{
			//return [];//new List<Bookmark>();
			throw new NotImplementedException( );
		}

		public void Save(string guidToSave, string name)
		{
			throw new NotImplementedException( );
		}

		public void Forget(string guidToForget, string name)
		{
			throw new NotImplementedException( );
		}

		public IDictionary<string, List<Bookmark>> GetUsersContextBookmarks(string username, string usercontext, string activeIdentityUserName)
		{
			throw new NotImplementedException( );
		}
	}
}
