CREATE TABLE [Rly].RelayHubModel
	(
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(MAX) NOT NULL,
	ManufactureCode int NOT NULL,
	BrandId int NOT NULL,
	DefaultCapacity int NOT NULL,
	DefaultPortNumber int NULL,
	Description nvarchar(MAX) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [Rly].RelayHubModel ADD CONSTRAINT
	PK_RelayHubModel PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE [Rly].RelayHubModel SET (LOCK_ESCALATION = TABLE)
GO