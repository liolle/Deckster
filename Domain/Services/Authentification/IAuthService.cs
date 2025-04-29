using deckster.cqs;
using deckster.database;
using deckster.dto;
using deckster.services.commands;
using deckster.services.queries;

namespace deckster.services;
public interface IAuthService:
ICommandHandler<RegisterUserCommand>,
ICommandHandler<PromoteAdminCommand>,
  IQueryHandler<CredentialLoginQuery,string>,
  IQueryHandler<UserFromUserNameQuery,CredentialInfoModel?>
{
}

//public partial class AuthService( HttpClient httpClient) : IAuthService
// HttpClient used for Oauth2
public partial class AuthService(IDataContext context, IHashService hash, IConfiguration configuration, IJWTService jwt) : IAuthService
{
}

