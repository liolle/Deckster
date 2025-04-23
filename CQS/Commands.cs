namespace deckster.cqs;

public interface ICommandDefinition
{
}

public interface ICommandResult : IResult
{
  static CommandResult Success()
  {
    return new CommandResult(true, "", null);
  }

  static CommandResult Failure(string errorMessage, Exception? exception = null)
  {
    return new CommandResult(false, errorMessage, exception);
  }
}

public class CommandResult : ICommandResult
{
  internal CommandResult(bool isSuccess, string? errorMessage, Exception? exception)
  {
    IsSuccess = isSuccess;
    ErrorMessage = errorMessage;
    Exception = exception;
  }

  public bool IsSuccess { get; } 

  public string? ErrorMessage { get; } 

  public Exception? Exception { get; } 
}

// Handlers
public interface ICommandHandler<T> where T : ICommandDefinition
{
  CommandResult Execute(T command);
}

public interface ICommandHandlerAsync<T> where T : ICommandDefinition
{
  Task<CommandResult> Execute(T command);
}

