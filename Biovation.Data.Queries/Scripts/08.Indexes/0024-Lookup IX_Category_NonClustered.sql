
IF NOT EXISTS (
			SELECT object_id 
			FROM sys.indexes 
			WHERE NAME='IX_Category_NonClustered' AND object_id = OBJECT_ID('[dbo].[Lookup]')
		)
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Category_NonClustered] ON [dbo].[Lookup]
	(
		[LookupCategoryId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90)
END