
CREATE TABLE Rly.Tmp_RelayHubModel
	(
	[Id] int NOT NULL,
	[Name] nvarchar(MAX) NOT NULL,
	[ManufactureCode] int NOT NULL,
	[BrandId] int NOT NULL,
	[DefaultCapacity] int NOT NULL,
	[DefaultPortNumber] int NULL,
	[Description] nvarchar(MAX) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE Rly.Tmp_RelayHubModel SET (LOCK_ESCALATION = TABLE)
GO

IF EXISTS(SELECT * FROM Rly.RelayHubModel)
	 EXEC('INSERT INTO Rly.Tmp_RelayHubModel (Id, Name, ManufactureCode, BrandId, DefaultCapacity, DefaultPortNumber, Description)
		SELECT Id, Name, ManufactureCode, BrandId, DefaultCapacity, DefaultPortNumber, Description FROM Rly.RelayHubModel WITH (HOLDLOCK TABLOCKX)')
GO

DROP TABLE Rly.RelayHubModel
GO

EXECUTE sp_rename N'Rly.Tmp_RelayHubModel', N'RelayHubModel', 'OBJECT' 
GO

ALTER TABLE Rly.RelayHubModel ADD CONSTRAINT
	PK_RelayHubModel PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
