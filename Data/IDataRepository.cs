using Microsoft.Data.SqlClient;

namespace reffffound.Data
{
	public interface IDataRepository
  {
    SqlConnection? GetConnection(string connectionString);
  }
}
