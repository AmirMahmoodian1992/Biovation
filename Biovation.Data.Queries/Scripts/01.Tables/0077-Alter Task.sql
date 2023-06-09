
IF COL_LENGTH('dbo.Task', 'UpdatedBy') IS NULL
BEGIN
ALTER TABLE [dbo].[Task] ADD [UpdatedBy] INT NULL
END
GO

IF COL_LENGTH('dbo.Task', 'UpdatedAt') IS NULL
BEGIN
ALTER TABLE [dbo].[Task] ADD [UpdatedAt] DATETIMEOFFSET(7) NULL
END
GO 

IF COL_LENGTH('dbo.Task', 'QueuedAt') IS NULL
BEGIN
ALTER TABLE [dbo].[Task] ADD [QueuedAt] DATETIMEOFFSET(7) NULL
END
GO

IF COL_LENGTH('dbo.Task', 'SchedulingPattern') IS NULL
BEGIN
ALTER TABLE [dbo].[Task] ADD [SchedulingPattern] NVARCHAR(50) NULL
END
GO

IF COL_LENGTH('dbo.Task', 'DueDate') IS NULL
BEGIN
ALTER TABLE [dbo].[Task] ADD [DueDate] DATETIMEOFFSET(7) NULL
END
GO