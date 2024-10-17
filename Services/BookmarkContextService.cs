using Microsoft.EntityFrameworkCore;
using reffffound.Data;
using reffffound.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using System.ComponentModel.DataAnnotations;
using NuGet.Packaging.Signing;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.CodeAnalysis;

namespace reffffound.Services
{
	public class BookmarkContextService : IBookmarkService
	{
		private readonly ApplicationDbContext _context;
		private DbSet<Bookmark> _data;
		public BookmarkContextService(ApplicationDbContext context)
		{
			_context = context;
			_data = _context.Bookmarks;
		}

		public void Create(Bookmark bookmark)
		{
			_data.Add( bookmark );

			var contentUser = _context.ContentUsers.SingleOrDefault( cu => cu.Name.Equals( bookmark.Username ) );
			contentUser.Count++;
			_context.ContentUsers.Update( contentUser );

			_context.SaveChanges( );
		}
		public Bookmark? Read(string guid)
		{
			var bookmark = _data.FirstOrDefault( x => x.Guid == guid );
			return bookmark;
		}

		public void Update(Bookmark bookmark)
		{
			_data.Update( bookmark );
			_context.SaveChanges( );
		}

		public void Delete(string guid)
		{
			Bookmark toDelete = _data.SingleOrDefault( x => x.Guid == guid );
			if (toDelete != null)
			{
				_data.Remove( toDelete );
				_context.SaveChanges( );
			}
		}
		public void Delete(string guid, string username = "")
		{
			var toDelete = Read( guid );

			var contentUserNames = toDelete.Usercontext.Replace( " ", "" ).Split( "," ).ToList( );
			var userService = new UserContextService( _context );
			contentUserNames.ForEach( un => userService.DecreaseBookmarkCount( un ) );

			Delete( guid );

		}
		public List<Bookmark> ListAll()
		{
			return _data.ToList( );
		}

		public List<Bookmark> List(int page)
		{
			return List( "", "", page );
		}
		public List<Bookmark> List(string username, string filter = "post", int page = 1)
		{
			var bookmarks = new List<Bookmark>( );

			int skip = (page - 1) * 10;

			if (!string.IsNullOrEmpty( username ))
			{
				if (filter.Equals( "post" ))
				{
					bookmarks = _data.Where( b => b.Username.Equals( username ) )
						.OrderByDescending( b => b.Timestamp ).Skip( skip ).Take( 10 ).ToList( );
				}
				else if (filter.Equals( "found" ))
				{
					bookmarks = _data.Where( b => !b.Username.Equals( username ) && !b.Usercontext.Contains( username ) )
						.OrderByDescending( b => b.Timestamp ).Skip( skip ).Take( 10 ).ToList( );
				}
			}
			else
			{
				bookmarks = _data.OrderByDescending( b => b.Timestamp ).Skip( skip ).Take( 10 ).ToList( );
			}

			AddContext( bookmarks );

			return bookmarks;
		}

		public int GetBookmarkCount(string username)
		{
			int count = 0;
			if (string.IsNullOrWhiteSpace( username ))
			{
				count = _data.Count( );
			}
			else
			{
				count = _data.Count( x => x.Username == username );
			}

			return count;
		}

		public int GetPageCount(string username)
		{
			int bookmarkCount = GetBookmarkCount( username );
			int count = (int)Math.Ceiling( bookmarkCount / 10.0 );

			return count;
		}

		public List<Bookmark> GetUsersRelatedBookmarks(string username, string guid)
		{
			var usersBookmarks = _data.Where( b => b.Username.Equals( username ) && !b.Guid.Equals( guid ) );
			var lastFiveBookmarks = usersBookmarks.OrderByDescending( b => b.Timestamp ).Take( 5 ).ToList( );
			var randomFiveBookmarks = usersBookmarks.Where( b => !lastFiveBookmarks.Contains( b ) ).OrderBy( b => EF.Functions.Random( ) ).Take( 5 ).ToList( );

			var result = new List<Bookmark>( );
			result.AddRange( lastFiveBookmarks );
			result.AddRange( randomFiveBookmarks );

			return result;
		}


		public IDictionary<string, List<Bookmark>> GetUsersContextBookmarks(string username, string usercontext, string activeIdentityUserName)
		{
			var contextBookmarks = new Dictionary<string, List<Bookmark>>( );
			var userContextNames = usercontext.Replace( " ", "" ).Split( ',' ).Where( u => u != username && u != activeIdentityUserName).Take(10).ToList( );

			foreach (var userContextName in userContextNames)
			{
				var bookmarks = _data.Where( b => b.Username.Equals( userContextName ) )
					.OrderByDescending( b => b.Timestamp ).Take( 5 ).ToList( );
				contextBookmarks.Add( userContextName, bookmarks );
			}

			return contextBookmarks;
		}

		private void AddContext(List<Bookmark> bookmarks)
		{
			var contextualisedBookmarks = new List<Bookmark>( );
			var bookmarksByUsers = bookmarks.GroupBy( b => b.Username ).ToDictionary( b => b.Key, b => b.ToList( ) );

			foreach (var usersBookmarkGroup in bookmarksByUsers)
			{
				var username = usersBookmarkGroup.Key;
				var usersBookmark = usersBookmarkGroup.Value;
				int maxContextLinks = usersBookmark.Count * 3;

				var data = _data.Where( b => b.Username.Equals( username ) && !usersBookmark.Contains( b ) )
					.OrderBy( b => EF.Functions.Random( ) )
					.Take( maxContextLinks )
					.ToList( );

				Stack<Bookmark> stack = new Stack<Bookmark>( );
				foreach (var bookmark in data)
				{
					stack.Push( bookmark );
				}

				foreach (var bookmark in usersBookmark)
				{
					if (stack.Count == 0) break;
					var context1 = stack.Pop( );
					if (context1 == null) break;

					bookmark.Context1img = context1.Image;
					bookmark.Context1link = context1.Guid;
				}

				foreach (var bookmark in usersBookmark)
				{
					if (stack.Count == 0) break;
					var context2 = stack.Pop( );
					if (context2 == null) break;

					bookmark.Context2img = context2.Image;
					bookmark.Context2link = context2.Guid;
				}

				foreach (var bookmark in usersBookmark)
				{
					if (stack.Count == 0) break;
					var context3 = stack.Pop( );
					if (context3 == null) break;

					bookmark.Context3img = context3.Image;
					bookmark.Context3link = context3.Guid;
				}

				contextualisedBookmarks.AddRange( usersBookmark );
			}

			bookmarks = contextualisedBookmarks.OrderByDescending( b => b.Timestamp ).ToList( );
		}

		public void Save(string guidToSave, string username)
		{
			var bookmarkToSave = Read( guidToSave );
			if (bookmarkToSave != null)
			{
				bookmarkToSave.Savedby += 1;
				bookmarkToSave.Usercontext += ", " + username;

				Update( bookmarkToSave );

				new UserContextService( _context ).IncreaseBookmarkCount( username );
			}
		}

		public void Forget(string guidToForget, string username)
		{
			var bookmarkToForget = Read( guidToForget );
			if (bookmarkToForget != null)
			{
				bookmarkToForget.Savedby -= 1;
				bookmarkToForget.Usercontext = bookmarkToForget.Usercontext.Replace( ", " + username, "", true, System.Globalization.CultureInfo.InvariantCulture );

				Update( bookmarkToForget );

				new UserContextService( _context ).DecreaseBookmarkCount( username );
			}
		}

	}
}
