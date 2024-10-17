using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using reffffound.Data;
using reffffound.Models;
using reffffound.Services;

namespace reffffound.Controllers
{
	public class UserHelperController : Controller
	{
		private string _connectionString;
		private ApplicationDbContext _context;
		private UserService _userService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserStore<ApplicationUser> _userStore;
		private readonly IUserEmailStore<ApplicationUser> _emailStore;
		private readonly SignInManager<ApplicationUser> _signInManager;
		public UserHelperController(ApplicationDbContext applicationDbContext, IConfiguration configuration)
		{
			_context = applicationDbContext;
			_userService = new UserService( configuration );
			_connectionString =  configuration["ConnectionStrings:DataConnection"] ?? "";
		}
	}
}
