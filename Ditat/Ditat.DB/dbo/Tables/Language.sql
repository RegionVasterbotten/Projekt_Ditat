CREATE TABLE [dbo].[Language] (
    [Id]        INT           NOT NULL,
    [Name]      NVARCHAR (50) NOT NULL,
    [Shortname] NVARCHAR (10) NULL,
    [Active]    BIT           NOT NULL,
    CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED ([Id] ASC)
);

