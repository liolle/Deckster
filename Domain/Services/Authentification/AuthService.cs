// package
using Microsoft.Data.SqlClient;

// Refs
using deckster.cqs;
using deckster.exceptions;
using deckster.services.commands;
using deckster.entities;
using System.Data;
using deckster.services.queries;
using deckster.dto;

namespace deckster.services;

public partial class AuthService
{
  public CommandResult Execute(RegisterUserCommand command)
  {

    // Generate an Account to get the AccountId
    UserEntity user = UserEntity.Create(command.Email,command.NickName);
    AccountEntity acc = AccountEntity.Create("credential",user.Id,user.Id); 

    try
    {
      string hashedPassword = hash.HashPassword(command.Password);
      using SqlConnection conn = context.CreateConnection();
      string query = $@"RegisterUserTransaction";

      using SqlCommand cmd = new(query, conn);
      cmd.CommandType = System.Data.CommandType.StoredProcedure;
      cmd.Parameters.AddWithValue("@Email", command.Email);
      cmd.Parameters.AddWithValue("@AccountId", acc.Id);
      cmd.Parameters.AddWithValue("@UserId", user.Id);
      cmd.Parameters.AddWithValue("@NickName", command.NickName);
      cmd.Parameters.AddWithValue("@UserName", command.UserName);
      cmd.Parameters.Add("@RowsAffected", SqlDbType.Int).Direction = ParameterDirection.Output;
      cmd.Parameters.AddWithValue("@Password", hashedPassword);

      int res = context.ExecuteNonQuery(query,cmd);
      object val = cmd.Parameters["@RowsAffected"].Value; 

      int rowsAffected = res ;
      if (val is not null){rowsAffected = (int) val;}

      if ((rowsAffected + res) < 1 )
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

  
  public QueryResult<string> Execute(CredentialLoginQuery query)
  {
    try
    {
      QueryResult<CredentialInfoModel?> qr = Execute(new UserFromUserNameQuery(query.UserName));
      if (!qr.IsSuccess )
      {
        return IQueryResult<string>.Failure("",new InvalidCredentialException());
      }

      if (!hash.VerifyPassword(qr.Result!.Password, query.Password))
      {
        return IQueryResult<string>.Failure("",new InvalidCredentialException());
      }

      return IQueryResult<string>.Success(jwt.Generate(qr.Result.GetClaims()));
    }
    catch (Exception e)
    {
      return IQueryResult<string>.Failure("Server error",e);
    }
  }


    public QueryResult<CredentialInfoModel?> Execute(UserFromUserNameQuery query)
  {
    try
    {
      using SqlConnection conn = context.CreateConnection();
      string sql_query = @"
        SELECT 
        u.id,
        u.email,
        u.nickname,
        u.created_at,
        cred.password
        FROM [Accounts] acc
        LEFT JOIN [Credentials] cred ON cred.account_id = acc.id 
        LEFT JOIN [Users] u ON u.id = acc.user_id
        WHERE cred.user_name = @UserName
        ";
      using SqlCommand cmd = new(sql_query, conn);
      cmd.Parameters.AddWithValue("@UserName", query.UserName);

      using SqlDataReader reader = context.ExecuteReader(sql_query,cmd);
      if (reader.Read())
      {
        CredentialInfoModel u = new CredentialInfoModel(
            (string)reader[nameof(UserEntity.Id)],
            (string)reader[nameof(UserEntity.Email)],
            (string)reader[nameof(UserEntity.NickName)],
            (string)reader[nameof(CredentialEntity.Password)],
            (DateTime)reader[nameof(UserEntity.Created_At)]
            );
        return IQueryResult<CredentialInfoModel?>.Success(u);
      }
      return IQueryResult<CredentialInfoModel?>.Failure($"Could not find UserName {query.UserName}");
    }
    catch (Exception e)
    {
      return IQueryResult<CredentialInfoModel?>.Failure("Server error",e);
    }
  }

  
}





