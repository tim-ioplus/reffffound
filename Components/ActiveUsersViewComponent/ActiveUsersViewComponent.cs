using Microsoft.AspNetCore.Mvc;
using reffffound.Data;
using reffffound.Services;

namespace reffffound.Components.ActiveUsersViewComponent
{
	public class ActiveUsersViewComponent : ViewComponent
	{
		private readonly IUserService _userService;

		public ActiveUsersViewComponent(IUserService userService, ApplicationDbContext context)
		{
			_userService = new UserContextService(context);
		}
		public IViewComponentResult Invoke()
		{
			var model = _userService.GetActiveUsers();
			return View("Default", model);
		}
	}
}
