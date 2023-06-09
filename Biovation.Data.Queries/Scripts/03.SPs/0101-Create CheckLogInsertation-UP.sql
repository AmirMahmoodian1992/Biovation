
CREATE PROCEDURE [dbo].[CheckLogInsertation]
@LogTimes nvarchar(MAX), @DeviceId bigint
AS
BEGIN

		EXEC ('SELECT [Id],
		   [DeviceId],
           [EventId],
           [UserId],
           [DateTime] AS LogDateTime,
           [Ticks] AS DateTimeTicks,
           [TNAEvent],
		   [SuccessTransfer],

		   [MatchingTypeLookup].[Code] AS MatchingType_Code,
		   [MatchingTypeLookup].[Name] AS MatchingType_Name,
		   [MatchingTypeLookup].[OrderIndex] AS MatchingType_OrderIndex,
		   [MatchingTypeLookup].[Description] AS MatchingType_Description,

		   ISNULL([SubEventLookup].[Code], 17001) AS SubEvent_Code,
		   ISNULL([SubEventLookup].[Name], N''Normal'') AS SubEvent_Name,
		   ISNULL([SubEventLookup].[OrderIndex], 1) AS SubEvent_OrderIndex,
		   ISNULL([SubEventLookup].[Description], N''بدون حالت خاص انتخاب شده'') AS SubEvent_Description
		 FROM   [dbo].[Log] AS L LEFT JOIN [dbo].[Lookup] AS [SubEventLookup]
						 ON L.[SubEvent] = [SubEventLookup].[Code]
			     LEFT JOIN	[dbo].[Lookup] AS [MatchingTypeLookup]
			               ON L.[MatchingType] = [MatchingTypeLookup].[Code]
				WHERE [Ticks] IN' + @LogTimes +
                                      ' AND [DeviceId] = ' + @DeviceId +
                                        ' ORDER BY [DateTime] DESC')
END
GO
