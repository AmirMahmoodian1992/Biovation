
IF NOT EXISTS (
			SELECT object_id 
			FROM sys.indexes 
			WHERE NAME='IX_Brand_NonClustered' AND object_id = OBJECT_ID('[dbo].[Task]')
		)
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Brand_NonClustered] ON [dbo].[Task]
	(
		[DeviceBrandId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90)
END