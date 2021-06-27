ALTER TABLE [dbo].[User]
ALTER COLUMN StartDate DATETIME;

ALTER TABLE [dbo].[User]
ALTER COLUMN EndDate DATETIME;

ALTER TABLE [dbo].[User] DROP CONSTRAINT [DF_User_RegisterDate]

ALTER TABLE [dbo].[User]
ALTER COLUMN RegisterDate DATETIME NOT NULL;

ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_RegisterDate]  DEFAULT (getdate()) FOR [RegisterDate]