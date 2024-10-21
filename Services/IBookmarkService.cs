using Microsoft.Extensions.Primitives;
using reffffound.Models;

namespace reffffound.Services
{
	public interface IBookmarkService
	{
		void Create(Bookmark bookmark);
		Bookmark? Read(string guid);
		void Update(Bookmark bookmark);
		void Delete(string guid);
		void Delete(string guid, string username="");

		List<Bookmark> ListAll();
		List<Bookmark> List(int page);
		List<Bookmark> List(string username, string filter, int page);
		int GetPageCount(string username, string filter);
		int GetBookmarkCount(string username, string filter);
		List<Bookmark> GetUsersRelatedBookmarks(string username, string guid);
		IDictionary<string,List<Bookmark>> GetUsersContextBookmarks(string username, string usercontext, string identityUserName="");
		void Save(string guidToSave, string name);
		void Forget(string guidToForget, string name);
	}
}
