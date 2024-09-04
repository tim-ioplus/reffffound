using Azure.Core.Serialization;
using reffffound.Models;
using System.Text.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Linq.Expressions;
using System.Security.Policy;
using Microsoft.Data.SqlClient;
using System.Text;
using NuGet.Packaging.Signing;

namespace reffffound.Data
{
    public class UserRepository
    {
        private List<ContentUser> _contentUsers = [];
        public string ContentRootPath = "";
        public bool Hydrated = false;
        private readonly IConfiguration _configuration;
        private string _connectionString;
        private ApplicationDbContext _context;
        private SqlConnection _DbConnection;

        public UserRepository(ApplicationDbContext context, string connectionString)
        {
            _context = context;
            _connectionString = connectionString;
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

        public ContentUser Read(string username)
        {
            var contentUser = new ContentUser();

            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    var sql = "SELECT * FROM dbo.ContentUsers where Name = @username";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("username", username);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                contentUser = Parse(reader);                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return contentUser;
        }

    public void Update(ContentUser contentUser)
      {
      try
      {
        using (SqlConnection connection = GetConnection())
        {
          string sql = $@"UPDATE [dbo].[ContentUsers]
                   SET [Name] = @name
                      ,[EMail] = @email
                      ,[Role] = @role
                      ,[Link] = @link
                      ,[Count] = @count
                      WHERE [Id] = @id";

          using (SqlCommand command = new SqlCommand(sql, connection))
          {
            command.Parameters.AddWithValue("@name", contentUser.Name);
            command.Parameters.AddWithValue("@email", contentUser.Email);
            command.Parameters.AddWithValue("@role", contentUser.Role);
            command.Parameters.AddWithValue("@link", contentUser.Link);
            command.Parameters.AddWithValue("@count", contentUser.Count);
            command.Parameters.AddWithValue("@id", contentUser.Id);

            connection.Open();
            command.ExecuteNonQuery();
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
      }

        public List<ContentUser> GetTopTenActiveUsers()
        {
            var contentUsers = new List<ContentUser>();

            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    var sql = "SELECT Top 10 * FROM dbo.ContentUsers Order BY Count DESC";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var contentUser = Parse(reader);
                                contentUsers.Add(contentUser);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return contentUsers;
        }
        public List<ContentUser> List()
        {
            var contentUsers = new List<ContentUser>();

            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    var sql = "SELECT * FROM dbo.ContentUsers Order BY Count DESC";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var contentUser = Parse(reader);
                                contentUsers.Add(contentUser);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return contentUsers;
        }


        #region Helper
        private ContentUser Parse(SqlDataReader reader)
        {
            var contentUser = new ContentUser();

            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            string email = reader.GetString(2);
            string role = reader.GetString(3);
            string link = reader.GetString(4);
            int count = reader.GetInt32(5);

            contentUser.Id = id;
            contentUser.Name = name;
            contentUser.Email = email;
            contentUser.Role = role;
            contentUser.Link = link;
            contentUser.Count = count;

            return contentUser;
        }

        #endregion

      

    }
}
