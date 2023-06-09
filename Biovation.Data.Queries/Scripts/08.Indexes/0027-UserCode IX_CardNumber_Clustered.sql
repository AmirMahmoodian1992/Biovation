
IF NOT EXISTS (
			SELECT object_id 
			FROM sys.indexes 
			WHERE NAME='IX_CardNumber_Clustered' AND object_id = OBJECT_ID('[dbo].[UserCard]')
		)
BEGIN
	CREATE CLUSTERED INDEX [IX_CardNumber_Clustered] ON [dbo].[UserCard]
	(
		[CardNum] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90)
END