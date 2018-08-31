CREATE TABLE [dbo].[LanguageText] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [LanguageId] INT            NOT NULL,
    [KeyName]    NVARCHAR (20)  NOT NULL,
    [Value]      NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_LanguageText] PRIMARY KEY CLUSTERED ([Id] ASC)
);

