using Microsoft.AspNetCore.Mvc;
using reffffound.Services;

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
			var model = _userService.GetActiveUsers();
			return View("Default", model);
		}
	}
}
