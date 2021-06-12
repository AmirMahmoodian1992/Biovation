/*
   Monday, February 15, 20214:51:41 PM
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
CREATE TABLE [Rly].[RelayScheduling]
	(
	RelayId int NOT NULL,
	SchedulingId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE [Rly].[RelayScheduling] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
