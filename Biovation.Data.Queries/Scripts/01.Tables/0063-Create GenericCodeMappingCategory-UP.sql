
CREATE TABLE [dbo].[GenericCodeMappingCategory]
	(
	[Id] int NOT NULL,
	[Name] nvarchar(50) NOT NULL,
	[Description] nvarchar(50) NULL
	)  ON [PRIMARY]
GO

IF OBJECT_ID('PK_GenericCodeMappingCategory', 'PK') IS NULL
BEGIN
ALTER TABLE dbo.GenericCodeMappingCategory ADD CONSTRAINT
	PK_GenericCodeMappingCategory PRIMARY KEY CLUSTERED 
	(  Id  ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO