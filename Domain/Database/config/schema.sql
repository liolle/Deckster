IF DB_ID('deckster') IS NULL 
BEGIN
  CREATE DATABASE deckster;

  PRINT 'Database deckster created';
END
GO
USE [deckster];

IF OBJECT_ID('Users', 'U') IS NULL 
BEGIN
  CREATE TABLE [Users] (
    [id] [nvarchar] (100) NOT NULL,
    [email] [nvarchar] (100) NOT NULL,
    [nickname] [nvarchar] (100) NOT NULL,
    [created_at] [datetime] CONSTRAINT DF_users_created_at DEFAULT GETDATE(),
    CONSTRAINT PK_users_id PRIMARY KEY ([id]),
    CONSTRAINT U_users_email UNIQUE ([email]),
    CONSTRAINT U_users_nickname UNIQUE ([nickname]),
  );

  PRINT 'Table Users created';
END;

IF OBJECT_ID('Accounts', 'U') IS NULL 
BEGIN
  CREATE TABLE [Accounts] (
    [id] [nvarchar] (100) NOT NULL,
    [provider] [nvarchar] (100) NOT NULL,
    [user_id] [nvarchar] (100) NOT NULL,
    [created_at] [datetime] CONSTRAINT DF_Accounts_created_at DEFAULT GETDATE(),
    CONSTRAINT PK_accounts_id PRIMARY KEY ([id]),
    CONSTRAINT CK_accounts_provider CHECK ([provider] IN ('microsoft', 'google', 'credential')),
    CONSTRAINT FK_accounts_user_id FOREIGN KEY ([user_id]) REFERENCES [Users] ([id]) ON DELETE CASCADE
  );

  PRINT 'Table Accounts created';
END;

IF OBJECT_ID('Credentials', 'U') IS NULL 
BEGIN
  CREATE TABLE [Credentials] (
    [account_id] [nvarchar] (100) NOT NULL, -- Changed to match Accounts.id type
    [user_name] [varchar] (50),
    [password] [nvarchar] (100) NOT NULL,
    CONSTRAINT FK_credentials_account_id FOREIGN KEY ([account_id]) REFERENCES [Accounts] ([id]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT U_credentials_user_name UNIQUE ([user_name])
  );

  PRINT 'Table Credentials created';
END;

IF OBJECT_ID('Decks', 'U') IS NULL
BEGIN
  CREATE TABLE [Decks] (
    [id] [nvarchar] (100) NOT NULL,
    [account_id] [nvarchar] (100) NOT NULL,
    [name] [nvarchar] (100) NOT NULL,
    CONSTRAINT PK_deck_id PRIMARY KEY ([id]),
    CONSTRAINT FK_deck_account_id FOREIGN KEY ([account_id]) REFERENCES [Accounts] ([id]) ON DELETE CASCADE ON UPDATE CASCADE
  );
  PRINT 'Table Decks created';
END

IF OBJECT_ID('Cards', 'U') IS NULL
BEGIN
  CREATE TABLE [Cards] (
    [id] [nvarchar](100) NOT NULL,
    [name] [nvarchar](100) NOT NULL,
    [cost] [int] CONSTRAINT DF_card_cost DEFAULT 0,
    [defense] [int] CONSTRAINT DF_defense_cost DEFAULT 0,
    [strength] [int] CONSTRAINT DF_strenght_cost DEFAULT 0,
    [image] [nvarchar](500) NOT NULL
    CONSTRAINT PK_card_id PRIMARY KEY ([id]),
    CONSTRAINT U_card_name UNIQUE ([name])
  );
  PRINT 'Table Cards created';
END

IF OBJECT_ID('Decks_cards', 'U') IS NULL
BEGIN
  CREATE TABLE [Decks_cards] (
    [deck_id] [nvarchar] (100) NOT NULL,
    [card_id] [nvarchar] (100) NOT NULL,
    [quantity] [int] NOT NULL

    CONSTRAINT FK_deck_deck_id FOREIGN KEY ([deck_id]) REFERENCES [Decks] ([id]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_deck_card_id FOREIGN KEY ([card_id]) REFERENCES [Cards] ([id]) ON DELETE CASCADE ON UPDATE CASCADE
  );
  
  PRINT 'Table Decks_cards created';
END

IF OBJECT_ID('Roles', 'U') IS NULL
BEGIN
  CREATE TABLE [Roles] (
    [account_id] [nvarchar] (100) NOT NULL,
    [role] [nvarchar] (100) NOT NULL

    CONSTRAINT U_role_account_id  UNIQUE ([account_id],[role]),
    CONSTRAINT FK_role_account_id FOREIGN KEY ([account_id]) REFERENCES [Accounts] ([id]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT CK_role_role CHECK(
      [role] = 'admin'  
    )
  );
  
  PRINT 'Table Roles created';
END
