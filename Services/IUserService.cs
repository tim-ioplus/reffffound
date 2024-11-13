using reffffound.Models;

namespace reffffound.Services
{
	public interface IUserService
	{
		public void Create(ContentUser contentUser);
		public ContentUser? Read(string username);
		public void Update(ContentUser contentUser);
		public void Delete(ContentUser contentUser);
		public List<ContentUser> List();
		public IDictionary<string, string> GetActiveUsers();
		public void IncreaseBookmarkCount(string username);
		public void DecreaseBookmarkCount(string username);
	}
}
