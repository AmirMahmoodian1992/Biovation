
IF EXISTS (
			SELECT object_id 
			FROM sys.indexes 
			WHERE NAME='IX_User_DateTime_SuccessTransfer_NonClustered' AND object_id = OBJECT_ID('[dbo].[Log]')
		)
BEGIN
	DROP INDEX [Log].[IX_User_DateTime_SuccessTransfer_NonClustered]
END