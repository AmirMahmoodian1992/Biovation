
CREATE PROCEDURE [dbo].[SelectDeviceBasicInfosOfGroup]
@GroupId INT,
@AdminUserId INT = null
AS
BEGIN
      SELECT [Dev].[Id] AS DeviceId,
           [Dev].[Code],
           [Dev].[DeviceModelId],
           [DevModel].[Id] AS Model_Id,
           [DevModel].[Name] AS Model_Name,
           [DevBrand].[code] AS Model_Brand_Code,
		   [DevBrand].[Name] AS Model_Brand_Name,
           [DevBrand].[Description] AS Model_Brand_Description,
           [DevModel].[GetLogMethodType] AS Model_GetLogMethodType,
           [DevModel].[Description] AS Model_Description,
		   [DevModel].[DefaultPortNumber] AS Model_DefaultPortNumber,
           [DevBrand].[code] AS Brand_Code,
           [DevBrand].[Name] AS Brand_Name,
           [DevBrand].[Description] AS Brand_Description,
           [Dev].[Id],
           [Dev].[DeviceModelId],
           [Dev].[Name],
           [Dev].[Active],
           [Dev].[IPAddress],
           [Dev].[Port],
           [Dev].[MacAddress],
           [Dev].[RegisterDate],
           [Dev].[HardwareVersion],
           [Dev].[FirmwareVersion],
           [Dev].[DeviceLockPassword],
           [DevModel].[ManufactureCode],
           [Dev].[SSL],
           [Dev].[TimeSync],
           [Dev].[SerialNumber],
           [Dev].[DeviceTypeId]
    FROM   [dbo].[Device] AS Dev
           INNER JOIN
           [dbo].[DeviceGroupMember] AS DGM
           ON DGM.[DeviceId] = Dev.[Id]
           INNER JOIN
           [dbo].[DeviceModel] AS DevModel
           ON Dev.[DeviceModelId] = DevModel.[Id]
           INNER JOIN
           [dbo].[LookUp] AS DevBrand
           ON DevModel.[BrandId] = DevBrand.[code]
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
    WHERE  isnull(@AdminUserId, 0) = 0
           OR AAG.UserId = @AdminUserId
           OR EXISTS (SELECT Id
                      FROM   [dbo].[USER]
                      WHERE  IsMasterAdmin = 1
                             AND ID = @AdminUserId)
              AND DGM.[GroupId] = @GroupId;
END
GO
