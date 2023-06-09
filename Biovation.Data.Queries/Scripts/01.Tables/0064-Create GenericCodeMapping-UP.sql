
CREATE TABLE [dbo].[GenericCodeMapping]
	(
	[Id] int NOT NULL IDENTITY (1, 1),
	[CategoryId] int NOT NULL,
	[ManufactureCode] nvarchar(50) NOT NULL,
	[GenericCode] nvarchar(50) NOT NULL,
	[BrandCode] nvarchar(50) NOT NULL,
	[Description] nvarchar(100) NULL
	)  ON [PRIMARY]
GO

IF OBJECT_ID('PK_GenericCodeMapping', 'PK') IS NULL
BEGIN
ALTER TABLE dbo.GenericCodeMapping ADD CONSTRAINT
	PK_GenericCodeMapping PRIMARY KEY CLUSTERED (	Id	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO


--ALTER TABLE dbo.GenericCodeMapping ADD CONSTRAINT
--	FK_GenericCodeMapping_Lookup FOREIGN KEY (	BrandCode	) REFERENCES [dbo].[Lookup]	(  Code  ) ON UPDATE  NO ACTION 
--	 ON DELETE  NO ACTION 
--GO

IF OBJECT_ID('FK_GenericCodeMapping_GenericCodeMappingCategory', 'F') IS NULL
BEGIN
ALTER TABLE dbo.GenericCodeMapping ADD CONSTRAINT
	FK_GenericCodeMapping_GenericCodeMappingCategory FOREIGN KEY (	CategoryId	) REFERENCES [dbo].[GenericCodeMappingCategory]	(  Id  ) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
END
GO


GO