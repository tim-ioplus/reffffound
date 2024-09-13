using Microsoft.Data.SqlClient;
using NuGet.Versioning;
using reffffound.Models;

namespace reffffound.Data
{
	public interface IBookmarkService
	{
		Bookmark GetBookmark(int id);
		int GetPageCount(string username);
		int GetBookmarkCount(string username);
	}
	public class BookmarkService : IBookmarkService
	{
		private string _connectionString;
		public BookmarkService(IConfiguration configuration)
		{
			_connectionString = configuration["ConnectionStrings:AzureSqlConnection"] ?? configuration["ConnectionStrings:DataConnection"] ?? "";
		}
		public Bookmark GetBookmark(int id)
		{
			throw new NotImplementedException();
		}

		public int GetPageCount(string user = "")
		{
			var pagesize = 10;
			var count = 0;

			var bookmarkcount = GetBookmarkCount(user);
			if (bookmarkcount > 0)
			{
				count = (int)Math.Ceiling((decimal)bookmarkcount / pagesize);
			}

			return count;
		}
		public int GetBookmarkCount(string username = "")
		{
			var count = new BookmarkRepository(_connectionString).GetCount(username);
			return count;
		}
	}
}
