CREATE PROCEDURE [dbo].[SelectRelay]
@AdminUserId INT=NULL, @Id INT=NULL, @Name NVARCHAR (MAX)=NULL, @NodeNumber INT=NULL, @RelayHubId INT=NULL, @RelayTypeId INT=NULL, @SchedulingId INT=NULL, @DeviceId INT=NULL, @CameraId INT=NULL, @PageNumber INT=0, @PageSize INT=0
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (5) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
    IF @HasPaging = 1
        BEGIN
            SELECT   [R].[Id] AS Data_Id,
                     [R].[Name] AS Data_Name,
                     [R].[NodeNumber] AS Data_NodeNumber,
                     [R].[RelayHubId] AS Data_RelayHub_Id,
                     [R].[RelayTypeId] AS Data_RelayType_Code,
                     [R].[Description] AS Data_Description,
                     [S].[Id] AS Data_Schedulings_Id,
                     [S].[StartTime] AS Data_Schedulings_StartTime,
                     [S].[EndTime] AS Data_Schedulings_EndTime,
                     [S].[Mode] AS Data_Schedulings_Mode_Code,
                     [SM].[Description] AS Data_Schedulings_Mode_Description,
                     [SM].[Name] AS Data_Schedulings_Mode_Name,
                     [SM].[OrderIndex] AS Data_Schedulings_Mode_OrderIndex,
                     [SM].[LookupCategoryId] AS Data_Schedulings_Mode_Category_Id,
                     [Dev].[Id] AS Data_Devices_DeviceId,
                     [Dev].[Code] AS Data_Devices_Code,
                     [dev].[DeviceModelId] AS Data_Devices_DeviceModelId,
                     [DevModel].[Id] AS Data_Devices_Model_Id,
                     [DevModel].[Name] AS Data_Devices_Model_Name,
                     [DevModel].[GetLogMethodType] AS Data_Devices_Model_GetLogMethodType,
                     [DevModel].[Description] AS Data_Devices_Model_Description,
                     [DevModel].[DefaultPortNumber] AS Data_Devices_Model_DefaultPortNumber,
                     [DevModel].[BrandId] AS Data_Devices_Model_Brand_Code,
                     [DevBrand].[Name] AS Data_Devices_Model_Brand_Name,
                     [DevBrand].[Description] AS Data_Devices_Model_Brand_Description,
                     [DevBrand].[Code] AS Data_Devices_Brand_Code,
                     [DevBrand].[Name] AS Data_Devices_Brand_Name,
                     [DevBrand].[Description] AS Data_Devices_Brand_Description,
                     [Dev].[Name] AS Data_Devices_Name,
                     [dev].[Active] AS Data_Devices_Active,
                     [dev].[IPAddress] AS Data_Devices_IPAddress,
                     [dev].[Port] AS Data_Devices_Port,
                     [dev].[MacAddress] AS Data_Devices_MacAddress,
                     [dev].[RegisterDate] AS Data_Devices_RegisterDate,
                     [dev].[HardwareVersion] AS Data_Devices_HardwareVersion,
                     [dev].[FirmwareVersion] AS Data_Devices_FirmwareVersion,
                     [dev].[DeviceLockPassword] AS Data_Devices_DeviceLockPassword,
                     [DevModel].[ManufactureCode] AS Data_Devices_ManufactureCode,
                     [dev].[SSL] AS Data_Devices_SSL,
                     [dev].[TimeSync] AS Data_Devices_TimeSync,
                     [dev].[SerialNumber] AS Data_Devices_SerialNumber,
                     [dev].[DeviceTypeId] AS Data_Devices_DeviceTypeId,
                     [RH].[IpAddress] AS Data_RelayHub_IpAddress,
                     [RH].[Port] AS Data_RelayHub_Port,
                     [RH].[Capacity] AS Data_RelayHub_Capacity,
                     [RH].[Description] AS Data_RelayHub_Description,
                     [RH].[RelayHubModelId] AS Data_RelayHub_RelayHubModel_Id,
                     [RM].[Name] AS Data_RelayHub_RelayHubModel_Name,
                     [RM].[DefaultPortNumber] AS Data_RelayHub_RelayHubModel_DefaultPortNumber,
                     [RM].[ManufactureCode] AS Data_RelayHub_RelayHubModel_ManufactureCode,
                     [RM].[Description] AS Data_RelayHub_RelayHubModel_Description,
                     [RM].[BrandId] AS Data_RelayHub_RelayHubModel_Brand_Code,
					 [RT].[Name] AS Data_RelayHub_RelayType_Name,
                     [RT].[OrderIndex] AS Data_RelayType_OrderIndex,
                     [RT].[Description] AS Data_RelayType_Description,
                     [RT].[LookupCategoryId] AS Data_RelayType_Category_Id,
                     [L].[Name] AS Data_RelayHub_RelayHubModel_Brand_Name,
                     [L].[OrderIndex] AS Data_RelayHub_RelayHubModel_Brand_OrderIndex,
                     [L].[Description] AS Data_RelayHub_RelayHubModel_Brand_Description,
                     [L].[LookupCategoryId] AS Data_RelayHub_RelayHubModel_Brand_Category_Id,
                     [C].[Id] AS Data_Cameras_Id,
                     [C].[Code] AS Data_Cameras_Code,
                     [C].[Active] AS Data_Cameras_Active,
                     [C].[ModelId] AS Data_Cameras_Model_Id,
                     [C].[Name] AS Data_Cameras_Name,
                     [C].[Ip] AS Data_Cameras_Ip,
                     [C].[Port] AS Data_Cameras_Port,
                     [C].[MacAddress] AS Data_Cameras_MacAddress,
                     [C].[RegisterDate] AS Data_Cameras_RegisterDate,
                     [C].[HardwareVersion] AS Data_Cameras_HardwareVersion,
                     [C].[BrandCode] AS Data_Cameras_Brand_Code,
                     [C].[UserName] AS Data_Cameras_UserName,
                     [C].[Password] AS Data_Cameras_Password,
                     [C].[ConnectionUrl] AS Data_Cameras_ConnectionUrl,
                     [C].[LiveStreamUrl] AS Data_Cameras_LiveStreamUrl,
                     [C].[SerialNumber] AS Data_Cameras_SerialNumber,
                     (@PageNumber - 1) * @PageSize AS [from],
                     @PageNumber AS PageNumber,
                     @PageSize AS PageSize,
                     count(*) OVER () AS [Count],
                     @Message AS e_Message,
                     @Code AS e_Code,
                     @Validate AS e_Validate
            FROM     [Rly].Relay AS R
                     LEFT OUTER JOIN
                     [Rly].[RelayScheduling] AS RS
                     ON [R].[Id] = [RS].[RelayId]
                     LEFT OUTER JOIN
                     [Rly].[Scheduling] AS S
                     ON [S].[Id] = [RS].[SchedulingId]
                     LEFT OUTER JOIN
                     [dbo].[Lookup] AS SM
                     ON [SM].Code = [S].Mode
                     LEFT OUTER JOIN
                     [Rly].[RelayDevice] AS RD
                     ON [R].[Id] = [RD].[RelayId]
                     LEFT OUTER JOIN
                     [dbo].[Device] AS Dev
                     ON [Dev].[Id] = [RD].[DeviceId]
                     LEFT OUTER JOIN
                     [dbo].[DeviceModel] AS DevModel
                     ON Dev.[DeviceModelId] = DevModel.[Id]
                     LEFT OUTER JOIN
                     [dbo].[LookUp] AS DevBrand
                     ON DevModel.[BrandId] = DevBrand.[Code]
                     LEFT OUTER JOIN
                     [dbo].[DeviceGroupMember] AS dgm
                     ON dgm.DeviceId = dev.Id
                     LEFT OUTER JOIN
                     [dbo].[DeviceGroup] AS dg
                     ON dg.Id = dgm.GroupId
                     LEFT OUTER JOIN
                     [dbo].[AccessGroupDevice] AS agd
                     ON agd.DeviceGroupId = dg.Id
                     LEFT OUTER JOIN
                     [dbo].[AccessGroup] AS ag
                     ON ag.Id = agd.AccessGroupId
                     LEFT OUTER JOIN
                     [dbo].[AdminAccessGroup] AS AAG
                     ON ag.Id = AAG.AccessGroupId
                     LEFT OUTER JOIN
                     [Rly].[RelayHub] AS RH
                     ON [RH].[Id] = [R].[RelayHubId]
                     LEFT OUTER JOIN
                     [Rly].[RelayHubModel] AS RM
                     ON [RH].[RelayHubModelId] = [RM].[Id]
                     LEFT OUTER JOIN
                     [dbo].[Lookup] AS L
                     ON [L].[Code] = [RM].[BrandId]
                     LEFT OUTER JOIN
                     [Rly].[RelayCamera] AS RC
                     ON RC.RelayId = [R].[Id]
                     LEFT OUTER JOIN
                     [Rly].Camera AS C
                     ON C.[Id] = [Rc].[CameraId]
					 LEFT OUTER JOIN
                     [dbo].[Lookup] AS RT
                     ON [RT].[Code] = [R].[RelayTypeId]
            WHERE    (isnull(@AdminUserId, 0) = 0
                      OR EXISTS (SELECT Id
                                 FROM   [dbo].[USER]
                                 WHERE  IsMasterAdmin = 1
                                        AND ID = @AdminUserId))
                     AND ([R].Id = @Id
                          OR ISNULL(@Id, 0) = 0)
                     AND ([S].Id = @SchedulingId
                          OR ISNULL(@SchedulingId, 0) = 0)
                     AND ([Dev].Id = @DeviceId
                          OR ISNULL(@DeviceId, 0) = 0)
                     AND (C.[Id] = @CameraId
                          OR ISNULL(@CameraId, 0) = 0)
                     AND ([R].Name = @Name
                          OR ISNULL(@Name, '') = '')
                     AND ([R].[NodeNumber] = @NodeNumber
                          OR ISNULL(@NodeNumber, 0) = 0)
                     AND ([R].[RelayHubId] = @RelayHubId
                          OR ISNULL(@RelayHubId, 0) = 0)
                     AND ([R].[RelayTypeId] = @RelayTypeId
                          OR ISNULL(@RelayTypeId, 0) = 0)
            ORDER BY [R].Id
            OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
        END
    ELSE
        BEGIN
            SELECT [R].[Id] AS Data_Id,
                   [R].[Name] AS Data_Name,
                   [R].[NodeNumber] AS Data_NodeNumber,
                   [R].[RelayHubId] AS Data_RelayHub_Id,
                   [R].[RelayTypeId] AS Data_RelayType_Code,
                   [R].[Description] AS Data_Description,
                   [S].[Id] AS Data_Schedulings_Id,
                   [S].[StartTime] AS Data_Schedulings_StartTime,
                   [S].[EndTime] AS Data_Schedulings_EndTime,
                   [S].[Mode] AS Data_Schedulings_Mode_Code,
                   [SM].[Description] AS Data_Schedulings_Mode_Description,
                   [SM].[Name] AS Data_Schedulings_Mode_Name,
                   [SM].[OrderIndex] AS Data_Schedulings_Mode_OrderIndex,
                   [SM].[LookupCategoryId] AS Data_Schedulings_Mode_Category_Id,
                   [Dev].[Id] AS Data_Devices_DeviceId,
                   [Dev].[Code] AS Data_Devices_Code,
                   [dev].[DeviceModelId] AS Data_Devices_DeviceModelId,
                   [DevModel].[Id] AS Data_Devices_Model_Id,
                   [DevModel].[Name] AS Data_Devices_Model_Name,
                   [DevModel].[GetLogMethodType] AS Data_Devices_Model_GetLogMethodType,
                   [DevModel].[Description] AS Data_Devices_Model_Description,
                   [DevModel].[DefaultPortNumber] AS Data_Devices_Model_DefaultPortNumber,
                   [DevModel].[BrandId] AS Data_Devices_Model_Brand_Code,
                   [DevBrand].[Name] AS Data_Devices_Model_Brand_Name,
                   [DevBrand].[Description] AS Data_Devices_Model_Brand_Description,
                   [DevBrand].[Code] AS Data_Devices_Brand_Code,
                   [DevBrand].[Name] AS Data_Devices_Brand_Name,
                   [DevBrand].[Description] AS Data_Devices_Brand_Description,
                   [Dev].[Name] AS Data_Devices_Name,
                   [dev].[Active] AS Data_Devices_Active,
                   [dev].[IPAddress] AS Data_Devices_IPAddress,
                   [dev].[Port] AS Data_Devices_Port,
                   [dev].[MacAddress] AS Data_Devices_MacAddress,
                   [dev].[RegisterDate] AS Data_Devices_RegisterDate,
                   [dev].[HardwareVersion] AS Data_Devices_HardwareVersion,
                   [dev].[FirmwareVersion] AS Data_Devices_FirmwareVersion,
                   [dev].[DeviceLockPassword] AS Data_Devices_DeviceLockPassword,
                   [DevModel].[ManufactureCode] AS Data_Devices_ManufactureCode,
                   [dev].[SSL] AS Data_Devices_SSL,
                   [dev].[TimeSync] AS Data_Devices_TimeSync,
                   [dev].[SerialNumber] AS Data_Devices_SerialNumber,
                   [dev].[DeviceTypeId] AS Data_Devices_DeviceTypeId,
                   [RH].[IpAddress] AS Data_RelayHub_IpAddress,
                   [RH].[Port] AS Data_RelayHub_Port,
                   [RH].[Capacity] AS Data_RelayHub_Capacity,
                   [RH].[Description] AS Data_RelayHub_Description,
                   [RH].[RelayHubModelId] AS Data_RelayHub_RelayHubModel_Id,
                   [RM].[Name] AS Data_RelayHub_RelayHubModel_Name,
                   [RM].[DefaultPortNumber] AS Data_RelayHub_RelayHubModel_DefaultPortNumber,
                   [RM].[ManufactureCode] AS Data_RelayHub_RelayHubModel_ManufactureCode,
                   [RM].[Description] AS Data_RelayHub_RelayHubModel_Description,
                   [RM].[BrandId] AS Data_RelayHub_RelayHubModel_Brand_Code,
				   [RT].[Name] AS Data_RelayHub_RelayType_Name,
                   [RT].[OrderIndex] AS Data_RelayType_OrderIndex,
                   [RT].[Description] AS Data_RelayType_Description,
                   [RT].[LookupCategoryId] AS Data_RelayType_Category_Id,
                   [L].[Name] AS Data_RelayHub_RelayHubModel_Brand_Name,
                   [L].[OrderIndex] AS Data_RelayHub_RelayHubModel_Brand_OrderIndex,
                   [L].[Description] AS Data_RelayHub_RelayHubModel_Brand_Description,
                   [L].[LookupCategoryId] AS Data_RelayHub_RelayHubModel_Brand_Category_Id,
                   [C].[Id] AS Data_Cameras_Id,
                   [C].[Code] AS Data_Cameras_Code,
                   [C].[Active] AS Data_Cameras_Active,
                   [C].[ModelId] AS Data_Cameras_Model_Id,
                   [C].[Name] AS Data_Cameras_Name,
                   [C].[Ip] AS Data_Cameras_Ip,
                   [C].[Port] AS Data_Cameras_Port,
                   [C].[MacAddress] AS Data_Cameras_MacAddress,
                   [C].[RegisterDate] AS Data_Cameras_RegisterDate,
                   [C].[HardwareVersion] AS Data_Cameras_HardwareVersion,
                   [C].[BrandCode] AS Data_Cameras_Brand_Code,
                   [C].[UserName] AS Data_Cameras_UserName,
                   [C].[Password] AS Data_Cameras_Password,
                   [C].[ConnectionUrl] AS Data_Cameras_ConnectionUrl,
                   [C].[LiveStreamUrl] AS Data_Cameras_LiveStreamUrl,
                   [C].[SerialNumber] AS Data_Cameras_SerialNumber,
                   1 AS [from],
                   1 AS PageNumber,
                   count(*) OVER () AS PageSize,
                   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @Code AS e_Code,
                   @Validate AS e_Validate
            FROM   [Rly].Relay AS R
                   LEFT OUTER JOIN
                   [Rly].[RelayScheduling] AS RS
                   ON [R].[Id] = [RS].[RelayId]
                   LEFT OUTER JOIN
                   [Rly].[Scheduling] AS S
                   ON [S].[Id] = [RS].[SchedulingId]
                   LEFT OUTER JOIN
                   [dbo].[Lookup] AS SM
                   ON [SM].Code = [S].Mode
                   LEFT OUTER JOIN
                   [Rly].[RelayDevice] AS RD
                   ON [R].[Id] = [RD].[RelayId]
                   LEFT OUTER JOIN
                   [dbo].[Device] AS Dev
                   ON [Dev].[Id] = [RD].[DeviceId]
                   LEFT OUTER JOIN
                   [dbo].[DeviceModel] AS DevModel
                   ON Dev.[DeviceModelId] = DevModel.[Id]
                   LEFT OUTER JOIN
                   [dbo].[LookUp] AS DevBrand
                   ON DevModel.[BrandId] = DevBrand.[Code]
                   LEFT OUTER JOIN
                   [dbo].[DeviceGroupMember] AS dgm
                   ON dgm.DeviceId = dev.Id
                   LEFT OUTER JOIN
                   [dbo].[DeviceGroup] AS dg
                   ON dg.Id = dgm.GroupId
                   LEFT OUTER JOIN
                   [dbo].[AccessGroupDevice] AS agd
                   ON agd.DeviceGroupId = dg.Id
                   LEFT OUTER JOIN
                   [dbo].[AccessGroup] AS ag
                   ON ag.Id = agd.AccessGroupId
                   LEFT OUTER JOIN
                   [dbo].[AdminAccessGroup] AS AAG
                   ON ag.Id = AAG.AccessGroupId
                   LEFT OUTER JOIN
                   [Rly].[RelayHub] AS RH
                   ON [RH].[Id] = [R].[RelayHubId]
                   LEFT OUTER JOIN
                   [Rly].[RelayHubModel] AS RM
                   ON [RH].[RelayHubModelId] = [RM].[Id]
                   LEFT OUTER JOIN
                   [dbo].[Lookup] AS L
                   ON [L].[Code] = [RM].[BrandId]
                   LEFT OUTER JOIN
                   [Rly].[RelayCamera] AS RC
                   ON RC.RelayId = [R].[Id]
                   LEFT OUTER JOIN
                   [Rly].Camera AS C
                   ON C.[Id] = [Rc].[CameraId]
				   LEFT OUTER JOIN
				   [dbo].[Lookup] AS RT
				   ON [RT].[Code] = [R].[RelayTypeId]
            WHERE  (isnull(@AdminUserId, 0) = 0
                    OR EXISTS (SELECT Id
                               FROM   [dbo].[USER]
                               WHERE  IsMasterAdmin = 1
                                      AND ID = @AdminUserId))
                   AND ([R].Id = @Id
                        OR ISNULL(@Id, 0) = 0)
                   AND ([S].Id = @SchedulingId
                        OR ISNULL(@SchedulingId, 0) = 0)
                   AND ([Dev].Id = @DeviceId
                        OR ISNULL(@DeviceId, 0) = 0)
                   AND (C.[Id] = @CameraId
                        OR ISNULL(@CameraId, 0) = 0)
                   AND ([R].Name = @Name
                        OR ISNULL(@Name, '') = '')
                   AND ([R].[NodeNumber] = @NodeNumber
                        OR ISNULL(@NodeNumber, 0) = 0)
                   AND ([R].[RelayHubId] = @RelayHubId
                        OR ISNULL(@RelayHubId, 0) = 0)
                   AND ([R].[RelayTypeId] = @RelayTypeId
                        OR ISNULL(@RelayTypeId, 0) = 0);
        END
END