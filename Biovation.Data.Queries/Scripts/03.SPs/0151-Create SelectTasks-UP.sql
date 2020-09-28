
CREATE PROCEDURE [dbo].[SelectTasks]
@taskId INT=0,
@taskItemId INT=0,
@brandId INT=0,
@deviceId INT=0, 
@taskTypeCode NVARCHAR (100)=NULL,
@taskStatusCodes NVARCHAR (200)=NULL,
@excludedTaskStatusCodes NVARCHAR (200)=NULL
AS
BEGIN
    DECLARE @query AS NVARCHAR (MAX);
    DECLARE @paramDefinition AS NVARCHAR (MAX) = '@taskId int = 0,@taskItemId int = 0, @brandId int = 0, @deviceId int = 0, @taskTypeCode NVARCHAR(100) = NULL, @taskStatusCodes NVARCHAR(200) = NULL, @excludedTaskStatusCodes NVARCHAR(200) = NULL';
   SELECT @query = 'SELECT  
	[Id], [TaskTypeCode], [PriorityLevelCode], [CreatedBy_Id], [CreatedAt], [DeviceBrand_Code], 
	[DeviceBrand_Name], [DeviceBrand_OrderIndex], [DeviceBrand_Description], [TaskType_Code], 
	[TaskType_Name], [TaskType_OrderIndex], [TaskType_Description], [TaskItems_Id], [TaskItems_Data], 
	[TaskItems_DeviceId], [TaskItems_OrderIndex], [TaskItems_IsScheduled], [TaskItems_DueDate], 
	[TaskItems_IsParallelRestricted], [TaskItems_Device_DeviceId], [TaskItems_Device_DeviceModelId], 
	[TaskItems_Device_Name], [TaskItems_Device_Active], [TaskItems_Device_IpAddress], 
	[TaskItems_Device_Port], [TaskItems_Device_MacAddress], [TaskItems_Device_RegisterDate], 
	[TaskItems_Device_HardwareVersion], [TaskItems_Device_FirmwareVersion], 
	[TaskItems_Device_DeviceLockPassword], [TaskItems_Device_SSL], [TaskItems_Device_TimeSync], 
	[TaskItems_Device_SerialNumber], [TaskItems_Device_DeviceType_Id], [TaskItems_Device_Code], 
	[TaskItems_Status_Code], [TaskItems_Status_Name], [TaskItems_Status_OrderIndex], 
	[TaskItems_Status_Description], [TaskItems_TaskItemType_Code], [TaskItems_TaskItemType_Name], 
	[TaskItems_TaskItemType_OrderIndex], [TaskItems_TaskItemType_Description], [TaskItems_Priority_Code],
	[TaskItems_Priority_Name], [TaskItems_Priority_OrderIndex], [TaskItems_Priority_Description]
	FROM TaskOrdered
    WHERE   ([Id] = @taskId OR ISNULL(@taskId, 0) = 0) 
		AND (TaskItems_Device_DeviceId = @deviceId OR ISNULL(@deviceId, 0) = 0)
		AND (TaskItems_Id = @taskItemId OR ISNULL(@taskItemId, 0) = 0) 
		AND (DeviceBrand_Code = @brandId OR ISNULL(@brandId, 0) = 0)
	
		AND (TaskTypeCode = @taskTypeCode OR ISNULL(@taskTypeCode, 0) = 0)' + CASE WHEN @taskStatusCodes IS NOT NULL THEN ' AND TaskItems_Status_Code IN ' + @taskStatusCodes ELSE '' END + CASE WHEN @excludedTaskStatusCodes IS NOT NULL THEN ' AND TaskItems_Status_Code NOT IN ' + @excludedTaskStatusCodes ELSE '' END;
		EXECUTE sp_executesql @query, @paramDefinition, @taskId,@taskItemId, @brandId, @deviceId, @taskTypeCode, @taskStatusCodes, @excludedTaskStatusCodes;
END

