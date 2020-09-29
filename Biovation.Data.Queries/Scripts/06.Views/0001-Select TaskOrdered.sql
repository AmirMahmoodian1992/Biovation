CREATE VIEW TaskOrdered AS
SELECT [TA].[Id]
      ,[TA].[TaskTypeCode]
      ,[TA].[PriorityLevelCode]
      ,[TA].[CreatedBy] AS CreatedBy_Id
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
	WHERE [TI].[StatusCode] = 10004
	UNION ALL
	SELECT [TA].[Id]
      ,[TA].[TaskTypeCode]
      ,[TA].[PriorityLevelCode]
      ,[TA].[CreatedBy] AS CreatedBy_Id
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
	WHERE [TI].[StatusCode] = 10005
	UNION ALL
	SELECT [TA].[Id]
      ,[TA].[TaskTypeCode]
      ,[TA].[PriorityLevelCode]
      ,[TA].[CreatedBy] AS CreatedBy_Id
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
	WHERE [TI].[StatusCode] = 10003
	UNION ALL
	SELECT [TA].[Id]
      ,[TA].[TaskTypeCode]
      ,[TA].[PriorityLevelCode]
      ,[TA].[CreatedBy] AS CreatedBy_Id
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
	WHERE [TI].[StatusCode] = 10007
	UNION ALL
	SELECT [TA].[Id]
      ,[TA].[TaskTypeCode]
      ,[TA].[PriorityLevelCode]
      ,[TA].[CreatedBy] AS CreatedBy_Id
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
	WHERE [TI].[StatusCode] = 10001
	UNION ALL
	SELECT [TA].[Id]
      ,[TA].[TaskTypeCode]
      ,[TA].[PriorityLevelCode]
      ,[TA].[CreatedBy] AS CreatedBy_Id
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
	WHERE [TI].[StatusCode] = 10006
	UNION ALL
	SELECT [TA].[Id]
      ,[TA].[TaskTypeCode]
      ,[TA].[PriorityLevelCode]
      ,[TA].[CreatedBy] AS CreatedBy_Id
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
	WHERE [TI].[StatusCode] = 10003