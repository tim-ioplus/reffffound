using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using reffffound.Data;
using reffffound.Models;
using reffffound.Services;

namespace reffffound.Controllers
{
	public class BookmarksController : Controller
	{
		private readonly ApplicationDbContext _context;
		private BookmarkService _bookmarkService;
		private bool _showUserFunctions;

		public BookmarksController(ApplicationDbContext context, IConfiguration configuration)
		{
			_context = context;
			var connectionString = configuration["ConnectionStrings:AzureSqlConnection"] ?? configuration["ConnectionStrings:DataConnection"] ?? "";

			_bookmarkService = new BookmarkService(connectionString);

			_showUserFunctions = configuration["ASPNETCORE_ENVIRONMENT"].Equals("Development");
		}

		// GET: Bookmarks/index/1
		public ActionResult Index(int page = 1)
		{
			var bookmarks = _bookmarkService.List(page);

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
				return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = "", filter = "", page = page });
			}
			else
			{
				return View("Index", bookmarks);
			}
		}

		// GET: Bookmarks/5
		public ActionResult Details(string guid, string referAction = "", string referUsername = "", int referPage = 0)
		{
			if (string.IsNullOrWhiteSpace(guid)) return View("Error");
			var miv = ModelState.IsValid;

			var bookmark = _bookmarkService.Read(guid);
			if (bookmark == null || string.IsNullOrWhiteSpace(bookmark.Guid)) return View("Error");

			ViewBag.ShowUserFunctions = _showUserFunctions;

			ViewBag.ReferAction = referAction;
			ViewBag.ReferUsername = referUsername;
			ViewBag.ReferPage = referPage;
			ViewBag.Username = bookmark.Username;
			ViewBag.Guid = guid;

			return View("Detail", bookmark);
		}

		// GET: Bookmarks/List/username/filter/1
		public ActionResult List(string username, string filter = "post", int page = 1)
		{
			var usersBookmarks = _bookmarkService.List(username, filter, page);

			if (usersBookmarks.Count == 0)
			{
				return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = username, filter = filter, page = page });
			}
			else
			{
				ViewBag.ShowUserFunctions = User != null && User.Identity.IsAuthenticated && User.Identity.Name.Equals(username);

				page = page < 1 ? 1 : page;
				ViewBag.PreviousPage = page > 1 ? page - 1 : 1;
				ViewBag.CurrentPage = page;
				ViewBag.NextPage = page + 1;
				ViewBag.PaginationFirstCss = (page == 1 || page == 0) ? "current" : "";
				ViewBag.Action = "List";
				ViewBag.Filter = filter;

				ViewBag.Username = username;
				ViewBag.IsAdminUser = UserHelper.IsAdmin(username);

				return View("List", usersBookmarks);
			}
		}

		public ActionResult FeedNullFour(string username = "", string filter = "", int page = 0)
		{
			var bookmark = _bookmarkService.GetFeedNullFour(username, filter, page);
			page = page < 1 ? 1 : page;
			ViewBag.PreviousPage = page > 1 ? page - 1 : 1;
			ViewBag.CurrentPage = page;
			ViewBag.NextPage = page + 1;

			ViewBag.Username = username;
			ViewBag.NextAction = string.IsNullOrWhiteSpace(username) ? "Index" : "List";

			return View("FeedNullFour", bookmark);
		}


		// GET: Bookmarks/Create/Username
		public ActionResult Create(string username)
		{
			if (!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = "", filter = "", page = 1 });

			ViewBag.Username = username;
			return View("Create");
		}


		// POST: Bookmarks/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(IFormCollection collection)
		{
			if (!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = "", filter = "", page = 1 });
			ViewBag.ShowValidationMessage = false;

			try
			{
				
				var bookmark = new Bookmark().CreateFrom(collection);
				if (!bookmark.IsValid(out string validationMessage))
				{
					ViewBag.ShowValidationMessage = true;
					ViewBag.ValidationMessage = validationMessage;
					return View("Create", bookmark);
				}

				_bookmarkService.Create(bookmark);

				return RedirectToAction(nameof(Details), "Bookmarks", new { guid = bookmark.Guid });
			}
			catch
			{
				return View("Error");
			}
		}

		// GET: BookmarkController/Edit/5
		public ActionResult Edit(string guid)
		{
			if (!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = "", filter = "", page = 1 });
			if (String.IsNullOrWhiteSpace(guid)) return View("Error");

			var bm = _bookmarkService.Read(guid);
			ViewBag.Username = bm.Username;

			return View("Edit", bm);
		}

		// POST: BookmarkController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(string guid, IFormCollection collection)
		{
			if (!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = "", filter = "", page = 1 });
			try
			{
				if (String.IsNullOrWhiteSpace(guid)) return View("Error");
				var bookmark = _bookmarkService.Read(guid);
				if (bookmark == null) return View("Error");

				bookmark.UpdateFrom(collection);
				if (!bookmark.IsValid(out string validationMessage))
				{
					ViewBag.ShowValidationMessage = true;
					ViewBag.ValidatioNMessage = validationMessage;

					return View("Edit", bookmark);
				}

				_bookmarkService.Update(bookmark);

				return RedirectToAction(nameof(Details), "Bookmarks", new { guid = bookmark.Guid });
			}
			catch
			{
				return View();
			}
		}

		// GET: BookmarkController/Delete/5
		public ActionResult Delete(string guid)
		{
			if (!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = "", filter = "", page = 1 });
			var bookmark = _bookmarkService.Read(guid);

			if (bookmark == null)
			{
				return View("Error");
			}
			else
			{
				ViewBag.Username = bookmark.Username;
				return View("Delete", bookmark);
			}
		}

		// POST: BookmarkController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(string guid, IFormCollection collection)
		{
			if (!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = "", filter = "", page = 1 });
			try
			{
				_bookmarkService.Delete(guid);
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: BookmarksController/Hydrate
		public ActionResult Hydrate()
		{
			if (!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = "", filter = "", page = 1 });
			ViewBag.ValidationMessage = "";

			return View("Hydrate");
		}

		// POST: BookmarkController/Hydrate
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Hydrate(IFormFile dataFile)
		{
			if (!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = "", filter = "", page = 1 });
			ViewBag.ValidationMessage = "";

			try
			{
				if(dataFile == null)
				{
					if(this.HttpContext.Request.Form.Files.Any() && this.HttpContext.Request.Form.Files[0] != null)
					{
						dataFile = this.HttpContext.Request.Form.Files[0];
					}
					else
					{
						ViewBag.ValidationMessage = "Please provide a datafile.";
						return View("Hydrate");
					}
				}

				_bookmarkService.Hydrate(dataFile);

				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View("Error");
			}
		}

		#region Helper

		// GET: BookmarksController/UpdateUsercounts
		public ActionResult UpdateUsercounts()
		{
			if (!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = "", filter = "", page = 1 });

			if (_bookmarkService.UpdateUsercounts())
			{
				return RedirectToAction(nameof(Index), "Bookmarks");
			}
			else
			{
				return View("Error");
			}
		}

		// GET: BookmarksController/UpdateUsernames
		public ActionResult UpdateUsernames()
		{
			if (!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = "", filter = "", page = 1 });
			var success = false;

			success = _bookmarkService.UpdateUsernames();

			if (success)
			{
				return RedirectToAction(nameof(Index), "Bookmarks");
			}
			else
			{
				return View("Error");
			}
		}

		// GET: BookmarksController/HydrateMockData
		public ActionResult HydrateMockData()
		{
			if (!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = "", filter = "", page = 1 });

			bool success = _bookmarkService.HydrateMockData();
			if (success)
			{
				return RedirectToAction(nameof(Index), "Bookmarks");
			}
			else
			{
				return View("Error");
			}
		}

		#endregion

	}
}
