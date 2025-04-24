// package
using Microsoft.Data.SqlClient;

// Refs
using deckster.cqs;
using deckster.services.commands;

namespace deckster.services;

public partial class AuthService
{
  public CommandResult Execute(RegisterUserCommand command)
  {
    try
    {
      using SqlConnection conn = context.CreateConnection();
      string hashedPassword = hash.HashPassword(command.Password);

      string query = $@"RegisterUserTransaction";

      using SqlCommand cmd = new(query, conn);
      cmd.CommandType = System.Data.CommandType.StoredProcedure;
      cmd.Parameters.AddWithValue("@Email", command.Email);
      cmd.Parameters.AddWithValue("@UserName", command.UserName);
      cmd.Parameters.AddWithValue("@Password", hashedPassword);

      int result = context.ExecuteNonQuery(query,cmd);
      if (result < 1)
      {
      Console.WriteLine(result);
        return ICommandResult.Failure("User insertion failed.");
      }

      return ICommandResult.Success();
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
      return ICommandResult.Failure("Server error",e);
    }
  }
}





