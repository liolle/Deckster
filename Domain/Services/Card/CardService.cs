using deckster.cqs;
using deckster.entities;
using deckster.services.commands;
using Microsoft.Data.SqlClient;

namespace deckster.services;


public partial class CardService
{

  public CommandResult Execute(AddCardCommand command)
  {

    CardEntity card = CardEntity.Create(command.Name,command.Cost,command.Defense,command.Strength,command.Name);

    try
    {
      string query  = @"
        INSERT INTO [Cards]([id],[name],[cost],[defense],[strength],[image])
        VALUES(@Id,@Name,@Cost,@Defense,@strength,@Image)
        ";

      using SqlConnection conn = context.CreateConnection();

      using SqlCommand cmd = new(query, conn);
      cmd.Parameters.AddWithValue("@Id", card.Id);
      cmd.Parameters.AddWithValue("@Name", card.Name);
      cmd.Parameters.AddWithValue("@Cost", command.Cost);
      cmd.Parameters.AddWithValue("@Defense", card.Defense);
      cmd.Parameters.AddWithValue("@Strength", card.strength);
      cmd.Parameters.AddWithValue("@Image", command.Image);

      int rowsAffected = context.ExecuteNonQuery(query,cmd);

      if (rowsAffected<1)
      {
        return ICommandResult.Failure("Car insertion failed");


      }

      return ICommandResult.Success();

    }
    catch (Exception e)
    {
      return ICommandResult.Failure("Server error",e);
    }
  }
}
