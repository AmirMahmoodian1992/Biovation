CREATE PROCEDURE [dbo].[GetLog]
@Id bigint
AS
BEGIN
   
	SELECT [L].[Id],
		   [L].[DeviceId],
           [L].[EventId],
           [L].[UserId],
           [L].[DateTime],
           [L].[Ticks],
           [L].[TNAEvent],
		   [L].[InOutMode],
		   [L].[SuccessTransfer],
		   [L].[Image],
           [MatchingTypeLookup].[Code] AS MatchingType_Code,
		   [MatchingTypeLookup].[Name] AS MatchingType_Name,
		   [MatchingTypeLookup].[OrderIndex] AS MatchingType_OrderIndex,
		   [MatchingTypeLookup].[Description] AS MatchingType_Description,
		   ISNULL([SubEventLookup].[Code], 17001) AS SubEvent_Code,
		   ISNULL([SubEventLookup].[Name], N'Normal') AS SubEvent_Name,
		   ISNULL([SubEventLookup].[OrderIndex], 1) AS SubEvent_OrderIndex,
		   ISNULL([SubEventLookup].[Description], N'بدون حالت خاص انتخاب شده') AS SubEvent_Description
	  FROM   [dbo].[Log] AS L
      LEFT JOIN [dbo].[Lookup] AS [SubEventLookup] ON L.[SubEvent] = [SubEventLookup].[Code]
	  LEFT JOIN	[dbo].[Lookup] AS [MatchingTypeLookup]
			ON L.[MatchingType] = [MatchingTypeLookup].[Code]

	WHERE l.Id = @Id
END