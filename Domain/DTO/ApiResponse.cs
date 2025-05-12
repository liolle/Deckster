using Microsoft.AspNetCore.Mvc;
using Shared.exceptions;
namespace deckster.dto;
public enum ERROR_TYPES
{
  DEFAULT_ERROR,
  SERVER_ERROR,
  DUPLICATE_FIELD,
  INVALID_MODEL,
  INVALID_CREDENTIALS,
  INVALID_HEADER,
  UNKNOWN_ACCOUNT,
  ACTION_DENIED,
  NOT_FOUND_ELEMENT
}

public class ApiError
{
  public string Type { get; set; }
  public Object? Value { get; set; }

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
    switch (e)
    {
      case MissingConfigException ex:
        Console.WriteLine(ex.Message);
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.SERVER_ERROR, default));

      case DuplicateFieldException ex:
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.DUPLICATE_FIELD, ex.Field));

      case InvalidCredException ex:
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.INVALID_CREDENTIALS, default));

      case InvalidRequestModelException<List<string>> ex:
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.INVALID_MODEL, ex.Errors));

      case InvalidRequestModelException<KeyValuePair<string, string[]?>[]?> ex:
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.INVALID_MODEL, ex.Errors));

      case InvalidHeaderException ex:
        return new UnauthorizedObjectResult(new ApiError(ERROR_TYPES.INVALID_HEADER, ex.Key));

      case NotFoundElementException ex:
        return new ObjectResult(new ApiError(ERROR_TYPES.NOT_FOUND_ELEMENT, ex.Message)) { StatusCode = 400 };

      case UnknownFieldException ex:
        return new UnauthorizedObjectResult(new ApiError(ERROR_TYPES.UNKNOWN_ACCOUNT, ex.Field));

      case UnAuthorizeActionException ex:
        return new ObjectResult(new ApiError(ERROR_TYPES.ACTION_DENIED, ex.Message)) { StatusCode = 403 };
      default:
        Console.WriteLine(e.Message);
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.SERVER_ERROR, default));
    }
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
