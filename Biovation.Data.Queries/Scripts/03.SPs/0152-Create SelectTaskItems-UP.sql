
CREATE PROCEDURE [dbo].[SelectTaskItems]
@Id int = 0,
@taskId int = 0
AS
DECLARE @Message AS NVARCHAR (200) = N' درخواست با موفقیت انجام گرفت', @Validate AS INT = 1,  @Code AS NVARCHAR (15) = N'200';
BEGIN
    SELECT [TI].[Id]
      ,[TI].[DeviceId]
      ,[TI].[Data]
      ,[TI].[OrderIndex]
      ,[TI].[IsScheduled]
      ,[TI].[IsParallelRestricted]
      ,[TI].[CurrentIndex],
	   [TI].[TotalCount],
	   [TI].[FinishedAt],
	   [TI].[ExecutionAt]

	  ,[Dev].[Id] AS Device_DeviceId
      ,[Dev].[DeviceModelId] AS Device_DeviceModelId
      ,[Dev].[Name] AS Device_Name
      ,[Dev].[Active] AS Device_Active
      ,[Dev].[IPAddress] AS Device_IpAddress
      ,[Dev].[Port] AS Device_Port
      ,[Dev].[MacAddress] AS Device_MacAddress
      ,[Dev].[RegisterDate] AS Device_RegisterDate
      ,[Dev].[HardwareVersion] AS Device_HardwareVersion
      ,[Dev].[FirmwareVersion] AS Device_FirmwareVersion
      ,[Dev].[DeviceLockPassword] AS Device_DeviceLockPassword
      ,[Dev].[SSL] AS Device_SSL
      ,[Dev].[TimeSync] AS Device_TimeSync
      ,[Dev].[SerialNumber] AS Device_SerialNumber
      ,[Dev].[DeviceTypeId] AS Device_DeviceType_Id
      ,[Dev].[Code] AS Device_Code

	  ,[SL].[Code] AS Status_Code
      ,[SL].[Name] AS Status_Name
      ,[SL].[OrderIndex] AS Status_OrderIndex
      ,[SL].[Description] AS Status_Description

	  ,[TL].[Code] AS TaskItemType_Code
      ,[TL].[Name] AS TaskItemType_Name
      ,[TL].[OrderIndex] AS TaskItemType_OrderIndex
      ,[TL].[Description] AS TaskItemType_Description

	  ,[PL].[Code] AS Priority_Code
      ,[PL].[Name] AS Priority_Name
      ,[PL].[OrderIndex] AS Priority_OrderIndex
      ,[PL].[Description] AS Priority_Description


       ,@Message AS e_Message
       ,@Validate AS e_Validate
       ,@Code AS e_Code

  FROM [dbo].[TaskItem] AS [TI]
    JOIN [dbo].[Task] AS [TA] ON [TI].[TaskId] = [TA].[Id]
    JOIN [dbo].[Lookup] AS [SL] ON [TI].[StatusCode] = [SL].[Code]
    JOIN [dbo].[Lookup] AS [TL] ON [TI].[TaskItemTypeCode] = [TL].[Code]
    JOIN [dbo].[Lookup] AS [PL] ON [TI].[PriorityLevelCode] = [PL].[Code]
	LEFT JOIN [dbo].[Device] AS [Dev] ON [TI].[DeviceId] = [Dev].[Id]
    WHERE  ([TI].[Id] = @Id
           OR ISNULL(@Id, 0) = 0)
		AND ([TA].[Id] = @taskId
           OR ISNULL(@taskId, 0) = 0)
	ORDER BY [TI].[OrderIndex] ASC
           
END
