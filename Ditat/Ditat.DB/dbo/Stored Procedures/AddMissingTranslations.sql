
CREATE PROC dbo.AddMissingTranslations
as

-- Categories
INSERT INTO [dbo].[LanguageText]
           ([LanguageId]
           ,[KeyName]
           ,[Value])
SELECT
	L.Id			[LanguageId],	
	C.Name			[KeyName],
	'Missing translation: ' + C.Name		[Value]

FROM 
	dbo.Category C,
	dbo.Language L

EXCEPT
SELECT
	LT.LanguageId			[LanguageId],
	LT.KeyName		[KeyName]   ,	
	'Missing translation: ' + LT.KeyName		[Value] 
FROM dbo.LanguageText LT