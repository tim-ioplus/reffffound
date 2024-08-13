using Microsoft.AspNetCore.Identity;
using reffffound.Models;
public static class UserHelperService
{
  private static readonly List<ContentUser> _initUsers = new List<ContentUser>()
  {
    new ContentUser(){Id=100, Name="koelleforniadreamin", Role=Role.Administrator },
    new ContentUser(){Id=101, Name="hypetype", Role= Role.User }
    };

  public static bool IsAdmin(string username)
  {
    return HasRole(username, "Administrator");
  }
  public static bool IsUser(string username)
  {
    return HasRole(username, "User");
  }
  public static bool HasRole(string username, string role)
  {
    return _initUsers.Any(cu =>
      cu.Name.Equals(username) && cu.Role.Equals(role));
  }
}

public static class Role
{
  public static string User = "User";
  public static string Administrator = "Administrator";
}
