CREATE PROCEDURE [dbo].[SelectRelay]
@AdminUserId INT=NULL, @Id INT = NULL,@Name NVARCHAR(MAX) = NULL, @NodeNumber INT = NULL, @RelayHubId INT = NULL, @EntranceId INT = NULL, @ShedulingId INT = NULL,@PageNumber INT=0, @PageSize INT=0
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (5) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
    IF @HasPaging = 1
        BEGIN
			SELECT [R].[Id] AS Data_Id,
				   [R].[Name] AS Data_Name,
				   [R].[NodeNumber] AS Data_NodeNumber,
				   [R].[RelayHubId] AS Data_RelayHub_Id,
				   [R].[EntranceId] AS Data_Entrance_Id,
				   [R].[Description] AS Data_Description,
				   [S].[Id] AS Data_Schedulings_Id,
				   [S].[Mode] AS Data_Schedulings_Mode,
				   [S].[StartTime] AS Data_Schedulings_StartTime,
				   [S].[EndTime] AS Data_Schedulings_EndTime,
				   [E].[Name] AS Data_Entrance_Name,
				   [E].[Description] AS Data_Entrance_Description,
				   [SS].[Id] AS Data_Entrance_Schedulings_Id,
				   [SS].[Mode] AS Data_Entrance_Schedulings_Mode,
				   [SS].[StartTime] AS Data_Entrance_Schedulings_StartTime,
				   [SS].[EndTime] AS Data_Entrance_Schedulings_EndTime,
				   [D].[Id] AS Data_Entrance_Devices_DeviceId,
				   [D].[Code] AS Data_Entrance_Devices_Code,
				   [D].[Active] AS Data_Entrance_Devices_Active,
				   [D].[DeviceModelId] AS Data_Entrance_Devices_ModelId,
				   [D].[DeviceModelId] AS Data_Entrance_Devices_Model_Id,
				   [D].[Name] AS Data_Entrance_Devices_Name,
				   [D].[IpAddress] AS Data_Entrance_Devices_IpAddress,
				   [D].[Port] AS Data_Entrance_Devices_Port,
				   [D].[MacAddress] AS Data_Entrance_Devices_MacAddress,
				   [D].[RegisterDate] AS Data_Entrance_Devices_RegisterDate,
				   [D].[HardwareVersion] AS Data_Entrance_Devices_HardwareVersion,
				   [D].[FirmwareVersion] AS Data_Entrance_Devices_FirmwareVersion,
				   [D].[DeviceTypeId] AS Data_Entrance_Devices_DeviceTypeId,
				   [D].[DeviceLockPassword] AS Data_Entrance_Devices_DeviceLockPassword,
				   [D].[SSL] AS Data_Entrance_Devices_SSL,
				   [D].[TimeSync] AS Data_Entrance_Devices_TimeSync,
				   [D].[SerialNumber] AS Data_Entrance_Devices_SerialNumber,
				   [RH].[IpAddress] AS Data_RelayHub_IpAddress,
				   [RH].[Port] AS Data_RelayHub_Port,
				   [RH].[Capacity] AS Data_RelayHub_Capacity,
				   [RH].[Description] AS Data_RelayHub_Description,
				   [RH].[RelayHubModelId] AS Data_RelayHub_RelayHubModel_Id,
				   [DM].[Name] AS Data_RelayHub_RelayHubModel_Name,
				   [DM].[DefaultPortNumber] AS Data_RelayHub_RelayHubModel_DefaultPortNumber,
				   [DM].[ManufactureCode] AS Data_RelayHub_RelayHubModel_ManufactureCode,
				   [DM].[GetLogMethodType] AS Data_RelayHub_RelayHubModel_GetLogMethodType,
				   [DM].[Description] AS Data_RelayHub_RelayHubModel_Description,
				   [DM].[BrandId] AS Data_RelayHub_RelayHubModel_Brand_Code,
				   [L].[Name] AS Data_RelayHub_RelayHubModel_Brand_Name,
				   [L].[OrderIndex] AS Data_RelayHub_RelayHubModel_Brand_OrderIndex,
				   [L].[Description] AS Data_RelayHub_RelayHubModel_Brand_Description,
				   [L].[LookupCategoryId] AS Data_RelayHub_RelayHubModel_Brand_Category_Id,
				    (@PageNumber - 1) * @PageSize AS [from],
                     @PageNumber AS PageNumber,
                     @PageSize AS PageSize,
                     count(*) OVER () AS [Count],
                     @Message AS e_Message,
                     @Code AS e_Code,
                     @Validate AS e_Validate
			FROM [Rly].Relay AS R
				 LEFT JOIN
				 [Rly].[RelayScheduling] AS RS
				 ON [R].[Id] = [RS].[RelayId]
				 INNER JOIN
				 [Rly].[Scheduling] AS S
				 ON [S].[Id] = [RS].[SchedulingId]
				 LEFT JOIN
				 [Rly].[Entrance] AS E
				 ON [E].[Id] = [R].[EntranceId]
				 LEFT JOIN
				 [Rly].[EntranceScheduling] AS ES
				 ON [ES].[EntranceId] = [E].[Id]
				 INNER JOIN
				 [Rly].[Scheduling] AS SS
				 ON [SS].[Id] = [ES].SchedulingId
				 LEFT JOIN
				 [Rly].[EntranceDevice] AS ED
				 ON [ED].[EntranceId] = [E].Id
				 INNER JOIN
				 [dbo].[Device] AS D
				 ON [D].[Id] = [ED].[DeviceId]
				 LEFT JOIN
				 [Rly].[RelayHub] AS RH
				 ON [RH].[Id] = [R].[RelayHubId]
				 INNER JOIN
				 [dbo].[DeviceModel] AS DM
				 ON [RH].[RelayHubModelId] = [DM].[Id]
				 INNER JOIN
				 [dbo].[Lookup] AS L
				 ON [L].[Code] = [DM].[BrandId]
			WHERE (isnull(@AdminUserId, 0) = 0
                      OR EXISTS (SELECT Id
                                 FROM   [dbo].[USER]
                                 WHERE  IsMasterAdmin = 1
                                        AND ID = @AdminUserId))
					 AND ([R].Id = @Id
						  OR ISNULL(@Id,0) = 0)
					 AND ([S].Id = @ShedulingId
						  OR ISNULL(@ShedulingId,0) = 0)
                     AND ([R].Name = @Name
						  OR ISNULL(@Name,'') = '')
					 AND ([R].[NodeNumber] = @NodeNumber
						  OR ISNULL(@NodeNumber,0) = 0)
   				     AND ([R].[RelayHubId] = @RelayHubId
						  OR ISNULL(@RelayHubId,0) = 0)
					 AND ([R].[EntranceId] = @EntranceId
						  OR ISNULL(@EntranceId,0) = 0)
			ORDER BY [R].Id
            OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
		END
		ELSE
			BEGIN
				SELECT  [R].[Id] AS Data_Id,
				   [R].[Name] AS Data_Name,
				   [R].[NodeNumber] AS Data_NodeNumber,
				   [R].[RelayHubId] AS Data_RelayHub_Id,
				   [R].[EntranceId] AS Data_Entrance_Id,
				   [R].[Description] AS Data_Description,
				   [S].[Id] AS Data_Schedulings_Id,
				   [S].[Mode] AS Data_Schedulings_Mode,
				   [S].[StartTime] AS Data_Schedulings_StartTime,
				   [S].[EndTime] AS Data_Schedulings_EndTime,
				   [E].[Name] AS Data_Entrance_Name,
				   [E].[Description] AS Data_Entrance_Description,
				   [SS].[Id] AS Data_Entrance_Schedulings_Id,
				   [SS].[Mode] AS Data_Entrance_Schedulings_Mode,
				   [SS].[StartTime] AS Data_Entrance_Schedulings_StartTime,
				   [SS].[EndTime] AS Data_Entrance_Schedulings_EndTime,
				   [D].[Id] AS Data_Entrance_Devices_DeviceId,
				   [D].[Code] AS Data_Entrance_Devices_Code,
				   [D].[Active] AS Data_Entrance_Devices_Active,
				   [D].[DeviceModelId] AS Data_Entrance_Devices_ModelId,
				   [D].[DeviceModelId] AS Data_Entrance_Devices_Model_Id,
				   [D].[Name] AS Data_Entrance_Devices_Name,
				   [D].[IpAddress] AS Data_Entrance_Devices_IpAddress,
				   [D].[Port] AS Data_Entrance_Devices_Port,
				   [D].[MacAddress] AS Data_Entrance_Devices_MacAddress,
				   [D].[RegisterDate] AS Data_Entrance_Devices_RegisterDate,
				   [D].[HardwareVersion] AS Data_Entrance_Devices_HardwareVersion,
				   [D].[FirmwareVersion] AS Data_Entrance_Devices_FirmwareVersion,
				   [D].[DeviceTypeId] AS Data_Entrance_Devices_DeviceTypeId,
				   [D].[DeviceLockPassword] AS Data_Entrance_Devices_DeviceLockPassword,
				   [D].[SSL] AS Data_Entrance_Devices_SSL,
				   [D].[TimeSync] AS Data_Entrance_Devices_TimeSync,
				   [D].[SerialNumber] AS Data_Entrance_Devices_SerialNumber,
				   [RH].[IpAddress] AS Data_RelayHub_IpAddress,
				   [RH].[Port] AS Data_RelayHub_Port,
				   [RH].[Capacity] AS Data_RelayHub_Capacity,
				   [RH].[Description] AS Data_RelayHub_Description,
				   [RH].[RelayHubModelId] AS Data_RelayHub_RelayHubModel_Id,
				   [DM].[Name] AS Data_RelayHub_RelayHubModel_Name,
				   [DM].[DefaultPortNumber] AS Data_RelayHub_RelayHubModel_DefaultPortNumber,
				   [DM].[ManufactureCode] AS Data_RelayHub_RelayHubModel_ManufactureCode,
				   [DM].[GetLogMethodType] AS Data_RelayHub_RelayHubModel_GetLogMethodType,
				   [DM].[Description] AS Data_RelayHub_RelayHubModel_Description,
				   [DM].[BrandId] AS Data_RelayHub_RelayHubModel_Brand_Code,
				   [L].[Name] AS Data_RelayHub_RelayHubModel_Brand_Name,
				   [L].[OrderIndex] AS Data_RelayHub_RelayHubModel_Brand_OrderIndex,
				   [L].[Description] AS Data_RelayHub_RelayHubModel_Brand_Description,
				   [L].[LookupCategoryId] AS Data_RelayHub_RelayHubModel_Brand_Category_Id,
				   1 AS [from],
				   1 AS PageNumber,
				   count(*) OVER () AS PageSize,
				   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @Code AS e_Code,
                   @Validate AS e_Validate
		FROM [Rly].Relay AS R
				 LEFT JOIN
				 [Rly].[RelayScheduling] AS RS
				 ON [R].[Id] = [RS].[RelayId]
				 INNER JOIN
				 [Rly].[Scheduling] AS S
				 ON [S].[Id] = [RS].[SchedulingId]
				 LEFT JOIN
				 [Rly].[Entrance] AS E
				 ON [E].[Id] = [R].[EntranceId]
				 LEFT JOIN
				 [Rly].[EntranceScheduling] AS ES
				 ON [ES].[EntranceId] = [E].[Id]
				 INNER JOIN
				 [Rly].[Scheduling] AS SS
				 ON [SS].[Id] = [ES].SchedulingId
				 LEFT JOIN
				 [Rly].[EntranceDevice] AS ED
				 ON [ED].[EntranceId] = [E].Id
				 INNER JOIN
				 [dbo].[Device] AS D
				 ON [D].[Id] = [ED].[DeviceId]
				 LEFT JOIN
				 [Rly].[RelayHub] AS RH
				 ON [RH].[Id] = [R].[RelayHubId]
				 INNER JOIN
				 [dbo].[DeviceModel] AS DM
				 ON [RH].[RelayHubModelId] = [DM].[Id]
				 INNER JOIN
				 [dbo].[Lookup] AS L
				 ON [L].[Code] = [DM].[BrandId]
			WHERE (isnull(@AdminUserId, 0) = 0
                      OR EXISTS (SELECT Id
                                 FROM   [dbo].[USER]
                                 WHERE  IsMasterAdmin = 1
                                        AND ID = @AdminUserId))
					 AND ([R].Id = @Id
						  OR ISNULL(@Id,0) = 0)
					 AND ([S].Id = @ShedulingId
						  OR ISNULL(@ShedulingId,0) = 0)
                     AND ([R].Name = @Name
						  OR ISNULL(@Name,'') = '')
					 AND ([R].[NodeNumber] = @NodeNumber
						  OR ISNULL(@NodeNumber,0) = 0)
   				     AND ([R].[RelayHubId] = @RelayHubId
						  OR ISNULL(@RelayHubId,0) = 0)
					 AND ([R].[EntranceId] = @EntranceId
						  OR ISNULL(@EntranceId,0) = 0)
			END
	 
END