using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using reffffound.Data;
using reffffound.Models;
using System.Security.Claims;
public static class UserManagerHelper
{

	private static readonly List<ContentUser> _initUsers = new List<ContentUser>( )
  {
	 new ContentUser(){Id=100, Name="koelleforniadreamin", Role=Role.Administrator },
	 };

	public static bool IsAdmin(string username)
	{
		return HasRole( username, Role.Administrator );
	}
	public static bool IsUser(string username)
	{
		return HasRole( username, Role.User );
	}
	public static bool HasRole(string username, string role)
	{
		return _initUsers.Any( cu =>
		  cu.Name.Equals( username ) && cu.Role.Equals( role ) );
	}

	public static string GetRole(string username)
	{
		return IsAdmin( username ) ? Role.Administrator : Role.User;
	}

	public static bool CanDo(ClaimsPrincipal? user, bool enabledByEnvironment)
	{
		if(enabledByEnvironment) return true;

		if(user != null && user.Identity != null && user.Identity.IsAuthenticated) return true;

		return false;
	}

	internal static bool CanWrite(ClaimsPrincipal user, string username)
	{
		return CanDo(user, false) && (user.Identity.Name.Equals(username) || IsAdmin(user.Identity.Name));
	}
}

public static class Role
{
	public static string User = "User";
	public static string Administrator = "Administrator";
}
