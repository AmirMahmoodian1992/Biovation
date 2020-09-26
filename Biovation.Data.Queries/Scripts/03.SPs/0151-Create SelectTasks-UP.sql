
CREATE PROCEDURE [dbo].[SelectTasks]
@taskId INT=0,
@brandId INT=0,
@deviceId INT=0, 
@taskTypeCode NVARCHAR (100)=NULL,
@taskStatusCodes NVARCHAR (200)=NULL,
@excludedTaskStatusCodes NVARCHAR (200)=NULL
AS
BEGIN
    DECLARE @query AS NVARCHAR (MAX);
    DECLARE @paramDefinition AS NVARCHAR (MAX) = '@taskId int = 0, @brandId int = 0, @deviceId int = 0, @taskTypeCode NVARCHAR(100) = NULL, @taskStatusCodes NVARCHAR(200) = NULL, @excludedTaskStatusCodes NVARCHAR(200) = NULL';
    SELECT @query = 'SELECT [TA].[Id]
      ,[TA].[TaskTypeCode]
      ,[TA].[PriorityLevelCode]
      ,[TA].[CreatedBy]
      ,[TA].[CreatedAt]

	  
	  ,[DBL].[Code] AS DeviceBrand_Code
      ,[DBL].[Name] AS DeviceBrand_Name
      ,[DBL].[OrderIndex] AS DeviceBrand_OrderIndex
      ,[DBL].[Description] AS DeviceBrand_Description



	  ,[TT].[Code] AS TaskType_Code
      ,[TT].[Name] AS TaskType_Name
      ,[TT].[OrderIndex] AS TaskType_OrderIndex
      ,[TT].[Description] AS TaskType_Description

	  ,[TI].[Id] AS TaskItems_Id
      ,[TI].[Data] AS TaskItems_Data
      ,[TI].[DeviceId] AS TaskItems_DeviceId
      ,[TI].[OrderIndex] AS TaskItems_OrderIndex
      ,[TI].[IsScheduled] AS TaskItems_IsScheduled
      ,[TI].[DueDate] AS TaskItems_DueDate
      ,[TI].[IsParallelRestricted] AS TaskItems_IsParallelRestricted

	  ,[Dev].[Id] AS TaskItems_Device_DeviceId
      ,[Dev].[DeviceModelId] AS TaskItems_Device_DeviceModelId
      ,[Dev].[Name] AS TaskItems_Device_Name
      ,[Dev].[Active] AS TaskItems_Device_Active
      ,[Dev].[IPAddress] AS TaskItems_Device_IpAddress
      ,[Dev].[Port] AS TaskItems_Device_Port
      ,[Dev].[MacAddress] AS TaskItems_Device_MacAddress
      ,[Dev].[RegisterDate] AS TaskItems_Device_RegisterDate
      ,[Dev].[HardwareVersion] AS TaskItems_Device_HardwareVersion
      ,[Dev].[FirmwareVersion] AS TaskItems_Device_FirmwareVersion
      ,[Dev].[DeviceLockPassword] AS TaskItems_Device_DeviceLockPassword
      ,[Dev].[SSL] AS TaskItems_Device_SSL
      ,[Dev].[TimeSync] AS TaskItems_Device_TimeSync
      ,[Dev].[SerialNumber] AS TaskItems_Device_SerialNumber
      ,[Dev].[DeviceTypeId] AS TaskItems_Device_DeviceType_Id
      ,[Dev].[Code] AS TaskItems_Device_Code

	  ,[SL].[Code] AS TaskItems_Status_Code
      ,[SL].[Name] AS TaskItems_Status_Name
      ,[SL].[OrderIndex] AS TaskItems_Status_OrderIndex
      ,[SL].[Description] AS TaskItems_Status_Description

	  ,[TL].[Code] AS TaskItems_TaskItemType_Code
      ,[TL].[Name] AS TaskItems_TaskItemType_Name
      ,[TL].[OrderIndex] AS TaskItems_TaskItemType_OrderIndex
      ,[TL].[Description] AS TaskItems_TaskItemType_Description

	  ,[PL].[Code] AS TaskItems_Priority_Code
      ,[PL].[Name] AS TaskItems_Priority_Name
      ,[PL].[OrderIndex] AS TaskItems_Priority_OrderIndex
      ,[PL].[Description] AS TaskItems_Priority_Description

  FROM [dbo].[Task] AS [TA]
    JOIN [dbo].[TaskItem] AS [TI] ON [TI].[TaskId] = [TA].[Id]
    JOIN [dbo].[Lookup] AS [TT] ON [TA].[TaskTypeCode] = [TT].[Code]
    JOIN [dbo].[Lookup] AS [SL] ON [TI].[StatusCode] = [SL].[Code]
    JOIN [dbo].[Lookup] AS [TL] ON [TI].[TaskItemTypeCode] = [TL].[Code]
	JOIN [dbo].[Lookup] AS [DBL] ON [TA].[DeviceBrandId] = [DBL].[Code]
    JOIN [dbo].[Lookup] AS [PL] ON [TI].[PriorityLevelCode] = [PL].[Code]
	LEFT JOIN [dbo].[Device] AS [Dev] ON [TI].[DeviceId] = [Dev].[Id]
    WHERE   ([TA].[Id] = @taskId OR ISNULL(@taskId, 0) = 0) 
		AND ([Dev].[Id] = @deviceId OR ISNULL(@deviceId, 0) = 0)
		AND ([TA].[DeviceBrandId] = @brandId OR ISNULL(@brandId, 0) = 0)
		AND ([TA].[TaskTypeCode] = @taskTypeCode OR ISNULL(@taskTypeCode, 0) = 0)' + CASE WHEN @taskStatusCodes IS NOT NULL THEN ' AND [TI].[StatusCode] IN ' + @taskStatusCodes ELSE '' END + CASE WHEN @excludedTaskStatusCodes IS NOT NULL THEN ' AND [TI].[StatusCode] NOT IN ' + @excludedTaskStatusCodes ELSE '' END + ' ORDER BY [TA].[PriorityLevelCode] DESC, [TI].[OrderIndex] ASC';
    EXECUTE sp_executesql @query, @paramDefinition, @taskId, @brandId, @deviceId, @taskTypeCode, @taskStatusCodes, @excludedTaskStatusCodes;
END
