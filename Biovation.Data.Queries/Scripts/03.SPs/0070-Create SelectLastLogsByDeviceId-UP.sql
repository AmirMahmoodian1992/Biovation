
CREATE PROCEDURE [dbo].[SelectLastLogsByDeviceId]
@DeviceId BIGINT
AS
BEGIN
    SELECT   TOP 5 [Id],
				   [DeviceId],
                   [EventId],
                   [UserId],
                   [DateTime],
                   [Ticks],
                   [TNAEvent],
				   [InOutMode],

           [MatchingTypeLookup].[Code] AS MatchingType_Code,
		   [MatchingTypeLookup].[Name] AS MatchingType_Name,
		   [MatchingTypeLookup].[OrderIndex] AS MatchingType_OrderIndex,
		   [MatchingTypeLookup].[Description] AS MatchingType_Description,           

                   ISNULL([SubEventLookup].[Code], 17001) AS SubEvent_Code,
				   ISNULL([SubEventLookup].[Name], N'Normal') AS SubEvent_Name,
				   ISNULL([SubEventLookup].[OrderIndex], 1) AS SubEvent_OrderIndex,
				   ISNULL([SubEventLookup].[Description], N'بدون حالت خاص انتخاب شده') AS SubEvent_Description
    FROM     [dbo].[Log] AS L LEFT JOIN [dbo].[Lookup] AS [SubEventLookup]
			ON L.[SubEvent] = [SubEventLookup].[Code]
         LEFT JOIN	[dbo].[Lookup] AS [MatchingTypeLookup]
			ON L.[MatchingType] = [MatchingTypeLookup].[Code]
    WHERE    [DeviceId] = @DeviceId
			AND [EventId] IN ('16003', '16004')
    ORDER BY [DateTime] DESC;
END
GO
