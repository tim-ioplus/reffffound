using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using reffffound.Services;

namespace reffffound.Components.PaginationViewComponent
{
	public class PaginationViewComponent : ViewComponent
  {
    private IBookmarkService _bookmarkService;

    public PaginationViewComponent(IBookmarkService bookmarkService)
    {
      _bookmarkService = bookmarkService;
    }

    public IViewComponentResult Invoke()
    {
      int model_pagecount = _bookmarkService.GetPageCount(ViewBag.Username);

      return View("Default", model_pagecount);
    }
  }
}
