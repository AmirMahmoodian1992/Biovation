
CREATE TABLE [Rly].Scheduling
	(
	Id int IDENTITY(1,1),
	StartTime BIGINT NOT NULL,
	EndTime BIGINT NOT NULL,
	Mode int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE [Rly].Scheduling ADD CONSTRAINT
	PK_Scheduling PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE [Rly].Scheduling SET (LOCK_ESCALATION = TABLE)
GO
