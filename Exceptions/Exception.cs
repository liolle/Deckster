namespace deckster.exceptions;

public class MissingConfigException(string config_key) : Exception($"Missing configuration:\n  - {config_key}\n")
{
}
