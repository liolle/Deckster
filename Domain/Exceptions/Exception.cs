namespace deckster.exceptions;

public class MissingConfigException(string config_key) : Exception($"Missing configuration:\n  - {config_key}\n")
{
}

public class DuplicateFieldException(string field_name) : Exception($"Unavailable field:\n - {field_name}")
{
}
