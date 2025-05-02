using deckster.cqs;
using deckster.database;
using deckster.entities;
using deckster.exceptions;
using deckster.services.commands;
using deckster.services.queries;
using Microsoft.Data.SqlClient;

namespace deckster.services;

public interface ICardService :
ICommandHandler<AddCardCommand>,
ICommandHandler<AddDeckCommand>,
  IQueryHandler<CardsQuery, List<CardEntity>>
{
}

public partial class CardService(IDataContext context) : ICardService
{
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
}


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
}
