using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using reffffound.Data;
using reffffound.Models;
public static class UserHelper
{
  
  private static readonly List<ContentUser> _initUsers = new List<ContentUser>()
  {
    new ContentUser(){Id=100, Name="koelleforniadreamin", Role=Role.Administrator },
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

  public static string GetRole(string username)
  {
    return IsAdmin(username) ? Role.Administrator : Role.User;
  }
}

public static class Role
{
  public static string User = "User";
  public static string Administrator = "Administrator";
}
