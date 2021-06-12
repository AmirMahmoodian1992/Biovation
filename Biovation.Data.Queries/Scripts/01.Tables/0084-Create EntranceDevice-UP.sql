/*
   Monday, February 15, 20214:55:19 PM
   User: sa
   Server: PT-ASARHADI
   Database: Biovation_test
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
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
CREATE TABLE [Rly].[EntranceDevice]
	(
	DeviceId int NOT NULL,
	EntranceId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE [Rly].[EntranceDevice] ADD CONSTRAINT
	PK_EntranceDevice PRIMARY KEY CLUSTERED 
	(
	DeviceId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE [Rly].[EntranceDevice] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT


