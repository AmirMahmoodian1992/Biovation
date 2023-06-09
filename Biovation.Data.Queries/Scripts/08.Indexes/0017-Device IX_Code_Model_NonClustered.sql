
IF NOT EXISTS (
			SELECT object_id 
			FROM sys.indexes 
			WHERE NAME='IX_Code_Model_NonClustered' AND object_id = OBJECT_ID('[dbo].[Device]')
		)
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Code_Model_NonClustered] ON [dbo].[Device]
	(
		[DeviceModelId] ASC,
		[Code] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90)
END