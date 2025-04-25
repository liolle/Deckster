USE [deckster];
GO

IF OBJECT_ID('RegisterUserTransaction', 'P') IS NOT NULL 
BEGIN 
  DROP PROCEDURE dbo.RegisterUserTransaction;
END
GO

CREATE PROCEDURE RegisterUserTransaction 
  @UserName NVARCHAR(100),       -- Changed to NVARCHAR for consistency
  @AccountId NVARCHAR(100),
  @UserId NVARCHAR(100),
  @NickName NVARCHAR(100),
  @Email NVARCHAR(100),
  @RowsAffected INT OUTPUT,
  @Password NVARCHAR(100) 
AS 
BEGIN 
  SET NOCOUNT ON;  -- Recommended to reduce network traffic
  
  BEGIN TRY 
    BEGIN TRANSACTION;
    -- Create User
    INSERT INTO [Users] ([id], [email], [nickName]) 
    VALUES (@UserId, @Email, @NickName);  -- Fixed @AccountId instead of @UserId
    SET @RowsAffected = @RowsAffected + @@ROWCOUNT;

    -- Create Account (Fixed column order and values)
    INSERT INTO [Accounts] ([id], [provider], [user_id]) 
    VALUES (@AccountId, 'credential', @UserId);  -- Fixed value order
    SET @RowsAffected = @RowsAffected + @@ROWCOUNT;

    -- Create Credential
    INSERT INTO [Credentials] ([user_name], [account_id], [password]) 
    VALUES (@UserName, @AccountId, @Password);
    SET @RowsAffected = @RowsAffected + @@ROWCOUNT;

    SELECT @RowsAffected AS RowsAffected

    SET @RowsAffected = 3;
    COMMIT TRANSACTION;
  END TRY 
  BEGIN CATCH 
    IF @@TRANCOUNT > 0  
      ROLLBACK TRANSACTION;

    SET @RowsAffected = -1;
    THROW;
  END CATCH 
END;
