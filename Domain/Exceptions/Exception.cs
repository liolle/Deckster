namespace deckster.exceptions;

public class MissingConfigException(string config_key) : Exception($"Missing configuration:\n- {config_key}\n")
{
}

public class DuplicateFieldException(string field_name) : Exception($"Unavailable field:\n- {field_name}")
{
  public string Field {get;init;} = field_name;
}

public class InvalidRequestModelException(KeyValuePair<string, string[]?>[]? errors) : Exception($"Invalid Model:\n -")
{
  public KeyValuePair<string, string[]?>[]? Errors {get;init;} = errors;
}
