CREATE TABLE [Rly].[RelayScheduling]
	(
	RelayId int NOT NULL,
	SchedulingId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE [Rly].[RelayScheduling] SET (LOCK_ESCALATION = TABLE)
GO
