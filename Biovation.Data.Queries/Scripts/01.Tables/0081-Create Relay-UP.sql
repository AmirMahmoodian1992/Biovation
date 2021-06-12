/*
   Monday, February 15, 20214:45:53 PM
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
CREATE TABLE [Rly].Relay
	(
	Id int IDENTITY(1,1),
	Name nvarchar(100) NULL,
	NodeNumber int NOT NULL,
	RelayHubId int NOT NULL,
	EntranceId int NOT NULL,
	Description nvarchar(MAX) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [Rly].Relay ADD CONSTRAINT
	PK_Relay PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE [Rly].Relay SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
