CREATE TABLE Rly.CameraModel
	(
	Id bigint NOT NULL,
	Name nvarchar(MAX) NULL,
	ManufactureCode int NULL,
	BrandCode int NOT NULL,
	Description nvarchar(MAX) NULL,
	RegisterDate datetime NOT NULL,
	DefaultPortNumber int NULL,
	DefaultUserName nvarchar(MAX) NULL,
	DefaultPassword nvarchar(MAX) NULL,
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE Rly.CameraModel ADD CONSTRAINT
	PK_CameraModel PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Rly.CameraModel SET (LOCK_ESCALATION = TABLE)
GO
