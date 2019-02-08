IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [contacts] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(450) NULL,
    CONSTRAINT [PK_contacts] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [contacts_addresses] (
    [Id] int NOT NULL IDENTITY,
    [Street] nvarchar(max) NOT NULL,
    [State] nvarchar(max) NOT NULL,
    [ZipCode] nvarchar(max) NULL,
    [ContactId] int NOT NULL,
    CONSTRAINT [PK_contacts_addresses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_contacts_addresses_contacts_ContactId] FOREIGN KEY ([ContactId]) REFERENCES [contacts] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [contacts_phones] (
    [Id] int NOT NULL IDENTITY,
    [Phone] nvarchar(max) NOT NULL,
    [ContactId] int NOT NULL,
    CONSTRAINT [PK_contacts_phones] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_contacts_phones_contacts_ContactId] FOREIGN KEY ([ContactId]) REFERENCES [contacts] ([Id]) ON DELETE CASCADE
);

GO

CREATE UNIQUE INDEX [IX_contacts_Email] ON [contacts] ([Email]) WHERE [Email] IS NOT NULL;

GO

CREATE INDEX [IX_contacts_addresses_ContactId] ON [contacts_addresses] ([ContactId]);

GO

CREATE INDEX [IX_contacts_phones_ContactId] ON [contacts_phones] ([ContactId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190207224403_initial', N'2.1.4-rtm-31024');

GO

