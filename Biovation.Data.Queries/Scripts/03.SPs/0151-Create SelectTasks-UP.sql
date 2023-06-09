CREATE PROCEDURE [dbo].[SelectTasks]
@taskId INT=0, @taskItemId INT=0, @brandId INT=0, @deviceId INT=0, @pageNumber INT=0, @pageSize INT=0, @taskTypeCode NVARCHAR (100)=NULL, @taskStatusCodes NVARCHAR (200)=NULL, @excludedTaskStatusCodes NVARCHAR (200)=NULL
AS
BEGIN
    CREATE TABLE #TempTasks (
        [Id]                             INT                NOT NULL,
        [TaskTypeCode]                   NVARCHAR (50)      NOT NULL,
        [PriorityLevelCode]              NVARCHAR (50)      NOT NULL,
        [CreatedBy]                      INT                NULL,
        [CreatedAt]                      DATETIMEOFFSET (7) NOT NULL,
        [DeviceBrandId]                  INT                NULL,
        [DueDate]                        DATETIMEOFFSET (7) NULL,
        [UpdatedAt]                      DATETIMEOFFSET (7) NULL,
        [UpdatedBy]                      INT                NULL,
        [SchedulingPattern]              NVARCHAR (50)      NULL,
        [TaskItems_Id]                   INT                NOT NULL,
        [TaskItems_Data]                 NVARCHAR (MAX)     NOT NULL,
        [TaskItems_DeviceId]             INT                NULL,
        [TaskItems_OrderIndex]           INT                NOT NULL,
        [TaskItems_IsScheduled]          BIT                NOT NULL,
        [TaskItems_StatusCode]           NVARCHAR (50)      NULL,
        [TaskItems_IsParallelRestricted] BIT                NOT NULL,
        TaskItems_TaskItemTypeCode		 NVARCHAR (50)      NOT NULL,
        TaskItems_PriorityLevelCode      NVARCHAR (50)      NOT NULL,
        TaskItems_CurrentIndex           INT				NULL,
        [TaskItems_Device_DeviceId]      INT                NULL
    );
    DECLARE @HasPaging AS BIT;
    DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام شد', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
    DECLARE @query AS NVARCHAR (MAX);
    DECLARE @paramDefinition AS NVARCHAR (MAX) = '@taskId int = 0,@taskItemId int = 0, @brandId int = 0, @deviceId int = 0, @taskTypeCode NVARCHAR(100) = NULL, @taskStatusCodes NVARCHAR(200) = NULL, @excludedTaskStatusCodes NVARCHAR(200) = NULL';
    SELECT @query = 'INSERT INTO #TempTasks 
		   SELECT [TA].[Id],
           [TA].[TaskTypeCode],
           [TA].[PriorityLevelCode],
           [TA].[CreatedBy],
           [TA].[CreatedAt],
		   [TA].[DeviceBrandId],
           [TA].[DueDate] AS DueDate,
           [TA].[UpdatedAt],
           [TA].[UpdatedBy],
           [TA].[SchedulingPattern],
           [TI].[Id] AS TaskItems_Id,
           [TI].[Data] AS TaskItems_Data,
           [TI].[DeviceId] AS TaskItems_DeviceId,
           [TI].[OrderIndex] AS TaskItems_OrderIndex,
           [TI].[IsScheduled] AS TaskItems_IsScheduled,
           [TI].[StatusCode] AS TaskItems_StatusCode,
           [TI].[IsParallelRestricted] AS TaskItems_IsParallelRestricted,
           [TI].[TaskItemTypeCode] AS TaskItems_TaskItemTypeCode,
           [TI].[PriorityLevelCode] AS TaskItems_PriorityLevelCode,
           [TI].[CurrentIndex] AS TaskItems_CurrentIndex,
           [Dev].[Id] AS TaskItems_Device_DeviceId
    FROM   [dbo].[Task] AS [TA]
           INNER JOIN
           [dbo].[TaskItem] AS [TI]
           ON [TI].[TaskId] = [TA].[Id]
           LEFT OUTER JOIN
           [dbo].[Device] AS [Dev]
           ON [TI].[DeviceId] = [Dev].[Id]

		WHERE   ([TA].[Id] = @taskId OR ISNULL(@taskId, 0) = 0) 
		AND ([Dev].[Id] = @deviceId OR ISNULL(@deviceId, 0) = 0)
		AND ([TI].[Id] = @taskItemId OR ISNULL(@taskItemId, 0) = 0) 
		AND ([TA].[DeviceBrandId] = @brandId OR ISNULL(@brandId, 0) = 0)
	    AND (ISNULL([TI].[DeviceId], 0) <> 0)
	    AND (ISNULL([Dev].[Code], 0) <> 0)
		AND ([TA].[TaskTypeCode] = @taskTypeCode OR ISNULL(@taskTypeCode, 0) = 0)' + CASE WHEN @taskStatusCodes IS NOT NULL THEN ' AND [TI].[StatusCode] IN ' + @taskStatusCodes ELSE '' END + CASE WHEN @excludedTaskStatusCodes IS NOT NULL THEN ' AND [TI].[StatusCode] NOT IN ' + @excludedTaskStatusCodes ELSE '' END;
    EXECUTE sp_executesql @query, @paramDefinition, @taskId, @taskItemId, @brandId, @deviceId, @taskTypeCode, @taskStatusCodes, @excludedTaskStatusCodes;
    SET @HasPaging = CASE WHEN ISNULL(@PageSize, 0) = 0
                               OR ISNULL(@PageNumber, 0) = 0 THEN 0 ELSE 1 END;
    IF @HasPaging = 1
        BEGIN
            SELECT *,
                   (SELECT   DISTINCT COUNT(1) OVER ()
                    FROM     #TempTasks
                    GROUP BY Id) AS [Count]
            FROM   (SELECT DENSE_RANK() OVER (ORDER BY CASE WHEN Tasks.[Data_Status_Code] = 10004 THEN 1 WHEN Tasks.[Data_Status_Code] = 10001 THEN 2 WHEN Tasks.[Data_Status_Code] = 10006 THEN 3 WHEN Tasks.[Data_Status_Code] = 10002 THEN 4 WHEN Tasks.[Data_Status_Code] = 10003 THEN 5 WHEN Tasks.[Data_Status_Code] = 10007 THEN 6 WHEN Tasks.[Data_Status_Code] = 10005 THEN 7 WHEN Tasks.[Data_Status_Code] = 10008 THEN 8 ELSE 9 END ASC, Tasks.[Data_CreatedAt] DESC) AS RowNumber,
                           [TSL].[Name] AS Data_Status_Name,
                           [TSL].[OrderIndex] AS Data_Status_OrderIndex,
                           [TSL].[Description] AS Data_Status_Description,
                           *
                    FROM   (SELECT [TA].[Id] AS Data_Id,
                                   [TA].[TaskTypeCode] AS Data_TaskTypeCode,
                                   [TPL].[Code] AS Priority_Code,
                                   [TPL].[Name] AS Priority_Name,
                                   [TPL].[OrderIndex] AS Priority_OrderIndex,
                                   [TPL].[Description] AS Priority_Description,
                                   [TA].[CreatedBy] AS Data_CreatedBy_Id,
                                   [CU].[Code] AS Data_CreatedBy_Code,
                                   [CU].[UniqueId] AS Data_CreatedBy_UniqueId,
                                   ISNULL([CU].[UserName], ISNULL([CU].[FirstName], '') + ' ' + ISNULL([CU].[SurName], '')) AS Data_CreatedBy_UserName,
                                   [TA].[CreatedAt] AS Data_CreatedAt,
                                   [TA].[DueDate] AS Data_DueDate,
                                   [DBL].[Code] AS Data_DeviceBrand_Code,
                                   [DBL].[Name] AS Data_DeviceBrand_Name,
                                   [DBL].[OrderIndex] AS Data_DeviceBrand_OrderIndex,
                                   [DBL].[Description] AS Data_DeviceBrand_Description,
                                   [TA].[UpdatedAt] AS Data_UpdatedAt,
                                   [TA].[UpdatedBy] AS Data_UpdatedBy_Id,
                                   [TA].[SchedulingPattern] AS Data_SchedulingPattern,
                                   (SELECT CASE WHEN EXISTS (SELECT top 1  t.Id
                                                             FROM     #TempTasks AS t
                                                             WHERE    t.Id = [TA].Id
                                                                      AND t.TaskItems_StatusCode = 10003
                                                             GROUP BY t.Id, TaskItems_StatusCode) THEN 10003 WHEN EXISTS (SELECT top 1  t.Id
                                                                                                                          FROM     #TempTasks AS t
                                                                                                                          WHERE    t.Id = [TA].Id
                                                                                                                                   AND t.TaskItems_StatusCode = 10007
                                                                                                                          GROUP BY t.Id, TaskItems_StatusCode) THEN 10007 WHEN EXISTS (SELECT  top 1 t.Id
                                                                                                                                                                                       FROM     #TempTasks AS t
                                                                                                                                                                                       WHERE    t.Id = [TA].Id
                                                                                                                                                                                                AND t.TaskItems_StatusCode = 10001
                                                                                                                                                                                       GROUP BY t.Id, TaskItems_StatusCode) THEN 10001 WHEN EXISTS (SELECT top 1  t.Id
                                                                                                                                                                                                                                                    FROM     #TempTasks AS t
                                                                                                                                                                                                                                                    WHERE    t.Id = [TA].Id
                                                                                                                                                                                                                                                             AND t.TaskItems_StatusCode = 10006
                                                                                                                                                                                                                                                    GROUP BY t.Id, TaskItems_StatusCode) THEN 10006 WHEN EXISTS (SELECT  top 1 t.Id
                                                                                                                                                                                                                                                                                                                 FROM     #TempTasks AS t
                                                                                                                                                                                                                                                                                                                 WHERE    t.Id = [TA].Id
                                                                                                                                                                                                                                                                                                                          AND t.TaskItems_StatusCode = 10004
                                                                                                                                                                                                                                                                                                                 GROUP BY t.Id, TaskItems_StatusCode) THEN 10004 WHEN EXISTS (SELECT top 1  t.Id
                                                                                                                                                                                                                                                                                                                                                                              FROM     #TempTasks AS t
                                                                                                                                                                                                                                                                                                                                                                              WHERE    t.Id = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                                       AND t.TaskItems_StatusCode = 10005
                                                                                                                                                                                                                                                                                                                                                                              GROUP BY t.Id, TaskItems_StatusCode) THEN 10005 WHEN EXISTS (SELECT top 1  t.Id
                                                                                                                                                                                                                                                                                                                                                                                                                                           FROM     #TempTasks AS t
                                                                                                                                                                                                                                                                                                                                                                                                                                           WHERE    t.Id = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                                                                                                    AND t.TaskItems_StatusCode = 10008
                                                                                                                                                                                                                                                                                                                                                                                                                                           GROUP BY t.Id, TaskItems_StatusCode) THEN 10008 WHEN EXISTS (SELECT top 1  t.Id
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        FROM     #TempTasks AS t
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        WHERE    t.Id = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 AND t.TaskItems_StatusCode = 10002
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        GROUP BY t.Id, TaskItems_StatusCode) THEN 10002 ELSE 10007 END AS codeStatus) AS Data_Status_Code,
                                   (SELECT SUM(TaskItems_CurrentIndex)
                                    FROM   #TempTasks AS t
                                    WHERE  t.TaskItems_Id = [TA].Id) AS Data_CurrentIndex,
                                   (SELECT SUM(TaskItems_CurrentIndex)
                                    FROM   #TempTasks AS t
                                    WHERE  t.TaskItems_Id = [TA].Id) AS Data_TotalCount,
                                   [TT].[Code] AS Data_TaskType_Code,
                                   [TT].[Name] AS Data_TaskType_Name,
                                   [TT].[OrderIndex] AS Data_TaskType_OrderIndex,
                                   [TT].[Description] AS Data_TaskType_Description,
                                   [TA].[TaskItems_Id] AS Data_TaskItems_Id,
                                   [TA].[TaskItems_Data] AS Data_TaskItems_Data,
                                   [TA].[TaskItems_DeviceId] AS Data_TaskItems_DeviceId,
                                   [TA].[TaskItems_OrderIndex] AS Data_TaskItems_OrderIndex,
                                   [TA].[TaskItems_IsScheduled] AS Data_TaskItems_IsScheduled,
                                   [TA].[TaskItems_IsParallelRestricted] AS Data_TaskItems_IsParallelRestricted,
                                   [Dev].[Id] AS Data_TaskItems_Device_DeviceId,
                                   [Dev].[DeviceModelId] AS Data_TaskItems_Device_DeviceModelId,
                                   [Dev].[Name] AS Data_TaskItems_Device_Name,
                                   [Dev].[IPAddress] AS Data_TaskItems_Device_IpAddress,
                                   [Dev].[SerialNumber] AS Data_TaskItems_Device_SerialNumber,
                                   [Dev].[DeviceTypeId] AS Data_TaskItems_Device_DeviceType_Id,
                                   [Dev].[Code] AS Data_TaskItems_Device_Code,
                                   [SL].[Code] AS Data_TaskItems_Status_Code,
                                   [SL].[Name] AS Data_TaskItems_Status_Name,
                                   [SL].[OrderIndex] AS Data_TaskItems_Status_OrderIndex,
                                   [SL].[Description] AS Data_TaskItems_Status_Description,
                                   [TL].[Code] AS Data_TaskItems_TaskItemType_Code,
                                   [TL].[Name] AS Data_TaskItems_TaskItemType_Name,
                                   [TL].[OrderIndex] AS Data_TaskItems_TaskItemType_OrderIndex,
                                   [TL].[Description] AS Data_TaskItems_TaskItemType_Description,
                                   [PL].[Code] AS Data_TaskItems_Priority_Code,
                                   [PL].[Name] AS Data_TaskItems_Priority_Name,
                                   [PL].[OrderIndex] AS Data_TaskItems_Priority_OrderIndex,
                                   [PL].[Description] AS Data_TaskItems_Priority_Description,
                                   (@PageNumber - 1) * @PageSize AS [from],
                                   @PageNumber AS PageNumber,
                                   @PageSize AS PageSize,
                                   @Message AS e_Message,
                                   @Code AS e_Code,
                                   @Validate AS e_Validate
                            FROM   #TempTasks AS [TA]
                                   --INNER JOIN
                                   --[dbo].[TaskItem] AS [TI]
                                   --ON [TI].[TaskId] = [TA].[Id]
                                   INNER JOIN
                                   [dbo].[Lookup] AS [TT]
                                   ON [TA].[TaskTypeCode] = [TT].[Code]
                                   INNER JOIN
                                   [dbo].[Lookup] AS [SL]
                                   ON [TA].TaskItems_StatusCode = [SL].[Code]
                                   INNER JOIN
                                   [dbo].[Lookup] AS [TL]
                                   ON [TA].TaskItems_TaskItemTypeCode = [TL].[Code]
                                   INNER JOIN
                                   [dbo].[Lookup] AS [DBL]
                                   ON [TA].[DeviceBrandId] = [DBL].[Code]
                                   INNER JOIN
                                   [dbo].[Lookup] AS [PL]
                                   ON [TA].TaskItems_PriorityLevelCode = [PL].[Code]
                                   INNER JOIN
                                   [dbo].[Lookup] AS [TPL]
                                   ON [TA].[PriorityLevelCode] = [TPL].[Code]
                                   inner JOIN
                                   [dbo].[Device] AS [Dev]
                                   ON [TA].[TaskItems_DeviceId] = [Dev].[Id]
                                   inner  JOIN
                                   [dbo].[User] AS [CU]
                                   ON [TA].[CreatedBy] = [CU].[Id]
								   
								   ) AS Tasks
								   INNER JOIN
								   [dbo].[Lookup] AS [TSL]
								   ON Data_Status_Code = [TSL].[Code]
								   ) AS TasksOrdered
            WHERE  RowNumber > (@PageNumber - 1) * @PageSize
                   AND RowNumber <= @PageNumber * @PageSize;
        END
    ELSE
        BEGIN
            SELECT DENSE_RANK() OVER (ORDER BY CASE WHEN Tasks.[Data_Status_Code] = 10004 THEN 1 WHEN Tasks.[Data_Status_Code] = 10001 THEN 2 WHEN Tasks.[Data_Status_Code] = 10006 THEN 3 WHEN Tasks.[Data_Status_Code] = 10002 THEN 4 WHEN Tasks.[Data_Status_Code] = 10003 THEN 5 WHEN Tasks.[Data_Status_Code] = 10007 THEN 6 WHEN Tasks.[Data_Status_Code] = 10005 THEN 7 WHEN Tasks.[Data_Status_Code] = 10008 THEN 8 ELSE 9 END ASC, Tasks.[Data_CreatedAt] DESC) AS RowNumber,
                   [TSL].[Name] AS Data_Status_Name,
                   [TSL].[OrderIndex] AS Data_Status_OrderIndex,
                   [TSL].[Description] AS Data_Status_Description,
                   *
            FROM   (SELECT [TA].[Id] AS Data_Id,
                           [TA].[TaskTypeCode] AS Data_TaskTypeCode,
                           [TA].[PriorityLevelCode] AS Data_PriorityLevelCode,
                           [TA].[CreatedBy] AS Data_CreatedBy_Id,
                           [CU].[Code] AS Data_CreatedBy_Code,
                           [CU].[UniqueId] AS Data_CreatedBy_UniqueId,
                           ISNULL([CU].[UserName], ISNULL([CU].[FirstName], '') + ' ' + ISNULL([CU].[SurName], '')) AS Data_CreatedBy_UserName,
                           [TA].[CreatedAt] AS Data_CreatedAt,
                           [TA].[DueDate] AS Data_DueDate,
                           [DBL].[Code] AS Data_DeviceBrand_Code,
                           [DBL].[Name] AS Data_DeviceBrand_Name,
                           [DBL].[OrderIndex] AS Data_DeviceBrand_OrderIndex,
                           [DBL].[Description] AS Data_DeviceBrand_Description,
                           [TA].[UpdatedAt] AS Data_UpdatedAt,
                           [TA].[UpdatedBy] AS Data_UpdatedBy_Id,
                           [TA].[SchedulingPattern] AS Data_SchedulingPattern,
                           (SELECT CASE WHEN EXISTS (SELECT top 1  t.Id
                                                     FROM     #TempTasks AS t
                                                     WHERE    t.Id = [TA].Id
                                                              AND t.TaskItems_StatusCode = 10003
                                                     GROUP BY t.Id, TaskItems_StatusCode) THEN 10003 WHEN EXISTS (SELECT top 1  t.Id
                                                                                                                  FROM     #TempTasks AS t
                                                                                                                  WHERE    t.Id = [TA].Id
                                                                                                                           AND t.TaskItems_StatusCode = 10007
                                                                                                                  GROUP BY t.Id, TaskItems_StatusCode) THEN 10007 WHEN EXISTS (SELECT top 1  t.Id
                                                                                                                                                                               FROM     #TempTasks AS t
                                                                                                                                                                               WHERE    t.Id = [TA].Id
                                                                                                                                                                                        AND t.TaskItems_StatusCode = 10001
                                                                                                                                                                               GROUP BY t.Id, TaskItems_StatusCode) THEN 10001 WHEN EXISTS (SELECT  top 1 t.Id
                                                                                                                                                                                                                                            FROM     #TempTasks AS t
                                                                                                                                                                                                                                            WHERE    t.Id = [TA].Id
                                                                                                                                                                                                                                                     AND t.TaskItems_StatusCode = 10006
                                                                                                                                                                                                                                            GROUP BY t.Id, TaskItems_StatusCode) THEN 10006 WHEN EXISTS (SELECT top 1  t.Id
                                                                                                                                                                                                                                                                                                         FROM     #TempTasks AS t
                                                                                                                                                                                                                                                                                                         WHERE    t.Id = [TA].Id
                                                                                                                                                                                                                                                                                                                  AND t.TaskItems_StatusCode = 10004
                                                                                                                                                                                                                                                                                                         GROUP BY t.Id, TaskItems_StatusCode) THEN 10004 WHEN EXISTS (SELECT top 1  t.Id
                                                                                                                                                                                                                                                                                                                                                                      FROM     #TempTasks AS t
                                                                                                                                                                                                                                                                                                                                                                      WHERE    t.Id = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                               AND t.TaskItems_StatusCode = 10005
                                                                                                                                                                                                                                                                                                                                                                      GROUP BY t.Id, TaskItems_StatusCode) THEN 10005 WHEN EXISTS (SELECT top 1  t.Id
                                                                                                                                                                                                                                                                                                                                                                                                                                   FROM     #TempTasks AS t
                                                                                                                                                                                                                                                                                                                                                                                                                                   WHERE    t.Id = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                                                                                            AND t.TaskItems_StatusCode = 10008
                                                                                                                                                                                                                                                                                                                                                                                                                                   GROUP BY t.Id, TaskItems_StatusCode) THEN 10008 WHEN EXISTS (SELECT top 1  t.Id
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                FROM     #TempTasks AS t
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                WHERE    t.Id = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         AND t.TaskItems_StatusCode = 10002
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                GROUP BY t.Id, TaskItems_StatusCode) THEN 10002 ELSE 10007 END AS codeStatus) AS Data_Status_Code,
                           (SELECT SUM(TaskItems_CurrentIndex)
								FROM   #TempTasks AS t
								WHERE  t.TaskItems_Id = [TA].Id) AS Data_CurrentIndex,
                           (SELECT SUM(TaskItems_CurrentIndex)
								FROM   #TempTasks AS t
								WHERE  t.TaskItems_Id = [TA].Id) AS Data_TotalCount,
                           [TT].[Code] AS Data_TaskType_Code,
                           [TT].[Name] AS Data_TaskType_Name,
                           [TT].[OrderIndex] AS Data_TaskType_OrderIndex,
                           [TT].[Description] AS Data_TaskType_Description,
                           [TA].[TaskItems_Id] AS Data_TaskItems_Id,
                           [TA].[TaskItems_Data] AS Data_TaskItems_Data,
                           [TA].[TaskItems_DeviceId] AS Data_TaskItems_DeviceId,
                           [TA].[TaskItems_OrderIndex] AS Data_TaskItems_OrderIndex,
                           [TA].[TaskItems_IsScheduled] AS Data_TaskItems_IsScheduled,
                           [TA].[TaskItems_IsParallelRestricted] AS Data_TaskItems_IsParallelRestricted,
                           [Dev].[Id] AS Data_TaskItems_Device_DeviceId,
                           [Dev].[DeviceModelId] AS Data_TaskItems_Device_DeviceModelId,
                           [Dev].[Name] AS Data_TaskItems_Device_Name,
                           [Dev].[IPAddress] AS Data_TaskItems_Device_IpAddress,
                           [Dev].[SerialNumber] AS Data_TaskItems_Device_SerialNumber,
                           [Dev].[DeviceTypeId] AS Data_TaskItems_Device_DeviceType_Id,
                           [Dev].[Code] AS Data_TaskItems_Device_Code,
                           [SL].[Code] AS Data_TaskItems_Status_Code,
                           [SL].[Name] AS Data_TaskItems_Status_Name,
                           [SL].[OrderIndex] AS Data_TaskItems_Status_OrderIndex,
                           [SL].[Description] AS Data_TaskItems_Status_Description,
                           [TL].[Code] AS Data_TaskItems_TaskItemType_Code,
                           [TL].[Name] AS Data_TaskItems_TaskItemType_Name,
                           [TL].[OrderIndex] AS Data_TaskItems_TaskItemType_OrderIndex,
                           [TL].[Description] AS Data_TaskItems_TaskItemType_Description,
                           [PL].[Code] AS Data_TaskItems_Priority_Code,
                           [PL].[Name] AS Data_TaskItems_Priority_Name,
                           [PL].[OrderIndex] AS Data_TaskItems_Priority_OrderIndex,
                           [PL].[Description] AS Data_TaskItems_Priority_Description,
                           0 AS [from],
                           0 AS PageNumber,
                           0 AS PageSize,
                           @Message AS e_Message,
                           @Code AS e_Code,
                           @Validate AS e_Validate
                   FROM   #TempTasks AS [TA]
                                   --INNER JOIN
                                   --[dbo].[TaskItem] AS [TI]
                                   --ON [TI].[TaskId] = [TA].[Id]
                                   INNER JOIN
                                   [dbo].[Lookup] AS [TT]
                                   ON [TA].[TaskTypeCode] = [TT].[Code]
                                   INNER JOIN
                                   [dbo].[Lookup] AS [SL]
                                   ON [TA].TaskItems_StatusCode = [SL].[Code]
                                   INNER JOIN
                                   [dbo].[Lookup] AS [TL]
                                   ON [TA].TaskItems_TaskItemTypeCode = [TL].[Code]
                                   INNER JOIN
                                   [dbo].[Lookup] AS [DBL]
                                   ON [TA].[DeviceBrandId] = [DBL].[Code]
                                   INNER JOIN
                                   [dbo].[Lookup] AS [PL]
                                   ON [TA].TaskItems_PriorityLevelCode = [PL].[Code]
                                   INNER JOIN
                                   [dbo].[Lookup] AS [TPL]
                                   ON [TA].[PriorityLevelCode] = [TPL].[Code]
                                   inner JOIN
                                   [dbo].[Device] AS [Dev]
                                   ON [TA].[TaskItems_DeviceId] = [Dev].[Id]
                                   inner  JOIN
                                   [dbo].[User] AS [CU]
                                   ON [TA].[CreatedBy] = [CU].[Id]
								   
								   ) AS Tasks
                   INNER JOIN
                   [dbo].[Lookup] AS [TSL]
                   ON Data_Status_Code = [TSL].[Code];
        END
END