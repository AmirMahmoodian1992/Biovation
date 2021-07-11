
IF EXISTS (
			SELECT object_id 
			FROM sys.indexes 
			WHERE NAME='IX_Model_NonClustered' AND object_id = OBJECT_ID('[dbo].[Device]')
		)
BEGIN
	DROP INDEX [Device].[IX_Model_NonClustered]
END