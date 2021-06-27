
IF EXISTS (
			SELECT object_id 
			FROM sys.indexes 
			WHERE NAME='IX_Task_NonClustered' AND object_id = OBJECT_ID('[dbo].[TaskItem]')
		)
BEGIN
	DROP INDEX [TaskItem].[IX_Task_NonClustered]
END