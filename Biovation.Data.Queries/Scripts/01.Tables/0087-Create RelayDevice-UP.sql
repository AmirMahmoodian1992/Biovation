CREATE TABLE [Rly].[RelayDevice]
	(
	RelayId int NOT NULL,
	DeviceId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE [Rly].[RelayDevice] SET (LOCK_ESCALATION = TABLE)
GO
