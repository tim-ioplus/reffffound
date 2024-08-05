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
using NuGet.Versioning;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace reffffound.Data
{
    public class BookmarkRepository
    {
        public string ContentRootPath = "";
        private string _connectionString;
        private ApplicationDbContext _context;
        private SqlConnection _DbConnection;

        public BookmarkRepository(ApplicationDbContext context, string connectionString)
        {
            _context = context;
            _connectionString = connectionString;
        }


        private SqlConnection GetConnection()
        {
            var connectionString = _connectionString != null ? _connectionString : "";

            var _settings = new Dictionary<string, string>();

            foreach (string setting in connectionString.Split(";"))
            {
                var settingKV = setting.Split("=");
                _settings.Add(settingKV[0], settingKV[1]);
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = _settings["Server"];
            builder.InitialCatalog = _settings["Database"];
            builder.UserID = _settings["User"];
            builder.Password = _settings["Password"];
            
            builder.MultipleActiveResultSets = bool.Parse(_settings["MultipleActiveResultSets"]); //true
            builder.TrustServerCertificate = bool.Parse(_settings["TrustedConnection"]); //true;

            return new SqlConnection(builder.ConnectionString);
        }

        public void Create(Bookmark bookmark)
        {
            if (string.IsNullOrWhiteSpace(bookmark.Guid)) throw new NoNullAllowedException();
            if (string.IsNullOrWhiteSpace(bookmark.Url)) throw new NoNullAllowedException();
            if (string.IsNullOrWhiteSpace(bookmark.Title)) throw new NoNullAllowedException();
            if (bookmark.Title.Length > 64) bookmark.Title = bookmark.Title.Substring(0,64);
            if (string.IsNullOrWhiteSpace(bookmark.Image)) throw new NoNullAllowedException();
            if (bookmark.Savedby == 0) bookmark.Savedby = 1;
            if (string.IsNullOrWhiteSpace(bookmark.Timestamp)) throw new NoNullAllowedException();
            if (string.IsNullOrWhiteSpace(bookmark.Usercontext)) throw new NoNullAllowedException();
            
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    string sql = $@"INSERT INTO[dbo].[Findlings] ([Guid], [Url]
                , [Title]
                , [Image]
                , [SavedBy]
                , [Timestamp]
                , [Usercontext]
                , [FullURL]
                , [Context1link]
                , [Context1Img]
                , [Context2link]
                , [Context2Img]
                , [Context3link]
                , [Context3Img])
                VALUES
                (@guid, @url,@title,@image, @savedBy, 
                @timestamp,@usercontext,@fullUrl, 
                @context1link,@context1img,@context2link,@context2img,@context3link,@context3img);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@guid", bookmark.Guid );
                        command.Parameters.AddWithValue("@url", bookmark.Url);
                        command.Parameters.AddWithValue("@title", bookmark.Title);
                        command.Parameters.AddWithValue("@image", bookmark.Image);
                        command.Parameters.AddWithValue("@savedBy", bookmark.Savedby);
                        command.Parameters.AddWithValue("@timestamp", bookmark.Timestamp);
                        command.Parameters.AddWithValue("@usercontext", bookmark.Usercontext);
                        command.Parameters.AddWithValue("@fullUrl", bookmark.FullUrl ?? "");
                        command.Parameters.AddWithValue("@context1link", bookmark.Context1link ?? "");
                        command.Parameters.AddWithValue("@context1img", bookmark.Context1img ?? "");
                        command.Parameters.AddWithValue("@context2link", bookmark.Context2link ?? "");
                        command.Parameters.AddWithValue("@context2img", bookmark.Context2img ?? "");
                        command.Parameters.AddWithValue("@context3link", bookmark.Context3link ?? "");
                        command.Parameters.AddWithValue("@context3img", bookmark.Context3img ?? "");

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

        public Bookmark? Read(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid)) return null;

            Bookmark bookmark = null;

            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    String sql = string.Format("SELECT * FROM dbo.Findlings Where Guid = @guid");

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@guid", guid);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bookmark = Parse(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bookmark;
        }

        public List<Bookmark> ReadThreeContextBookmarks(string user, string timestamp)
        {
            var bookmarks = new List<Bookmark>();

            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    var sql = $"SELECT TOP 3 Guid, Image FROM [dbo].[Findlings] WHERE Usercontext LIKE '" + user + "%' AND timestamp < '"+ timestamp +"' ORDER BY RAND() ";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string guid = reader.GetString(0);
                                string image = reader.GetString(1);
                                var bookmark = new Bookmark() { Guid = guid, Image = image };
                                bookmarks.Add(bookmark);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bookmarks;
        }

        public List<Bookmark> List(int page)
        {
            return List("", "", page);
        }

        public List<Bookmark> List(string username = "", string filter ="post", int page = 1)
        {
            var bookmarks = new List<Bookmark>();
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    string userFragment = "";
                    if (!string.IsNullOrEmpty(username))
                    {
                        if (filter == "" || filter == "post")
                        {
                            userFragment = " WHERE Usercontext LIKE '" + username + "%'";

                        }
                        else if (filter == "found")
                        {
                            userFragment = " WHERE Usercontext LIKE '%" + username + "%'";
                        }
                    }

                    int skip = page > 0 ? ((page - 1) * 10) : 0;
                    string sql = $"SELECT * FROM [dbo].[Findlings] " + userFragment + " ORDER BY ID desc OFFSET " + skip + " ROWS Fetch FIRST 10 ROWS ONLY;";                    

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        /* @todo 
                         * fix query with parameters 
                         command.Parameters.AddWithValue("@userFragment", userFragment);
                        command.Parameters.AddWithValue("@ skip", skip);
                        */

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var bookmark = Parse(reader);
                                bookmarks.Add(bookmark);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bookmarks;
        }

        public void Update(Bookmark bookmark)
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                   string sql = $@"UPDATE [dbo].[Findlings]
                   SET [Guid] = {bookmark.Guid}
                      ,[Url] = {bookmark.Url}
                      ,[Title] = {bookmark.Guid}
                      ,[Image] = {bookmark.Image}
                      ,[SavedBy] = {bookmark.Savedby}
                      ,[Timestamp] = {bookmark.Timestamp}
                      ,[Usercontext] = {bookmark.Usercontext}
                      ,[FullURL] = {bookmark.FullUrl}
                      ,[Context1link] = {bookmark.Context1link}
                      ,[Context1Img] = {bookmark.Context1img}
                      ,[Context2link] = {bookmark.Context2link}
                      ,[Context2Img] = {bookmark.Context2img}
                      ,[Context3link] = {bookmark.Context3link}
                      ,[Context3Img] = {bookmark.Context3img}
                      WHERE [Id] = {bookmark.Id},";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Updated
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(int id)
        {
            var bookmark = new Bookmark();

            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    String sql = string.Format("Delete FROM dbo.Findlings Where Id = {0}", id);

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Deleted
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #region Helper
        private Bookmark Parse(SqlDataReader reader)
        {
            var bookmark = new Bookmark();

            int id = reader.GetInt32(0);
            string guid = reader.GetString(1);
            string url = reader.GetString(2);
            string title = reader.GetString(3);
            string image = reader.GetString(4);
            int savedBy = reader.GetInt32(5);
            string timestamp = reader.GetString(6);
            string usercontext = reader.GetString(7);
            string fullurl = reader.IsDBNull(8) ? "" : reader.GetString(8);
            string context1link = reader.IsDBNull(9) ? "" : reader.GetString(9);
            string context1img = reader.IsDBNull(10) ? "" : reader.GetString(10);
            string context2link = reader.IsDBNull(11) ? "" : reader.GetString(11);
            string context2img = reader.IsDBNull(12) ? "" : reader.GetString(12);
            string context3link = reader.IsDBNull(13) ? "" : reader.GetString(13);
            string context3img = reader.IsDBNull(14) ? "" : reader.GetString(14);

            bookmark.Id = id;
            bookmark.Guid = guid;
            bookmark.Url = url;
            bookmark.Title = title;
            bookmark.Image = image;
            bookmark.Savedby = savedBy;
            bookmark.Timestamp = timestamp;
            bookmark.Usercontext = usercontext;
            bookmark.FullUrl = fullurl;
            bookmark.Context1link = context1link;
            bookmark.Context1img = context1img;
            bookmark.Context2link = context2link;
            bookmark.Context2img = context2img;
            bookmark.Context3link = context3link;
            bookmark.Context3img = context3img;

            return bookmark;
        }

      private DateTime GetLastPostingTimestamp()
      {
          DateTime timestamp = DateTime.MinValue;
          try
            {
                using (SqlConnection connection = GetConnection())
                {
                  String sql = string.Format("SELECT TOP 1 [Timestamp] FROM [dbo].[Findlings] ORDER BY Timestamp desc");

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string timestring = reader.GetString(0);
                                timestamp = DateTime.ParseExact(timestring,"yyyy-MM-dd HH:mm:ss", null);
                            }
                        }
                    }
                }
              }
          catch(Exception ex)
          {
            throw ex;
          }

        return timestamp;
      }

        #endregion

        #region Test und local data
        public List<Bookmark>? ListMockData()
        {
            var dataPath = Path.Combine(ContentRootPath, "Data/bookmarks.json");
            if (File.Exists(dataPath))
            {
                string dataText = File.ReadAllText(dataPath).Replace("\r\n", "").Replace("\n", "");
                if (string.IsNullOrWhiteSpace(dataText)) return null;

                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var bookmarks = JsonSerializer.Deserialize<List<Bookmark>>(dataText, jsonOptions);
                if (bookmarks != null && bookmarks.Count > 0)
                {
                    return bookmarks;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

    /// <summary>
    /// Adds new Sample Post Data to the Database
    /// Keeps the Database Row Ids and Timestamps in Order
    /// </summary>
    /// <returns></returns>
    public bool Hydrate()
    {
      bool updateTimestamp = true;

      var mockData = ListMockData();
      if (mockData == null || mockData.Count == 0) return false;

      var lastPostingTime = GetLastPostingTimestamp();
      var earliestTimestamp = lastPostingTime.AddHours(1);//new DateTime(2024,7,31,13,10,42).AddHours(1);
      var justNowTimestamp = DateTime.Now.AddHours(-1);
      var diffTimespan = justNowTimestamp.Subtract(earliestTimestamp);

      var timeSpanPerPost = diffTimespan.Divide(mockData.Count);
      var currentTime = earliestTimestamp;

      foreach (var bookmark in mockData)
      {
          var savedBookmark = Read(bookmark.Guid);
          if(savedBookmark == null)
          {
            var bookmarkWithContext = AddContext(bookmark);

            if(updateTimestamp)
            {
              currentTime = currentTime.Add(timeSpanPerPost);            
              bookmarkWithContext.Timestamp = currentTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            Create(bookmarkWithContext);
          }
      }

      foreach (var bookmark in mockData) 
      {
          var bookmarkToCheck = Read(bookmark.Guid);
          if (bookmarkToCheck == null)
          {
              return false;
          }
      }

      return true;
    }

        #endregion

        public List<Bookmark> AddContext(List<Bookmark> bookmarks)
        {
            foreach (var bookmark in bookmarks)
            {
                AddContext(bookmark);
            }

            return bookmarks;
        }

        public Bookmark AddContext(Bookmark bookmark)
        {
            var user = bookmark.Usercontext.Replace(" ", "").Split(",").First();
            var contextBookmarks = ReadThreeContextBookmarks(user, bookmark.Timestamp);

            if (contextBookmarks.Count >= 1)
            {
                bookmark.Context1img = contextBookmarks.ElementAt(0).Image;
                bookmark.Context1link = contextBookmarks.ElementAt(0).Guid ?? "";
            }

            if (contextBookmarks.Count >= 2)
            {
                bookmark.Context2img = contextBookmarks.ElementAt(1).Image ?? "";
                bookmark.Context2link = contextBookmarks.ElementAt(1).Guid ?? ""; 
            }

            if (contextBookmarks.Count == 3)
            {
                bookmark.Context3img = contextBookmarks.ElementAt(2).Image ?? "";
                bookmark.Context3link = contextBookmarks.ElementAt(2).Guid ?? ""; 
            }

            return bookmark;
        }
    }
}
