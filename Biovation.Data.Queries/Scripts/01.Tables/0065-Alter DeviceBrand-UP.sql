
IF OBJECT_ID('FK_FingerTemplateType_DeviceBrand', 'F') IS NOT NULL
BEGIN
ALTER TABLE [dbo].[FingerTemplateType]
	DROP CONSTRAINT [FK_FingerTemplateType_DeviceBrand]
END
GO

IF OBJECT_ID('FK_Task_DeviceBrand', 'F') IS NOT NULL
BEGIN
ALTER TABLE [dbo].[Task]
	DROP CONSTRAINT [FK_Task_DeviceBrand]
END
GO

IF OBJECT_ID('FK_DeviceModel_DeviceBrand', 'F') IS NOT NULL
BEGIN
ALTER TABLE [dbo].[DeviceModel]
	DROP CONSTRAINT [FK_DeviceModel_DeviceBrand]
END
GO

DECLARE @constrantName nvarchar(100),
		@dropQuery nvarchar(300)

SELECT @constrantName = [CONSTRAINT_NAME]
FROM information_schema.table_constraints  
WHERE constraint_type = 'PRIMARY KEY'   
AND table_name = 'DeviceBrand'

IF @constrantName IS NOT NULL
BEGIN
SELECT @dropQuery = 
'ALTER TABLE [dbo].[DeviceBrand]
	DROP CONSTRAINT ' + @constrantName

EXEC (@dropQuery)
END
GO

ALTER TABLE [dbo].[DeviceBrand]
	ALTER COLUMN [Id] nvarchar(50) NOT NULL
GO

ALTER TABLE [dbo].[DeviceBrand]
	ADD PRIMARY KEY ([Id])
GO