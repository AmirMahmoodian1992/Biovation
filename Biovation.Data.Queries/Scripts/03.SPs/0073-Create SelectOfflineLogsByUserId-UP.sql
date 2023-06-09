
CREATE PROCEDURE [dbo].[SelectOfflineLogsByUserId]
@UserId INT
AS
BEGIN
    SELECT [Id],
		   [DeviceId],
           [EventId],
           [UserId],
           [DateTime],
           [Ticks],
           [TNAEvent],
		   [InOutMode],
		   [SuccessTransfer],


		   [MatchingTypeLookup].[Code] AS MatchingType_Code,
		   [MatchingTypeLookup].[Name] AS MatchingType_Name,
		   [MatchingTypeLookup].[OrderIndex] AS MatchingType_OrderIndex,
		   [MatchingTypeLookup].[Description] AS MatchingType_Description,
		   
           [EventId] AS EventLog_Code,
		   [LogEventLookUp].[Name] AS EventLog_Name,
		   [LogEventLookUp].[OrderIndex] AS EventLog_OrderIndex,
		   [LogEventLookUp].[Description] AS EventLog_Description,
		
           ISNULL([SubEventLookup].[Code], 17001) AS SubEvent_Code,
		   ISNULL([SubEventLookup].[Name], N'Normal') AS SubEvent_Name,
		   ISNULL([SubEventLookup].[OrderIndex], 1) AS SubEvent_OrderIndex,
		   ISNULL([SubEventLookup].[Description], N'بدون حالت خاص انتخاب شده') AS SubEvent_Description
    FROM   [dbo].[Log] AS L LEFT JOIN [dbo].[Lookup] AS [SubEventLookup]
			ON L.[SubEvent] = [SubEventLookup].[Code]
			  LEFT JOIN
			 [dbo].[Lookup] AS [MatchingTypeLookup]
			ON L.[MatchingType] = [MatchingTypeLookup].[Code]
			 LEFT OUTER JOIN
                   [dbo].[Lookup] AS [LogEventLookUp]
                   ON L.[EventId]  = [LogEventLookUp].[Code]
		   
    WHERE  [UserId] = @UserId;
END
GO
