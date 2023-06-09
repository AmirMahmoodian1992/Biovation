CREATE TABLE Rly.Camera
	(
	Id bigint IDENTITY(1,1) NOT NULL,
	Code int NOT NULL,
	Name nvarchar(MAX) NULL,
	Ip nvarchar(50) NOT NULL,
	Port int NOT NULL,
	UserName nvarchar(MAX) NULL,
	Password nvarchar(MAX) NULL,
	MacAddress nvarchar(MAX) NULL,
	BrandCode int NOT NULL,
	Description nvarchar(MAX) NULL,
	Active bit NOT NULL,
	RegisterDate datetime NOT NULL,
	HardwareVersion nvarchar(MAX) NULL,
	SerialNumber nvarchar(MAX) NULL,
	ConnectionUrl nvarchar(MAX) NULL,
	LiveStreamUrl nvarchar(MAX) NULL,
	ModelId int NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE Rly.Camera ADD CONSTRAINT
	PK_Camera PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Rly.Camera SET (LOCK_ESCALATION = TABLE)
GO
