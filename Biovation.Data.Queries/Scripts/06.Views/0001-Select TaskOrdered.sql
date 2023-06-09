CREATE VIEW [dbo].[TaskOrdered]
AS
    SELECT [TA].[Id],
           [TA].[TaskTypeCode],
           [TPL].[Code] AS Priority_Code,
           [TPL].[Name] AS Priority_Name,
           [TPL].[OrderIndex] AS Priority_OrderIndex,
           [TPL].[Description] AS Priority_Description,
		   [TA].[CreatedBy] AS CreatedBy_Id,
           [TA].[CreatedAt],
           [TA].[DueDate] AS DueDate,
           [DBL].[Code] AS DeviceBrand_Code,
           [DBL].[Name] AS DeviceBrand_Name,
           [DBL].[OrderIndex] AS DeviceBrand_OrderIndex,
           [DBL].[Description] AS DeviceBrand_Description,
           [TA].[UpdatedAt],
           [TA].[UpdatedBy] AS UpdatedBy_Id,
           [TA].[SchedulingPattern],
           (SELECT CASE WHEN EXISTS (SELECT   TaskId
                                     FROM     TaskItem AS t
                                     WHERE    t.StatusCode = 10003
                                              AND TaskId = [TA].Id
                                     GROUP BY StatusCode, TaskId) THEN 10003 WHEN EXISTS (SELECT   TaskId
                                                                                          FROM     TaskItem AS t
                                                                                          WHERE    t.StatusCode = 10007
                                                                                                   AND TaskId = [TA].Id
                                                                                          GROUP BY StatusCode, TaskId) THEN 10007 WHEN EXISTS (SELECT   TaskId
                                                                                                                                               FROM     TaskItem AS t
                                                                                                                                               WHERE    t.StatusCode = 10001
                                                                                                                                                        AND TaskId = [TA].Id
                                                                                                                                               GROUP BY StatusCode, TaskId) THEN 10001 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                    FROM     TaskItem AS t
                                                                                                                                                                                                    WHERE    t.StatusCode = 10006
                                                                                                                                                                                                             AND TaskId = [TA].Id
                                                                                                                                                                                                    GROUP BY StatusCode, TaskId) THEN 10006 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                         FROM     TaskItem AS t
                                                                                                                                                                                                                                                         WHERE    t.StatusCode = 10004
                                                                                                                                                                                                                                                                  AND TaskId = [TA].Id
                                                                                                                                                                                                                                                         GROUP BY StatusCode, TaskId) THEN 10004 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                              FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                              WHERE    t.StatusCode = 10005
                                                                                                                                                                                                                                                                                                                       AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                              GROUP BY StatusCode, TaskId) THEN 10005 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                   FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                   WHERE    t.StatusCode = 10008
                                                                                                                                                                                                                                                                                                                                                                            AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                   GROUP BY StatusCode, TaskId) THEN 10008 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                                                                        FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                                                                        WHERE    t.StatusCode = 10002
                                                                                                                                                                                                                                                                                                                                                                                                                                 AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                                                                        GROUP BY StatusCode, TaskId) THEN 10002 ELSE 10007 END AS codeStatus) AS Status_Code,
           (SELECT SUM(CurrentIndex)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS CurrentIndex,
           (SELECT SUM(TotalCount)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS TotalCount,
           [TT].[Code] AS TaskType_Code,
           [TT].[Name] AS TaskType_Name,
           [TT].[OrderIndex] AS TaskType_OrderIndex,
           [TT].[Description] AS TaskType_Description,
           [TI].[Id] AS TaskItems_Id,
           [TI].[Data] AS TaskItems_Data,
           [TI].[DeviceId] AS TaskItems_DeviceId,
           [TI].[OrderIndex] AS TaskItems_OrderIndex,
           [TI].[IsScheduled] AS TaskItems_IsScheduled,
           [TI].[IsParallelRestricted] AS TaskItems_IsParallelRestricted,
           [Dev].[Id] AS TaskItems_Device_DeviceId,
           [Dev].[DeviceModelId] AS TaskItems_Device_DeviceModelId,
           [Dev].[Name] AS TaskItems_Device_Name,
           [Dev].[Active] AS TaskItems_Device_Active,
           [Dev].[IPAddress] AS TaskItems_Device_IpAddress,
           [Dev].[Port] AS TaskItems_Device_Port,
           [Dev].[MacAddress] AS TaskItems_Device_MacAddress,
           [Dev].[RegisterDate] AS TaskItems_Device_RegisterDate,
           [Dev].[HardwareVersion] AS TaskItems_Device_HardwareVersion,
           [Dev].[FirmwareVersion] AS TaskItems_Device_FirmwareVersion,
           [Dev].[DeviceLockPassword] AS TaskItems_Device_DeviceLockPassword,
           [Dev].[SSL] AS TaskItems_Device_SSL,
           [Dev].[TimeSync] AS TaskItems_Device_TimeSync,
           [Dev].[SerialNumber] AS TaskItems_Device_SerialNumber,
           [Dev].[DeviceTypeId] AS TaskItems_Device_DeviceType_Id,
           [Dev].[Code] AS TaskItems_Device_Code,
           [SL].[Code] AS TaskItems_Status_Code,
           [SL].[Name] AS TaskItems_Status_Name,
           [SL].[OrderIndex] AS TaskItems_Status_OrderIndex,
           [SL].[Description] AS TaskItems_Status_Description,
           [TL].[Code] AS TaskItems_TaskItemType_Code,
           [TL].[Name] AS TaskItems_TaskItemType_Name,
           [TL].[OrderIndex] AS TaskItems_TaskItemType_OrderIndex,
           [TL].[Description] AS TaskItems_TaskItemType_Description,
           [PL].[Code] AS TaskItems_Priority_Code,
           [PL].[Name] AS TaskItems_Priority_Name,
           [PL].[OrderIndex] AS TaskItems_Priority_OrderIndex,
           [PL].[Description] AS TaskItems_Priority_Description
    FROM   [dbo].[Task] AS [TA]
           INNER JOIN
           [dbo].[TaskItem] AS [TI]
           ON [TI].[TaskId] = [TA].[Id]
           INNER JOIN
           [dbo].[Lookup] AS [TT]
           ON [TA].[TaskTypeCode] = [TT].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [SL]
           ON [TI].[StatusCode] = [SL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [TL]
           ON [TI].[TaskItemTypeCode] = [TL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [DBL]
           ON [TA].[DeviceBrandId] = [DBL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [PL]
           ON [TI].[PriorityLevelCode] = [PL].[Code]
		   INNER JOIN
           [dbo].[Lookup] AS [TPL]
           ON [TA].[PriorityLevelCode] = [TPL].[Code]
           LEFT OUTER JOIN
           [dbo].[Device] AS [Dev]
           ON [TI].[DeviceId] = [Dev].[Id]
    WHERE  [TI].[StatusCode] = 10004
    UNION ALL
    SELECT [TA].[Id],
           [TA].[TaskTypeCode],
           [TPL].[Code] AS Priority_Code,
           [TPL].[Name] AS Priority_Name,
           [TPL].[OrderIndex] AS Priority_OrderIndex,
           [TPL].[Description] AS Priority_Description,
		   [TA].[CreatedBy] AS CreatedBy_Id,
           [TA].[CreatedAt],
           [TA].[DueDate] AS DueDate,
           [DBL].[Code] AS DeviceBrand_Code,
           [DBL].[Name] AS DeviceBrand_Name,
           [DBL].[OrderIndex] AS DeviceBrand_OrderIndex,
           [DBL].[Description] AS DeviceBrand_Description,
           [TA].[UpdatedAt],
           [TA].[UpdatedBy] AS UpdatedBy_Id,
           [TA].[schedulingPattern],
           (SELECT CASE WHEN EXISTS (SELECT   TaskId
                                     FROM     TaskItem AS t
                                     WHERE    t.StatusCode = 10003
                                              AND TaskId = [TA].Id
                                     GROUP BY StatusCode, TaskId) THEN 10003 WHEN EXISTS (SELECT   TaskId
                                                                                          FROM     TaskItem AS t
                                                                                          WHERE    t.StatusCode = 10007
                                                                                                   AND TaskId = [TA].Id
                                                                                          GROUP BY StatusCode, TaskId) THEN 10007 WHEN EXISTS (SELECT   TaskId
                                                                                                                                               FROM     TaskItem AS t
                                                                                                                                               WHERE    t.StatusCode = 10001
                                                                                                                                                        AND TaskId = [TA].Id
                                                                                                                                               GROUP BY StatusCode, TaskId) THEN 10001 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                    FROM     TaskItem AS t
                                                                                                                                                                                                    WHERE    t.StatusCode = 10006
                                                                                                                                                                                                             AND TaskId = [TA].Id
                                                                                                                                                                                                    GROUP BY StatusCode, TaskId) THEN 10006 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                         FROM     TaskItem AS t
                                                                                                                                                                                                                                                         WHERE    t.StatusCode = 10004
                                                                                                                                                                                                                                                                  AND TaskId = [TA].Id
                                                                                                                                                                                                                                                         GROUP BY StatusCode, TaskId) THEN 10004 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                              FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                              WHERE    t.StatusCode = 10005
                                                                                                                                                                                                                                                                                                                       AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                              GROUP BY StatusCode, TaskId) THEN 10005 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                   FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                   WHERE    t.StatusCode = 10008
                                                                                                                                                                                                                                                                                                                                                                            AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                   GROUP BY StatusCode, TaskId) THEN 10008 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                                                                        FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                                                                        WHERE    t.StatusCode = 10002
                                                                                                                                                                                                                                                                                                                                                                                                                                 AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                                                                        GROUP BY StatusCode, TaskId) THEN 10002 ELSE 10007 END AS codeStatus) AS Status_Code,
           (SELECT SUM(CurrentIndex)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS CurrentIndex,
           (SELECT SUM(TotalCount)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS TotalCount,
           [TT].[Code] AS TaskType_Code,
           [TT].[Name] AS TaskType_Name,
           [TT].[OrderIndex] AS TaskType_OrderIndex,
           [TT].[Description] AS TaskType_Description,
           [TI].[Id] AS TaskItems_Id,
           [TI].[Data] AS TaskItems_Data,
           [TI].[DeviceId] AS TaskItems_DeviceId,
           [TI].[OrderIndex] AS TaskItems_OrderIndex,
           [TI].[IsScheduled] AS TaskItems_IsScheduled,
           [TI].[IsParallelRestricted] AS TaskItems_IsParallelRestricted,
           [Dev].[Id] AS TaskItems_Device_DeviceId,
           [Dev].[DeviceModelId] AS TaskItems_Device_DeviceModelId,
           [Dev].[Name] AS TaskItems_Device_Name,
           [Dev].[Active] AS TaskItems_Device_Active,
           [Dev].[IPAddress] AS TaskItems_Device_IpAddress,
           [Dev].[Port] AS TaskItems_Device_Port,
           [Dev].[MacAddress] AS TaskItems_Device_MacAddress,
           [Dev].[RegisterDate] AS TaskItems_Device_RegisterDate,
           [Dev].[HardwareVersion] AS TaskItems_Device_HardwareVersion,
           [Dev].[FirmwareVersion] AS TaskItems_Device_FirmwareVersion,
           [Dev].[DeviceLockPassword] AS TaskItems_Device_DeviceLockPassword,
           [Dev].[SSL] AS TaskItems_Device_SSL,
           [Dev].[TimeSync] AS TaskItems_Device_TimeSync,
           [Dev].[SerialNumber] AS TaskItems_Device_SerialNumber,
           [Dev].[DeviceTypeId] AS TaskItems_Device_DeviceType_Id,
           [Dev].[Code] AS TaskItems_Device_Code,
           [SL].[Code] AS TaskItems_Status_Code,
           [SL].[Name] AS TaskItems_Status_Name,
           [SL].[OrderIndex] AS TaskItems_Status_OrderIndex,
           [SL].[Description] AS TaskItems_Status_Description,
           [TL].[Code] AS TaskItems_TaskItemType_Code,
           [TL].[Name] AS TaskItems_TaskItemType_Name,
           [TL].[OrderIndex] AS TaskItems_TaskItemType_OrderIndex,
           [TL].[Description] AS TaskItems_TaskItemType_Description,
           [PL].[Code] AS TaskItems_Priority_Code,
           [PL].[Name] AS TaskItems_Priority_Name,
           [PL].[OrderIndex] AS TaskItems_Priority_OrderIndex,
           [PL].[Description] AS TaskItems_Priority_Description
    FROM   [dbo].[Task] AS [TA]
           INNER JOIN
           [dbo].[TaskItem] AS [TI]
           ON [TI].[TaskId] = [TA].[Id]
           INNER JOIN
           [dbo].[Lookup] AS [TT]
           ON [TA].[TaskTypeCode] = [TT].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [SL]
           ON [TI].[StatusCode] = [SL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [TL]
           ON [TI].[TaskItemTypeCode] = [TL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [DBL]
           ON [TA].[DeviceBrandId] = [DBL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [PL]
           ON [TI].[PriorityLevelCode] = [PL].[Code]
		   INNER JOIN
           [dbo].[Lookup] AS [TPL]
           ON [TA].[PriorityLevelCode] = [TPL].[Code]
		   LEFT OUTER JOIN
           [dbo].[Device] AS [Dev]
           ON [TI].[DeviceId] = [Dev].[Id]
    WHERE  [TI].[StatusCode] = 10005
    UNION ALL
    SELECT [TA].[Id],
           [TA].[TaskTypeCode],
           [TPL].[Code] AS Priority_Code,
           [TPL].[Name] AS Priority_Name,
           [TPL].[OrderIndex] AS Priority_OrderIndex,
           [TPL].[Description] AS Priority_Description,
		   [TA].[CreatedBy] AS CreatedBy_Id,
           [TA].[CreatedAt],
           [TA].[DueDate] AS DueDate,
           [DBL].[Code] AS DeviceBrand_Code,
           [DBL].[Name] AS DeviceBrand_Name,
           [DBL].[OrderIndex] AS DeviceBrand_OrderIndex,
           [DBL].[Description] AS DeviceBrand_Description,
           [TA].[UpdatedAt],
           [TA].[UpdatedBy] AS UpdatedBy_Id,
           [TA].[schedulingPattern],
           (SELECT CASE WHEN EXISTS (SELECT   TaskId
                                     FROM     TaskItem AS t
                                     WHERE    t.StatusCode = 10003
                                              AND TaskId = [TA].Id
                                     GROUP BY StatusCode, TaskId) THEN 10003 WHEN EXISTS (SELECT   TaskId
                                                                                          FROM     TaskItem AS t
                                                                                          WHERE    t.StatusCode = 10007
                                                                                                   AND TaskId = [TA].Id
                                                                                          GROUP BY StatusCode, TaskId) THEN 10007 WHEN EXISTS (SELECT   TaskId
                                                                                                                                               FROM     TaskItem AS t
                                                                                                                                               WHERE    t.StatusCode = 10001
                                                                                                                                                        AND TaskId = [TA].Id
                                                                                                                                               GROUP BY StatusCode, TaskId) THEN 10001 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                    FROM     TaskItem AS t
                                                                                                                                                                                                    WHERE    t.StatusCode = 10006
                                                                                                                                                                                                             AND TaskId = [TA].Id
                                                                                                                                                                                                    GROUP BY StatusCode, TaskId) THEN 10006 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                         FROM     TaskItem AS t
                                                                                                                                                                                                                                                         WHERE    t.StatusCode = 10004
                                                                                                                                                                                                                                                                  AND TaskId = [TA].Id
                                                                                                                                                                                                                                                         GROUP BY StatusCode, TaskId) THEN 10004 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                              FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                              WHERE    t.StatusCode = 10005
                                                                                                                                                                                                                                                                                                                       AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                              GROUP BY StatusCode, TaskId) THEN 10005 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                   FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                   WHERE    t.StatusCode = 10008
                                                                                                                                                                                                                                                                                                                                                                            AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                   GROUP BY StatusCode, TaskId) THEN 10008 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                                                                        FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                                                                        WHERE    t.StatusCode = 10002
                                                                                                                                                                                                                                                                                                                                                                                                                                 AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                                                                        GROUP BY StatusCode, TaskId) THEN 10002 ELSE 10007 END AS codeStatus) AS Status_Code,
           (SELECT SUM(CurrentIndex)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS CurrentIndex,
           (SELECT SUM(TotalCount)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS TotalCount,
           [TT].[Code] AS TaskType_Code,
           [TT].[Name] AS TaskType_Name,
           [TT].[OrderIndex] AS TaskType_OrderIndex,
           [TT].[Description] AS TaskType_Description,
           [TI].[Id] AS TaskItems_Id,
           [TI].[Data] AS TaskItems_Data,
           [TI].[DeviceId] AS TaskItems_DeviceId,
           [TI].[OrderIndex] AS TaskItems_OrderIndex,
           [TI].[IsScheduled] AS TaskItems_IsScheduled,
           [TI].[IsParallelRestricted] AS TaskItems_IsParallelRestricted,
           [Dev].[Id] AS TaskItems_Device_DeviceId,
           [Dev].[DeviceModelId] AS TaskItems_Device_DeviceModelId,
           [Dev].[Name] AS TaskItems_Device_Name,
           [Dev].[Active] AS TaskItems_Device_Active,
           [Dev].[IPAddress] AS TaskItems_Device_IpAddress,
           [Dev].[Port] AS TaskItems_Device_Port,
           [Dev].[MacAddress] AS TaskItems_Device_MacAddress,
           [Dev].[RegisterDate] AS TaskItems_Device_RegisterDate,
           [Dev].[HardwareVersion] AS TaskItems_Device_HardwareVersion,
           [Dev].[FirmwareVersion] AS TaskItems_Device_FirmwareVersion,
           [Dev].[DeviceLockPassword] AS TaskItems_Device_DeviceLockPassword,
           [Dev].[SSL] AS TaskItems_Device_SSL,
           [Dev].[TimeSync] AS TaskItems_Device_TimeSync,
           [Dev].[SerialNumber] AS TaskItems_Device_SerialNumber,
           [Dev].[DeviceTypeId] AS TaskItems_Device_DeviceType_Id,
           [Dev].[Code] AS TaskItems_Device_Code,
           [SL].[Code] AS TaskItems_Status_Code,
           [SL].[Name] AS TaskItems_Status_Name,
           [SL].[OrderIndex] AS TaskItems_Status_OrderIndex,
           [SL].[Description] AS TaskItems_Status_Description,
           [TL].[Code] AS TaskItems_TaskItemType_Code,
           [TL].[Name] AS TaskItems_TaskItemType_Name,
           [TL].[OrderIndex] AS TaskItems_TaskItemType_OrderIndex,
           [TL].[Description] AS TaskItems_TaskItemType_Description,
           [PL].[Code] AS TaskItems_Priority_Code,
           [PL].[Name] AS TaskItems_Priority_Name,
           [PL].[OrderIndex] AS TaskItems_Priority_OrderIndex,
           [PL].[Description] AS TaskItems_Priority_Description
    FROM   [dbo].[Task] AS [TA]
           INNER JOIN
           [dbo].[TaskItem] AS [TI]
           ON [TI].[TaskId] = [TA].[Id]
           INNER JOIN
           [dbo].[Lookup] AS [TT]
           ON [TA].[TaskTypeCode] = [TT].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [SL]
           ON [TI].[StatusCode] = [SL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [TL]
           ON [TI].[TaskItemTypeCode] = [TL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [DBL]
           ON [TA].[DeviceBrandId] = [DBL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [PL]
           ON [TI].[PriorityLevelCode] = [PL].[Code]
		   INNER JOIN
           [dbo].[Lookup] AS [TPL]
           ON [TA].[PriorityLevelCode] = [TPL].[Code]
		   LEFT OUTER JOIN
           [dbo].[Device] AS [Dev]
           ON [TI].[DeviceId] = [Dev].[Id]
    WHERE  [TI].[StatusCode] = 10003
    UNION ALL
    SELECT [TA].[Id],
           [TA].[TaskTypeCode],
           [TPL].[Code] AS Priority_Code,
           [TPL].[Name] AS Priority_Name,
           [TPL].[OrderIndex] AS Priority_OrderIndex,
           [TPL].[Description] AS Priority_Description,
		   [TA].[CreatedBy] AS CreatedBy_Id,
           [TA].[CreatedAt],
           [TA].[DueDate] AS DueDate,
           [DBL].[Code] AS DeviceBrand_Code,
           [DBL].[Name] AS DeviceBrand_Name,
           [DBL].[OrderIndex] AS DeviceBrand_OrderIndex,
           [DBL].[Description] AS DeviceBrand_Description,
           [TA].[UpdatedAt],
           [TA].[UpdatedBy] AS UpdatedBy_Id,
           [TA].[schedulingPattern],
           (SELECT CASE WHEN EXISTS (SELECT   TaskId
                                     FROM     TaskItem AS t
                                     WHERE    t.StatusCode = 10003
                                              AND TaskId = [TA].Id
                                     GROUP BY StatusCode, TaskId) THEN 10003 WHEN EXISTS (SELECT   TaskId
                                                                                          FROM     TaskItem AS t
                                                                                          WHERE    t.StatusCode = 10007
                                                                                                   AND TaskId = [TA].Id
                                                                                          GROUP BY StatusCode, TaskId) THEN 10007 WHEN EXISTS (SELECT   TaskId
                                                                                                                                               FROM     TaskItem AS t
                                                                                                                                               WHERE    t.StatusCode = 10001
                                                                                                                                                        AND TaskId = [TA].Id
                                                                                                                                               GROUP BY StatusCode, TaskId) THEN 10001 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                    FROM     TaskItem AS t
                                                                                                                                                                                                    WHERE    t.StatusCode = 10006
                                                                                                                                                                                                             AND TaskId = [TA].Id
                                                                                                                                                                                                    GROUP BY StatusCode, TaskId) THEN 10006 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                         FROM     TaskItem AS t
                                                                                                                                                                                                                                                         WHERE    t.StatusCode = 10004
                                                                                                                                                                                                                                                                  AND TaskId = [TA].Id
                                                                                                                                                                                                                                                         GROUP BY StatusCode, TaskId) THEN 10004 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                              FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                              WHERE    t.StatusCode = 10005
                                                                                                                                                                                                                                                                                                                       AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                              GROUP BY StatusCode, TaskId) THEN 10005 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                   FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                   WHERE    t.StatusCode = 10008
                                                                                                                                                                                                                                                                                                                                                                            AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                   GROUP BY StatusCode, TaskId) THEN 10008 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                                                                        FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                                                                        WHERE    t.StatusCode = 10002
                                                                                                                                                                                                                                                                                                                                                                                                                                 AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                                                                        GROUP BY StatusCode, TaskId) THEN 10002 ELSE 10007 END AS codeStatus) AS Status_Code,
           (SELECT SUM(CurrentIndex)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS CurrentIndex,
           (SELECT SUM(TotalCount)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS TotalCount,
           [TT].[Code] AS TaskType_Code,
           [TT].[Name] AS TaskType_Name,
           [TT].[OrderIndex] AS TaskType_OrderIndex,
           [TT].[Description] AS TaskType_Description,
           [TI].[Id] AS TaskItems_Id,
           [TI].[Data] AS TaskItems_Data,
           [TI].[DeviceId] AS TaskItems_DeviceId,
           [TI].[OrderIndex] AS TaskItems_OrderIndex,
           [TI].[IsScheduled] AS TaskItems_IsScheduled,
           [TI].[IsParallelRestricted] AS TaskItems_IsParallelRestricted,
           [Dev].[Id] AS TaskItems_Device_DeviceId,
           [Dev].[DeviceModelId] AS TaskItems_Device_DeviceModelId,
           [Dev].[Name] AS TaskItems_Device_Name,
           [Dev].[Active] AS TaskItems_Device_Active,
           [Dev].[IPAddress] AS TaskItems_Device_IpAddress,
           [Dev].[Port] AS TaskItems_Device_Port,
           [Dev].[MacAddress] AS TaskItems_Device_MacAddress,
           [Dev].[RegisterDate] AS TaskItems_Device_RegisterDate,
           [Dev].[HardwareVersion] AS TaskItems_Device_HardwareVersion,
           [Dev].[FirmwareVersion] AS TaskItems_Device_FirmwareVersion,
           [Dev].[DeviceLockPassword] AS TaskItems_Device_DeviceLockPassword,
           [Dev].[SSL] AS TaskItems_Device_SSL,
           [Dev].[TimeSync] AS TaskItems_Device_TimeSync,
           [Dev].[SerialNumber] AS TaskItems_Device_SerialNumber,
           [Dev].[DeviceTypeId] AS TaskItems_Device_DeviceType_Id,
           [Dev].[Code] AS TaskItems_Device_Code,
           [SL].[Code] AS TaskItems_Status_Code,
           [SL].[Name] AS TaskItems_Status_Name,
           [SL].[OrderIndex] AS TaskItems_Status_OrderIndex,
           [SL].[Description] AS TaskItems_Status_Description,
           [TL].[Code] AS TaskItems_TaskItemType_Code,
           [TL].[Name] AS TaskItems_TaskItemType_Name,
           [TL].[OrderIndex] AS TaskItems_TaskItemType_OrderIndex,
           [TL].[Description] AS TaskItems_TaskItemType_Description,
           [PL].[Code] AS TaskItems_Priority_Code,
           [PL].[Name] AS TaskItems_Priority_Name,
           [PL].[OrderIndex] AS TaskItems_Priority_OrderIndex,
           [PL].[Description] AS TaskItems_Priority_Description
    FROM   [dbo].[Task] AS [TA]
           INNER JOIN
           [dbo].[TaskItem] AS [TI]
           ON [TI].[TaskId] = [TA].[Id]
           INNER JOIN
           [dbo].[Lookup] AS [TT]
           ON [TA].[TaskTypeCode] = [TT].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [SL]
           ON [TI].[StatusCode] = [SL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [TL]
           ON [TI].[TaskItemTypeCode] = [TL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [DBL]
           ON [TA].[DeviceBrandId] = [DBL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [PL]
           ON [TI].[PriorityLevelCode] = [PL].[Code]
		   INNER JOIN
           [dbo].[Lookup] AS [TPL]
           ON [TA].[PriorityLevelCode] = [TPL].[Code]
		   LEFT OUTER JOIN
           [dbo].[Device] AS [Dev]
           ON [TI].[DeviceId] = [Dev].[Id]
    WHERE  [TI].[StatusCode] = 10007
    UNION ALL
    SELECT [TA].[Id],
           [TA].[TaskTypeCode],
           [TPL].[Code] AS Priority_Code,
           [TPL].[Name] AS Priority_Name,
           [TPL].[OrderIndex] AS Priority_OrderIndex,
           [TPL].[Description] AS Priority_Description,
		   [TA].[CreatedBy] AS CreatedBy_Id,
           [TA].[CreatedAt],
           [TA].[DueDate] AS DueDate,
           [DBL].[Code] AS DeviceBrand_Code,
           [DBL].[Name] AS DeviceBrand_Name,
           [DBL].[OrderIndex] AS DeviceBrand_OrderIndex,
           [DBL].[Description] AS DeviceBrand_Description,
           [TA].[UpdatedAt],
           [TA].[UpdatedBy] AS UpdatedBy_Id,
           [TA].[schedulingPattern],
           (SELECT CASE WHEN EXISTS (SELECT   TaskId
                                     FROM     TaskItem AS t
                                     WHERE    t.StatusCode = 10003
                                              AND TaskId = [TA].Id
                                     GROUP BY StatusCode, TaskId) THEN 10003 WHEN EXISTS (SELECT   TaskId
                                                                                          FROM     TaskItem AS t
                                                                                          WHERE    t.StatusCode = 10007
                                                                                                   AND TaskId = [TA].Id
                                                                                          GROUP BY StatusCode, TaskId) THEN 10007 WHEN EXISTS (SELECT   TaskId
                                                                                                                                               FROM     TaskItem AS t
                                                                                                                                               WHERE    t.StatusCode = 10001
                                                                                                                                                        AND TaskId = [TA].Id
                                                                                                                                               GROUP BY StatusCode, TaskId) THEN 10001 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                    FROM     TaskItem AS t
                                                                                                                                                                                                    WHERE    t.StatusCode = 10006
                                                                                                                                                                                                             AND TaskId = [TA].Id
                                                                                                                                                                                                    GROUP BY StatusCode, TaskId) THEN 10006 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                         FROM     TaskItem AS t
                                                                                                                                                                                                                                                         WHERE    t.StatusCode = 10004
                                                                                                                                                                                                                                                                  AND TaskId = [TA].Id
                                                                                                                                                                                                                                                         GROUP BY StatusCode, TaskId) THEN 10004 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                              FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                              WHERE    t.StatusCode = 10005
                                                                                                                                                                                                                                                                                                                       AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                              GROUP BY StatusCode, TaskId) THEN 10005 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                   FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                   WHERE    t.StatusCode = 10008
                                                                                                                                                                                                                                                                                                                                                                            AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                   GROUP BY StatusCode, TaskId) THEN 10008 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                                                                        FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                                                                        WHERE    t.StatusCode = 10002
                                                                                                                                                                                                                                                                                                                                                                                                                                 AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                                                                        GROUP BY StatusCode, TaskId) THEN 10002 ELSE 10007 END AS codeStatus) AS Status_Code,
           (SELECT SUM(CurrentIndex)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS CurrentIndex,
           (SELECT SUM(TotalCount)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS TotalCount,
           [TT].[Code] AS TaskType_Code,
           [TT].[Name] AS TaskType_Name,
           [TT].[OrderIndex] AS TaskType_OrderIndex,
           [TT].[Description] AS TaskType_Description,
           [TI].[Id] AS TaskItems_Id,
           [TI].[Data] AS TaskItems_Data,
           [TI].[DeviceId] AS TaskItems_DeviceId,
           [TI].[OrderIndex] AS TaskItems_OrderIndex,
           [TI].[IsScheduled] AS TaskItems_IsScheduled,
           [TI].[IsParallelRestricted] AS TaskItems_IsParallelRestricted,
           [Dev].[Id] AS TaskItems_Device_DeviceId,
           [Dev].[DeviceModelId] AS TaskItems_Device_DeviceModelId,
           [Dev].[Name] AS TaskItems_Device_Name,
           [Dev].[Active] AS TaskItems_Device_Active,
           [Dev].[IPAddress] AS TaskItems_Device_IpAddress,
           [Dev].[Port] AS TaskItems_Device_Port,
           [Dev].[MacAddress] AS TaskItems_Device_MacAddress,
           [Dev].[RegisterDate] AS TaskItems_Device_RegisterDate,
           [Dev].[HardwareVersion] AS TaskItems_Device_HardwareVersion,
           [Dev].[FirmwareVersion] AS TaskItems_Device_FirmwareVersion,
           [Dev].[DeviceLockPassword] AS TaskItems_Device_DeviceLockPassword,
           [Dev].[SSL] AS TaskItems_Device_SSL,
           [Dev].[TimeSync] AS TaskItems_Device_TimeSync,
           [Dev].[SerialNumber] AS TaskItems_Device_SerialNumber,
           [Dev].[DeviceTypeId] AS TaskItems_Device_DeviceType_Id,
           [Dev].[Code] AS TaskItems_Device_Code,
           [SL].[Code] AS TaskItems_Status_Code,
           [SL].[Name] AS TaskItems_Status_Name,
           [SL].[OrderIndex] AS TaskItems_Status_OrderIndex,
           [SL].[Description] AS TaskItems_Status_Description,
           [TL].[Code] AS TaskItems_TaskItemType_Code,
           [TL].[Name] AS TaskItems_TaskItemType_Name,
           [TL].[OrderIndex] AS TaskItems_TaskItemType_OrderIndex,
           [TL].[Description] AS TaskItems_TaskItemType_Description,
           [PL].[Code] AS TaskItems_Priority_Code,
           [PL].[Name] AS TaskItems_Priority_Name,
           [PL].[OrderIndex] AS TaskItems_Priority_OrderIndex,
           [PL].[Description] AS TaskItems_Priority_Description
    FROM   [dbo].[Task] AS [TA]
           INNER JOIN
           [dbo].[TaskItem] AS [TI]
           ON [TI].[TaskId] = [TA].[Id]
           INNER JOIN
           [dbo].[Lookup] AS [TT]
           ON [TA].[TaskTypeCode] = [TT].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [SL]
           ON [TI].[StatusCode] = [SL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [TL]
           ON [TI].[TaskItemTypeCode] = [TL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [DBL]
           ON [TA].[DeviceBrandId] = [DBL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [PL]
           ON [TI].[PriorityLevelCode] = [PL].[Code]
		   INNER JOIN
           [dbo].[Lookup] AS [TPL]
           ON [TA].[PriorityLevelCode] = [TPL].[Code]
		   LEFT OUTER JOIN
           [dbo].[Device] AS [Dev]
           ON [TI].[DeviceId] = [Dev].[Id]
    WHERE  [TI].[StatusCode] = 10001
    UNION ALL
    SELECT [TA].[Id],
           [TA].[TaskTypeCode],
           [TPL].[Code] AS Priority_Code,
           [TPL].[Name] AS Priority_Name,
           [TPL].[OrderIndex] AS Priority_OrderIndex,
           [TPL].[Description] AS Priority_Description,
		   [TA].[CreatedBy] AS CreatedBy_Id,
           [TA].[CreatedAt],
           [TA].[DueDate] AS DueDate,
           [DBL].[Code] AS DeviceBrand_Code,
           [DBL].[Name] AS DeviceBrand_Name,
           [DBL].[OrderIndex] AS DeviceBrand_OrderIndex,
           [DBL].[Description] AS DeviceBrand_Description,
           [TA].[UpdatedAt],
           [TA].[UpdatedBy] AS UpdatedBy_Id,
           [TA].[schedulingPattern],
           (SELECT CASE WHEN EXISTS (SELECT   TaskId
                                     FROM     TaskItem AS t
                                     WHERE    t.StatusCode = 10003
                                              AND TaskId = [TA].Id
                                     GROUP BY StatusCode, TaskId) THEN 10003 WHEN EXISTS (SELECT   TaskId
                                                                                          FROM     TaskItem AS t
                                                                                          WHERE    t.StatusCode = 10007
                                                                                                   AND TaskId = [TA].Id
                                                                                          GROUP BY StatusCode, TaskId) THEN 10007 WHEN EXISTS (SELECT   TaskId
                                                                                                                                               FROM     TaskItem AS t
                                                                                                                                               WHERE    t.StatusCode = 10001
                                                                                                                                                        AND TaskId = [TA].Id
                                                                                                                                               GROUP BY StatusCode, TaskId) THEN 10001 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                    FROM     TaskItem AS t
                                                                                                                                                                                                    WHERE    t.StatusCode = 10006
                                                                                                                                                                                                             AND TaskId = [TA].Id
                                                                                                                                                                                                    GROUP BY StatusCode, TaskId) THEN 10006 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                         FROM     TaskItem AS t
                                                                                                                                                                                                                                                         WHERE    t.StatusCode = 10004
                                                                                                                                                                                                                                                                  AND TaskId = [TA].Id
                                                                                                                                                                                                                                                         GROUP BY StatusCode, TaskId) THEN 10004 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                              FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                              WHERE    t.StatusCode = 10005
                                                                                                                                                                                                                                                                                                                       AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                              GROUP BY StatusCode, TaskId) THEN 10005 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                   FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                   WHERE    t.StatusCode = 10008
                                                                                                                                                                                                                                                                                                                                                                            AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                   GROUP BY StatusCode, TaskId) THEN 10008 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                                                                        FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                                                                        WHERE    t.StatusCode = 10002
                                                                                                                                                                                                                                                                                                                                                                                                                                 AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                                                                        GROUP BY StatusCode, TaskId) THEN 10002 ELSE 10007 END AS codeStatus) AS Status_Code,
           (SELECT SUM(CurrentIndex)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS CurrentIndex,
           (SELECT SUM(TotalCount)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS TotalCount,
           [TT].[Code] AS TaskType_Code,
           [TT].[Name] AS TaskType_Name,
           [TT].[OrderIndex] AS TaskType_OrderIndex,
           [TT].[Description] AS TaskType_Description,
           [TI].[Id] AS TaskItems_Id,
           [TI].[Data] AS TaskItems_Data,
           [TI].[DeviceId] AS TaskItems_DeviceId,
           [TI].[OrderIndex] AS TaskItems_OrderIndex,
           [TI].[IsScheduled] AS TaskItems_IsScheduled,
           [TI].[IsParallelRestricted] AS TaskItems_IsParallelRestricted,
           [Dev].[Id] AS TaskItems_Device_DeviceId,
           [Dev].[DeviceModelId] AS TaskItems_Device_DeviceModelId,
           [Dev].[Name] AS TaskItems_Device_Name,
           [Dev].[Active] AS TaskItems_Device_Active,
           [Dev].[IPAddress] AS TaskItems_Device_IpAddress,
           [Dev].[Port] AS TaskItems_Device_Port,
           [Dev].[MacAddress] AS TaskItems_Device_MacAddress,
           [Dev].[RegisterDate] AS TaskItems_Device_RegisterDate,
           [Dev].[HardwareVersion] AS TaskItems_Device_HardwareVersion,
           [Dev].[FirmwareVersion] AS TaskItems_Device_FirmwareVersion,
           [Dev].[DeviceLockPassword] AS TaskItems_Device_DeviceLockPassword,
           [Dev].[SSL] AS TaskItems_Device_SSL,
           [Dev].[TimeSync] AS TaskItems_Device_TimeSync,
           [Dev].[SerialNumber] AS TaskItems_Device_SerialNumber,
           [Dev].[DeviceTypeId] AS TaskItems_Device_DeviceType_Id,
           [Dev].[Code] AS TaskItems_Device_Code,
           [SL].[Code] AS TaskItems_Status_Code,
           [SL].[Name] AS TaskItems_Status_Name,
           [SL].[OrderIndex] AS TaskItems_Status_OrderIndex,
           [SL].[Description] AS TaskItems_Status_Description,
           [TL].[Code] AS TaskItems_TaskItemType_Code,
           [TL].[Name] AS TaskItems_TaskItemType_Name,
           [TL].[OrderIndex] AS TaskItems_TaskItemType_OrderIndex,
           [TL].[Description] AS TaskItems_TaskItemType_Description,
           [PL].[Code] AS TaskItems_Priority_Code,
           [PL].[Name] AS TaskItems_Priority_Name,
           [PL].[OrderIndex] AS TaskItems_Priority_OrderIndex,
           [PL].[Description] AS TaskItems_Priority_Description
    FROM   [dbo].[Task] AS [TA]
           INNER JOIN
           [dbo].[TaskItem] AS [TI]
           ON [TI].[TaskId] = [TA].[Id]
           INNER JOIN
           [dbo].[Lookup] AS [TT]
           ON [TA].[TaskTypeCode] = [TT].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [SL]
           ON [TI].[StatusCode] = [SL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [TL]
           ON [TI].[TaskItemTypeCode] = [TL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [DBL]
           ON [TA].[DeviceBrandId] = [DBL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [PL]
           ON [TI].[PriorityLevelCode] = [PL].[Code]
		   INNER JOIN
           [dbo].[Lookup] AS [TPL]
           ON [TA].[PriorityLevelCode] = [TPL].[Code]
		   LEFT OUTER JOIN
           [dbo].[Device] AS [Dev]
           ON [TI].[DeviceId] = [Dev].[Id]
    WHERE  [TI].[StatusCode] = 10006
    UNION ALL
    SELECT [TA].[Id],
           [TA].[TaskTypeCode],
           [TPL].[Code] AS Priority_Code,
           [TPL].[Name] AS Priority_Name,
           [TPL].[OrderIndex] AS Priority_OrderIndex,
           [TPL].[Description] AS Priority_Description,
           [TA].[CreatedBy] AS CreatedBy_Id,
           [TA].[CreatedAt],
           [TA].[DueDate] AS DueDate,
           [DBL].[Code] AS DeviceBrand_Code,
           [DBL].[Name] AS DeviceBrand_Name,
           [DBL].[OrderIndex] AS DeviceBrand_OrderIndex,
           [DBL].[Description] AS DeviceBrand_Description,
           [TA].[UpdatedAt],
           [TA].[UpdatedBy] AS UpdatedBy_Id,
           [TA].[schedulingPattern],
           (SELECT CASE WHEN EXISTS (SELECT   TaskId
                                     FROM     TaskItem AS t
                                     WHERE    t.StatusCode = 10003
                                              AND TaskId = [TA].Id
                                     GROUP BY StatusCode, TaskId) THEN 10003 WHEN EXISTS (SELECT   TaskId
                                                                                          FROM     TaskItem AS t
                                                                                          WHERE    t.StatusCode = 10007
                                                                                                   AND TaskId = [TA].Id
                                                                                          GROUP BY StatusCode, TaskId) THEN 10007 WHEN EXISTS (SELECT   TaskId
                                                                                                                                               FROM     TaskItem AS t
                                                                                                                                               WHERE    t.StatusCode = 10001
                                                                                                                                                        AND TaskId = [TA].Id
                                                                                                                                               GROUP BY StatusCode, TaskId) THEN 10001 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                    FROM     TaskItem AS t
                                                                                                                                                                                                    WHERE    t.StatusCode = 10006
                                                                                                                                                                                                             AND TaskId = [TA].Id
                                                                                                                                                                                                    GROUP BY StatusCode, TaskId) THEN 10006 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                         FROM     TaskItem AS t
                                                                                                                                                                                                                                                         WHERE    t.StatusCode = 10004
                                                                                                                                                                                                                                                                  AND TaskId = [TA].Id
                                                                                                                                                                                                                                                         GROUP BY StatusCode, TaskId) THEN 10004 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                              FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                              WHERE    t.StatusCode = 10005
                                                                                                                                                                                                                                                                                                                       AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                              GROUP BY StatusCode, TaskId) THEN 10005 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                   FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                   WHERE    t.StatusCode = 10008
                                                                                                                                                                                                                                                                                                                                                                            AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                   GROUP BY StatusCode, TaskId) THEN 10008 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                                                                        FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                                                                        WHERE    t.StatusCode = 10002
                                                                                                                                                                                                                                                                                                                                                                                                                                 AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                                                                        GROUP BY StatusCode, TaskId) THEN 10002 ELSE 10007 END AS codeStatus) AS Status_Code,
           (SELECT SUM(CurrentIndex)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS CurrentIndex,
           (SELECT SUM(TotalCount)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS TotalCount,
           [TT].[Code] AS TaskType_Code,
           [TT].[Name] AS TaskType_Name,
           [TT].[OrderIndex] AS TaskType_OrderIndex,
           [TT].[Description] AS TaskType_Description,
           [TI].[Id] AS TaskItems_Id,
           [TI].[Data] AS TaskItems_Data,
           [TI].[DeviceId] AS TaskItems_DeviceId,
           [TI].[OrderIndex] AS TaskItems_OrderIndex,
           [TI].[IsScheduled] AS TaskItems_IsScheduled,
           [TI].[IsParallelRestricted] AS TaskItems_IsParallelRestricted,
           [Dev].[Id] AS TaskItems_Device_DeviceId,
           [Dev].[DeviceModelId] AS TaskItems_Device_DeviceModelId,
           [Dev].[Name] AS TaskItems_Device_Name,
           [Dev].[Active] AS TaskItems_Device_Active,
           [Dev].[IPAddress] AS TaskItems_Device_IpAddress,
           [Dev].[Port] AS TaskItems_Device_Port,
           [Dev].[MacAddress] AS TaskItems_Device_MacAddress,
           [Dev].[RegisterDate] AS TaskItems_Device_RegisterDate,
           [Dev].[HardwareVersion] AS TaskItems_Device_HardwareVersion,
           [Dev].[FirmwareVersion] AS TaskItems_Device_FirmwareVersion,
           [Dev].[DeviceLockPassword] AS TaskItems_Device_DeviceLockPassword,
           [Dev].[SSL] AS TaskItems_Device_SSL,
           [Dev].[TimeSync] AS TaskItems_Device_TimeSync,
           [Dev].[SerialNumber] AS TaskItems_Device_SerialNumber,
           [Dev].[DeviceTypeId] AS TaskItems_Device_DeviceType_Id,
           [Dev].[Code] AS TaskItems_Device_Code,
           [SL].[Code] AS TaskItems_Status_Code,
           [SL].[Name] AS TaskItems_Status_Name,
           [SL].[OrderIndex] AS TaskItems_Status_OrderIndex,
           [SL].[Description] AS TaskItems_Status_Description,
           [TL].[Code] AS TaskItems_TaskItemType_Code,
           [TL].[Name] AS TaskItems_TaskItemType_Name,
           [TL].[OrderIndex] AS TaskItems_TaskItemType_OrderIndex,
           [TL].[Description] AS TaskItems_TaskItemType_Description,
           [PL].[Code] AS TaskItems_Priority_Code,
           [PL].[Name] AS TaskItems_Priority_Name,
           [PL].[OrderIndex] AS TaskItems_Priority_OrderIndex,
           [PL].[Description] AS TaskItems_Priority_Description
    FROM   [dbo].[Task] AS [TA]
           INNER JOIN
           [dbo].[TaskItem] AS [TI]
           ON [TI].[TaskId] = [TA].[Id]
           INNER JOIN
           [dbo].[Lookup] AS [TT]
           ON [TA].[TaskTypeCode] = [TT].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [SL]
           ON [TI].[StatusCode] = [SL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [TL]
           ON [TI].[TaskItemTypeCode] = [TL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [DBL]
           ON [TA].[DeviceBrandId] = [DBL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [PL]
           ON [TI].[PriorityLevelCode] = [PL].[Code]
		   INNER JOIN
           [dbo].[Lookup] AS [TPL]
           ON [TA].[PriorityLevelCode] = [TPL].[Code]
		   LEFT OUTER JOIN
           [dbo].[Device] AS [Dev]
           ON [TI].[DeviceId] = [Dev].[Id]
    WHERE  [TI].[StatusCode] = 10002
    UNION ALL
    SELECT [TA].[Id],
           [TA].[TaskTypeCode],
           [TPL].[Code] AS Priority_Code,
           [TPL].[Name] AS Priority_Name,
           [TPL].[OrderIndex] AS Priority_OrderIndex,
           [TPL].[Description] AS Priority_Description,
           [TA].[CreatedBy] AS CreatedBy_Id,
           [TA].[CreatedAt],
           [TA].[DueDate] AS DueDate,
           [DBL].[Code] AS DeviceBrand_Code,
           [DBL].[Name] AS DeviceBrand_Name,
           [DBL].[OrderIndex] AS DeviceBrand_OrderIndex,
           [DBL].[Description] AS DeviceBrand_Description,
           [TA].[UpdatedAt],
           [TA].[UpdatedBy] AS UpdatedBy_Id,
           [TA].[schedulingPattern],
           (SELECT CASE WHEN EXISTS (SELECT   TaskId
                                     FROM     TaskItem AS t
                                     WHERE    t.StatusCode = 10003
                                              AND TaskId = [TA].Id
                                     GROUP BY StatusCode, TaskId) THEN 10003 WHEN EXISTS (SELECT   TaskId
                                                                                          FROM     TaskItem AS t
                                                                                          WHERE    t.StatusCode = 10007
                                                                                                   AND TaskId = [TA].Id
                                                                                          GROUP BY StatusCode, TaskId) THEN 10007 WHEN EXISTS (SELECT   TaskId
                                                                                                                                               FROM     TaskItem AS t
                                                                                                                                               WHERE    t.StatusCode = 10001
                                                                                                                                                        AND TaskId = [TA].Id
                                                                                                                                               GROUP BY StatusCode, TaskId) THEN 10001 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                    FROM     TaskItem AS t
                                                                                                                                                                                                    WHERE    t.StatusCode = 10006
                                                                                                                                                                                                             AND TaskId = [TA].Id
                                                                                                                                                                                                    GROUP BY StatusCode, TaskId) THEN 10006 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                         FROM     TaskItem AS t
                                                                                                                                                                                                                                                         WHERE    t.StatusCode = 10004
                                                                                                                                                                                                                                                                  AND TaskId = [TA].Id
                                                                                                                                                                                                                                                         GROUP BY StatusCode, TaskId) THEN 10004 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                              FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                              WHERE    t.StatusCode = 10005
                                                                                                                                                                                                                                                                                                                       AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                              GROUP BY StatusCode, TaskId) THEN 10005 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                   FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                   WHERE    t.StatusCode = 10008
                                                                                                                                                                                                                                                                                                                                                                            AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                   GROUP BY StatusCode, TaskId) THEN 10008 WHEN EXISTS (SELECT   TaskId
                                                                                                                                                                                                                                                                                                                                                                                                                        FROM     TaskItem AS t
                                                                                                                                                                                                                                                                                                                                                                                                                        WHERE    t.StatusCode = 10002
                                                                                                                                                                                                                                                                                                                                                                                                                                 AND TaskId = [TA].Id
                                                                                                                                                                                                                                                                                                                                                                                                                        GROUP BY StatusCode, TaskId) THEN 10002 ELSE 10007 END AS codeStatus) AS Status_Code,
           (SELECT SUM(CurrentIndex)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS CurrentIndex,
           (SELECT SUM(TotalCount)
            FROM   TaskItem AS t
            WHERE  t.TaskId = [TA].Id) AS TotalCount,
           [TT].[Code] AS TaskType_Code,
           [TT].[Name] AS TaskType_Name,
           [TT].[OrderIndex] AS TaskType_OrderIndex,
           [TT].[Description] AS TaskType_Description,
           [TI].[Id] AS TaskItems_Id,
           [TI].[Data] AS TaskItems_Data,
           [TI].[DeviceId] AS TaskItems_DeviceId,
           [TI].[OrderIndex] AS TaskItems_OrderIndex,
           [TI].[IsScheduled] AS TaskItems_IsScheduled,
           [TI].[IsParallelRestricted] AS TaskItems_IsParallelRestricted,
           [Dev].[Id] AS TaskItems_Device_DeviceId,
           [Dev].[DeviceModelId] AS TaskItems_Device_DeviceModelId,
           [Dev].[Name] AS TaskItems_Device_Name,
           [Dev].[Active] AS TaskItems_Device_Active,
           [Dev].[IPAddress] AS TaskItems_Device_IpAddress,
           [Dev].[Port] AS TaskItems_Device_Port,
           [Dev].[MacAddress] AS TaskItems_Device_MacAddress,
           [Dev].[RegisterDate] AS TaskItems_Device_RegisterDate,
           [Dev].[HardwareVersion] AS TaskItems_Device_HardwareVersion,
           [Dev].[FirmwareVersion] AS TaskItems_Device_FirmwareVersion,
           [Dev].[DeviceLockPassword] AS TaskItems_Device_DeviceLockPassword,
           [Dev].[SSL] AS TaskItems_Device_SSL,
           [Dev].[TimeSync] AS TaskItems_Device_TimeSync,
           [Dev].[SerialNumber] AS TaskItems_Device_SerialNumber,
           [Dev].[DeviceTypeId] AS TaskItems_Device_DeviceType_Id,
           [Dev].[Code] AS TaskItems_Device_Code,
           [SL].[Code] AS TaskItems_Status_Code,
           [SL].[Name] AS TaskItems_Status_Name,
           [SL].[OrderIndex] AS TaskItems_Status_OrderIndex,
           [SL].[Description] AS TaskItems_Status_Description,
           [TL].[Code] AS TaskItems_TaskItemType_Code,
           [TL].[Name] AS TaskItems_TaskItemType_Name,
           [TL].[OrderIndex] AS TaskItems_TaskItemType_OrderIndex,
           [TL].[Description] AS TaskItems_TaskItemType_Description,
           [PL].[Code] AS TaskItems_Priority_Code,
           [PL].[Name] AS TaskItems_Priority_Name,
           [PL].[OrderIndex] AS TaskItems_Priority_OrderIndex,
           [PL].[Description] AS TaskItems_Priority_Description
    FROM   [dbo].[Task] AS [TA]
           INNER JOIN
           [dbo].[TaskItem] AS [TI]
           ON [TI].[TaskId] = [TA].[Id]
           INNER JOIN
           [dbo].[Lookup] AS [TT]
           ON [TA].[TaskTypeCode] = [TT].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [SL]
           ON [TI].[StatusCode] = [SL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [TL]
           ON [TI].[TaskItemTypeCode] = [TL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [DBL]
           ON [TA].[DeviceBrandId] = [DBL].[Code]
           INNER JOIN
           [dbo].[Lookup] AS [PL]
           ON [TI].[PriorityLevelCode] = [PL].[Code]
		   INNER JOIN
           [dbo].[Lookup] AS [TPL]
           ON [TA].[PriorityLevelCode] = [TPL].[Code]
		   LEFT OUTER JOIN
           [dbo].[Device] AS [Dev]
           ON [TI].[DeviceId] = [Dev].[Id]
    WHERE  [TI].[StatusCode] = 10008;