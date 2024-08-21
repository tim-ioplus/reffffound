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
    private SqlConnection GetConnection()
    {
      if(_connectionString.Contains("database.windows.net"))
        {
          return new SqlConnection(_connectionString);
        }
      else
        {
          var connectionString = _connectionString != null ? _connectionString : "";

          var _settings = new Dictionary<string, string>();

          foreach (string setting in connectionString.Split(";"))
          {
            var settingKV = setting.Split("=");
            _settings.Add(settingKV[0], settingKV[1]);
          }

          SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder( );
          builder.DataSource = _settings["Server"];
          builder.InitialCatalog = _settings["Database"];
          builder.UserID = _settings["User"];
          builder.Password = _settings["Password"];

          builder.MultipleActiveResultSets = bool.Parse(_settings["MultipleActiveResultSets"]); //true
          builder.TrustServerCertificate = bool.Parse(_settings["TrustedConnection"]); //true;

          return new SqlConnection(builder.ConnectionString);
        }
    }

    public IDictionary<string, string> GetActiveUsers()
    {
      var activeUsers = new Dictionary<string, string>();
      try
      {
        using (SqlConnection connection = GetConnection())
        {
          String sql = string.Format("SELECT Top 10 Name, Count FROM dbo.ContentUsers ORDER BY Count desc");

          using (SqlCommand command = new SqlCommand(sql, connection))
          {
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
              while (reader.Read())
              {
                var user = reader.GetString(0);
                var count = reader.GetInt32(1);

                activeUsers.Add(user, count.ToString());
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }

      return activeUsers;
      }
  }
}
