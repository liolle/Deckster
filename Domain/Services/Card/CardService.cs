using deckster.cqs;
using deckster.database;
using deckster.dto;
using deckster.entities;
using deckster.exceptions;
using deckster.services.commands;
using deckster.services.queries;
using Microsoft.Data.SqlClient;

namespace deckster.services;

public interface ICardService :
ICommandHandler<AddCardCommand>,
ICommandHandler<AddDeckCommand>,
ICommandHandler<DeleteDeckCommand>,
ICommandHandler<PatchDeckCommand>,
ICommandHandler<GetDeckPermission>,
ICommandHandler<DeleteDeckCardsCommand>,
  IQueryHandler<CardsQuery, List<CardEntity>>,
  IQueryHandler<UserDecksQuery, List<DeckEntity>>,
  IQueryHandler<UserDecksInfoQuery, DeckModel>,
  IQueryHandler<DeckCardsQuery, List<DeckCard>>
{
}

public partial class CardService(IDataContext context) : ICardService
{

}

// Commands
public partial class CardService
{

  public CommandResult Execute(AddCardCommand command)
  {

    CardEntity card = CardEntity.Create(command.Name, command.Cost, command.Defense, command.Strength, command.Name);
    string query = @"
        INSERT INTO [Cards]([id],[name],[cost],[defense],[strength],[image])
        VALUES(@Id,@Name,@Cost,@Defense,@strength,@Image)
        ";

    try
    {
      using SqlConnection conn = context.CreateConnection();

      using SqlCommand cmd = new(query, conn);
      cmd.Parameters.AddWithValue("@Id", card.Id);
      cmd.Parameters.AddWithValue("@Name", card.Name);
      cmd.Parameters.AddWithValue("@Cost", command.Cost);
      cmd.Parameters.AddWithValue("@Defense", card.Defense);
      cmd.Parameters.AddWithValue("@Strength", card.Strength);
      cmd.Parameters.AddWithValue("@Image", command.Image);

      int rowsAffected = context.ExecuteNonQuery(query, cmd);

      if (rowsAffected < 1)
      {
        return ICommandResult.Failure("Card insertion failed");
      }

      return ICommandResult.Success();
    }
    catch (SqlException sqlEx) when (sqlEx.Number == 2627) // SQL Server unique constraint violation error code
    {
      string duplicateField = "default"; // default message

      if (sqlEx.Message.Contains("U_card_name"))
      {
        duplicateField = "name";
      }

      return ICommandResult.Failure("", new DuplicateFieldException(duplicateField));
    }

    catch (Exception e)
    {
      return ICommandResult.Failure("Server error", e);
    }
  }

  public CommandResult Execute(AddDeckCommand command)
  {

    string query = @"
        INSERT INTO [Decks]([name],[account_id],[id])
        VALUES(@Name,@AccountId,@DeckId)
        ";

    try
    {

      using SqlConnection conn = context.CreateConnection();

      using SqlCommand cmd = new(query, conn);

      cmd.Parameters.AddWithValue("@DeckId", command.DeckId);
      cmd.Parameters.AddWithValue("@AccountId", command.AccountId);
      cmd.Parameters.AddWithValue("@Name", command.Name);

      int rowsAffected = context.ExecuteNonQuery(query, cmd);

      if (rowsAffected < 1)
      {
        return ICommandResult.Failure("Card insertion failed");
      }

      return ICommandResult.Success();
    }
    catch (SqlException sqlEx) when (sqlEx.Number == 2627) // SQL Server unique constraint violation error code
    {
      string duplicateField = "default"; // default message

      if (sqlEx.Message.Contains("U_deck_name"))
      {
        duplicateField = "name";
      }

      return ICommandResult.Failure("", new DuplicateFieldException(duplicateField));
    }
    catch (Exception e)
    {
      return ICommandResult.Failure("Server error", e);
    }
  }

  public CommandResult Execute(DeleteDeckCommand command)
  {
    string query = @"
        DELETE FROM [Decks]
        WHERE [id] = @DeckId AND [account_id] = @AccountId
        ";
    try
    {

      using SqlConnection conn = context.CreateConnection();
      using SqlCommand cmd = new(query, conn);

      cmd.Parameters.AddWithValue("@DeckId", command.DeckId);
      cmd.Parameters.AddWithValue("@AccountId", command.AccountId);

      int rowsAffected = context.ExecuteNonQuery(query, cmd);

      if (rowsAffected < 1)
      {
        return ICommandResult.Failure("", new UnAuthorizeActionException("Permission Denied or Deck does not exist"));
      }

      return ICommandResult.Success();
    }
    catch (Exception e)
    {
      return ICommandResult.Failure("Server error", e);
    }
  }


  public CommandResult Execute(DeleteDeckCardsCommand command)
  {
    string query = @"
        DELETE FROM [Decks_cards]
        WHERE [deck_id] = @DeckId 
        ";

    try
    {

      using SqlConnection conn = context.CreateConnection();
      using SqlCommand cmd = new(query, conn);

      cmd.Parameters.AddWithValue("@DeckId", command.DeckId);

      int rowsAffected = context.ExecuteNonQuery(query, cmd);

      if (rowsAffected < 1)
      {
        return ICommandResult.Failure("", new UnAuthorizeActionException("Permission Denied or No card in the deck"));
      }

      return ICommandResult.Success();
    }
    catch (Exception e)
    {
      return ICommandResult.Failure("Server error", e);
    }
  }


  public CommandResult Execute(PatchDeckCommand command)
  {

    string valuesClause = string.Join(",", command.Cards.Select((_, i) =>
    $"(@DeckId, @Card_{i}, @Quantity_{i})"));

    string query = $@"
    INSERT INTO [Decks_cards]([deck_id], [card_id], [quantity])
    VALUES {valuesClause}";

    try
    {


      using SqlConnection conn = context.CreateConnection();
      using SqlCommand cmd = new(query, conn);

      cmd.Parameters.AddWithValue($"@DeckId", command.DeckId);
      for (int i = 0; i < command.Cards.Count; i++)
      {
        cmd.Parameters.AddWithValue($"@Card_{i}", command.Cards[i].CardId);
        cmd.Parameters.AddWithValue($"@Quantity_{i}", command.Cards[i].Quantity);
      }

      int rowsAffected = context.ExecuteNonQuery(query, cmd);
      if (rowsAffected < 1)
      {
        return ICommandResult.Failure("Failed to update deck");
      }

      return ICommandResult.Success();
    }
    catch (Exception e)
    {
      return ICommandResult.Failure("Server error", e);
    }
  }

  public CommandResult Execute(GetDeckPermission command)
  {
    string query = @"
    SELECT * FROM [Decks]
    WHERE [id] = @DeckId AND [account_id] = @AccountId
    ";

    try
    {
      using SqlConnection conn = context.CreateConnection();
      using SqlCommand cmd = new(query, conn);

      cmd.Parameters.AddWithValue($"@DeckId", command.DeckId);
      cmd.Parameters.AddWithValue($"@AccountId", command.AccountId);

      SqlDataReader reader = context.ExecuteReader(query, cmd);
      if (!reader.Read())
      {
        return ICommandResult.Failure("", new UnAuthorizeActionException("Permission Denied"));
      }

      return ICommandResult.Success();
    }
    catch (Exception e)
    {
      return ICommandResult.Failure("Server error", e);
    }

  }
}

// Queries
public partial class CardService
{

  public QueryResult<List<CardEntity>> Execute(CardsQuery query)
  {
    try
    {
      List<CardEntity> cards = [];

      string sql_query = @"
        SELECT * FROM [Cards]
        ORDER BY [name]
        OFFSET @Offset ROWS
        FETCH NEXT @Size ROW ONLY
        ";

      using SqlConnection conn = context.CreateConnection();

      using SqlCommand cmd = new(sql_query, conn);
      cmd.Parameters.AddWithValue("@Offset", query.Size * query.Page);
      cmd.Parameters.AddWithValue("@Size", query.Size);

      using SqlDataReader reader = context.ExecuteReader(sql_query, cmd);
      while (reader.Read())
      {
        CardEntity card = new(
            (string)reader[nameof(CardEntity.Id)],
            (string)reader[nameof(CardEntity.Name)],
            (int)reader[nameof(CardEntity.Cost)],
            (int)reader[nameof(CardEntity.Defense)],
            (int)reader[nameof(CardEntity.Strength)],
            (string)reader[nameof(CardEntity.Image)]
            );
        cards.Add(card);
      }

      return IQueryResult<List<CardEntity>>.Success(cards);
    }
    catch (Exception e)
    {
      return IQueryResult<List<CardEntity>>.Failure("", e);
    }
  }

  public QueryResult<List<DeckEntity>> Execute(UserDecksQuery query)
  {
    try
    {
      List<DeckEntity> decks = [];

      string sql_query = @"
        SELECT * FROM [Decks]
        WHERE [account_id] = @AccountId
        ";

      using SqlConnection conn = context.CreateConnection();

      using SqlCommand cmd = new(sql_query, conn);
      cmd.Parameters.AddWithValue("@AccountId", query.AccountId);

      using SqlDataReader reader = context.ExecuteReader(sql_query, cmd);
      while (reader.Read())
      {
        DeckEntity deck = new(
            (string)reader[nameof(DeckEntity.Id)],
            (string)reader[nameof(DeckEntity.Account_id)],
            (string)reader[nameof(DeckEntity.Name)]
            );
        decks.Add(deck);
      }

      return IQueryResult<List<DeckEntity>>.Success(decks);
    }
    catch (Exception e)
    {
      return IQueryResult<List<DeckEntity>>.Failure("", e);
    }
  }

  public QueryResult<DeckModel> Execute(UserDecksInfoQuery query)
  {
    try
    {
      DeckEntity? deck = null;

      string sql_query = @"
        SELECT * FROM [Decks]
        WHERE [id] = @DeckId
        ";

      using SqlConnection conn = context.CreateConnection();

      using SqlCommand cmd = new(sql_query, conn);
      cmd.Parameters.AddWithValue("@DeckId", query.DeckId);

      using SqlDataReader reader = context.ExecuteReader(sql_query, cmd);
      if (reader.Read())
      {
        deck = new(
           (string)reader[nameof(DeckEntity.Id)],
           (string)reader[nameof(DeckEntity.Account_id)],
           (string)reader[nameof(DeckEntity.Name)]
           );
      }
      if (deck is null)
      {


        return IQueryResult<DeckModel>.Failure("", new NotFoundElementException(query.DeckId));
      }

      QueryResult<List<DeckCard>> res = Execute(new DeckCardsQuery(deck.Id));

      if (!res.IsSuccess)
      {
        return IQueryResult<DeckModel>.Failure("", res.Exception);
      }

      return IQueryResult<DeckModel>.Success(new DeckModel(deck, res.Result));

    }
    catch (Exception e)
    {

      return IQueryResult<DeckModel>.Failure("", e);
    }
  }

  public QueryResult<List<DeckCard>> Execute(DeckCardsQuery query)
  {
    try
    {
      List<DeckCard> cards = [];

      string sql_query = @"
        SELECT 
        d.card_id AS cardId,
        d.quantity AS quantity,
        c.name AS name,
        c.cost AS cost,
        c.defense AS defense,
        c.strength AS strength,
        c.image AS image
        FROM [Decks_cards] d
        LEFT JOIN [Cards] c ON c.id = d.card_id
        WHERE [deck_id] = @DeckId
        ";

      using SqlConnection conn = context.CreateConnection();

      using SqlCommand cmd = new(sql_query, conn);
      cmd.Parameters.AddWithValue("@DeckId", query.DeckId);

      using SqlDataReader reader = context.ExecuteReader(sql_query, cmd);
      while (reader.Read())
      {
        DeckCard card = new(
            (string)reader[nameof(DeckCard.CardId)],
            (string)reader[nameof(DeckCard.Name)],
            (int)reader[nameof(DeckCard.Quantity)],
            (int)reader[nameof(DeckCard.Defense)],
            (int)reader[nameof(DeckCard.Cost)],
            (int)reader[nameof(DeckCard.Strength)],
            (string)reader[nameof(DeckCard.Image)]
            );
        cards.Add(card);
      }

      return IQueryResult<List<DeckCard>>.Success(cards);
    }
    catch (Exception e)
    {
      return IQueryResult<List<DeckCard>>.Failure("", e);
    }
  }
}
