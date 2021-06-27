ALTER PROCEDURE [dbo].[SelectRelay]
@AdminUserId INT=NULL, @Id INT=NULL, @Name NVARCHAR (MAX)=NULL, @NodeNumber INT=NULL, @RelayHubId INT=NULL, @RelayTypeId INT=NULL, @SchedulingId INT=NULL, @DeviceId INT=NULL, @PageNumber INT=0, @PageSize INT=0
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
					 [R].RelayTypeId AS Data_RelayType_Id,
                     [R].[Description] AS Data_Description,
                     [S].[Id] AS Data_Schedulings_Id,
                     [S].[Mode] AS Data_Schedulings_Mode,
                     [S].[StartTime] AS Data_Schedulings_StartTime,
                     [S].[EndTime] AS Data_Schedulings_EndTime,
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
            FROM     [Rly].Relay AS R
                     LEFT OUTER JOIN
                     [Rly].[RelayScheduling] AS RS
                     ON [R].[Id] = [RS].[RelayId]
                     LEFT JOIN
                     [Rly].[Scheduling] AS S
                     ON [S].[Id] = [RS].[SchedulingId]
                     LEFT OUTER JOIN
                     [Rly].[RelayDevice] AS RD
                     ON [R].[Id] = [RD].[RelayId]
                     LEFT JOIN
                     [dbo].[Device] AS Dev
                     ON [Dev].[Id] = [RD].[DeviceId]
                     LEFT JOIN
                     [dbo].[DeviceModel] AS DevModel
                     ON Dev.[DeviceModelId] = DevModel.[Id]
                     LEFT JOIN
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
                     LEFT JOIN
                     [dbo].[DeviceModel] AS DM
                     ON [RH].[RelayHubModelId] = [DM].[Id]
                     LEFT JOIN
                     [dbo].[Lookup] AS L
                     ON [L].[Code] = [DM].[BrandId]
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
					 [R].RelayTypeId AS Data_RelayType_Id,
                     [R].[Description] AS Data_Description,
                     [S].[Id] AS Data_Schedulings_Id,
                     [S].[Mode] AS Data_Schedulings_Mode,
                     [S].[StartTime] AS Data_Schedulings_StartTime,
                     [S].[EndTime] AS Data_Schedulings_EndTime,
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
            FROM   [Rly].Relay AS R
                     LEFT OUTER JOIN
                     [Rly].[RelayScheduling] AS RS
                     ON [R].[Id] = [RS].[RelayId]
                     LEFT JOIN
                     [Rly].[Scheduling] AS S
                     ON [S].[Id] = [RS].[SchedulingId]
                     LEFT OUTER JOIN
                     [Rly].[RelayDevice] AS RD
                     ON [R].[Id] = [RD].[RelayId]
                     LEFT JOIN
                     [dbo].[Device] AS Dev
                     ON [Dev].[Id] = [RD].[DeviceId]
                     LEFT JOIN
                     [dbo].[DeviceModel] AS DevModel
                     ON Dev.[DeviceModelId] = DevModel.[Id]
                     LEFT JOIN
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
                     LEFT JOIN
                     [dbo].[DeviceModel] AS DM
                     ON [RH].[RelayHubModelId] = [DM].[Id]
                     LEFT JOIN
                     [dbo].[Lookup] AS L
                     ON [L].[Code] = [DM].[BrandId]
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
                   AND ([R].Name = @Name
                        OR ISNULL(@Name, '') = '')
                   AND ([R].[NodeNumber] = @NodeNumber
                        OR ISNULL(@NodeNumber, 0) = 0)
                   AND ([R].[RelayHubId] = @RelayHubId
                        OR ISNULL(@RelayHubId, 0) = 0)
				   AND ([R].[RelayTypeId] = @RelayTypeId
						OR ISNULL(@RelayTypeId, 0) = 0)
        END
END