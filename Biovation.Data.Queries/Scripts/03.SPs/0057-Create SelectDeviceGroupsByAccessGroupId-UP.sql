
CREATE PROCEDURE [dbo].[SelectDeviceGroupsByAccessGroupId]
@AccessGroupId INT
AS
BEGIN
    SELECT [DevGroup].[Id],
           [DevGroup].[Name],
           [DevGroup].[Description],
           [Dev].[Id] AS Devices_DeviceId,
		   [Dev].[Code] AS Devices_Code,
           [Dev].[DeviceModelId] AS Devices_ModelId,
		   [DevModel].[BrandId] AS Devices_BrandId,
           [Dev].[Name] AS Devices_Name,
           [Active] AS Devices_Active,
           [IPAddress] AS Devices_IPAddress,
           [Port] AS Devices_Port,
           [MacAddress] AS Devices_MacAddress,
           [RegisterDate] AS Devices_RegisterDate,
           [HardwareVersion] AS Devices_HardwareVersion,
           [FirmwareVersion] AS Devices_FirmwareVersion,
           [DeviceLockPassword] AS Devices_DeviceLockPassword,
           [SSL] AS Devices_SSL,
           [TimeSync] AS Devices_TimeSync,
           [SerialNumber] AS Devices_SerialNumber
    FROM   [dbo].[DeviceGroup] AS DevGroup
           LEFT OUTER JOIN
           [dbo].[DeviceGroupMember] AS DGM
           ON DevGroup.[Id] = DGM.[GroupId]
           LEFT OUTER JOIN
           [dbo].[Device] AS Dev
           ON Dev.[Id] = DGM.[DeviceId]
		   LEFT OUTER JOIN
           [dbo].[DeviceModel] AS DevModel
           ON [DevModel].[Id] = [Dev].[DeviceModelId]
		   LEFT JOIN
		   [dbo].[AccessGroupDevice] AS AGD
		   ON AGD.[DeviceGroupId] = DevGroup.[Id]
    WHERE  (isnull(@AccessGroupId, 0) = 0
            OR AGD.[AccessGroupId] = @AccessGroupId);
END
GO
