
IF EXISTS (
			SELECT object_id 
			FROM sys.indexes 
			WHERE NAME='IX_Device_DateTime_NonClustered' AND object_id = OBJECT_ID('[dbo].[Log]')
		)
BEGIN
	DROP INDEX [Log].[IX_Device_DateTime_NonClustered]
END