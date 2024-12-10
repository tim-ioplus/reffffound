using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using reffffound.Data;
using reffffound.Services;

namespace reffffound.Controllers
{
	public class BookmarksHelperController : Controller
	{
		private IBookmarkService _bookmarkService;
		private IUserService _userService;
		private string _connectionString;
		private bool _showUserFunctions;
		private ApplicationDbContext _context;

		public BookmarksHelperController(ApplicationDbContext context, IConfiguration configuration)
		{
			_context = context;
			var connectionString = configuration["ConnectionStrings:AzureSqlConnection"] ?? configuration["ConnectionStrings:DataConnection"] ?? "";
			_connectionString = connectionString;

			if (context != null && context.Bookmarks.Any( ) && context.ContentUsers.Any( ))
			{
				_bookmarkService = new BookmarkContextService( context );
				_userService = new UserContextService( context );
			}
			else
			{
				_bookmarkService = new BookmarkService( connectionString );
			}

			_showUserFunctions = configuration["ASPNETCORE_ENVIRONMENT"].Equals( "Development" );
		}

		// GET: BookmarksController/Hydrate
		public ActionResult Hydrate()
		{
			if (!_showUserFunctions) return RedirectToAction( nameof( BookmarksController.FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = 1 } );
			ViewBag.ValidationMessage = "";

			return View( "Hydrate" );
		}

		// POST: BookmarkController/Hydrate
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Hydrate(IFormFile dataFile)
		{
			if (!_showUserFunctions) return RedirectToAction( nameof( BookmarksController.FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = 1 } );
			ViewBag.ValidationMessage = "";

			try
			{
				if (dataFile == null)
				{
					if (this.HttpContext.Request.Form.Files.Any( ) && this.HttpContext.Request.Form.Files[0] != null)
					{
						dataFile = this.HttpContext.Request.Form.Files[0];
					}
					else
					{
						ViewBag.ValidationMessage = "Please provide a datafile.";
						return View( "Hydrate" );
					}
				}

				var bookmarks = new BookmarkHelper( _bookmarkService, _userService ).ReadFromFile( dataFile );
				_bookmarkService.Create( bookmarks );

				return RedirectToAction( "Index", "Bookmarks" );
			}
			catch
			{
				return View( "Error" );
			}
		}


		// GET: BookmarksController/UpdateUsercounts
		public ActionResult UpdateUsercounts()
		{
			if (!_showUserFunctions) return RedirectToAction( nameof( BookmarksController.FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = 1 } );

			if (new BookmarkHelper( _bookmarkService, _userService ).UpdateUsercounts( ))
			{
				return RedirectToAction( "Index", "Bookmarks" );
			}
			else
			{
				return View( "Error" );
			}
		}

		// GET: BookmarksController/UpdateUsernames
		public ActionResult UpdateUsernames()
		{
			if (!_showUserFunctions) return RedirectToAction( nameof( BookmarksController.FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = 1 } );
			var success = false;

			success = new BookmarkHelper( _bookmarkService, _userService ).UpdateUsernames( );

			if (success)
			{
				return RedirectToAction( "Index", "Bookmarks" );
			}
			else
			{
				return View( "Error" );
			}
		}

		// GET: BookmarksController/HydrateMockData
		public ActionResult HydrateMockData()
		{
			if (!_showUserFunctions) return RedirectToAction( nameof( BookmarksController.FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = 1 } );

			bool success = false;// _bookmarkService.HydrateMockData();
			if (success)
			{
				return RedirectToAction( "Index", "Bookmarks" );
			}
			else
			{
				return View( "Error" );
			}
		}


		// GET: BookmarksController/MigrateContentUsers
		public ActionResult MigrateContentUsers()
		{
			var success = false;

			var userRepository = new UserRepository( _connectionString );
			var users = userRepository.List( );
			foreach (var item in users)
			{
				if (!_context.ContentUsers.Any( cu => cu.Name.Equals( item.Name ) ))
				{
					_context.ContentUsers.Add( item );
				}
			}

			_context.SaveChanges( );

			int checkedCount = 0;
			foreach (var item in users)
			{
				if (_context.ContentUsers.Any( cu => cu.Name.Equals( item.Name ) && cu.Count.Equals( item.Count ) ))
				{
					checkedCount++;
				}
			}

			success = checkedCount == users.Count;

			if (success)
			{
				return RedirectToAction( "Index", "Bookmarks" );
			}
			else
			{
				return View( "Error" );
			}
		}

		// GET: BookmarksController/MigrateBookmarks
		public ActionResult MigrateBookmarks()
		{
			var success = false;
			var bookmarkService = new BookmarkService( _connectionString );
			var bookmarks = bookmarkService.ListAll( );


			foreach (var dbBookmark in bookmarks)
			{
				if (!_context.Bookmarks.Any( cu => cu.Guid.Equals( dbBookmark.Guid ) ))
				{
					_context.Bookmarks.Add( dbBookmark );
				}
			}

			_context.SaveChanges( );

			int checkedCount = 0;
			foreach (var dbBookmark in bookmarks)
			{
				foreach (var ctxBookmark in _context.Bookmarks)
				{
					if (dbBookmark.DataEquals( ctxBookmark ))
					{
						checkedCount++;
						continue;
					}
				}
			}

			success = checkedCount == bookmarks.Count;

			if (success)
			{
				return RedirectToAction( "Index", "Bookmarks" );
			}
			else
			{
				return View( "Error" );
			}
		}


		// GET: BookmarksHelperController/AdminEdit/5
		public ActionResult AdminEdit(string guid)
		{
			if (!UserManagerHelper.CanDo( User, _showUserFunctions )) return RedirectToAction( "FeedNullFour", "Bookmarks", new { username = "", filter = "", page = 1 } );
			if (String.IsNullOrWhiteSpace( guid )) return View( "Error" );
			ViewBag.ShowValidationMessage = false;
			ViewBag.ValidationMessage = "";

			var bm = _bookmarkService.Read( guid );

			if (bm != null && UserManagerHelper.IsAdmin(User.Identity.Name))
			{
				ViewBag.Username = bm.Username;
				ViewBag.AllUsernames = _userService.ListUsernames();
				return View( "AdminEdit", bm );
			}
			else
			{
				return RedirectToAction( nameof( Details ), "Bookmarks", new { guid } );
			}
		}

		// POST: BookmarkController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AdminEdit(string guid, IFormCollection collection)
		{
			if (!UserManagerHelper.CanDo( User, _showUserFunctions )) return RedirectToAction( "FeedNullFour", "Bookmarks", new { username = "", filter = "", page = 1 } );
			ViewBag.ShowValidationMessage = false;
			ViewBag.ValidationMessage = "";

			try
			{
				if (String.IsNullOrWhiteSpace( guid )) return View( "Error" );
				var bookmark = _bookmarkService.Read( guid );
				if (bookmark == null) return View( "Error" );

				if (UserManagerHelper.CanWrite( User, bookmark.Username ))
				{
					bookmark.UpdateFrom( collection, true );
					if (!bookmark.IsValid( out string validationMessage ))
					{
						ViewBag.ShowValidationMessage = true;
						ViewBag.ValidationMessage = validationMessage;

						return View( "AdminEdit", bookmark );
					}

					_bookmarkService.Update( bookmark );
				}

				return RedirectToAction( nameof( Details ), "Bookmarks", new { guid = bookmark.Guid } );
			}
			catch
			{
				return View( );
			}
		}

		// GET: BookmarksHelperController/AdminMenu
		public ActionResult AdminMenu()
		{
			if (!UserManagerHelper.IsAdmin(User.Identity.Name)) return RedirectToAction( "FeedNullFour", "Bookmarks", new { username = "", filter = "", page = 1 } );

			return View();
		}
		
	}
}
