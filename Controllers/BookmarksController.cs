using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using reffffound.Data;
using reffffound.Models;
using reffffound.Services;
using System.ComponentModel.Design;

namespace reffffound.Controllers
{
	public class BookmarksController : Controller
	{
		//private readonly ApplicationDbContext _context;
		private IBookmarkService _bookmarkService;
		private bool _showUserFunctions;
		private string _connectionString;

		public BookmarksController(ApplicationDbContext context, IConfiguration configuration)
		{
			//_context = context;
			var connectionString = configuration["ConnectionStrings:AzureSqlConnection"] ?? configuration["ConnectionStrings:DataConnection"] ?? "";
			_connectionString = connectionString;

			if (context.Bookmarks != null)
			{
				_bookmarkService = new BookmarkContextService( context );
			}
			else
			{
				_bookmarkService = new BookmarkService( connectionString );
			}

			if (false && configuration != null && !string.IsNullOrWhiteSpace( configuration["ASPNETCORE_ENVIRONMENT"] ))
			{
				_showUserFunctions = configuration["ASPNETCORE_ENVIRONMENT"].Equals( "Development" );
			}
		}

		// GET: Bookmarks/index/1
		public ActionResult Index(int page = 1)
		{
			var bookmarks = _bookmarkService.List( page );

			page = page < 1 ? 1 : page;
			ViewBag.PreviousPage = page > 1 ? page - 1 : 1;
			ViewBag.CurrentPage = page;
			ViewBag.NextPage = page + 1;
			ViewBag.PaginationFirstCss = (page == 1 || page == 0) ? "current" : "";
			ViewBag.Action = "Index";
			ViewBag.Filter = "";
			ViewBag.Username = "";

			if (bookmarks.Count == 0)
			{
				return RedirectToAction( nameof( FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = page } );
			}
			else
			{
				return View( "Index", bookmarks );
			}
		}

		// GET: Bookmarks/5
		public ActionResult Details(string guid, string referAction = "", string referUsername = "", int referPage = 0)
		{
			if (string.IsNullOrWhiteSpace( guid )) return View( "Error" );
			var miv = ModelState.IsValid;

			var bookmark = _bookmarkService.Read( guid );
			if (bookmark == null || string.IsNullOrWhiteSpace( bookmark.Guid )) return View( "Error" );

			if (User.Identity != null && User.Identity.Name != null && User.Identity.IsAuthenticated)
			{
				bool userHasPosted = bookmark.Username.Equals( User.Identity.Name );
				bool isAdminUser = UserManagerHelper.IsAdmin(User.Identity.Name);

				ViewBag.IsAdminUser = isAdminUser;
				ViewBag.ShowUserFunctions = true;
				ViewBag.IdentityUserName = User.Identity.Name;
				ViewBag.ShowEditFunctions = (userHasPosted|| isAdminUser);
				ViewBag.UserHasPosted = userHasPosted;
				ViewBag.UserHasSaved = !ViewBag.UserHasPosted &&
												bookmark.Usercontext.Split( "," ).Any( x => x.Replace( " ", "" ).Equals( User.Identity.Name ) );
			}
			else
			{
				ViewBag.IsAdminUser = ViewBag.ShowUserFunctions = ViewBag.ShowEditFunctions =
				ViewBag.UserHasPosted = ViewBag.UserHasSaved = false;
				ViewBag.IdentityUserName = "";
			}

			ViewBag.ReferAction = referAction;
			ViewBag.ReferUsername = referUsername;
			ViewBag.ReferPage = referPage;
			ViewBag.Username = bookmark.Username;
			ViewBag.Guid = guid;
			ViewBag.Usercontext = bookmark.Usercontext;

			return View( "Detail", bookmark );
		}

		// GET: Bookmarks/List/username/filter/1
		public ActionResult List(string username, string filter = "", int page = 1)
		{
			var usersBookmarks = _bookmarkService.List( username, filter, page );

			if (usersBookmarks.Count == 0)
			{
				return RedirectToAction( nameof( FeedNullFour ), "Bookmarks", new { username = username, filter = filter, page = page } );
			}
			else
			{
				ViewBag.Username = username;
				ViewBag.ShowUserFunctions = User.Identity != null && User.Identity.IsAuthenticated;
				ViewBag.ShowEditFunctions = User.Identity.Name == username;
				ViewBag.IsAdminUser = UserManagerHelper.IsAdmin( User.Identity.Name );

				page = page < 1 ? 1 : page;
				ViewBag.PreviousPage = page > 1 ? page - 1 : 1;
				ViewBag.CurrentPage = page;
				ViewBag.NextPage = page + 1;
				ViewBag.PaginationFirstCss = (page == 1 || page == 0) ? "current" : "";
				ViewBag.Action = "List";
				ViewBag.Filter = filter;

				ViewBag.FeedLinkCss = filter == "feed" ? "currentFilter" : "";
				ViewBag.PostLinkCss = filter == "post" ? "currentFilter" : "";
				ViewBag.FoundLinkCss = filter == "found" ? "currentFilter" : "";

				return View( "List", usersBookmarks );
			}
		}

		public ActionResult FeedNullFour(string username = "", string filter = "", int page = 0)
		{
			var bookmark = _bookmarkService.GetFeedNullFour( username, filter, page );
			page = page < 1 ? 1 : page;
			ViewBag.PreviousPage = page > 1 ? page - 1 : 1;
			ViewBag.CurrentPage = page;
			ViewBag.NextPage = page + 1;

			ViewBag.Username = username;
			ViewBag.NextAction = string.IsNullOrWhiteSpace( username ) ? "Index" : "List";

			return View( "FeedNullFour", bookmark );
		}


		// GET: Bookmarks/Create/Username
		public ActionResult Create(string username)
		{
			if (!UserManagerHelper.CanDo( User, _showUserFunctions )) return RedirectToAction( nameof( FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = 1 } );

			ViewBag.Username = username;
			return View( "Create" );
		}


		// POST: Bookmarks/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(IFormCollection collection)
		{
			if (!UserManagerHelper.CanDo( User, _showUserFunctions )) return RedirectToAction( nameof( FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = 1 } );
			ViewBag.ShowValidationMessage = false;

			try
			{
				var bookmark = new BookmarkHelper( _bookmarkService, null).CreateFrom( collection );
				if (!bookmark.IsValid( out string validationMessage ))
				{
					ViewBag.ShowValidationMessage = true;
					ViewBag.ValidationMessage = validationMessage;
					return View( "Create", bookmark );
				}

				_bookmarkService.Create( bookmark );

				return RedirectToAction( nameof( Details ), "Bookmarks", new { guid = bookmark.Guid } );
			}
			catch
			{
				return View( "Error" );
			}
		}

		// POST: Bookmarks/Save
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Save(IFormCollection collection)
		{
			if (!UserManagerHelper.CanDo( User, false )) return RedirectToAction( nameof( FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = 1 } );

			var guidToSave = collection["Guid"];

			if (!string.IsNullOrWhiteSpace( guidToSave ))
			{
				if (User.Identity.IsAuthenticated)
				{
					_bookmarkService.Save( guidToSave, User.Identity.Name );
				}

				return RedirectToAction( nameof( Details ), "Bookmarks", new { guid = guidToSave } );
			}
			else
			{
				return View( "Error" );
			}
		}
		// POST: Bookmarks/Forget
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Forget(IFormCollection collection)
		{
			if (!UserManagerHelper.CanDo( User, false )) return RedirectToAction( nameof( FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = 1 } );

			var guidToForget = collection["Guid"];

			if (!string.IsNullOrWhiteSpace( guidToForget ))
			{
				if (User.Identity.IsAuthenticated)
				{
					_bookmarkService.Forget( guidToForget, User.Identity.Name );
				}

				return RedirectToAction( nameof( Details ), "Bookmarks", new { guid = guidToForget } );
			}
			else
			{
				return View( "Error" );
			}
		}

		// GET: BookmarkController/Edit/5
		public ActionResult Edit(string guid)
		{
			if (!UserManagerHelper.CanDo( User, _showUserFunctions )) return RedirectToAction( nameof( FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = 1 } );
			if (String.IsNullOrWhiteSpace( guid )) return View( "Error" );

			var bm = _bookmarkService.Read( guid );

			if (bm != null && UserManagerHelper.CanWrite( User, bm.Username ))
			{
				ViewBag.Username = bm.Username;
				return View( "Edit", bm );
			}
			else
			{
				return RedirectToAction( nameof( Details ), "Bookmarks", new { guid } );
			}
		}

		// POST: BookmarkController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(string guid, IFormCollection collection)
		{
			if (!UserManagerHelper.CanDo( User, _showUserFunctions )) return RedirectToAction( nameof( FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = 1 } );

			try
			{
				if (String.IsNullOrWhiteSpace( guid )) return View( "Error" );
				var bookmark = _bookmarkService.Read( guid );
				if (bookmark == null) return View( "Error" );

				if (UserManagerHelper.CanWrite( User, bookmark.Username ))
				{
					bookmark.UpdateFrom( collection );
					if (!bookmark.IsValid( out string validationMessage ))
					{
						ViewBag.ShowValidationMessage = true;
						ViewBag.ValidatioNMessage = validationMessage;

						return View( "Edit", bookmark );
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

		// GET: BookmarkController/Delete/5
		public ActionResult Delete(string guid)
		{
			if (!UserManagerHelper.CanDo( User, _showUserFunctions )) return RedirectToAction( nameof( FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = 1 } );
			var bookmark = _bookmarkService.Read( guid );

			if (bookmark == null)
			{
				return View( "Error" );
			}
			else
			{
				if (UserManagerHelper.CanWrite( User, bookmark.Username ))
				{
					ViewBag.Username = bookmark.Username;
					return View( "Delete", bookmark );
				}
				else
				{
					return RedirectToAction( nameof( Details ), "Bookmarks", new { guid = bookmark.Guid } );
				}
			}
		}

		// POST: BookmarkController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(string guid, IFormCollection collection)
		{
			if (!UserManagerHelper.CanDo( User, _showUserFunctions )) return RedirectToAction( nameof( FeedNullFour ), "Bookmarks", new { username = "", filter = "", page = 1 } );

			var username = collection["Username"];

			if (!string.IsNullOrEmpty( username ))
			{
				try
				{
					_bookmarkService.Delete( guid, username );
					return RedirectToAction( nameof( Index ) );
				}
				catch
				{
					return View( );
				}
			}
			else
			{
				return RedirectToAction( "Delete", "Bookmarks", guid );
			}
		}
	}
}
