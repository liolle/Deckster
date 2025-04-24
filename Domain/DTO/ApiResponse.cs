using Microsoft.AspNetCore.Mvc;
using deckster.exceptions;
namespace deckster.dto;
public enum ERROR_TYPES
{
  DEFAULT_ERROR,
  SERVER_ERROR,
  DUPLICATE_FIELD
}

public class ApiError 
{
  public ERROR_TYPES Type { get; set; }
  public object Value { get; set; }

  public ApiError(ERROR_TYPES type, object value)
  {
    Type = type;
    Value = value;
  }
}

public interface IApiOutput
{
  static IActionResult ResponseError(Exception e)
  {
    switch (e)
    {
      case MissingConfigException ex:
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.SERVER_ERROR, null));

      case DuplicateFieldException ex:
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.DUPLICATE_FIELD, ex.Message));

      default:
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.SERVER_ERROR, null));
    }
  }

  static IActionResult Reponse(object? obj)
  {
    if (obj is null)
    {
      return new NoContentResult();
    }

    return new OkObjectResult(obj);
  }
}
