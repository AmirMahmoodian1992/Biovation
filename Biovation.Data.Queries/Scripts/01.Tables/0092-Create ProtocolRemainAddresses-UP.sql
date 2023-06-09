CREATE TABLE Rly.ProtocolRemainAddresses
	(
	Id bigint NOT NULL,
	ProtocolCode int NOT NULL,
	RemainAddress nvarchar(MAX) NULL,
	OrderIndex int NULL,
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE Rly.ProtocolRemainAddresses ADD CONSTRAINT
	PK_ProtocolRemainAddresses PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Rly.ProtocolRemainAddresses SET (LOCK_ESCALATION = TABLE)
GO
