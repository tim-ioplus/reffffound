using Microsoft.EntityFrameworkCore;
using reffffound.Data;
using reffffound.Models;

namespace reffffound.Services
{
	public class UserContextService : IUserService
	{
		private ApplicationDbContext _context;
		private DbSet<ContentUser> _data;
		public UserContextService(ApplicationDbContext context)
		{
			_context = context;
			_data = _context.ContentUsers;
		}

		public IDictionary<string, string> GetActiveUsers()
		{
			var userActivity = new Dictionary<string, string>();

			foreach (var contentUser in _context.ContentUsers)
			{
				userActivity.Add(contentUser.Name, contentUser.Count.ToString());
			}

			return userActivity;
		}

		public ContentUser? Read(string username)
		{
			var contentUser = _data.FirstOrDefault(u => u.Name.Equals(username) );
			return contentUser;
		}

		public void Create(ContentUser contentUser)
		{
			_data.Add(contentUser);
			_context.SaveChanges();
		}

		public void Update(ContentUser user)
		{
			_data.Update(user);
			_context.SaveChanges();
		}

		public void Delete(ContentUser contentUser)
		{
			_data.Remove(contentUser);
			_context.SaveChanges();
		}

		public List<ContentUser> List()
		{
			return _data.ToList();
		}

		public void IncreaseBookmarkCount(string username)
		{
			var userToUpdate = Read(username);
			if(userToUpdate != null)
			{
				userToUpdate.Count++;

				Update(userToUpdate);
			}
		}
		
		public void DecreaseBookmarkCount(string username)
		{
			var userToUpdate = Read(username);
			if(userToUpdate != null)
			{
				userToUpdate.Count--;

				Update(userToUpdate);
			}
		}

	}
}
