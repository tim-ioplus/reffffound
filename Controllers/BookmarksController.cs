using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using reffffound.Data;
using reffffound.Models;

namespace reffffound.Controllers
{
	public class BookmarksController : Controller
	{
		private readonly ApplicationDbContext _context;
		private BookmarkRepository _bookmarkRepository;
		private UserRepository _userRepository;
		private UserService _userService;
		private bool _showUserFunctions;

		public BookmarksController(ApplicationDbContext context, IConfiguration configuration)
		{
			_context = context;
			var connectionString = configuration["ConnectionStrings:AzureSqlConnection"] ?? configuration["ConnectionStrings:DataConnection"] ?? "";
			_bookmarkRepository = new BookmarkRepository(_context, connectionString);
			_userRepository = new UserRepository(_context, connectionString);
			_userService = new UserService(connectionString);

			_showUserFunctions = configuration["ASPNETCORE_ENVIRONMENT"].Equals("Development");
		}

		// GET: Bookmarks/index/1
		public ActionResult Index(int page = 1)
		{
			var bookmarks = _bookmarkRepository.List(page);

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

			var bookmark = _bookmarkRepository.Read(guid);
			if (bookmark == null || string.IsNullOrWhiteSpace(bookmark.Guid)) return View("Error");

			ViewBag.ShowUserFunctions = _showUserFunctions;

			ViewBag.ReferAction = referAction;
			ViewBag.ReferUsername = referUsername;
			ViewBag.ReferPage = referPage;
			ViewBag.Username = bookmark.Username;

			return View("Detail", bookmark);
		}

		// GET: Bookmarks/List/username/filter/1
		public ActionResult List(string username, string filter = "post", int page = 1)
		{
			var usersBookmarks = _bookmarkRepository.List(username, filter, page);

			if (usersBookmarks.Count == 0)
			{
				return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new { username = username, filter = filter, page = page });
			}
			else
			{
				ViewBag.ShowUserFunctions = _showUserFunctions;

				page = page < 1 ? 1 : page;
				ViewBag.PreviousPage = page > 1 ? page - 1 : 1;
				ViewBag.CurrentPage = page;
				ViewBag.NextPage = page + 1;
				ViewBag.PaginationFirstCss = (page == 1 || page == 0) ? "current" : "";
				ViewBag.Action = "List";
				ViewBag.Filter = filter;

				ViewBag.Username = username;
				ViewBag.IsAdminUser = UserHelperService.IsAdmin(username);

				return View("List", usersBookmarks);
			}
		}

		public ActionResult FeedNullFour(string username = "", string filter = "", int page = 0)
		{
			var bookmark = _bookmarkRepository.GetFeedNullFour(username, filter, page);
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
				var bookmark = new Bookmark().FromCollection(collection);
				if (!bookmark.IsValid(out string validationMessage))
				{
					ViewBag.ShowValidationMessage = true;
					ViewBag.ValidationMessage = validationMessage;
					return View("Create", bookmark);
				}

				bookmark.Guid = Guid.NewGuid().ToString();
				bookmark.Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

				_bookmarkRepository.Create(bookmark);

				var user = _userRepository.Read(bookmark.Username);
				user.Count += 1;
				_userRepository.Update(user);

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

			var bm = _bookmarkRepository.Read(guid);
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
				var bookmark = _bookmarkRepository.Read(guid);
				if (bookmark == null) return View("Error");

				var url = collection["Url"][0];
				var title = collection["Title"][0];
				var image = collection["Image"][0];

				if (!string.IsNullOrEmpty(url) && !bookmark.Url.Equals(url))
				{
					bookmark.Url = url;
				}
				if (!string.IsNullOrEmpty(title) && !bookmark.Title.Equals(title))
				{
					bookmark.Title = title;
				}
				if (!string.IsNullOrEmpty(image) && !bookmark.Image.Equals(image))
				{
					bookmark.Image = image;
				}

				if (!bookmark.IsValid(out string validationMessage))
				{
					ViewBag.ShowValidationMessage = true;
					ViewBag.ValidatioNMessage = validationMessage;

					return View("Edit", bookmark);
				}

				_bookmarkRepository.Update(bookmark);

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
			var bookmark = _bookmarkRepository.Read(guid);

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
				_bookmarkRepository.Delete(guid);
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

				using(var reader = new StreamReader(dataFile.OpenReadStream(),System.Text.Encoding.UTF8,true))
				{
					int currentLine = 0;
					while(!reader.EndOfStream)
					{
						var line = reader.ReadLine();
						currentLine++;

						if(currentLine > 1 && !string.IsNullOrWhiteSpace(line))
						{
							var bookmarkValues = line.Split(';');
							Bookmark bookmark = new Bookmark();
							bookmark.Guid = bookmarkValues[1];

							var savedBookmark = _bookmarkRepository.Read(bookmark.Guid);
							if(savedBookmark != null) continue;

							bookmark.Url = bookmarkValues[2];
							bookmark.Title = bookmarkValues[3];
							bookmark.Image = bookmarkValues[4];
							bookmark.Savedby = 0;
							if(int.TryParse(bookmarkValues[5], out int savedby))
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
							bookmark.SetUsername();

							_bookmarkRepository.Create(bookmark);
							_userService.IncreaseBookmarkCount(bookmark.Username);
						}
					}
				}

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
			var success = false;
			var users = _userRepository.List();
			if (users.Any())
			{
				foreach (var user in users)
				{
					int count = _bookmarkRepository.GetCount(user.Name);
					user.Count = count;
					_userRepository.Update(user);
				}

				success = true;
			}

			if (success)
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

			var bookmarks = _bookmarkRepository.ListAll();
			var usernames = new List<string>();
			if (bookmarks.Any())
			{
				foreach (var bookmark in bookmarks)
				{
					bookmark.SetUsername();
					_bookmarkRepository.Update(bookmark);

					if(!usernames.Contains(bookmark.Username))
					{
						usernames.Add(bookmark.Username);
					}
				}

				foreach (var username in usernames)
				{
					var userName = username.Trim();
					var userRole = UserHelperService.GetRole(userName);
					var userCount = _bookmarkRepository.GetCount(userName);

					var storedUser = _userRepository.Read(userName);
					if (storedUser != null)
					{
						storedUser.Name = userName;
						storedUser.Role = userRole;
						storedUser.Count = userCount;

						_userRepository.Update(storedUser);
					}
					else
					{
						var user = new ContentUser();
						user.Name = userName;
						user.Email = "";
						user.Role = userRole;
						user.Link =	"";
						user.Count = userCount;

						_userRepository.Create(user);
					}
				}

				success = true;
			}

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

			bool success = _bookmarkRepository.Hydrate();
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
