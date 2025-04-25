USE [deckster];
GO

IF OBJECT_ID('RegisterUserTransaction', 'P') IS NOT NULL 
BEGIN 
  DROP PROCEDURE dbo.RegisterUserTransaction;
END
GO

CREATE PROCEDURE RegisterUserTransaction 
  @UserName NVARCHAR(100), 
  @AccountId NVARCHAR(100),
  @UserId NVARCHAR(100),
  @NickName NVARCHAR(100),
  @Email NVARCHAR(100),
  @RowsAffected INT OUTPUT,
  @Password NVARCHAR(100) 
AS 
BEGIN 
  SET NOCOUNT ON;  
  
  BEGIN TRY 
    BEGIN TRANSACTION;
    -- Create User
SET @RowsAffected = 0;
    INSERT INTO [Users] ([id], [email], [nickName]) 
    VALUES (@UserId, @Email, @NickName);  
    SET @RowsAffected = @RowsAffected + @@ROWCOUNT;

    -- Create Account 
    INSERT INTO [Accounts] ([id], [provider], [user_id]) 
    VALUES (@AccountId, 'credential', @UserId); 
    SET @RowsAffected = @RowsAffected + @@ROWCOUNT;

    -- Create Credential
    INSERT INTO [Credentials] ([user_name], [account_id], [password]) 
    VALUES (@UserName, @AccountId, @Password);
    SET @RowsAffected = @RowsAffected + @@ROWCOUNT;

    COMMIT TRANSACTION;
  END TRY 
  BEGIN CATCH 
    IF @@TRANCOUNT > 0  
      ROLLBACK TRANSACTION;

    SET @RowsAffected = -1;
    THROW;
  END CATCH 
END;
