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
  public string Type { get; set; }
  public Object? Value { get; set;  } 

  public ApiError(ERROR_TYPES type, object? value)
  {
    Type = Enum.GetName(type) ?? "default";
    Value = value;
  }
}

public interface IApiOutput
{
  static IActionResult ResponseError(Exception e)
  {
    Console.WriteLine(e.Message);
    switch (e)
    {
      case MissingConfigException ex:
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.SERVER_ERROR, default));

      case DuplicateFieldException ex:
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.DUPLICATE_FIELD, ex.Message));

      default:
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.SERVER_ERROR, default));
    }
  }

  static IActionResult Reponse(Object? obj)
  {
    if (obj is null)
    {
      return new NoContentResult();
    }

    return new OkObjectResult(obj);
  }
}
