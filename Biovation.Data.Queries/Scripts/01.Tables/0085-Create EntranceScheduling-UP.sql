CREATE TABLE [Rly].[EntranceScheduling]
	(
	SchedulingId int NOT NULL,
	EntranceId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE [Rly].[EntranceScheduling] SET (LOCK_ESCALATION = TABLE)
GO
