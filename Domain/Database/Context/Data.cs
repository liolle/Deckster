
namespace deckster.database;

using System.Data;
using deckster.exceptions;
using Microsoft.Data.SqlClient;

public interface IDataContext {
  SqlConnection CreateConnection();
  int ExecuteNonQuery(string query, SqlCommand cmd);
  SqlDataReader ExecuteReader(string query, SqlCommand cmd);
  DataTable ExecuteQuery(string query, SqlCommand cmd);
}

public class DataContext(IConfiguration configuration) : IDataContext
{
  private readonly string _connectionString = configuration["DB_CONNECTION_STRING"] ?? throw new MissingConfigException("DB_CONNECTION_STRING") ;

  public SqlConnection CreateConnection()
  {
    SqlConnection con = new SqlConnection(_connectionString);
    return con; 
  }

  public int ExecuteNonQuery(string query, SqlCommand cmd)
  {
    cmd.Connection.Open();
    return cmd.ExecuteNonQuery(); 
  }

  public DataTable ExecuteQuery(string query, SqlCommand cmd)
  {
    cmd.Connection.Open();
    using SqlDataAdapter adapter = new(cmd);
    DataTable resultTable = new();
    adapter.Fill(resultTable);
    return resultTable;
  }

  public SqlDataReader ExecuteReader(string query, SqlCommand cmd)
  {
    cmd.Connection.Open();
    return cmd.ExecuteReader(); 
  }
}

