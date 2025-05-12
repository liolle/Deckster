namespace Shared.exceptions;

public class MissingConfigException(string config_key) : Exception($"Missing configuration:\n- {config_key}\n")
{
}

public class DuplicateFieldException(string field_name) : Exception($"Unavailable field:\n- {field_name}")
{
  public string Field { get; init; } = field_name;
}


public class UnknownFieldException(string field_name) : Exception($"Unknown field:\n- {field_name}")
{
  public string Field { get; init; } = field_name;
}

public class InvalidRequestModelException<T>(T errors) : Exception($"Invalid Model")
{
  public T Errors { get; init; } = errors;
}

public class NotFoundElementException(string key) : Exception($"Not found element:\n {key}")
{
  public string Key { get; init; } = key;
}

public class InvalidCredException() : Exception("Invalid Connection Credentials")
{
}

public class UnAuthorizeActionException(string message) : Exception(message)
{
}

public class InvalidHeaderException(string key) : Exception($"Invalid Header\n - {key}")
{
  public string Key { get; init; } = key;
}

