
--BEGIN TRANSACTION
--GO
CREATE TABLE dbo.Tmp_Log
	(
	Id bigint NOT NULL IDENTITY (1, 1),
	DeviceId int NOT NULL,
	EventId int NOT NULL,
	UserId int NOT NULL,
	[DateTime] datetime NOT NULL,
	Ticks bigint NOT NULL,
	SubEvent int NOT NULL,
	TNAEvent int NOT NULL,
	InOutMode int NULL,
	MatchingType int NOT NULL,
	[Image] nvarchar(2000) NULL,
	SuccessTransfer bit NULL,
	CreateAt datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Log SET (LOCK_ESCALATION = TABLE)
GO
--SET IDENTITY_INSERT dbo.Tmp_Log ON
--GO
IF EXISTS(SELECT * FROM dbo.[Log])
	 EXEC('INSERT INTO dbo.Tmp_Log (DeviceId, EventId, UserId, DateTime, Ticks, SubEvent, TNAEvent, MatchingType, Image, SuccessTransfer, CreateAt)
		SELECT Distinct DeviceId, EventId, UserId, [DateTime], MAX(Ticks), MAX(SubEvent), 0 AS TNAEvent, MAX(MatchingType), MAX(Image), MAX(CAST(SuccessTransfer AS tinyint)) AS SuccessTransfer, MAX(CreateAt) FROM dbo.[Log] WITH (HOLDLOCK TABLOCKX) Group By DeviceId, EventId, UserId, [DateTime] ORDER BY [DateTime] ASC')
GO
--SET IDENTITY_INSERT dbo.Tmp_Log OFF
--GO
DROP TABLE dbo.[Log]
GO
EXECUTE sp_rename N'dbo.Tmp_Log', N'Log', 'OBJECT' 
GO
ALTER TABLE dbo.[Log] ADD CONSTRAINT
	PK_Log PRIMARY KEY CLUSTERED 
	(
	[DateTime],
	UserId,
	DeviceId,
	EventId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
--COMMIT
