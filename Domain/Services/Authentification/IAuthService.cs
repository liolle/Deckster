using deckster.cqs;
using deckster.database;
using deckster.services.commands;

namespace deckster.services;
public interface IAuthService:
ICommandHandler<RegisterUserCommand>
{
}

//public partial class AuthService( HttpClient httpClient) : IAuthService
// HttpClient used for Oauth2
public partial class AuthService(IDataContext context,IHashService hash, IConfiguration configuration, IJWTService jwt) : IAuthService
{
}

