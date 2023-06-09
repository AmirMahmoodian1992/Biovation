ALTER PROCEDURE [dbo].[SelectSearchedOfflineLogsWithPaging]
@UserId INT, @DeviceGroupId INT, @DeviceID INT, @State BIT=NULL, @FromDate DATETIME=NULL, @ToDate DATETIME=NULL, @PageSize INT, @PageNumber INT, @Where NVARCHAR (MAX)='', @Order NVARCHAR (1000)='', @adminUserId INT=0
AS
BEGIN
    DECLARE @skip AS INT, @fromRow AS INT, @toRow AS INT, @SqlQuery AS NVARCHAR (MAX);
    DECLARE @total AS INT;
    SET @toRow = ((@PageNumber - 1) * @PageSize) + @PageSize;
    SET @fromRow = CASE WHEN @PageSize = 0 THEN 0 ELSE ((@PageNumber - 1) * @PageSize) + 1 END;
    DECLARE @ParmDefinition AS NVARCHAR (500);
    DECLARE @count AS INT;
    DECLARE @SqlQuery2 AS NVARCHAR (MAX);
    SET @where = REPLACE(@where, 'DeviceCode', 'd.Code');
    SET @where = REPLACE(@where, 'DeviceName', 'd.Name');
    SET @where = REPLACE(@where, 'True', '1');
    SET @where = REPLACE(@where, 'False', '0');
    SET @where = REPLACE(@where, 'SLogDateTime', 'DateTime');
    SET @where = REPLACE(@where, 'UserCode', 'UserId');
    SET @where = REPLACE(@where, 'UserId', 'l.UserId');
    SET @where = REPLACE(@where, 'MatchingTypeTitle', '[MatchingTypeLookup].[Description]');
    SET @Order = REPLACE(@order, 'DeviceCode', 'd.Code');
    SET @Order = REPLACE(@order, 'DeviceName', 'd.Name');
    SET @Order = REPLACE(@order, 'UserCode', 'UserId');
    SET @Order = REPLACE(@order, 'UserId', 'l.UserId');
    SET @Order = REPLACE(@order, 'SLogDateTime', 'DateTime');
    SET @Order = REPLACE(@order, 'MatchingTypeTitle', '[MatchingTypeLookup].[Code]');
    IF (ISNULL(@adminUserId, 0) = 0
        OR EXISTS (SELECT top 1 1
                   FROM   [dbo].[USER]
                   WHERE  IsMasterAdmin = 1
                          AND [Id] = @AdminUserId))
        BEGIN
            SET @SqlQuery = N'SELECT @total = COUNT(1) FROM (SELECT DISTINCT l.Id FROM [dbo].[Log] l
			  inner join [dbo].[Device] d on l.DeviceId=d.ID
			  LEFT OUTER join [dbo].[DeviceGroupMember] dgm on d.ID=dgm.DeviceId
			  LEFT OUTER join [dbo].[User] u on l.UserId=u.Code
			  LEFT JOIN	[dbo].[Lookup] AS [MatchingTypeLookup]
			   ON l.[MatchingType] = [MatchingTypeLookup].[Code]
				WHERE     
					(l.[DeviceId] = @DeviceID or  Coalesce(@DeviceID,'''') = '''') AND 
					(dgm.GroupId = @DeviceGroupId or  Coalesce(@DeviceGroupId,'''') = '''') AND 
					--(isnull(@deviceId , 0) = 0 OR @DeviceID = D.ID) AND
					(		(@state=1 and @userid<>0 and u.id=@userid  and l.SuccessTransfer = @state) 
		or
		(@state=1 and @userid=0 and l.userid<>-1 and l.SuccessTransfer = @state)
		or
		(@state=0 and @userid<>0 and u.id=@userid and l.SuccessTransfer = @state)
		or
		(@state=0 and @userid = 0 and l.SuccessTransfer = @state)
		or
		(@State IS NULL and @userid<>0 and u.id=@userid)
		or
		(@State IS NULL and @userid=0 and l.userid<>-1)) AND 					
					(isnull(@fromDate , '''') = '''' or convert(date,l.[DateTime]) >= convert(date,@FromDate) ) and
					(isnull(@todate , '''') = '''' or convert(date,l.[DateTime]) <= convert(date,@ToDate) ) and (EventId = 16003 or EventId = 16004)  ' + @Where + ' ) AS Logs';
            SET @ParmDefinition = N'@DeviceGroupId int, @DeviceID int, @State Bit, @fromRow int ,@toRow int,@UserId int,@FromDate datetime,@ToDate datetime,@adminUserId int,@total int OUTPUT';
            EXECUTE sp_executesql @SqlQuery, @ParmDefinition,@DeviceGroupId=@DeviceGroupId, @DeviceID = @DeviceID, @State = @State, @fromRow = @fromRow, @toRow = @toRow, @UserId = @UserId, @FromDate = @FromDate, @ToDate = @ToDate, @adminUserId = @adminUserId, @total = @count OUTPUT;
            IF @Order = ''
                SET @Order = ' ORDER BY [LogDateTime] DESC';
            SET @SqlQuery2 = 'SELECT * FROM(
			SELECT ROW_NUMBER() OVER ( ' + @Order + ' ) AS RowNum
			  ,@counts as Total, * FROM (
			  SELECT DISTINCT l.[Id]
			  ,l.[DeviceId]	  
			  ,l.[EventId]
			  ,l.[UserId]
			  ,l.[DateTime] as LogDateTime
			  ,l.[Ticks]
			  ,l.[TNAEvent]
			  ,l.[InOutMode]
			  ,l.SuccessTransfer
			  ,isnull(u.FirstName , '''') + '' '' + u.SurName as ''SurName''
			  ,convert(varchar, l.[DateTime], 108) [Time]
	  
			  --cast(l.[DateTime] as time) [Time],
			  ,d.Name DeviceName,
			  d.code DeviceCode,
			  d.[DeviceTypeId] as ''DeviceIOType'',

		   ISNULL([MatchingTypeLookup].[Code], 19000) AS MatchingType_Code,
		   ISNULL([MatchingTypeLookup].[Name], N''Unknown'') AS MatchingType_Name,
		   ISNULL([MatchingTypeLookup].[OrderIndex], 1) AS MatchingType_OrderIndex,
		   ISNULL([MatchingTypeLookup].[Description], N''نامشخص'') AS MatchingType_Description,


		   L.[EventId] AS EventLog_Code,
		   [LogEventLookUp].[Name] AS EventLog_Name,
		   [LogEventLookUp].[OrderIndex] AS EventLog_OrderIndex,
		   [LogEventLookUp].[Description] AS EventLog_Description,

		   ISNULL([SubEventLookup].[Code], 17001) AS SubEvent_Code,
		   ISNULL([SubEventLookup].[Name], N''Normal'') AS SubEvent_Name,
		   ISNULL([SubEventLookup].[OrderIndex], 1) AS SubEvent_OrderIndex,
		   ISNULL([SubEventLookup].[Description], N''بدون حالت خاص انتخاب شده'') AS SubEvent_Description
		  
		  FROM [dbo].[Log] l
		  inner join [dbo].[Device] d on l.DeviceId=d.ID  
		  LEFT OUTER join [dbo].[DeviceGroupMember] dgm on d.ID=dgm.DeviceId
		  LEFT OUTER join [dbo].[User] u on l.UserId=u.Code	
		  LEFT JOIN [dbo].[Lookup] AS [SubEventLookup]
		  ON L.[SubEvent] = [SubEventLookup].[Code]
		  LEFT OUTER JOIN
          [dbo].[Lookup] AS [LogEventLookUp]
          ON L.[EventId]  = [LogEventLookUp].[Code]
		  LEFT JOIN	[dbo].[Lookup] AS [MatchingTypeLookup]
			ON L.[MatchingType] = [MatchingTypeLookup].[Code]
		WHERE     
			(l.[DeviceId] = @DeviceID or  Coalesce(@DeviceID,'''') = '''') AND 
			(dgm.GroupId = @DeviceGroupId or  Coalesce(@DeviceGroupId,'''') = '''') AND 
			--(isnull(@deviceId , 0) = 0 OR @DeviceID = D.ID) AND
			(	(@state=1 and @userid<>0 and u.id=@userid  and l.SuccessTransfer = @state) 
		or
		(@state=1 and @userid=0 and l.userid<>-1 and l.SuccessTransfer = @state)
		or
		(@state=0 and @userid<>0 and u.id=@userid and l.SuccessTransfer = @state)
		or
		(@state=0 and @userid = 0 and l.SuccessTransfer = @state)
		or
		(@State IS NULL and @userid<>0 and u.id=@userid)
		or
		(@State IS NULL and @userid=0 and l.userid<>-1)) AND 			
			(isnull(@fromDate , '''') = '''' or convert(date,l.[DateTime]) >= convert(date,@FromDate) ) and
				(isnull(@todate , '''') = '''' or convert(date,l.[DateTime]) <= convert(date,@ToDate) ) and (EventId = 16003 or EventId = 16004)  ' + @Where + ' ) AS Logs ) AS DataList
		WHERE (RowNum >= @fromRow
		AND RowNum <= @toRow) OR (@fromRow = 0 AND @toRow = 0) 
		ORDER BY RowNum';
            SET @ParmDefinition = N'@DeviceGroupId int,@DeviceID int,@State BIT, @fromRow int ,@toRow int,@UserId int,@FromDate datetime,@ToDate datetime,@adminUserId int,@counts int';
            EXECUTE sp_executesql @SqlQuery2, @ParmDefinition,@DeviceGroupId=@DeviceGroupId, @DeviceID = @DeviceID, @State = @State, @fromRow = @fromRow, @toRow = @toRow, @UserId = @UserId, @FromDate = @FromDate, @ToDate = @ToDate, @adminUserId = @adminUserId, @counts = @count;
        END
    ELSE
        BEGIN
            SET @SqlQuery = N'SELECT @total = COUNT(1) FROM (SELECT DISTINCT l.Id FROM [dbo].[Log] l
				  inner join [dbo].[Device] d on l.DeviceId=d.ID
			      --LEFT OUTER join [dbo].[DeviceGroupMember] dgm on d.ID=dgm.DeviceId
				  LEFT OUTER join [dbo].[User] u on l.UserId=u.Code
				   LEFT JOIN	[dbo].[Lookup] AS [MatchingTypeLookup]
			       ON l.[MatchingType] = [MatchingTypeLookup].[Code]
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
						 --ON ag2.Id = agu.AccessGroupId
						 -- INNER JOIN
						 --[dbo].[AdminAccessGroup] AS AAG2
						 --ON ag2.Id = AAG2.AccessGroupId			  
					WHERE     
						(l.[DeviceId] = @DeviceID or  Coalesce(@DeviceID,'''') = '''') AND  
					    (dgm.GroupId = @DeviceGroupId or  Coalesce(@DeviceGroupId,'''') = '''') AND 
						--(isnull(@deviceId , 0) = 0 OR @DeviceID = D.ID) AND
						(	(@state=1 and @userid<>0 and u.id=@userid  ) 
		or
		(@state=1 and @userid=0 and l.userid<>-1 and l.SuccessTransfer = @state)
		or
		(@state=0 and @userid<>0 and u.id=@userid and l.SuccessTransfer = @state)
		or
		(@state=0 and @userid = 0 and l.SuccessTransfer = @state)
		or
		(@State IS NULL and @userid<>0 and u.id=@userid)
		or
		(@State IS NULL and @userid=0 and l.userid<>-1)) AND 					
						(isnull(@fromDate , '''') = '''' or convert(date,l.[DateTime]) >= convert(date,@FromDate) ) and
						(isnull(@todate , '''') = '''' or convert(date,l.[DateTime]) <= convert(date,@ToDate) ) and (EventId = 16003 or EventId = 16004)  
						 AND AAG.UserId = @adminUserId ' + @Where + ' ) AS Logs';
            SET @ParmDefinition = N'@DeviceGroupId int,@DeviceID int,@State BIT, @fromRow int ,@toRow int,@UserId int,@FromDate datetime,@ToDate datetime,@adminUserId int,@total int OUTPUT';
            EXECUTE sp_executesql @SqlQuery, @ParmDefinition,@DeviceGroupId=@DeviceGroupId, @DeviceID = @DeviceID, @State = @State, @fromRow = @fromRow, @toRow = @toRow, @UserId = @UserId, @FromDate = @FromDate, @ToDate = @ToDate, @adminUserId = @adminUserId, @total = @count OUTPUT;
            IF @Order = ''
                SET @Order = ' ORDER BY [LogDateTime] DESC';
            SET @SqlQuery2 = 'SELECT * FROM (
				SELECT ROW_NUMBER() OVER ( ' + @Order + ' ) AS RowNum
				  ,@counts as Total, * FROM (
				  SELECT DISTINCT l.[Id]
				  ,l.[DeviceId]	  
				  ,l.[EventId]
				  ,l.[UserId]
				  ,l.[DateTime] as LogDateTime
				  ,l.[Ticks]
				  ,l.[SubEvent]
				  ,l.[TNAEvent]
				  ,l.[InOutMode]
				  ,l.SuccessTransfer
				  ,isnull(u.FirstName , '''') + '' '' + u.SurName as ''SurName''
				  ,convert(varchar, l.[DateTime], 108) [Time]
	  
				  --cast(l.[DateTime] as time) [Time],
				  ,d.Name DeviceName,
				  d.code DeviceCode,
				  d.[DeviceTypeId] as ''DeviceIOType'',

				   ISNULL([MatchingTypeLookup].[Code], 19000) AS MatchingType_Code,
				   ISNULL([MatchingTypeLookup].[Name], N''Unknown'') AS MatchingType_Name,
				   ISNULL([MatchingTypeLookup].[OrderIndex], 1) AS MatchingType_OrderIndex,
				   ISNULL([MatchingTypeLookup].[Description], N''نامشخص'') AS MatchingType_Description,

				  L.[EventId] AS EventLog_Code,
			      [LogEventLookUp].[Name] AS EventLog_Name,
			      [LogEventLookUp].[OrderIndex] AS EventLog_OrderIndex,
			      [LogEventLookUp].[Description] AS EventLog_Description,

				  ISNULL([SubEventLookup].[Code], 17001) AS SubEvent_Code,
			      ISNULL([SubEventLookup].[Name], N''Normal'') AS SubEvent_Name,
			      ISNULL([SubEventLookup].[OrderIndex], 1) AS SubEvent_OrderIndex,
			      ISNULL([SubEventLookup].[Description], N''بدون حالت خاص انتخاب شده'') AS SubEvent_Description
			  FROM [dbo].[Log] l
			  inner join [dbo].[Device] d on l.DeviceId=d.ID 
			  --LEFT OUTER join [dbo].[DeviceGroupMember] dgm on d.ID=dgm.DeviceId 
			  LEFT OUTER join [dbo].[User] u on l.UserId=u.Code	 		
						 LEFT JOIN [dbo].[Lookup] AS [SubEventLookup]
						 ON L.[SubEvent] = [SubEventLookup].[Code]
						 LEFT OUTER JOIN
						 [dbo].[Lookup] AS [LogEventLookUp]
						 ON L.[EventId]  = [LogEventLookUp].[Code]
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
						   LEFT JOIN	[dbo].[Lookup] AS [MatchingTypeLookup]
			             ON L.[MatchingType] = [MatchingTypeLookup].[Code]


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
						 --ON ag2.Id = agu.AccessGroupId
						 -- INNER JOIN
						 --[dbo].[AdminAccessGroup] AS AAG2
						 --ON ag2.Id = AAG2.AccessGroupId			  	 
			WHERE     
				(l.[DeviceId] = @DeviceID or  Coalesce(@DeviceID,'''') = '''') AND 
				(dgm.GroupId = @DeviceGroupId or  Coalesce(@DeviceGroupId,'''') = '''') AND  
				--(isnull(@deviceId , 0) = 0 OR @DeviceID = D.ID) AND
				(	(@state=1 and @userid<>0 and u.id=@userid and l.SuccessTransfer = @state  ) 
		or
		(@state=1 and @userid=0 and l.userid<>-1 and l.SuccessTransfer = @state)
		or
		(@state=0 and @userid<>0 and u.id=@userid and l.SuccessTransfer = @state)
		or
		(@state=0 and @userid = 0 and l.SuccessTransfer = @state)
		or
		(@State IS NULL and @userid<>0 and u.id=@userid)
		or
		(@State IS NULL and @userid=0 and l.userid<>-1)) AND 			
				(isnull(@fromDate , '''') = '''' or convert(date,l.[DateTime]) >= convert(date,@FromDate) ) and
					(isnull(@todate , '''') = '''' or convert(date,l.[DateTime]) <= convert(date,@ToDate) ) and (EventId = 16003 or EventId = 16004)
					AND AAG.UserId = @adminUserId ' + @Where + ' ) AS Logs ) AS DataList
			WHERE (RowNum >= @fromRow
		AND RowNum <= @toRow) OR (@fromRow = 0 AND @toRow = 0)';
            SET @ParmDefinition = N'@DeviceGroupId int,@DeviceID int,@State BIT, @fromRow int ,@toRow int,@UserId int,@FromDate datetime,@ToDate datetime,@adminUserId int,@counts int';
            EXECUTE sp_executesql @SqlQuery2, @ParmDefinition,@DeviceGroupId=@DeviceGroupId, @DeviceID = @DeviceID, @State = @State, @fromRow = @fromRow, @toRow = @toRow, @UserId = @UserId, @FromDate = @FromDate, @ToDate = @ToDate, @adminUserId = @adminUserId, @counts = @count;
        END
END