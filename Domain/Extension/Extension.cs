using deckster.exceptions;
using Microsoft.AspNetCore.Mvc;

namespace deckster.extensions;

public static class ModelStateExtesion
{
  public static void validModelOrThrow(this ControllerBase controller)
  {
    if (controller.ModelState.IsValid){return;}
    var errors = controller.ModelState
      .Where(x => x.Value?.Errors.Count > 0)
      .ToDictionary(
          val => val.Key,
          val => val.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
          ).ToArray();
    throw new InvalidRequestModelException(errors);
  }
}
