/*
   Monday, February 15, 20214:55:19 PM
   User: sa
   Server: PT-ASARHADI
   Database: Biovation_test
   Application: 
*/

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.[EntranceDevice]
	(
	DeviceId int NOT NULL,
	EntranceId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.[EntranceDevice] ADD CONSTRAINT
	[EntranceDevice] PRIMARY KEY CLUSTERED 
	(
	DeviceId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.[EntranceDevice] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
