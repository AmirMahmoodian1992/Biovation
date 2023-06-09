CREATE PROCEDURE [dbo].[SelectRelayHub]
@AdminUserId INT=NULL,@Id INT= NULL, @IpAddress NVARCHAR(50) = NULL, @Port INT = NULL, @Name NVARCHAR(MAX) = NULL, @Capacity INT = NULL, @RelayHubModelId INT = NULL, @Description NVARCHAR(MAX) = NULL, @PageNumber INT=0, @PageSize INT=0, @Where NVARCHAR (MAX)='', @Order NVARCHAR (MAX)='', @FilterText NVARCHAR (MAX) = NULL
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (5) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
	DECLARE @CountSqlQuery AS NVARCHAR (MAX);
    DECLARE @DataSqlQuery AS NVARCHAR (MAX);
    DECLARE @ParmDefinition AS NVARCHAR (500);
	SET @where = REPLACE(@where, 'RelayId', '[R].[Id]');
	SET @where = REPLACE(@where, 'EntranceId', '[E].[Id]');
	SET @where = REPLACE(@where, 'RelayName', '[R].[Name]');
	SET @where = REPLACE(@where, 'EntranceName', '[E].Name');
    SET @where = REPLACE(@where, 'True', '1');
    SET @where = REPLACE(@where, 'False', '0');
    SET @order = REPLACE(@order, 'Id', 'Data_Id');
    SET @Order = REPLACE(@order, 'Name', 'Data_Name');
    SET @Order = REPLACE(@order, 'EntranceId', 'Data_ENtrances_Id');
    SET @Order = REPLACE(@order, 'RelayId', 'Data_Relays_Id');
	SET @Order = REPLACE(@order, 'Active', 'Data_Active');
    IF @HasPaging = 1
        BEGIN
		IF ISNULL(@Order,N'') = N''
		BEGIN
			SET	@Order = N'ORDER BY Data_Id DESC';
		END
		SET @DataSqlQuery = N'
			SELECT 
				    Data_Id,
				    Data_IpAddress,
				    Data_Port,
				    Data_Name,
				    Data_Active,
				    Data_Capacity,
				    Data_RelayHubModel_Id,
				    Data_RelayHubModel_Name,
				    Data_RelayHubModel_ManufactureCode,
				    Data_RelayHubModel_Brand_Code,
				    Data_RelayHubModel_Description,
					Data_RelayHubModel_DefaultCapacity,
				    Data_RelayHubModel_DefaultPortNumber,
				    Data_Description,
				    Data_Relays_Id,
				    Data_Relays_Name,
				    Data_Relays_NodeNumber,
				    Data_Relays_RelayHub_Id,
				    Data_Relays_Description,
				    Data_Relays_RelayType_Code,
				    Data_Relays_RelayType_Name,
				    Data_Relays_RelayType_Description,
				    Data_Relays_Schedulings_Id,
				    Data_Relays_Schedulings_Mode,
				    Data_Relays_Schedulings_StartTime,
				    Data_Relays_Schedulings_EndTime,
				    Data_Relays_Devices_DeviceId, 
					Data_Relays_Devices_Code, 
					Data_Relays_Devices_DeviceModelId, 
					Data_Relays_Devices_Model_Id, 
					Data_Relays_Devices_Model_Name, 
					Data_Relays_Devices_Model_GetLogMethodType, 
					Data_Relays_Devices_Model_Description, 
					Data_Relays_Devices_Model_DefaultPortNumber, 
					Data_Relays_Devices_Model_Brand_Code, 
					Data_Relays_Devices_Model_Brand_Name, 
					Data_Relays_Devices_Model_Brand_Description, 
					Data_Relays_Devices_Brand_Code, 
					Data_Relays_Devices_Brand_Name, 
					Data_Relays_Devices_Brand_Description, 
					Data_Relays_Devices_Name, 
					Data_Relays_Devices_Active, 
					Data_Relays_Devices_IPAddress, 
					Data_Relays_Devices_Port, 
					Data_Relays_Devices_MacAddress, 
					Data_Relays_Devices_RegisterDate, 
					Data_Relays_Devices_HardwareVersion, 
					Data_Relays_Devices_FirmwareVersion, 
					Data_Relays_Devices_DeviceLockPassword, 
					Data_Relays_Devices_ManufactureCode, 
					Data_Relays_Devices_SSL, 
					Data_Relays_Devices_TimeSync, 
					Data_Relays_Devices_SerialNumber, 
					Data_Relays_Devices_DeviceTypeId,
					Data_Relays_Cameras_Id,
				    Data_Relays_Cameras_Code,
				    Data_Relays_Cameras_Active,
				    Data_Relays_Cameras_Model_Id,
				    Data_Relays_Cameras_Name,
				    Data_Relays_Cameras_Ip,
				    Data_Relays_Cameras_Port,
				    Data_Relays_Cameras_MacAddress,
				    Data_Relays_Cameras_RegisterDate,
				    Data_Relays_Cameras_HardwareVersion,
				    Data_Relays_Cameras_Brand_Code,
				    Data_Relays_Cameras_UserName,
				    Data_Relays_Cameras_Password,
				    Data_Relays_Cameras_ConnectionUrl,
				    Data_Relays_Cameras_LiveStreamUrl,
				    Data_Relays_Cameras_SerialNumber,
				    Data_Relays_Entrance_Id,
				    Data_Relays_Entrance_Name,
				    Data_Relays_Entrance_Description,
				    Data_Relays_Entrance_Schedulings_Id,
				    Data_Relays_Entrance_Schedulings_Mode,
				    Data_Relays_Entrance_Schedulings_StartTime,
				    Data_Relays_Entrance_Schedulings_EndTime,
				    Data_Relays_Entrance_Devices_DeviceId,
				    Data_Relays_Entrance_Devices_Code,
				    Data_Relays_Entrance_Devices_Active,
				    Data_Relays_Entrance_Devices_ModelId,
				    Data_Relays_Entrance_Devices_Model_Id,
				    Data_Relays_Entrance_Devices_Name,
				    Data_Relays_Entrance_Devices_IpAddress,
				    Data_Relays_Entrance_Devices_Port,
				    Data_Relays_Entrance_Devices_MacAddress,
				    Data_Relays_Entrance_Devices_RegisterDate,
				    Data_Relays_Entrance_Devices_HardwareVersion,
				    Data_Relays_Entrance_Devices_FirmwareVersion,
				    Data_Relays_Entrance_Devices_DeviceTypeId,
				    Data_Relays_Entrance_Devices_DeviceLockPassword,
				    Data_Relays_Entrance_Devices_SSL,
				    Data_Relays_Entrance_Devices_TimeSync,
				    Data_Relays_Entrance_Devices_SerialNumber,

				    (@PageNumber - 1) * @PageSize AS [from],
                     @PageNumber AS PageNumber,
                     @PageSize AS PageSize,
                     count(*) OVER () AS [Count],
                     @Message AS e_Message,
                     @Code AS e_Code,
                     @Validate AS e_Validate
			FROM(
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
				   [RM].[DefaultCapacity] AS  Data_RelayHubModel_DefaultCapacity,
				   [RM].[DefaultPortNumber] AS  Data_RelayHubModel_DefaultPortNumber,
				   [RH].[Description] AS  Data_Description,
				   [R].[Id] AS Data_Relays_Id,
				   [R].[Name] AS Data_Relays_Name,
				   [R].[NodeNumber] AS Data_Relays_NodeNumber,
				   [R].[RelayHubId] AS Data_Relays_RelayHub_Id,
				   [R].[Description] AS Data_Relays_Description,
				   [RT].[Code] AS Data_Relays_RelayType_Code,
				   [RT].[Name] AS Data_Relays_RelayType_Name,
				   [RT].[Description] AS Data_Relays_RelayType_Description,
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
					[C].[Id] AS  Data_Relays_Cameras_Id,
				   [C].[Code] AS  Data_Relays_Cameras_Code,
				   [C].[Active] AS  Data_Relays_Cameras_Active,
				   [C].[ModelId] AS  Data_Relays_Cameras_Model_Id,
				   [C].[Name] AS  Data_Relays_Cameras_Name,
				   [C].[Ip] AS  Data_Relays_Cameras_Ip,
				   [C].[Port] AS  Data_Relays_Cameras_Port,
				   [C].[MacAddress] AS  Data_Relays_Cameras_MacAddress,
				   [C].[RegisterDate] AS  Data_Relays_Cameras_RegisterDate,
				   [C].[HardwareVersion] AS  Data_Relays_Cameras_HardwareVersion,
				   [C].[BrandCode] AS  Data_Relays_Cameras_Brand_Code,
				   [C].[UserName] AS  Data_Relays_Cameras_UserName,
				   [C].[Password] AS  Data_Relays_Cameras_Password,
				   [C].[ConnectionUrl] AS  Data_Relays_Cameras_ConnectionUrl,
				   [C].[LiveStreamUrl] AS  Data_Relays_Cameras_LiveStreamUrl,
				   [C].[SerialNumber] AS Data_Relays_Cameras_SerialNumber,
				   [E].[Id] AS Data_Relays_Entrance_Id,
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
				   [D].[SerialNumber] AS Data_Relays_Entrance_Devices_SerialNumber

			FROM [Rly].RelayHub AS RH
				 LEFT JOIN
				 [Rly].RelayHubModel AS RM
				 ON [RH].[RelayHubModelId] = [RM].Id
				 LEFT JOIN
				 [Rly].Relay AS R
				 ON [R].[RelayHubId] = [RH].ID
				 LEFT JOIN
				 [dbo].[Lookup] AS RT
				 ON [R].RelayTypeId = [RT].Code
				 LEFT JOIN
				 [Rly].RelayScheduling AS RS ON
				 [RS].RelayId = [R].Id
				 LEFT JOIN
				 [Rly].[Scheduling] AS S
				 ON [S].[Id] = [RS].[SchedulingId]
				 LEFT JOIN
				 [Rly].[RelayDevice] AS RD
				 ON [R].[Id] = [RD].[RelayId]
				 LEFT JOIN
				 [dbo].[Device] AS Dev
				 ON [Dev].[Id] = [RD].[DeviceId]
				  LEFT JOIN [dbo].[DeviceModel] AS DevModel ON Dev.[DeviceModelId] = DevModel.[Id] 
				  LEFT JOIN [dbo].[LookUp] AS DevBrand ON DevModel.[BrandId] = DevBrand.[Code] 
				  LEFT OUTER JOIN [dbo].[DeviceGroupMember] AS dgm ON dgm.DeviceId = dev.Id 
				  LEFT OUTER JOIN [dbo].[DeviceGroup] AS dg ON dg.Id = dgm.GroupId 
				  LEFT OUTER JOIN [dbo].[AccessGroupDevice] AS agd ON agd.DeviceGroupId = dg.Id 
				  LEFT OUTER JOIN [dbo].[AccessGroup] AS ag ON ag.Id = agd.AccessGroupId 
				  LEFT OUTER JOIN [dbo].[AdminAccessGroup] AS AAG ON ag.Id = AAG.AccessGroupId
				 LEFT JOIN
				 [dbo].[Lookup] AS L
				 ON [L].[Code] = [RM].[BrandId]
				 LEFT OUTER JOIN
				 [Rly].[RelayCamera] AS RC
				 ON RC.RelayId = R.[Id]
				 LEFT OUTER JOIN
				 [Rly].Camera AS C
				 ON C.Id = RC.CameraId
				 LEFT JOIN 
				 [Rly].[EntranceCamera] AS EC
				 ON [EC].CameraId = [C].[Id]
				 LEFT JOIN
				 [Rly].[EntranceDevice] AS EDEV
				 ON [EDEV].[DeviceId] = [Dev].Id
				 LEFT JOIN
				 [Rly].[Entrance] AS E
				 ON [E].[Id] = EC.EntranceId OR [E].[Id]  = EDEV.DeviceId
				 LEFT JOIN
				 [Rly].[EntranceScheduling] AS ES
				 ON [ES].[EntranceId] = [E].[Id]
				 LEFT JOIN
				 [Rly].[Scheduling] AS SS
				 ON [SS].[Id] = [ES].SchedulingId
				 LEFT JOIN
				 [Rly].[EntranceDevice] AS ED
				 ON [ED].[EntranceId] = [E].Id
				 LEFT JOIN
				 [dbo].[Device] AS D
				 ON [D].[Id] = [ED].[DeviceId]
			WHERE (isnull(@AdminUserId, 0) = 0
                      OR EXISTS (SELECT Id
                                 FROM   [dbo].[USER]
                                 WHERE  IsMasterAdmin = 1
                                        AND ID = @AdminUserId))
					 AND ([RH].Id = @Id
						  OR ISNULL(@Id,0) = 0)
                     AND ([RH].IpAddress = @IpAddress
						  OR ISNULL(@IpAddress,'''') = '''')
					 AND ([RH].[Port] = @Port
						  OR ISNULL(@Port,0) = 0)
					 AND ([R].[Name] = @Name
						  OR ISNULL(@Name,'''') = '''')
   				     AND ([RH].[Capacity] = @Capacity
						  OR ISNULL(@Capacity,0) = 0)
					 AND ([RH].[RelayHubModelId] = @RelayHubModelId
						  OR ISNULL(@RelayHubModelId,0) = 0)
					 AND((@FilterText IS NULL)
						   OR ([Rh].[Name] LIKE ''%'' + @FilterText + ''%'')
						   OR ([R].[Name] LIKE ''%'' + @FilterText + ''%'')
						   OR ([RM].[Name] LIKE ''%'' + @FilterText + ''%''))' + ISNULL(@Where,'') +'
					UNION
				 
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
				   [RM].[DefaultCapacity] AS  Data_RelayHubModel_DefaultCapacity,
				   [RM].[DefaultPortNumber] AS  Data_RelayHubModel_DefaultPortNumber,
				   [RH].[Description] AS  Data_Description,
				   [R].[Id] AS Data_Relays_Id,
				   [R].[Name] AS Data_Relays_Name,
				   [R].[NodeNumber] AS Data_Relays_NodeNumber,
				   [R].[RelayHubId] AS Data_Relays_RelayHub_Id,
				   [R].[Description] AS Data_Relays_Description,
				   [RT].[Code] AS Data_Relays_RelayType_Code,
				   [RT].[Name] AS Data_Relays_RelayType_Name,
				   [RT].[Description] AS Data_Relays_RelayType_Description,
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
					[C].[Id] AS  Data_Relays_Cameras_Id,
					[C].[Code] AS  Data_Relays_Cameras_Code,
				   [C].[Active] AS  Data_Relays_Cameras_Active,
				   [C].[ModelId] AS  Data_Relays_Cameras_Model_Id,
				   [C].[Name] AS  Data_Relays_Cameras_Name,
				   [C].[Ip] AS  Data_Relays_Cameras_Ip,
				   [C].[Port] AS  Data_Relays_Cameras_Port,
				   [C].[MacAddress] AS  Data_Relays_Cameras_MacAddress,
				   [C].[RegisterDate] AS  Data_Relays_Cameras_RegisterDate,
				   [C].[HardwareVersion] AS  Data_Relays_Cameras_HardwareVersion,
				   [C].[BrandCode] AS  Data_Relays_Cameras_Brand_Code,
				   [C].[UserName] AS  Data_Relays_Cameras_UserName,
				   [C].[Password] AS  Data_Relays_Cameras_Password,
				   [C].[ConnectionUrl] AS  Data_Relays_Cameras_ConnectionUrl,
				   [C].[LiveStreamUrl] AS  Data_Relays_Cameras_LiveStreamUrl,
				   [C].[SerialNumber] AS Data_Relays_Cameras_SerialNumber,
				   [E].[Id] AS Data_Relays_Entrance_Id,
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
				   [D].[SerialNumber] AS Data_Relays_Entrance_Devices_SerialNumber
				   
			FROM [Rly].RelayHub AS RH
				 LEFT JOIN
				 [Rly].RelayHubModel AS RM
				 ON [RH].[RelayHubModelId] = [RM].Id
				 LEFT JOIN
				 [Rly].Relay AS R
				 ON [R].[RelayHubId] = [RH].ID
				 LEFT JOIN
				 [dbo].[Lookup] AS RT
				 ON [R].RelayTypeId = [RT].Code
				 LEFT JOIN
				 [Rly].RelayScheduling AS RS ON
				 [RS].RelayId = [R].Id
				 LEFT JOIN
				 [Rly].[Scheduling] AS S
				 ON [S].[Id] = [RS].[SchedulingId]
				 LEFT JOIN
				 [Rly].[RelayDevice] AS RD
				 ON [R].[Id] = [RD].[RelayId]
				 LEFT JOIN
				 [dbo].[Device] AS Dev
				 ON [Dev].[Id] = [RD].[DeviceId]
				  LEFT JOIN [dbo].[DeviceModel] AS DevModel ON Dev.[DeviceModelId] = DevModel.[Id] 
				  LEFT JOIN [dbo].[LookUp] AS DevBrand ON DevModel.[BrandId] = DevBrand.[Code] 
				  LEFT OUTER JOIN [dbo].[DeviceGroupMember] AS dgm ON dgm.DeviceId = dev.Id 
				  LEFT OUTER JOIN [dbo].[DeviceGroup] AS dg ON dg.Id = dgm.GroupId 
				  LEFT OUTER JOIN [dbo].[AccessGroupDevice] AS agd ON agd.DeviceGroupId = dg.Id 
				  LEFT OUTER JOIN [dbo].[AccessGroup] AS ag ON ag.Id = agd.AccessGroupId 
				  LEFT OUTER JOIN [dbo].[AdminAccessGroup] AS AAG ON ag.Id = AAG.AccessGroupId
				 LEFT JOIN
				 [dbo].[Lookup] AS L
				 ON [L].[Code] = [RM].[BrandId]
				 LEFT OUTER JOIN
				 [Rly].[RelayCamera] AS RC
				 ON RC.RelayId = R.[Id]
				 LEFT OUTER JOIN
				 [Rly].Camera AS C
				 ON C.Id = RC.CameraId
				 LEFT JOIN 
				 [Rly].[EntranceCamera] AS EC
				 ON [EC].CameraId = [C].[Id]
				 LEFT JOIN
				 [Rly].[EntranceDevice] AS EDEV
				 ON [EDEV].[DeviceId] = [Dev].Id
				 LEFT JOIN
				 [Rly].[Entrance] AS E
				 ON [E].[Id] = EDEV.EntranceId
				 LEFT JOIN
				 [Rly].[EntranceScheduling] AS ES
				 ON [ES].[EntranceId] = [E].[Id]
				 LEFT JOIN
				 [Rly].[Scheduling] AS SS
				 ON [SS].[Id] = [ES].SchedulingId
				 LEFT JOIN
				 [Rly].[EntranceDevice] AS ED
				 ON [ED].[EntranceId] = [E].Id
				 LEFT JOIN
				 [dbo].[Device] AS D
				 ON [D].[Id] = [ED].[DeviceId]
				 WHERE (isnull(@AdminUserId, 0) = 0
                      OR EXISTS (SELECT Id
                                 FROM   [dbo].[USER]
                                 WHERE  IsMasterAdmin = 1
                                        AND ID = @AdminUserId))
					 AND ([RH].Id = @Id
						  OR ISNULL(@Id,0) = 0)
                     AND ([RH].IpAddress = @IpAddress
						  OR ISNULL(@IpAddress,'''') = '''')
					 AND ([RH].[Port] = @Port
						  OR ISNULL(@Port,0) = 0)
					 AND ([R].[Name] = @Name
						  OR ISNULL(@Name,'''') = '''')
   				     AND ([RH].[Capacity] = @Capacity
						  OR ISNULL(@Capacity,0) = 0)
					 AND ([RH].[RelayHubModelId] = @RelayHubModelId
						  OR ISNULL(@RelayHubModelId,0) = 0)
						   AND((@FilterText IS NULL)
						   OR ([Rh].[Name] LIKE ''%'' + @FilterText + ''%'')
						   OR ([R].[Name] LIKE ''%'' + @FilterText + ''%'')
						   OR ([RM].[Name] LIKE ''%'' + @FilterText + ''%''))' +  ISNULL(@Where,'') + ') AS AllLog
						  ' + @Order +'
            OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;'
			SET @ParmDefinition = N'@AdminUserId int, @Id int, @IpAddress nvarchar(50),@Port INT, @Name nvarchar(MAX), @Capacity INT, @RelayHubModelId int ,@Description NVARCHAR(MAX)
								,@FilterText NVARCHAR(MAX), @PageNumber int, @PageSize int, @Message AS NVARCHAR (200), @Validate int, @Code nvarchar(5)';
                    EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @AdminUserId = @AdminUserId, @Id = @Id, @IpAddress = @IpAddress, @Port = @Port, @Name = @Name, @Capacity = @Capacity, @RelayHubModelId = @RelayHubModelId, @Description = @Description, @FilterText = @FilterText, @PageNumber = @PageNumber, @PageSize = @PageSize, @Message = @Message, @Validate = @Validate, @Code = @Code;

		END
		ELSE
			BEGIN
				IF ISNULL(@Order,N'') = N''
				BEGIN
					SET	@Order = N'ORDER BY Data_Id DESC';
				END
				SET @DataSqlQuery = N'
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
				   [RM].[DefaultCapacity] AS  Data_RelayHubModel_DefaultCapacity,
				   [RM].[DefaultPortNumber] AS  Data_RelayHubModel_DefaultPortNumber,
				   [RH].[Description] AS  Data_Description,
				   [R].[Id] AS Data_Relays_Id,
				   [R].[Name] AS Data_Relays_Name,
				   [R].[NodeNumber] AS Data_Relays_NodeNumber,
				   [R].[RelayHubId] AS Data_Relays_RelayHub_Id,
				   [R].[Description] AS Data_Relays_Description,
				   [RT].[Code] AS Data_Relays_RelayType_Code,
				   [RT].[Name] AS Data_Relays_RelayType_Name,
				   [RT].[Description] AS Data_Relays_RelayType_Description,
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
					[C].[Id] AS  Data_Relays_Cameras_Id,
					[C].[Code] AS  Data_Relays_Cameras_Code,
				   [C].[Active] AS  Data_Relays_Cameras_Active,
				   [C].[ModelId] AS  Data_Relays_Cameras_Model_Id,
				   [C].[Name] AS  Data_Relays_Cameras_Name,
				   [C].[Ip] AS  Data_Relays_Cameras_Ip,
				   [C].[Port] AS  Data_Relays_Cameras_Port,
				   [C].[MacAddress] AS  Data_Relays_Cameras_MacAddress,
				   [C].[RegisterDate] AS  Data_Relays_Cameras_RegisterDate,
				   [C].[HardwareVersion] AS  Data_Relays_Cameras_HardwareVersion,
				   [C].[BrandCode] AS  Data_Relays_Cameras_Brand_Code,
				   [C].[UserName] AS  Data_Relays_Cameras_UserName,
				   [C].[Password] AS  Data_Relays_Cameras_Password,
				   [C].[ConnectionUrl] AS  Data_Relays_Cameras_ConnectionUrl,
				   [C].[LiveStreamUrl] AS  Data_Relays_Cameras_LiveStreamUrl,
				   [C].[SerialNumber] AS Data_Relays_Cameras_SerialNumber,
				   [E].[Id] AS Data_Relays_Entrance_Id,
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
				 LEFT JOIN
				 [Rly].RelayHubModel AS RM
				 ON [RH].[RelayHubModelId] = [RM].Id
				 LEFT JOIN
				 [Rly].Relay AS R
				 ON [R].[RelayHubId] = [RH].ID
				 LEFT JOIN
				 [dbo].[Lookup] AS RT
				 ON [R].RelayTypeId = [RT].Code
				 LEFT JOIN
				 [Rly].RelayScheduling AS RS ON
				 [RS].RelayId = [R].Id
				 LEFT JOIN
				 [Rly].[Scheduling] AS S
				 ON [S].[Id] = [RS].[SchedulingId]
				 LEFT JOIN
				 [Rly].[RelayDevice] AS RD
				 ON [R].[Id] = [RD].[RelayId]
				 LEFT JOIN
				 [dbo].[Device] AS Dev
				 ON [Dev].[Id] = [RD].[DeviceId]
				  LEFT JOIN [dbo].[DeviceModel] AS DevModel ON Dev.[DeviceModelId] = DevModel.[Id] 
				  LEFT JOIN [dbo].[LookUp] AS DevBrand ON DevModel.[BrandId] = DevBrand.[Code] 
				  LEFT OUTER JOIN [dbo].[DeviceGroupMember] AS dgm ON dgm.DeviceId = dev.Id 
				  LEFT OUTER JOIN [dbo].[DeviceGroup] AS dg ON dg.Id = dgm.GroupId 
				  LEFT OUTER JOIN [dbo].[AccessGroupDevice] AS agd ON agd.DeviceGroupId = dg.Id 
				  LEFT OUTER JOIN [dbo].[AccessGroup] AS ag ON ag.Id = agd.AccessGroupId 
				  LEFT OUTER JOIN [dbo].[AdminAccessGroup] AS AAG ON ag.Id = AAG.AccessGroupId
				 LEFT JOIN
				 [dbo].[Lookup] AS L
				 ON [L].[Code] = [RM].[BrandId]
				 LEFT OUTER JOIN
				 [Rly].[RelayCamera] AS RC
				 ON RC.RelayId = R.[Id]
				 LEFT OUTER JOIN
				 [Rly].Camera AS C
				 ON C.Id = RC.CameraId
				 LEFT JOIN 
				 [Rly].[EntranceCamera] AS EC
				 ON [EC].CameraId = [C].[Id]
				 LEFT JOIN
				 [Rly].[EntranceDevice] AS EDEV
				 ON [EDEV].[DeviceId] = [Dev].Id
				 LEFT JOIN
				 [Rly].[Entrance] AS E
				 ON [E].[Id] = EC.EntranceId
				 LEFT JOIN
				 [Rly].[EntranceScheduling] AS ES
				 ON [ES].[EntranceId] = [E].[Id]
				 LEFT JOIN
				 [Rly].[Scheduling] AS SS
				 ON [SS].[Id] = [ES].SchedulingId
				 LEFT JOIN
				 [Rly].[EntranceDevice] AS ED
				 ON [ED].[EntranceId] = [E].Id
				 LEFT JOIN
				 [dbo].[Device] AS D
				 ON [D].[Id] = [ED].[DeviceId]

			WHERE (isnull(@AdminUserId, 0) = 0
                      OR EXISTS (SELECT Id
                                 FROM   [dbo].[USER]
                                 WHERE  IsMasterAdmin = 1
                                        AND ID = @AdminUserId))
					 AND ([RH].Id = @Id
						  OR ISNULL(@Id,0) = 0)
                     AND ([RH].IpAddress = @IpAddress
						  OR ISNULL(@IpAddress,'''') = '''')
					 AND ([RH].[Port] = @Port
						  OR ISNULL(@Port,0) = 0)
					 AND ([R].[Name] = @Name
						  OR ISNULL(@Name,'''') = '''')
   				     AND ([RH].[Capacity] = @Capacity
						  OR ISNULL(@Capacity,0) = 0)
					 AND ([RH].[RelayHubModelId] = @RelayHubModelId
						  OR ISNULL(@RelayHubModelId,0) = 0)
					 AND((@FilterText IS NULL)
						   OR ([Rh].[Name] LIKE ''%'' + @FilterText + ''%'')
						   OR ([R].[Name] LIKE ''%'' + @FilterText + ''%'')
						   OR ([RM].[Name] LIKE ''%'' + @FilterText + ''%''))' +  ISNULL(@Where,'') + '

				 UNION
				 
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
				   [RM].[DefaultCapacity] AS  Data_RelayHubModel_DefaultCapacity,
				   [RM].[DefaultPortNumber] AS  Data_RelayHubModel_DefaultPortNumber,
				   [RH].[Description] AS  Data_Description,
				   [R].[Id] AS Data_Relays_Id,
				   [R].[Name] AS Data_Relays_Name,
				   [R].[NodeNumber] AS Data_Relays_NodeNumber,
				   [R].[RelayHubId] AS Data_Relays_RelayHub_Id,
				   [R].[Description] AS Data_Relays_Description,
				   [RT].[Code] AS Data_Relays_RelayType_Code,
				   [RT].[Name] AS Data_Relays_RelayType_Name,
				   [RT].[Description] AS Data_Relays_RelayType_Description,
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
					[C].[Id] AS  Data_Relays_Cameras_Id,
					[C].[Code] AS  Data_Relays_Cameras_Code,
				   [C].[Active] AS  Data_Relays_Cameras_Active,
				   [C].[ModelId] AS  Data_Relays_Cameras_Model_Id,
				   [C].[Name] AS  Data_Relays_Cameras_Name,
				   [C].[Ip] AS  Data_Relays_Cameras_Ip,
				   [C].[Port] AS  Data_Relays_Cameras_Port,
				   [C].[MacAddress] AS  Data_Relays_Cameras_MacAddress,
				   [C].[RegisterDate] AS  Data_Relays_Cameras_RegisterDate,
				   [C].[HardwareVersion] AS  Data_Relays_Cameras_HardwareVersion,
				   [C].[BrandCode] AS  Data_Relays_Cameras_Brand_Code,
				   [C].[UserName] AS  Data_Relays_Cameras_UserName,
				   [C].[Password] AS  Data_Relays_Cameras_Password,
				   [C].[ConnectionUrl] AS  Data_Relays_Cameras_ConnectionUrl,
				   [C].[LiveStreamUrl] AS  Data_Relays_Cameras_LiveStreamUrl,
				   [C].[SerialNumber] AS Data_Relays_Cameras_SerialNumber,
				   [E].[Id] AS Data_Relays_Entrance_Id,
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
				 LEFT JOIN
				 [Rly].RelayHubModel AS RM
				 ON [RH].[RelayHubModelId] = [RM].Id
				 LEFT JOIN
				 [Rly].Relay AS R
				 ON [R].[RelayHubId] = [RH].ID
				 LEFT JOIN
				 [dbo].[Lookup] AS RT
				 ON [R].RelayTypeId = [RT].Code
				 LEFT JOIN
				 [Rly].RelayScheduling AS RS ON
				 [RS].RelayId = [R].Id
				 LEFT JOIN
				 [Rly].[Scheduling] AS S
				 ON [S].[Id] = [RS].[SchedulingId]
				 LEFT JOIN
				 [Rly].[RelayDevice] AS RD
				 ON [R].[Id] = [RD].[RelayId]
				 LEFT JOIN
				 [dbo].[Device] AS Dev
				 ON [Dev].[Id] = [RD].[DeviceId]
				  LEFT JOIN [dbo].[DeviceModel] AS DevModel ON Dev.[DeviceModelId] = DevModel.[Id] 
				  LEFT JOIN [dbo].[LookUp] AS DevBrand ON DevModel.[BrandId] = DevBrand.[Code] 
				  LEFT OUTER JOIN [dbo].[DeviceGroupMember] AS dgm ON dgm.DeviceId = dev.Id 
				  LEFT OUTER JOIN [dbo].[DeviceGroup] AS dg ON dg.Id = dgm.GroupId 
				  LEFT OUTER JOIN [dbo].[AccessGroupDevice] AS agd ON agd.DeviceGroupId = dg.Id 
				  LEFT OUTER JOIN [dbo].[AccessGroup] AS ag ON ag.Id = agd.AccessGroupId 
				  LEFT OUTER JOIN [dbo].[AdminAccessGroup] AS AAG ON ag.Id = AAG.AccessGroupId
				 LEFT JOIN
				 [dbo].[Lookup] AS L
				 ON [L].[Code] = [RM].[BrandId]
				 LEFT OUTER JOIN
				 [Rly].[RelayCamera] AS RC
				 ON RC.RelayId = R.[Id]
				 LEFT OUTER JOIN
				 [Rly].Camera AS C
				 ON C.Id = RC.CameraId
				 LEFT JOIN 
				 [Rly].[EntranceCamera] AS EC
				 ON [EC].CameraId = [C].[Id]
				 LEFT JOIN
				 [Rly].[EntranceDevice] AS EDEV
				 ON [EDEV].[DeviceId] = [Dev].Id
				 LEFT JOIN
				 [Rly].[Entrance] AS E
				 ON [E].[Id] = EDEV.EntranceId
				 LEFT JOIN
				 [Rly].[EntranceScheduling] AS ES
				 ON [ES].[EntranceId] = [E].[Id]
				 LEFT JOIN
				 [Rly].[Scheduling] AS SS
				 ON [SS].[Id] = [ES].SchedulingId
				 LEFT JOIN
				 [Rly].[EntranceDevice] AS ED
				 ON [ED].[EntranceId] = [E].Id
				 LEFT JOIN
				 [dbo].[Device] AS D
				 ON [D].[Id] = [ED].[DeviceId]
				 WHERE (isnull(@AdminUserId, 0) = 0
                      OR EXISTS (SELECT Id
                                 FROM   [dbo].[USER]
                                 WHERE  IsMasterAdmin = 1
                                        AND ID = @AdminUserId))
					 AND ([RH].Id = @Id
						  OR ISNULL(@Id,0) = 0)
                     AND ([RH].IpAddress = @IpAddress
						  OR ISNULL(@IpAddress,'''') = '''')
					 AND ([RH].[Port] = @Port
						  OR ISNULL(@Port,0) = 0)
					 AND ([R].[Name] = @Name
						  OR ISNULL(@Name,'''') = '''')
   				     AND ([RH].[Capacity] = @Capacity
						  OR ISNULL(@Capacity,0) = 0)
					 AND ([RH].[RelayHubModelId] = @RelayHubModelId
						  OR ISNULL(@RelayHubModelId,0) = 0)
					 AND((@FilterText IS NULL)
						   OR ([Rh].[Name] LIKE ''%'' + @FilterText + ''%'')
						   OR ([R].[Name] LIKE ''%'' + @FilterText + ''%'')
						   OR ([RM].[Name] LIKE ''%'' + @FilterText + ''%''))' +  ISNULL(@Where,'')
					SET @ParmDefinition = N'@AdminUserId int, @Id int, @IpAddress nvarchar(50),@Port INT, @Name nvarchar(MAX), @Capacity INT, @RelayHubModelId int ,@Description NVARCHAR(MAX)
								, @FilterText NVARCHAR(MAX), @Message AS NVARCHAR (200), @Validate int, @Code nvarchar(5)';
                    EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @AdminUserId = @AdminUserId, @Id = @Id, @IpAddress = @IpAddress, @Port = @Port, @Name = @Name, @Capacity = @Capacity, @RelayHubModelId = @RelayHubModelId, @Description = @Description, @FilterText = @FilterText, @Message = @Message, @Validate = @Validate, @Code = @Code;

			END
	 
END