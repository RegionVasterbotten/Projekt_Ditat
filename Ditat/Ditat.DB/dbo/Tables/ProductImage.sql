CREATE TABLE [dbo].[ProductImage] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [ProductId]    UNIQUEIDENTIFIER NOT NULL,
    [Path]         NVARCHAR (MAX)   NOT NULL,
    [PrimaryImage] BIT              NOT NULL,
    [Inserted]     DATETIME         CONSTRAINT [DF_ProductImages_Inserted] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_ProductImages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductImages_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([Id])
);

