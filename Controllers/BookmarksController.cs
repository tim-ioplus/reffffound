using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Versioning;
using reffffound.Data;
using reffffound.Models;

namespace reffffound.Controllers
{
    public class BookmarksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private BookmarkRepository _bookmarkRepository;
        private UserRepository _userRepository;

        public BookmarksController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            var connectionString = configuration["ConnectionStrings:DataConnection"] ?? "";
            _bookmarkRepository = new BookmarkRepository(_context, connectionString);
            _userRepository = new UserRepository(_context, connectionString);
        }
        // GET: Bookmarks/index/1
        public ActionResult Index(int page = 1)
        {
            var bookmarks = _bookmarkRepository.List(page);

            // 
            // ViewBag.Pagination
            
            return View("Index", bookmarks);
        }

        // GET: Bookmarks/5
        public ActionResult Details(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid)) return View("Error");
            var miv = ModelState.IsValid;

            var bookmark = _bookmarkRepository.Read(guid);
            if (bookmark == null || string.IsNullOrWhiteSpace(bookmark.Guid)) return View("Error");
            
            return View("Detail", bookmark);
        }

    /*
        // GET: Bookmarks/List/Username/1
        public ActionResult List(string username, int page)
        {
            var usersBookmarks = _bookmarkRepository.List(username, page);
            ViewBag.Username = username;
            return View("List", usersBookmarks);
        }
    */

        // GET: Bookmarks/List/Username/filter/1
        public ActionResult List(string username, string filter, int page)
        {
            var usersBookmarks = _bookmarkRepository.List(username, filter, page);
            ViewBag.Username = username;
            return View("List", usersBookmarks);
        }

        // GET: Bookmarks/Create/Username
        public ActionResult Create(string username)
        {
            ViewBag.Username = username;
            return View("Create");
        }


        // POST: Bookmarks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                var cks = this.Request.Cookies;

                var url = collection["Url"][0];
                var title = collection["Title"][0];
                var image = collection["Image"][0];
                var usercontext = collection["Usercontext"][0];
                
                var bookmark = new Bookmark
                {
                    Guid = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Url = url,
                    Title = title,
                    Image = image, 
                    Usercontext = usercontext,
                    Savedby = 1
                };

                bookmark = _bookmarkRepository.AddContext(bookmark);
                _bookmarkRepository.Create(bookmark);
                
                 return RedirectToAction(nameof(Details),"Bookmarks", new { guid = bookmark.Guid});
            }
            catch
            {
                return View("Error");
            }
        }

        // GET: BookmarksController/Hydrate
        public ActionResult Hydrate()
        {
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

        /*
        // GET: BookmarkController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BookmarkController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BookmarkController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BookmarkController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        */
    }
}
