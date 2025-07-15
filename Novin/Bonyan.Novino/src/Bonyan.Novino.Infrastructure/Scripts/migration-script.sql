IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(max) NULL,
        [Title] nvarchar(max) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [Link] nvarchar(max) NULL,
        [IsRead] bit NOT NULL,
        [ReadAt] datetime2 NULL,
        [Purpose] nvarchar(max) NULL,
        [IsSent] bit NOT NULL,
        [SentAt] datetime2 NULL,
        [Context] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeletedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ModifiedAt] datetime2 NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] nvarchar(450) NOT NULL,
        [Title] nvarchar(256) NOT NULL,
        [CanBeDeleted] bit NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE TABLE [Tenants] (
        [Id] uniqueidentifier NOT NULL,
        [Key] nvarchar(max) NOT NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeletedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ModifiedAt] datetime2 NULL,
        CONSTRAINT [PK_Tenants] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeletedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ModifiedAt] datetime2 NULL,
        [UserName] nvarchar(256) NOT NULL,
        [EmailAddress] nvarchar(max) NULL,
        [EmailIsVerified] bit NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberIsVerified] bit NULL,
        [StatusName] nvarchar(max) NOT NULL,
        [StatusId] int NOT NULL,
        [Version] uniqueidentifier NOT NULL,
        [PasswordHash] nvarchar(256) NOT NULL,
        [PasswordSalt] varbinary(max) NOT NULL,
        [CanBeDeleted] bit NOT NULL,
        [BannedUntil] datetime2 NULL,
        [FailedLoginAttemptCount] int NOT NULL,
        [AccountLockedUntil] datetime2 NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [DateOfBirth] datetime2 NULL,
        [NationalCode] nvarchar(50) NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE TABLE [RoleClaims] (
        [Id] uniqueidentifier NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(256) NOT NULL,
        [ClaimValue] nvarchar(1000) NOT NULL,
        [Issuer] nvarchar(256) NULL,
        CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE TABLE [UserClaims] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(256) NOT NULL,
        [ClaimValue] nvarchar(1000) NOT NULL,
        [Issuer] nvarchar(256) NULL,
        [ClaimValueType] nvarchar(max) NULL,
        CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE TABLE [UserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE TABLE [UserTokens] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Type] nvarchar(100) NOT NULL,
        [Value] nvarchar(2000) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [Expiration] datetime2 NULL,
        CONSTRAINT [PK_UserTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RoleClaims_ClaimType] ON [RoleClaims] ([ClaimType]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RoleClaims_ClaimValue] ON [RoleClaims] ([ClaimValue]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RoleClaims_RoleId_ClaimType] ON [RoleClaims] ([RoleId], [ClaimType]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RoleClaims_RoleId_ClaimType_ClaimValue] ON [RoleClaims] ([RoleId], [ClaimType], [ClaimValue]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Roles_CanBeDeleted] ON [Roles] ([CanBeDeleted]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Roles_Title] ON [Roles] ([Title]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserClaims_ClaimType] ON [UserClaims] ([ClaimType]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserClaims_ClaimValue] ON [UserClaims] ([ClaimValue]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserClaims_UserId_ClaimType] ON [UserClaims] ([UserId], [ClaimType]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserClaims_UserId_ClaimType_ClaimValue] ON [UserClaims] ([UserId], [ClaimType], [ClaimValue]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserRoles_UserId] ON [UserRoles] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Users_CreatedAt] ON [Users] ([CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Users_FullName] ON [Users] ([FirstName], [LastName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_UserName] ON [Users] ([UserName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserTokens_CreatedAt] ON [UserTokens] ([CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    EXEC(N'CREATE INDEX [IX_UserTokens_Expiration] ON [UserTokens] ([Expiration]) WHERE Expiration IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserTokens_UserId_Type] ON [UserTokens] ([UserId], [Type]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715121833_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250715121833_InitialCreate', N'8.0.12');
END;
GO

COMMIT;
GO

