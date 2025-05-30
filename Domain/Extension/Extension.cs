using Microsoft.AspNetCore.Mvc;
using Shared.exceptions;

namespace deckster.extensions;

public static class ModelStateExtension
{
  public static void validModelOrThrow(this ControllerBase controller)
  {
    if (controller.ModelState.IsValid) { return; }
    var errors = controller.ModelState
      .Where(x => x.Value?.Errors.Count > 0)
      .ToDictionary(
          val => val.Key,
          val => val.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
          ).ToArray();
    throw new InvalidRequestModelException<KeyValuePair<string, string[]?>[]?>(errors);
  }


  public static void validSuperAdminOrThrow(this ControllerBase controller, IConfiguration config, string? admin_token)
  {
    string admin_key = config["S_ADMIN"] ?? throw new MissingConfigException("S_ADMIN");
    if (admin_key != admin_token)
    {
      throw new InvalidHeaderException("X-ADMIN");
    }
  }
}
