
using deckster.cqs;
using deckster.database;
using deckster.dto;
using deckster.services.commands;
using deckster.services.queries;

namespace deckster.services;
public interface ICardService:
ICommandHandler<AddCardCommand>
{
}

//public partial class AuthService( HttpClient httpClient) : IAuthService
// HttpClient used for Oauth2
public partial class CardService(IDataContext context) : ICardService
{

}

