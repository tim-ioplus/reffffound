using Microsoft.AspNetCore.Mvc;
using reffffound.Data;

namespace reffffound.Components.ActiveUsersViewComponent 
{
  public class ActiveUsersViewComponent : ViewComponent
  {
    private readonly IUserService _userService;

    public ActiveUsersViewComponent(IUserService userService)
    {
      _userService = userService;
    }
    public IViewComponentResult Invoke()
    {
        var model = _userService.GetActiveUsers(); /*new Dictionary<string, string>
          {
            { "MagicNumberMonk", "23" },
            { "Univierse", "42" }
          };*/
        // Populate your model as needed
        return View("Default", model);
    }
  }
}
