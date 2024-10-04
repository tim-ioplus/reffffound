using Microsoft.AspNetCore.Mvc;
using reffffound.Services;

namespace reffffound.Components.UsersRelatedViewComponent
{
	public class UsersRelatedViewComponent : ViewComponent
	{
		private IBookmarkService _bookmarkService;

		public UsersRelatedViewComponent(IBookmarkService bookmarkService)
		{
			_bookmarkService = bookmarkService;
		}

		public IViewComponentResult Invoke()
		{
			var model = _bookmarkService.GetUsersRelatedBookmarks( ViewBag.Username + "", ViewBag.Guid + "" );

			return View( "Default", model );
		}
	}
}
