using Microsoft.Data.SqlClient;
using reffffound.Data;
using reffffound.Models;

namespace reffffound.Services
{
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
			_userRepository = new UserRepository(_connectionString);
		}
		public UserService(string connectionString)
		{
			_connectionString = connectionString;
			_userRepository = new UserRepository(_connectionString);
		}

		public void Create(ContentUser contentUser)
		{
			_userRepository.Create(contentUser);
		}

		public void Delete(ContentUser contentUser)
		{
			_userRepository.Delete(contentUser.Id);
		}

		public ContentUser Read(string username)
		{
			return _userRepository.Read(username);
		}
		public void Update(ContentUser contentUser)
		{
			_userRepository.Update(contentUser);
		}
		public List<ContentUser> List()
		{
			throw new NotImplementedException( );
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

		public void DecreaseBookmarkCount(string username)
		{
			var userRepository = new UserRepository(_connectionString);

			var user = userRepository.Read(username);
			if (user != null)
			{
				user.Count--;
				userRepository.Update(user);
			}
		}

		public List<string> ListUsernames()
		{
			throw new NotImplementedException( );
		}
	}
}
