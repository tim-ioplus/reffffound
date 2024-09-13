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
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace reffffound.Data
{
	public class UserRepository : DataRepository
	{

		private List<ContentUser> _contentUsers = [];
		public string ContentRootPath = "";
		public bool Hydrated = false;
		private readonly IConfiguration _configuration;
		private ApplicationDbContext _context;
		private string _connectionString;

		public UserRepository(ApplicationDbContext context, string connectionString)
		{
			_context = context;
			_connectionString = connectionString;
		}

		public UserRepository(string connectionString)
		{
			_connectionString = connectionString;
		}

		public void Create(ContentUser contentUser)
		{
			try
			{
				using (SqlConnection connection = GetConnection(_connectionString))
				{
					var sql = $@"Insert Into dbo.ContentUsers (Name, Email, Role, Link, Count) Values (@username, @useremail, @userrole,@userlink, @usercount);";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("username", contentUser.Name);
						command.Parameters.AddWithValue("useremail", contentUser.Email);
						command.Parameters.AddWithValue("userrole", contentUser.Role);
						command.Parameters.AddWithValue("userlink", contentUser.Link);
						command.Parameters.AddWithValue("usercount", contentUser.Count);

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

		public ContentUser Read(string username)
		{
			ContentUser? contentUser = null;

			try
			{
				using (SqlConnection connection = GetConnection(_connectionString))
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
				using (SqlConnection connection = GetConnection(_connectionString))
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

		public void Delete(int id)
		{
			try
			{
				using (SqlConnection connection = GetConnection(_connectionString))
				{
					var sql = "Delete from dbo.ContentUsers where Id=@id;";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("id", id);

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
				using (SqlConnection connection = GetConnection(_connectionString))
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
				using (SqlConnection connection = GetConnection(_connectionString))
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
