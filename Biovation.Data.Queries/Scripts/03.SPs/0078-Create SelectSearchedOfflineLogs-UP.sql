CREATE PROCEDURE [dbo].[SelectSearchedOfflineLogs]
@UserId INT, @DeviceID INT, @FromDate DATETIME=NULL, @ToDate DATETIME=NULL, @adminUserId INT=0, @State bit = NULL
AS
BEGIN
	IF(ISNULL(@adminUserId, 0) = 0
                  OR EXISTS (SELECT Id
                             FROM   [dbo].[USER]
                             WHERE  IsMasterAdmin = 1
                                    AND ID = @AdminUserId))
		BEGIN
						SELECT   l.[Id],
						 l.[DeviceId],
						 l.[UserId],
						 l.[DateTime] AS LogDateTime,
						 l.[Ticks],
						 l.[TNAEvent],
						 l.[InOutMode],
						 ISNULL([MatchingTypeLookup].[Code], 19000) AS MatchingType_Code,
		                 ISNULL([MatchingTypeLookup].[Name], N'Unknown') AS MatchingType_Name,
		                 ISNULL([MatchingTypeLookup].[OrderIndex], 1) AS MatchingType_OrderIndex,
		                 ISNULL([MatchingTypeLookup].[Description], N'نامشخص') AS MatchingType_Description,

						 l.SuccessTransfer,
						 isnull(u.FirstName, '') + ' ' + u.SurName AS 'SurName',
						 CONVERT (VARCHAR, l.[DateTime], 108) AS [Time],
						 d.Name AS DeviceName,
						 d.Code AS DeviceCode,
						 d.[DeviceTypeId] AS 'DeviceIOType',
						 
						 l.[EventId] AS EventLog_Code,
						 [LogEventLookUp].[Name] AS EventLog_Name,
					     [LogEventLookUp].[OrderIndex] AS EventLog_OrderIndex,
					     [LogEventLookUp].[Description] AS EventLog_Description,

						 ISNULL([SubEventLookup].[Code], 17001) AS SubEvent_Code,
					     ISNULL([SubEventLookup].[Name], N'Normal') AS SubEvent_Name,
					     ISNULL([SubEventLookup].[OrderIndex], 1) AS SubEvent_OrderIndex,
					     ISNULL([SubEventLookup].[Description], N'بدون حالت خاص انتخاب شده') AS SubEvent_Description
				FROM     [dbo].[Log] AS l
						 INNER JOIN
						 [dbo].[Device] AS d
						 ON l.DeviceId = d.ID
						 LEFT OUTER JOIN
						 [dbo].[User] AS u
						 ON l.UserId = u.Code						
						 LEFT JOIN [dbo].[Lookup] AS [SubEventLookup]
						 ON L.[SubEvent] = [SubEventLookup].[Code]
						  LEFT OUTER JOIN
                   [dbo].[Lookup] AS [LogEventLookUp]
                   ON L.[EventId]  = [LogEventLookUp].[Code]
				    LEFT JOIN	[dbo].[Lookup] AS [MatchingTypeLookup]
			           ON L.[MatchingType] = [MatchingTypeLookup].[Code]

				WHERE    (l.[DeviceId] = @DeviceID
						  OR COALESCE (@DeviceID, '') = '')
						 AND (l.[UserId] = @UserId
							  OR COALESCE (@UserId, '') = '')
						 AND (isnull(@fromDate, '') = ''
							  OR CONVERT (DATE, l.[DateTime]) >= CONVERT (DATE, @FromDate))
						 AND (isnull(@todate, '') = ''
							  OR CONVERT (DATE, l.[DateTime]) <= CONVERT (DATE, @ToDate))
						 AND (EventId = 16003 or EventId = 16004) and l.UserId <> -1
						 AND (l.SuccessTransfer = ISNULL(@State, 1)
							  OR @State IS NULL)
				ORDER BY [DateTime] DESC
		END
		ELSE
		BEGIN 
				SELECT DISTINCT
				         l.[Id],
						 l.[DeviceId],
						 l.[UserId],
						 l.[DateTime] AS LogDateTime,
						 l.[Ticks],
						 l.[TNAEvent],
						 l.[InOutMode],
						 l.[MatchingType],
						 l.SuccessTransfer,
						 isnull(u.FirstName, '') + ' ' + u.SurName AS 'SurName',
						 CONVERT (VARCHAR, l.[DateTime], 108) AS [Time],
						 d.Name AS DeviceName,
						 d.Code AS DeviceCode,
						 d.[DeviceTypeId] AS 'DeviceIOType',

						 ISNULL([MatchingTypeLookup].[Code], 19000) AS MatchingType_Code,
		                 ISNULL([MatchingTypeLookup].[Name], N'Unknown') AS MatchingType_Name,
		                 ISNULL([MatchingTypeLookup].[OrderIndex], 1) AS MatchingType_OrderIndex,
		                 ISNULL([MatchingTypeLookup].[Description], N'نامشخص') AS MatchingType_Description,


						 l.[EventId] AS EventLog_Code,
						[LogEventLookUp].[Name] AS EventLog_Name,
						[LogEventLookUp].[OrderIndex] AS EventLog_OrderIndex,
						[LogEventLookUp].[Description] AS EventLog_Description,

						 ISNULL([SubEventLookup].[Code], 17001) AS SubEvent_Code,
					     ISNULL([SubEventLookup].[Name], N'Normal') AS SubEvent_Name,
					     ISNULL([SubEventLookup].[OrderIndex], 1) AS SubEvent_OrderIndex,
					     ISNULL([SubEventLookup].[Description], N'بدون حالت خاص انتخاب شده') AS SubEvent_Description
				FROM     [dbo].[Log] AS l
						 INNER JOIN
						 [dbo].[Device] AS d
						 ON l.DeviceId = d.ID
						 LEFT OUTER JOIN
						 [dbo].[User] AS u
						 ON l.UserId = u.Code
						 INNER JOIN
						 [dbo].[DeviceGroupMember] AS dgm
						 ON dgm.DeviceId = d.Id
						 INNER JOIN
						 [dbo].[DeviceGroup] AS dg
						 ON dg.Id = dgm.GroupId
						 INNER JOIN
						 [dbo].[AccessGroupDevice] AS agd
						 ON agd.DeviceGroupId = dg.Id
						 INNER JOIN
						 [dbo].[AccessGroup] AS ag
						 ON ag.Id = agd.AccessGroupId
						 INNER JOIN
						 [dbo].[AdminAccessGroup] AS AAG
						 ON ag.Id = AAG.AccessGroupId
						 LEFT JOIN [dbo].[Lookup] AS [SubEventLookup]
						 ON L.[SubEvent] = [SubEventLookup].[Code]
						 LEFT JOIN	[dbo].[Lookup] AS [MatchingTypeLookup]
			              ON L.[MatchingType] = [MatchingTypeLookup].[Code]
					LEFT OUTER JOIN
                   [dbo].[Lookup] AS [LogEventLookUp]
                   ON L.[EventId]  = [LogEventLookUp].[Code]
						 --INNER JOIN
						 --[dbo].[UserGroupMember] AS ugm
						 --ON ugm.UserId = u.ID
						 --INNER JOIN
						 --[dbo].[UserGroup] AS ug
						 --ON ug.Id = ugm.GroupId
						 --INNER JOIN
						 --[dbo].[AccessGroupUser] AS agu
						 --ON agu.UserGroupId = ug.Id
						 --INNER JOIN
						 --[dbo].[AccessGroup] AS ag2
						 --ON ag2.Id = agd.AccessGroupId
						 -- INNER JOIN
						 --[dbo].[AdminAccessGroup] AS AAG2
						 --ON ag2.Id = AAG2.AccessGroupId

				WHERE    (l.[DeviceId] = @DeviceID
						  OR COALESCE (@DeviceID, '') = '')
						 AND (l.[UserId] = @UserId
							  OR COALESCE (@UserId, '') = '')
						 AND (isnull(@fromDate, '') = ''
							  OR CONVERT (DATE, l.[DateTime]) >= CONVERT (DATE, @FromDate))
						 AND (isnull(@todate, '') = ''
							  OR CONVERT (DATE, l.[DateTime]) <= CONVERT (DATE, @ToDate))
						 AND (EventId = 16003 or EventId = 16004) and l.UserId <> -1
						 AND (AAG.UserId = @adminUserId)
						 AND (l.SuccessTransfer = ISNULL(@State, 1)
							  OR @State IS NULL)
				ORDER BY [DateTime] DESC
	END
END

