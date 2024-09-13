using Microsoft.Data.SqlClient;
using reffffound.Data;

namespace reffffound.Services
{
	public interface IUserService
	{
		public IDictionary<string, string> GetActiveUsers();

	}
	public class UserService : IUserService
	{
		public string ContentRootPath = "";
		private string _connectionString;
		private SqlConnection _DbConnection;
		private UserRepository _userRepository;

		[Obsolete]
		public UserService(IConfiguration configuration)
		{
			_connectionString = configuration["ConnectionStrings:AzureSqlConnection"] ?? configuration["ConnectionStrings:DataConnection"] ?? "";
		}
		public UserService(string connectionString)
		{
			_connectionString = connectionString;
			_userRepository = new UserRepository(_connectionString);
		}

		public IDictionary<string, string> GetActiveUsers()
		{
			var activeUsers = new Dictionary<string, string>();

			var topTenUsers = _userRepository.GetTopTenActiveUsers();

			foreach (var user in topTenUsers)
			{
				activeUsers.Add( user.Name, user.Count + "");
			}

			return activeUsers;
		}

		public void IncreaseBookmarkCount(string username)
		{
			var userRepository = new UserRepository(_connectionString);

			var user = userRepository.Read(username);
			if (user != null)
			{
				user.Count++;
				userRepository.Update(user);
			}
		}
	}
}
