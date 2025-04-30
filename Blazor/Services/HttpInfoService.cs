namespace Blazor.services;

public class HttpInfoService
{
    public string ACCESS_TOKEN { get; init; }
    public string CSRF_TOKEN { get; init; }


    public HttpInfoService(IHttpContextAccessor httpContextAccessor, IConfiguration config)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext ?? throw new Exception("Missing dependency HttpContextAccessor");
        string COOKIE_NAME = config["AUTH_TOKEN_NAME"] ?? throw new Exception("Missing configuration: AUTH_TOKEN_NAME");
        string CSRF_COOKIE_NAME = config["CSRF_COOKIE_NAME"] ?? throw new Exception("Missing configuration: CSRF_COOKIE_NAME");
        string accessToken = httpContext.Request.Cookies[COOKIE_NAME] ?? "";
        string csrfToken = httpContext.Request.Cookies[CSRF_COOKIE_NAME] ?? "";

        ACCESS_TOKEN = accessToken;
        CSRF_TOKEN = csrfToken;
    }

}