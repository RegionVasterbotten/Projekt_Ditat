CREATE TABLE [dbo].[CategoryProperty] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [CategoryId] INT            NULL,
    [Name]       NVARCHAR (200) NULL,
    [SortValue]  INT            NOT NULL,
    [Creator]    INT            NULL,
    [Created]    NCHAR (10)     NULL,
    CONSTRAINT [PK_CategoryProperty] PRIMARY KEY CLUSTERED ([Id] ASC)
);

