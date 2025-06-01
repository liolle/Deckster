namespace Shared.exceptions;

public class MissingConfigException(string configKey) : Exception($"Missing configuration:\n- {configKey}\n");

public class MalformedConfiguration(string configKey) : Exception($"Malformed configuration:\n- {configKey}\n");

public class DuplicateFieldException(string fieldName) : Exception($"Unavailable field:\n- {fieldName}")
{
  public string Field { get; init; } = fieldName;
}


public class UnknownFieldException(string fieldName) : Exception($"Unknown field:\n- {fieldName}")
{
  public string Field { get; init; } = fieldName;
}

public class InvalidRequestModelException<T>(T errors) : Exception($"Invalid Model")
{
  public T Errors { get; init; } = errors;
}

public class NotFoundElementException(string key) : Exception($"Not found element:\n {key}")
{
  public string Key { get; init; } = key;
}

public class InvalidCredException() : Exception("Invalid Connection Credentials");

public class UnAuthorizeActionException(string message) : Exception(message);

public class InvalidHeaderException(string key) : Exception($"Invalid Header\n - {key}")
{
  public string Key { get; init; } = key;
}
