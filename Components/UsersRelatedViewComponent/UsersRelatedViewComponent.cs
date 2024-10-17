using Microsoft.AspNetCore.Mvc;
using reffffound.Data;
using reffffound.Services;

namespace reffffound.Components.UsersRelatedViewComponent
{
	public class UsersRelatedViewComponent : ViewComponent
	{
		private IBookmarkService _bookmarkService;

		public UsersRelatedViewComponent(ApplicationDbContext applicationDbContext)
		{
			_bookmarkService = new BookmarkContextService( applicationDbContext );
		}

		public IViewComponentResult Invoke()
		{
			var model = _bookmarkService.GetUsersRelatedBookmarks( ViewBag.Username + "", ViewBag.Guid + "" );

			return View( "Default", model );
		}
	}
}
