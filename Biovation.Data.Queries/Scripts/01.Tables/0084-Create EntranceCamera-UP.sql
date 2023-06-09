
CREATE TABLE [Rly].[EntranceCamera]
	(
	CameraId int NOT NULL,
	EntranceId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE [Rly].[EntranceCamera] ADD CONSTRAINT
	PK_EntranceCamera PRIMARY KEY CLUSTERED 
	(
	CameraId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE [Rly].[EntranceCamera] SET (LOCK_ESCALATION = TABLE)
GO