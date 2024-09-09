using Microsoft.Data.SqlClient;
using reffffound.Models;
using System;

namespace reffffound.Data
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

    public UserService(IConfiguration configuration)
    {
      _connectionString = configuration["ConnectionStrings:AzureSqlConnection"] ?? configuration["ConnectionStrings:DataConnection"] ?? "";
    }
    

    public IDictionary<string, string> GetActiveUsers()
    {
      var activeUsers = new Dictionary<string, string>();

      var topTenUsers = new UserRepository(_connectionString).GetTopTenActiveUsers();

      foreach (var user in topTenUsers)
      {
          activeUsers.Add(user.Name, user.Count + "");
      }

        return activeUsers;
      }
  }
}
