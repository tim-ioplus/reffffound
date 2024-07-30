namespace reffffound.Models;

public class BookmarksUsers
{
    public IEnumerable<ContentUser> Users { get; set; }
    public IEnumerable<Bookmark> Bookmarks { get; set; }
}