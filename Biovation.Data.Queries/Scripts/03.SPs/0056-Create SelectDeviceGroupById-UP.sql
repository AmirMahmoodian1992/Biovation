
CREATE PROCEDURE [dbo].[SelectDeviceGroupById]
@Id INT, @adminUserId INT = null
AS
BEGIN
    SELECT DISTINCT
		   [DevGroup].[Id],
           [DevGroup].[Name],
           [DevGroup].[Description],
           [Dev].[Id] AS Devices_DeviceId,
           [Dev].[DeviceModelId] AS Devices_ModelId,
           [Dev].[Name] AS Devices_Name,
           [Dev].[Code] AS Devices_Code,
           [Dev].[Active] AS Devices_Active,
           [Dev].[IPAddress] AS Devices_IPAddress,
           [Dev].[Port] AS Devices_Port,
           [Dev].[MacAddress] AS Devices_MacAddress,
           [Dev].[RegisterDate] AS Devices_RegisterDate,
           [Dev].[HardwareVersion] AS Devices_HardwareVersion,
           [Dev].[FirmwareVersion] AS Devices_FirmwareVersion,
           [Dev].[DeviceLockPassword] AS Devices_DeviceLockPassword,
           [Dev].[SSL] AS Devices_SSL,
           [Dev].[TimeSync] AS Devices_TimeSync,
           [Dev].[SerialNumber] AS Devices_SerialNumber
    FROM   [dbo].[DeviceGroup] AS DevGroup
           LEFT OUTER JOIN
           [dbo].[DeviceGroupMember] AS DGM
           ON DevGroup.[Id] = DGM.[GroupId]
           LEFT OUTER JOIN
           [dbo].[Device] AS Dev
           ON Dev.[Id] = DGM.[DeviceId]
           LEFT OUTER JOIN
           [dbo].[AccessGroupDevice] AS agd
           ON agd.DeviceGroupId = DevGroup.Id
           LEFT OUTER JOIN
           [dbo].[AccessGroup] AS ag
           ON ag.Id = agd.AccessGroupId
           LEFT OUTER JOIN
           [dbo].[AdminAccessGroup] AS AAG
           ON ag.Id = AAG.AccessGroupId
    WHERE  (isnull(@Id, 0) = 0
            OR DevGroup.Id = @Id)
			AND
            (isnull(@AdminUserId, 0) = 0
            OR AAG.UserId = @AdminUserId
            OR EXISTS (SELECT Id
                       FROM   [dbo].[USER]
                       WHERE  IsMasterAdmin = 1
                              AND ID = @AdminUserId))

 END