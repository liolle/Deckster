// package
using Microsoft.Data.SqlClient;

// Refs
using deckster.cqs;
using deckster.exceptions;
using deckster.services.commands;

namespace deckster.services;

public partial class AuthService
{
  public CommandResult Execute(RegisterUserCommand command)
  {
    try
    {
      string hashedPassword = hash.HashPassword(command.Password);
      using SqlConnection conn = context.CreateConnection();
      string query = $@"RegisterUserTransaction";

      using SqlCommand cmd = new(query, conn);
      cmd.CommandType = System.Data.CommandType.StoredProcedure;
      cmd.Parameters.AddWithValue("@Email", command.Email);
      cmd.Parameters.AddWithValue("@NickName", command.NickName);
      cmd.Parameters.AddWithValue("@UserName", command.UserName);
      cmd.Parameters.AddWithValue("@Password", hashedPassword);

      int result = context.ExecuteNonQuery(query,cmd);
      if (result < 1)
      {
        return ICommandResult.Failure("User insertion failed.");
      }

      return ICommandResult.Success();
    }
    catch (SqlException sqlEx) when (sqlEx.Number == 2627) // SQL Server unique constraint violation error code
    {
      string duplicateField = "default"; // default message

      if (sqlEx.Message.Contains("U_users_email"))
      {
        duplicateField = "email";
      }
      else if (sqlEx.Message.Contains("U_users_nickname"))
      {
        duplicateField = "nickName";
      }
      else if (sqlEx.Message.Contains("U_credentials_user_name"))
      {
        duplicateField = "userName";
      }

      return ICommandResult.Failure("", new DuplicateFieldException(duplicateField));
    }
    catch (Exception e)
    {
      return ICommandResult.Failure("Server error",e);
    }
  }
}





