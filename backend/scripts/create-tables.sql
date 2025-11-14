USE [123Vendas];
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Vendas')
BEGIN
    CREATE TABLE [Vendas] (
        [Id] uniqueidentifier NOT NULL,
        [NumeroVenda] nvarchar(50) NOT NULL,
        [Data] datetime2 NOT NULL,
        [ClienteId] uniqueidentifier NOT NULL,
        [ClienteNome] nvarchar(200) NOT NULL,
        [FilialId] uniqueidentifier NOT NULL,
        [FilialNome] nvarchar(200) NOT NULL,
        [Status] int NOT NULL,
        [ValorTotal] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Vendas] PRIMARY KEY ([Id])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VendaItens')
BEGIN
    CREATE TABLE [VendaItens] (
        [Id] uniqueidentifier NOT NULL,
        [VendaId] uniqueidentifier NOT NULL,
        [ProdutoId] uniqueidentifier NOT NULL,
        [ProdutoNome] nvarchar(200) NOT NULL,
        [Quantidade] int NOT NULL,
        [ValorUnitario] decimal(18,2) NOT NULL,
        [Desconto] decimal(18,2) NOT NULL,
        [ValorTotalItem] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_VendaItens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VendaItens_Vendas_VendaId] FOREIGN KEY ([VendaId]) REFERENCES [Vendas] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_VendaItens_VendaId] ON [VendaItens] ([VendaId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ('20251113232616_InitialCreate', '8.0.4');
END
GO

