using Microsoft.AspNetCore.Mvc;
using deckster.exceptions;
namespace deckster.dto;
public enum ERROR_TYPES
{
  DEFAULT_ERROR,
  SERVER_ERROR,
  DUPLICATE_FIELD,
  INVALID_MODEL,
  INVALID_CREDENTIALS,
  INVALID_HEADER,
  UNKNOW_ACCOUNT,
  ACTION_DENIED
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

      case InvalidCredentialException ex:
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.INVALID_CREDENTIALS, default));

      case InvalidRequestModelException ex:
        return new BadRequestObjectResult(new ApiError(ERROR_TYPES.INVALID_MODEL, ex.Errors));

      case InvalidHeaderException ex:
        return new UnauthorizedObjectResult(new ApiError(ERROR_TYPES.INVALID_HEADER, ex.Key));

      case UnknownFieldException ex:
        return new UnauthorizedObjectResult(new ApiError(ERROR_TYPES.UNKNOW_ACCOUNT, ex.Field));
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
