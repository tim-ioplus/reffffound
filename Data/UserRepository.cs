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
        private ApplicationDbContext _context;
        private SqlConnection _DbConnection;

        public UserRepository(ApplicationDbContext context, string connectionString)
        {
            _context = context;
            //_DbConnection = GetConnection();
        }


        private SqlConnection GetConnection()
        {
            var connectionString = _configuration != null ? _configuration["ConnectionStrings:SqlConnection"]
                    : "Server=DESKTOP-JQTJ275\\SQLEXPRESS;Database=reffffound;TrustedConnection=true;MultipleActiveResultSets=true;User=appsu;Password=appsu3000";
            var _settings = new Dictionary<string, string>();

            foreach (string setting in connectionString.Split(";"))
            {
                var settingKV = setting.Split("=");
                _settings.Add(settingKV[0], settingKV[1]);
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = _settings["Server"]; //"DESKTOP-JQTJ275\\SQLEXPRESS";
            builder.InitialCatalog = _settings["Database"]; //"reffffound";
            builder.UserID = _settings["User"]; //"appsu";
            builder.Password = _settings["Password"]; //"appsu3000";
            
            builder.MultipleActiveResultSets = bool.Parse(_settings["MultipleActiveResultSets"]); //true
            builder.TrustServerCertificate = bool.Parse(_settings["TrustedConnection"]); //true;

            return new SqlConnection(builder.ConnectionString);
        }

        public List<ContentUser> ReadActive()
        {
            var contentUsers = new List<ContentUser>();

            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    String sql = string.Format("SELECT TOP 10 * FROM dbo.ContentUsers Order BY Count DESC");

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
            string email = reader.GetString(1);
            string link = reader.GetString(2);
            int count = reader.GetInt32(3);

            contentUser.Id = id;
            contentUser.Email = email;
            contentUser.Link = link;
            contentUser.Count = count;

            return contentUser;
        }

        #endregion

      

    }
}
