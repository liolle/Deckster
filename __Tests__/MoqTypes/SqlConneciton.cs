// package
using Microsoft.Data.SqlClient;

public interface IMockSqlCon : IDisposable
{
   void Open();
}

public class MockSqlCon : IMockSqlCon
{
    private readonly SqlConnection _connection;
    
    public MockSqlCon(SqlConnection connection)
    {
        _connection = connection;
    }
    
    public void Open() => _connection.Open();
    
    public void Dispose() => _connection.Dispose();
}
