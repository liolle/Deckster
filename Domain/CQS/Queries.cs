namespace deckster.cqs;

public interface IQueryDefinition<TResult>
{
}

public interface IQueryResult<TResult> : IResult
{
  static QueryResult<TResult> Success(TResult result){
    return new QueryResult<TResult>(true,result,"",null);
  }

  static QueryResult<TResult> Failure(string errorMessage, Exception? exception = null){
    return new QueryResult<TResult>(false,default!,errorMessage,exception);
  }
}

public class QueryResult<TResult> : IQueryResult<TResult>
{
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }
    public Exception? Exception { get; }
    private readonly TResult _result;

    internal QueryResult(bool isSuccess, TResult result, string errorMessage, Exception? exception)
    {
        IsSuccess = isSuccess;
        _result = result;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    public TResult Result
    {
        get
        {
            if (!IsSuccess) { throw new InvalidOperationException(); }
            return _result;
        }
    }
}

// Handlers  
public interface IQueryHandler<TQuery,TResult> 
: IQueryDefinition<TResult> where TQuery : IQueryDefinition<TResult>
{
  QueryResult<TResult> Execute(TQuery query);
}

public interface IQueryHandlerAsync<TQuery,TResult> 
: IQueryDefinition<TResult> where TQuery : IQueryDefinition<Task<TResult>>
{
  Task<QueryResult<TResult>> Execute(TQuery query);
}
