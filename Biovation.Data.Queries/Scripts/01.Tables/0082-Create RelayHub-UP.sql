/*
   Monday, February 15, 20214:23:54 PM
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
CREATE TABLE [Rly].RelayHub
	(
	Id int IDENTITY(1,1) NOT NULL,
	IpAddress nvarchar(50) NOT NULL,
	Port int NOT NULL,
	Capacity int NOT NULL,
	RelayHubModelId int NOT NULL,
	Description nvarchar(MAX) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [Rly].RelayHub ADD CONSTRAINT
	PK_RelayHub PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE [Rly].RelayHub SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
