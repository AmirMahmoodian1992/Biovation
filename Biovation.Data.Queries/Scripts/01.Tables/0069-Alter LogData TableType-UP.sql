IF type_id('[dbo].[LogTable]') IS NULL
CREATE TYPE [dbo].[LogTable] AS TABLE(
	[Id] [int] NOT NULL,
	[DeviceId] [int] NOT NULL,
	[DeviceCode] [bigint] NOT NULL,
	[EventId] [int] NOT NULL,
	[UserId] [bigint] NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[Ticks] [bigint] NOT NULL,
	[SubEvent] [int] NOT NULL,
	[TNAEvent] [int] NOT NULL,
	[InOutMode] [int] NOT NULL,
	[MatchingType] [int] NOT NULL,
	[SuccessTransfer] [bit] NULL
)
ELSE
BEGIN
	EXEC sys.sp_rename '[dbo].[LogTable]', 'LogTable_';
	
	CREATE TYPE [dbo].[LogTable] AS TABLE(
	[Id] [int] NOT NULL,
	[DeviceId] [int] NOT NULL,
	[DeviceCode] [bigint] NOT NULL,
	[EventId] [int] NOT NULL,
	[UserId] [bigint] NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[Ticks] [bigint] NOT NULL,
	[SubEvent] [int] NOT NULL,
	[TNAEvent] [int] NOT NULL,
	[InOutMode] [int] NOT NULL,
	[MatchingType] [int] NOT NULL,
	[SuccessTransfer] [bit] NULL
)

	DECLARE @Name NVARCHAR(776);

	DECLARE REF_CURSOR CURSOR FOR
	SELECT referencing_schema_name + '.' + referencing_entity_name
	FROM sys.dm_sql_referencing_entities('[dbo].[LogTable]', 'TYPE');

	OPEN REF_CURSOR;

	FETCH NEXT FROM REF_CURSOR INTO @Name;
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		EXEC sys.sp_refreshsqlmodule @name = @Name;
		FETCH NEXT FROM REF_CURSOR INTO @Name;
	END;

	CLOSE REF_CURSOR;
	DEALLOCATE REF_CURSOR;

	DROP TYPE [dbo].[LogTable_];
END