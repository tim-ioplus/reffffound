using Microsoft.AspNetCore.Mvc;
using reffffound.Data;
using reffffound.Services;
using Microsoft.AspNetCore.Identity;

namespace reffffound.Components.UsersContextViewComponent
{
	public class UsersContextViewComponent : ViewComponent
	{
		private IBookmarkService _bookmarkService;

		public UsersContextViewComponent(ApplicationDbContext applicationDbContext)
		{
			_bookmarkService = new BookmarkContextService( applicationDbContext );
		}

		public IViewComponentResult Invoke()
		{
			var usercontext = ViewBag.Usercontext;
			var model = _bookmarkService.GetUsersContextBookmarks( ViewBag.Username, ViewBag.Usercontext, ViewBag.IdentityUserName);

			return View( "Default", model );
		}
	}
}
