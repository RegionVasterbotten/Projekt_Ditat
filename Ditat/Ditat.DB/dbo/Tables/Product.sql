CREATE TABLE [dbo].[Product] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [CategoryId] INT              NULL,
    [Name]       NVARCHAR (50)    NULL,
    [Barcode]    NVARCHAR (50)    NULL,
    [Creator]    NVARCHAR (50)    NULL,
    [Created]    NVARCHAR (50)    NULL,
    [Price]      NVARCHAR (50)    NULL,
    [Category]   NVARCHAR (50)    NULL,
    [Properties] NVARCHAR (MAX)   NULL,
    [Inserted]   DATETIME         CONSTRAINT [DF_Products_Inserted] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [Product Properies should be formatted as JSON] CHECK (isjson([Properties])=(1)),
    CONSTRAINT [FK_Product_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([Id])
);

