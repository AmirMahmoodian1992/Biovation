
IF NOT EXISTS (
			SELECT object_id 
			FROM sys.indexes 
			WHERE NAME='IX_User_TemplateType_NonClustered' AND object_id = OBJECT_ID('[dbo].[FingerTemplate]')
		)
BEGIN
	CREATE NONCLUSTERED INDEX [IX_User_TemplateType_NonClustered] ON [dbo].[FingerTemplate]
	(
		[UserId] ASC,
		[FingerTemplateType] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90)
END