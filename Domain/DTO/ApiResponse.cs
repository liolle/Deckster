using Microsoft.AspNetCore.Mvc;
using deckster.exceptions;
namespace deckster.dto;
public enum ERROR_TYPES
{
  DEFAULT_ERROR,
  SERVER_ERROR,
  DUPLICATE_FIELD,
  INVALID_MODEL
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
        return e switch
        {
            MissingConfigException ex => new BadRequestObjectResult(new ApiError(ERROR_TYPES.SERVER_ERROR, default)),
            DuplicateFieldException ex => new BadRequestObjectResult(new ApiError(ERROR_TYPES.DUPLICATE_FIELD, ex.Field)),
            InvalidRequestModelException ex => new BadRequestObjectResult(new ApiError(ERROR_TYPES.INVALID_MODEL, ex.Errors)),
            _ => new BadRequestObjectResult(new ApiError(ERROR_TYPES.SERVER_ERROR, default)),
        };
    }

  static IActionResult Response(Object? obj)
  {
    if (obj is null)
    {
      return new NoContentResult();
    }

    return new OkObjectResult(obj);
  }
}
