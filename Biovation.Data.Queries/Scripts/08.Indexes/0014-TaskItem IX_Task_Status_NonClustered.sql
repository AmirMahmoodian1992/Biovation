
IF NOT EXISTS (
			SELECT object_id 
			FROM sys.indexes 
			WHERE NAME='IX_Task_Status_NonClustered' AND object_id = OBJECT_ID('[dbo].[TaskItem]')
		)
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Task_Status_NonClustered] ON [dbo].[TaskItem]
	(
		[TaskId] DESC,
		[StatusCode] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90)
END