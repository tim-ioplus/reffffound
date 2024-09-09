using Microsoft.Data.SqlClient;

namespace reffffound.Data
{
  public interface IDataRepository
  {
    SqlConnection? GetConnection(string connectionString);
  }
  public class DataRepository : IDataRepository
  {
    public SqlConnection? GetConnection(string connectionString)
    {
      if (string.IsNullOrWhiteSpace(connectionString)) return null;

      if(connectionString.Contains("database.windows.net"))
        {
          return new SqlConnection(connectionString);
        }
      else
        {
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
  }
}
