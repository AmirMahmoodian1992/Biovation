
IF EXISTS (
			SELECT object_id 
			FROM sys.indexes 
			WHERE NAME='IX_User_NonClustered' AND object_id = OBJECT_ID('[dbo].[FingerTemplate]')
		)
BEGIN
	DROP INDEX [FingerTemplate].[IX_User_NonClustered]
END