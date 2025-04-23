IF DB_ID('deckster') IS NULL BEGIN
CREATE DATABASE deckster;

PRINT 'Database deckster created';

END
GO
USE [deckster];

IF OBJECT_ID('Users', 'U') IS NULL BEGIN
CREATE TABLE [Users] (
  [id] [nvarchar] (100) NOT NULL,
  [email] [nvarchar] (100) NOT NULL,
  [created_at] [datetime] CONSTRAINT DF_users_created_at DEFAULT GETDATE(),
  CONSTRAINT PK_users_id PRIMARY KEY ([id]),
  CONSTRAINT U_users_email_id UNIQUE ([email])
);

PRINT 'Table Users created';

END;

IF OBJECT_ID('Accounts', 'U') IS NULL BEGIN
CREATE TABLE [Accounts] (
  [id] [nvarchar] (100) NOT NULL,
  [provider] [nvarchar] (100) NOT NULL,
  [user_id] [nvarchar] (100) NOT NULL,
  [created_at] [datetime] CONSTRAINT DF_Accounts_created_at DEFAULT GETDATE(),
  CONSTRAINT PK_accounts_id PRIMARY KEY ([id]),
  CONSTRAINT CK_accounts_provider CHECK (
    [provider] IN ('microsoft', 'google', 'credential')
  ),
  CONSTRAINT FK_accounts_user_id FOREIGN KEY ([user_id]) REFERENCES [Users] ([id]) ON DELETE CASCADE
);

PRINT 'Table Accounts created';

END;

IF OBJECT_ID('Credentials', 'U') IS NULL BEGIN
CREATE TABLE [Credentials] (
  [account_id] [nvarchar] (100) NOT NULL, -- Changed to match Accounts.id type
  [user_name] [varchar] (50),
  [password] [nvarchar] (100) NOT NULL,
  CONSTRAINT FK_credentials_account_id FOREIGN KEY ([account_id]) REFERENCES [Accounts] ([id]) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT U_credentials_user_name UNIQUE ([user_name])
);

PRINT 'Table Credentials created';

END;
