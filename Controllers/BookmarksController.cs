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
    private bool _showUserFunctions;

    public BookmarksController(ApplicationDbContext context, IConfiguration configuration)
    {
      _context = context;
      var connectionString = configuration["ConnectionStrings:AzureSqlConnection"] ?? configuration["ConnectionStrings:DataConnection"] ?? "";
      _bookmarkRepository = new BookmarkRepository(_context, connectionString);
      _userRepository = new UserRepository(_context, connectionString);

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

      if (bookmarks.Count == 0)
      {
        return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new {username="", filter="", page=page });
      }
      else
      {
        return View("Index", bookmarks);
      }
    }

    // GET: Bookmarks/5
    public ActionResult Details(string guid, string referAction ="", string referUsername="", int referPage=0)
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
        return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new {username=username, filter=filter, page=page });
      }
      else
      {
        ViewBag.ShowUserFunctions = _showUserFunctions;

        page = page < 1 ? 1 : page; 
        ViewBag.PreviousPage = page > 1 ? page - 1 : 1;
        ViewBag.CurrentPage = page;
        ViewBag.NextPage = page + 1;
        ViewBag.PaginationFirstCss = (page == 1 || page == 0) ? "current" : "";

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
      if(!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new {username="", filter="", page=1 });

      ViewBag.Username = username;
      return View("Create");
    }


    // POST: Bookmarks/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(IFormCollection collection)
    {
      if(!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new {username="", filter="", page=1 });
      ViewBag.ShowValidationMessage = false;

      try
      {
        var bookmark = new Bookmark().FromCollection(collection);
        if(!bookmark.IsValid(out string validationMessage))
        {
          ViewBag.ShowValidationMessage = true;
          ViewBag.ValidatioNMessage = validationMessage;
          return View("Create", bookmark);
        }

        bookmark.Guid = Guid.NewGuid().ToString();
        bookmark.Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        _bookmarkRepository.Create(bookmark);

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
      if(!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new {username="", filter="", page=1 });
        if(String.IsNullOrWhiteSpace(guid)) return View("Error");

        var bm = _bookmarkRepository.Read(guid);
        ViewBag.Username = bm.Username;

        return View("Edit", bm);
    }

    // POST: BookmarkController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(string guid, IFormCollection collection)
    {
      if(!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new {username="", filter="", page=1 });
        try
        {
            if(String.IsNullOrWhiteSpace(guid)) return View("Error");
            var bookmark = _bookmarkRepository.Read(guid);
            if(bookmark == null) return View("Error");

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

            if(!bookmark.IsValid(out string validationMessage))
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
      if(!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new {username="", filter="", page=1 });
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
      if(!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new {username="", filter="", page=1 });
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
      if(!_showUserFunctions) return RedirectToAction(nameof(FeedNullFour), "Bookmarks", new {username="", filter="", page=1 });
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
    
  }
}
