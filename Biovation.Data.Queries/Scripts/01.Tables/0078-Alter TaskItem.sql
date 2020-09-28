IF COL_LENGTH('dbo.TaskItem', 'CurrentIndex') IS NULL
BEGIN
ALTER TABLE [dbo].[TaskItem] ADD [CurrentIndex]INT NULL
END
GO

IF COL_LENGTH('dbo.TaskItem', 'TotalCount') IS NULL
BEGIN
ALTER TABLE [dbo].[TaskItem] ADD [TotalCount]INT NULL
END
GO


IF COL_LENGTH('ddbo.TaskItem', 'ExecutionAt') IS NULL
BEGIN
ALTER TABLE [dbo].[TaskItem] ADD [ExecutionAt]DATETIMEOFFSET(7) NULL
END
GO 


IF COL_LENGTH('dbo.TaskItem', 'FinishedAt') IS NULL
BEGIN
ALTER TABLE [dbo].[TaskItem] ADD [FinishedAt]DATETIMEOFFSET(7) NULL
END
GO 