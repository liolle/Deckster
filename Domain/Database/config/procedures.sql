IF OBJECT_ID('dbo.RegisterUserTransaction', 'P') IS NOT NULL 
BEGIN 
  DROP PROCEDURE dbo.RegisterUserTransaction;
END
GO;

CREATE PROCEDURE RegisterUserTransaction 
  @UserName VARCHAR(50),
  @AccountId [nvarchar] (100),
  @UserId   [nvarchar] (100),
  @NickName [nvarchar] (100),
  @Email    [nvarchar] (100),
  @Password [nvarchar] (100) 
AS BEGIN 
  DECLARE @NewUserId [nvarchar](100); 
  DECLARE @NewAccountId [nvarchar](100);
  BEGIN TRY 
    BEGIN TRANSACTION
    -- Create User
    INSERT INTO [Users] ([id],[email],[nickName]) VALUES(@AccountId,@Email,@NickName);

    -- Create Account
    INSERT INTO [Accounts] ([AccountId],[provider], [user_id]) VALUES('credential', @UserId);

    -- Create Credential
    INSERT INTO [Credentials] ([user_name], [account_id], [password]) VALUES(@UserName, @AccountId, @Password);

    COMMIT TRANSACTION;

  END TRY 
  BEGIN CATCH 
    ROLLBACK TRANSACTION;
    -- Re-throw the error
    THROW;
  END CATCH 
END;
