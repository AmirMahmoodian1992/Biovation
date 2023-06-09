
--BEGIN TRANSACTION
--GO
CREATE TABLE dbo.Tmp_Log
	(
	Id bigint NOT NULL IDENTITY (1, 1),
	DeviceId int NOT NULL,
	EventId int NOT NULL,
	UserId bigint NOT NULL,
	DateTime datetime NOT NULL,
	Ticks bigint NOT NULL,
	SubEvent int NOT NULL,
	TNAEvent int NOT NULL,
	InOutMode int NULL,
	MatchingType int NOT NULL,
	Image nvarchar(2000) NULL,
	SuccessTransfer bit NULL,
	CreateAt datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Log SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Log ON
GO
IF EXISTS(SELECT * FROM dbo.[Log])
	 EXEC('INSERT INTO dbo.Tmp_Log (Id, DeviceId, EventId, UserId, DateTime, Ticks, SubEvent, TNAEvent, InOutMode, MatchingType, Image, SuccessTransfer, CreateAt)
		SELECT Id, DeviceId, EventId, CONVERT(bigint, UserId), DateTime, Ticks, SubEvent, TNAEvent, InOutMode, MatchingType, Image, SuccessTransfer, CreateAt FROM dbo.[Log] WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Log OFF
GO
DROP TABLE dbo.[Log]
GO
EXECUTE sp_rename N'dbo.Tmp_Log', N'Log', 'OBJECT' 
GO
ALTER TABLE dbo.[Log] ADD CONSTRAINT
	PK_Log PRIMARY KEY CLUSTERED 
	(
	DateTime,
	UserId,
	DeviceId,
	EventId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_Id_NonClustered ON dbo.[Log]
	(
	Id DESC
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_Event_NonClustered ON dbo.[Log]
	(
	EventId
	) INCLUDE (Id, Ticks, SubEvent, TNAEvent, InOutMode, MatchingType, SuccessTransfer) 
 WITH( PAD_INDEX = OFF, FILLFACTOR = 80, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_Device_Event_NonClustered ON dbo.[Log]
	(
	DeviceId,
	EventId
	) INCLUDE (Id, SuccessTransfer) 
 WITH( PAD_INDEX = OFF, FILLFACTOR = 80, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
--COMMIT
