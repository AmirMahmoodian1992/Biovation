CREATE PROCEDURE [dbo].[SelectRelayHub]
@AdminUserId INT=NULL,@Id INT= NULL, @IpAddress NVARCHAR(50) = NULL, @Port INT = NULL, @Name NVARCHAR(MAX), @Capacity INT = NULL, @RelayHubModelId INT = NULL, @Description NVARCHAR(MAX) = NULL, @PageNumber INT=0, @PageSize INT=0
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (5) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
    IF @HasPaging = 1
        BEGIN
			SELECT [RH].[Id] AS  Data_Id,
				   [RH].IpAddress AS  Data_IpAddress,
				   [RH].[Port] AS  Data_Port,
				   [RH].[Name] AS  Data_Name,
				   [RH].[Active] AS  Data_Active,
				   [RH].[Capacity] AS  Data_Capacity,
				   [RM].Id AS  Data_RelayHubModel_Id,
				   [RM].[Name] AS  Data_RelayHubModel_Name,
				   [RM].[ManufactureCode] AS  Data_RelayHubModel_ManufactureCode,
				   [RM].[BrandId] AS  Data_RelayHubModel_Brand_Code,
				   [RM].[Description] AS  Data_RelayHubModel_Description,
				   [RM].[DefaultPortNumber] AS  Data_RelayHubModel_DefaultPortNumber,
				   [RH].[Description] AS  Data_Description,

				   [R].[Id] AS Data_Relays_Id,
				   [R].[Name] AS Data_Relays_Name,
				   [R].[NodeNumber] AS Data_Relays_NodeNumber,
				   [R].[RelayHubId] AS Data_Relays_RelayHub_Id,
				   [R].[EntranceId] AS Data_Relays_Entrance_Id,
				   [R].[Description] AS Data_Relays_Description,
				   [S].[Id] AS Data_Relays_Schedulings_Id,
				   [S].[Mode] AS Data_Relays_Schedulings_Mode,
				   [S].[StartTime] AS Data_Relays_Schedulings_StartTime,
				   [S].[EndTime] AS Data_Relays_Schedulings_EndTime,
				   [Dev].[Id] AS Data_Relays_Devices_DeviceId, 
					[Dev].[Code] AS Data_Relays_Devices_Code, 
					[dev].[DeviceModelId] AS Data_Relays_Devices_DeviceModelId, 
					[DevModel].[Id] AS Data_Relays_Devices_Model_Id, 
					[DevModel].[Name] AS Data_Relays_Devices_Model_Name, 
					[DevModel].[GetLogMethodType] AS Data_Relays_Devices_Model_GetLogMethodType, 
					[DevModel].[Description] AS Data_Relays_Devices_Model_Description, 
					[DevModel].[DefaultPortNumber] AS Data_Relays_Devices_Model_DefaultPortNumber, 
					[DevModel].[BrandId] AS Data_Relays_Devices_Model_Brand_Code, 
					[DevBrand].[Name] AS Data_Relays_Devices_Model_Brand_Name, 
					[DevBrand].[Description] AS Data_Relays_Devices_Model_Brand_Description, 
					[DevBrand].[Code] AS Data_Relays_Devices_Brand_Code, 
					[DevBrand].[Name] AS Data_Relays_Devices_Brand_Name, 
					[DevBrand].[Description] AS Data_Relays_Devices_Brand_Description, 
					[Dev].[Name] AS Data_Relays_Devices_Name, 
					[dev].[Active] AS Data_Relays_Devices_Active, 
					[dev].[IPAddress] AS Data_Relays_Devices_IPAddress, 
					[dev].[Port] AS Data_Relays_Devices_Port, 
					[dev].[MacAddress] AS Data_Relays_Devices_MacAddress, 
					[dev].[RegisterDate] AS Data_Relays_Devices_RegisterDate, 
					[dev].[HardwareVersion] AS Data_Relays_Devices_HardwareVersion, 
					[dev].[FirmwareVersion] AS Data_Relays_Devices_FirmwareVersion, 
					[dev].[DeviceLockPassword] AS Data_Relays_Devices_DeviceLockPassword, 
					[DevModel].[ManufactureCode] AS Data_Relays_Devices_ManufactureCode, 
					[dev].[SSL] AS Data_Relays_Devices_SSL, 
					[dev].[TimeSync] AS Data_Relays_Devices_TimeSync, 
					[dev].[SerialNumber] AS Data_Relays_Devices_SerialNumber, 
					[dev].[DeviceTypeId] AS Data_Relays_Devices_DeviceTypeId,
				   [E].[Name] AS Data_Relays_Entrance_Name,
				   [E].[Description] AS Data_Relays_Entrance_Description,
				   [SS].[Id] AS Data_Relays_Entrance_Schedulings_Id,
				   [SS].[Mode] AS Data_Relays_Entrance_Schedulings_Mode,
				   [SS].[StartTime] AS Data_Relays_Entrance_Schedulings_StartTime,
				   [SS].[EndTime] AS Data_Relays_Entrance_Schedulings_EndTime,
				   [D].[Id] AS Data_Relays_Entrance_Devices_DeviceId,
				   [D].[Code] AS Data_Relays_Entrance_Devices_Code,
				   [D].[Active] AS Data_Relays_Entrance_Devices_Active,
				   [D].[DeviceModelId] AS Data_Relays_Entrance_Devices_ModelId,
				   [D].[DeviceModelId] AS Data_Relays_Entrance_Devices_Model_Id,
				   [D].[Name] AS Data_Relays_Entrance_Devices_Name,
				   [D].[IpAddress] AS Data_Relays_Entrance_Devices_IpAddress,
				   [D].[Port] AS Data_Relays_Entrance_Devices_Port,
				   [D].[MacAddress] AS Data_Relays_Entrance_Devices_MacAddress,
				   [D].[RegisterDate] AS Data_Relays_Entrance_Devices_RegisterDate,
				   [D].[HardwareVersion] AS Data_Relays_Entrance_Devices_HardwareVersion,
				   [D].[FirmwareVersion] AS Data_Relays_Entrance_Devices_FirmwareVersion,
				   [D].[DeviceTypeId] AS Data_Relays_Entrance_Devices_DeviceTypeId,
				   [D].[DeviceLockPassword] AS Data_Relays_Entrance_Devices_DeviceLockPassword,
				   [D].[SSL] AS Data_Relays_Entrance_Devices_SSL,
				   [D].[TimeSync] AS Data_Relays_Entrance_Devices_TimeSync,
				   [D].[SerialNumber] AS Data_Relays_Entrance_Devices_SerialNumber,

				    (@PageNumber - 1) * @PageSize AS [from],
                     @PageNumber AS PageNumber,
                     @PageSize AS PageSize,
                     count(*) OVER () AS [Count],
                     @Message AS e_Message,
                     @Code AS e_Code,
                     @Validate AS e_Validate
			FROM [Rly].RelayHub AS RH
				 INNER JOIN
				 [Rly].RelayHubModel AS RM
				 ON [RH].[RelayHubModelId] = [RM].Id
				 LEFT JOIN
				 [Rly].Relay AS R
				 ON [R].[RelayHubId] = [RH].ID
				 LEFT JOIN
				 [Rly].RelayScheduling AS RS ON
				 [RS].RelayId = [R].Id
				 INNER JOIN
				 [Rly].[Scheduling] AS S
				 ON [S].[Id] = [RS].[SchedulingId]
				 LEFT JOIN
				 [Rly].[RelayDevice] AS RD
				 ON [R].[Id] = [RD].[RelayId]
				 INNER JOIN
				 [dbo].[Device] AS Dev
				 ON [Dev].[Id] = [RD].[DeviceId]
				  INNER JOIN [dbo].[DeviceModel] AS DevModel ON Dev.[DeviceModelId] = DevModel.[Id] 
				  INNER JOIN [dbo].[LookUp] AS DevBrand ON DevModel.[BrandId] = DevBrand.[Code] 
				  LEFT OUTER JOIN [dbo].[DeviceGroupMember] AS dgm ON dgm.DeviceId = dev.Id 
				  LEFT OUTER JOIN [dbo].[DeviceGroup] AS dg ON dg.Id = dgm.GroupId 
				  LEFT OUTER JOIN [dbo].[AccessGroupDevice] AS agd ON agd.DeviceGroupId = dg.Id 
				  LEFT OUTER JOIN [dbo].[AccessGroup] AS ag ON ag.Id = agd.AccessGroupId 
				  LEFT OUTER JOIN [dbo].[AdminAccessGroup] AS AAG ON ag.Id = AAG.AccessGroupId
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
				 INNER JOIN
				 [dbo].[Lookup] AS L
				 ON [L].[Code] = [RM].[BrandId]
			WHERE (isnull(@AdminUserId, 0) = 0
                      OR EXISTS (SELECT Id
                                 FROM   [dbo].[USER]
                                 WHERE  IsMasterAdmin = 1
                                        AND ID = @AdminUserId))
					 AND ([RH].Id = @Id
						  OR ISNULL(@Id,0) = 0)
                     AND ([RH].IpAddress = @IpAddress
						  OR ISNULL(@IpAddress,'') = '')
					 AND ([RH].[Port] = @Port
						  OR ISNULL(@Port,0) = 0)
					 AND ([R].[Name] = @Name
						  OR ISNULL(@Name,'') = '')
   				     AND ([RH].[Capacity] = @Capacity
						  OR ISNULL(@Capacity,0) = 0)
					 AND ([RH].[RelayHubModelId] = @RelayHubModelId
						  OR ISNULL(@RelayHubModelId,0) = 0)
			ORDER BY [R].Id
            OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
		END
		ELSE
			BEGIN
				SELECT [RH].[Id] AS  Data_Id,
				   [RH].IpAddress AS  Data_IpAddress,
				   [RH].[Port] AS  Data_Port,
				   [RH].[Name] AS  Data_Name,
				   [RH].[Active] AS  Data_Active,
				   [RH].[Capacity] AS  Data_Capacity,
				   [RM].Id AS  Data_RelayHubModel_Id,
				   [RM].[Name] AS  Data_RelayHubModel_Name,
				   [RM].[ManufactureCode] AS  Data_RelayHubModel_ManufactureCode,
				   [RM].[BrandId] AS  Data_RelayHubModel_Brand_Code,
				   [RM].[Description] AS  Data_RelayHubModel_Description,
				   [RM].[DefaultPortNumber] AS  Data_RelayHubModel_DefaultPortNumber,
				   [RH].[Description] AS  Data_Description,

				   [R].[Id] AS Data_Relays_Id,
				   [R].[Name] AS Data_Relays_Name,
				   [R].[NodeNumber] AS Data_Relays_NodeNumber,
				   [R].[RelayHubId] AS Data_Relays_RelayHub_Id,
				   [R].[EntranceId] AS Data_Relays_Entrance_Id,
				   [R].[Description] AS Data_Relays_Description,
				   [S].[Id] AS Data_Relays_Schedulings_Id,
				   [S].[Mode] AS Data_Relays_Schedulings_Mode,
				   [S].[StartTime] AS Data_Relays_Schedulings_StartTime,
				   [S].[EndTime] AS Data_Relays_Schedulings_EndTime,
				   [Dev].[Id] AS Data_Relays_Devices_DeviceId, 
					[Dev].[Code] AS Data_Relays_Devices_Code, 
					[dev].[DeviceModelId] AS Data_Relays_Devices_DeviceModelId, 
					[DevModel].[Id] AS Data_Relays_Devices_Model_Id, 
					[DevModel].[Name] AS Data_Relays_Devices_Model_Name, 
					[DevModel].[GetLogMethodType] AS Data_Relays_Devices_Model_GetLogMethodType, 
					[DevModel].[Description] AS Data_Relays_Devices_Model_Description, 
					[DevModel].[DefaultPortNumber] AS Data_Relays_Devices_Model_DefaultPortNumber, 
					[DevModel].[BrandId] AS Data_Relays_Devices_Model_Brand_Code, 
					[DevBrand].[Name] AS Data_Relays_Devices_Model_Brand_Name, 
					[DevBrand].[Description] AS Data_Relays_Devices_Model_Brand_Description, 
					[DevBrand].[Code] AS Data_Relays_Devices_Brand_Code, 
					[DevBrand].[Name] AS Data_Relays_Devices_Brand_Name, 
					[DevBrand].[Description] AS Data_Relays_Devices_Brand_Description, 
					[Dev].[Name] AS Data_Relays_Devices_Name, 
					[dev].[Active] AS Data_Relays_Devices_Active, 
					[dev].[IPAddress] AS Data_Relays_Devices_IPAddress, 
					[dev].[Port] AS Data_Relays_Devices_Port, 
					[dev].[MacAddress] AS Data_Relays_Devices_MacAddress, 
					[dev].[RegisterDate] AS Data_Relays_Devices_RegisterDate, 
					[dev].[HardwareVersion] AS Data_Relays_Devices_HardwareVersion, 
					[dev].[FirmwareVersion] AS Data_Relays_Devices_FirmwareVersion, 
					[dev].[DeviceLockPassword] AS Data_Relays_Devices_DeviceLockPassword, 
					[DevModel].[ManufactureCode] AS Data_Relays_Devices_ManufactureCode, 
					[dev].[SSL] AS Data_Relays_Devices_SSL, 
					[dev].[TimeSync] AS Data_Relays_Devices_TimeSync, 
					[dev].[SerialNumber] AS Data_Relays_Devices_SerialNumber, 
					[dev].[DeviceTypeId] AS Data_Relays_Devices_DeviceTypeId,
				   [E].[Name] AS Data_Relays_Entrance_Name,
				   [E].[Description] AS Data_Relays_Entrance_Description,
				   [SS].[Id] AS Data_Relays_Entrance_Schedulings_Id,
				   [SS].[Mode] AS Data_Relays_Entrance_Schedulings_Mode,
				   [SS].[StartTime] AS Data_Relays_Entrance_Schedulings_StartTime,
				   [SS].[EndTime] AS Data_Relays_Entrance_Schedulings_EndTime,
				   [D].[Id] AS Data_Relays_Entrance_Devices_DeviceId,
				   [D].[Code] AS Data_Relays_Entrance_Devices_Code,
				   [D].[Active] AS Data_Relays_Entrance_Devices_Active,
				   [D].[DeviceModelId] AS Data_Relays_Entrance_Devices_ModelId,
				   [D].[DeviceModelId] AS Data_Relays_Entrance_Devices_Model_Id,
				   [D].[Name] AS Data_Relays_Entrance_Devices_Name,
				   [D].[IpAddress] AS Data_Relays_Entrance_Devices_IpAddress,
				   [D].[Port] AS Data_Relays_Entrance_Devices_Port,
				   [D].[MacAddress] AS Data_Relays_Entrance_Devices_MacAddress,
				   [D].[RegisterDate] AS Data_Relays_Entrance_Devices_RegisterDate,
				   [D].[HardwareVersion] AS Data_Relays_Entrance_Devices_HardwareVersion,
				   [D].[FirmwareVersion] AS Data_Relays_Entrance_Devices_FirmwareVersion,
				   [D].[DeviceTypeId] AS Data_Relays_Entrance_Devices_DeviceTypeId,
				   [D].[DeviceLockPassword] AS Data_Relays_Entrance_Devices_DeviceLockPassword,
				   [D].[SSL] AS Data_Relays_Entrance_Devices_SSL,
				   [D].[TimeSync] AS Data_Relays_Entrance_Devices_TimeSync,
				   [D].[SerialNumber] AS Data_Relays_Entrance_Devices_SerialNumber,
				   1 AS [from],
				   1 AS PageNumber,
				   count(*) OVER () AS PageSize,
				   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @Code AS e_Code,
                   @Validate AS e_Validate
			FROM [Rly].RelayHub AS RH
				 INNER JOIN
				 [Rly].RelayHubModel AS RM
				 ON [RH].[RelayHubModelId] = [RM].Id
				 LEFT JOIN
				 [Rly].Relay AS R
				 ON [R].[RelayHubId] = [RH].ID
				 LEFT JOIN
				 [Rly].RelayScheduling AS RS ON
				 [RS].RelayId = [R].Id
				 INNER JOIN
				 [Rly].[Scheduling] AS S
				 ON [S].[Id] = [RS].[SchedulingId]
				 LEFT JOIN
				 [Rly].[RelayDevice] AS RD
				 ON [R].[Id] = [RD].[RelayId]
				 INNER JOIN
				 [dbo].[Device] AS Dev
				 ON [Dev].[Id] = [RD].[DeviceId]
				  INNER JOIN [dbo].[DeviceModel] AS DevModel ON Dev.[DeviceModelId] = DevModel.[Id] 
				  INNER JOIN [dbo].[LookUp] AS DevBrand ON DevModel.[BrandId] = DevBrand.[Code] 
				  LEFT OUTER JOIN [dbo].[DeviceGroupMember] AS dgm ON dgm.DeviceId = dev.Id 
				  LEFT OUTER JOIN [dbo].[DeviceGroup] AS dg ON dg.Id = dgm.GroupId 
				  LEFT OUTER JOIN [dbo].[AccessGroupDevice] AS agd ON agd.DeviceGroupId = dg.Id 
				  LEFT OUTER JOIN [dbo].[AccessGroup] AS ag ON ag.Id = agd.AccessGroupId 
				  LEFT OUTER JOIN [dbo].[AdminAccessGroup] AS AAG ON ag.Id = AAG.AccessGroupId
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
				 INNER JOIN
				 [dbo].[Lookup] AS L
				 ON [L].[Code] = [RM].[BrandId]
			WHERE (isnull(@AdminUserId, 0) = 0
                      OR EXISTS (SELECT Id
                                 FROM   [dbo].[USER]
                                 WHERE  IsMasterAdmin = 1
                                        AND ID = @AdminUserId))
					 AND ([RH].Id = @Id
						  OR ISNULL(@Id,0) = 0)
                     AND ([RH].IpAddress = @IpAddress
						  OR ISNULL(@IpAddress,'') = '')
					 AND ([RH].[Port] = @Port
						  OR ISNULL(@Port,0) = 0)
					 AND ([R].[Name] = @Name
						  OR ISNULL(@Name,'') = '')
   				     AND ([RH].[Capacity] = @Capacity
						  OR ISNULL(@Capacity,0) = 0)
					 AND ([RH].[RelayHubModelId] = @RelayHubModelId
						  OR ISNULL(@RelayHubModelId,0) = 0)
			END
	 
END