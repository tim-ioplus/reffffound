using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using reffffound.Data;
using reffffound.Services;

namespace reffffound.Components.PaginationViewComponent
{
	public class PaginationViewComponent : ViewComponent
  {
    private IBookmarkService _bookmarkService;

    public PaginationViewComponent(ApplicationDbContext applicationDbContext)
		{
			_bookmarkService = new BookmarkContextService(applicationDbContext);
		}

    public IViewComponentResult Invoke()
    {
      int model_pagecount = _bookmarkService.GetPageCount(ViewBag.Username);

      return View("Default", model_pagecount);
    }
  }
}
