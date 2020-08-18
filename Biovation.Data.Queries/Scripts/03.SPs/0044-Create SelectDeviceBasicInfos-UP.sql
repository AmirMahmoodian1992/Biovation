
CREATE PROCEDURE [dbo].[SelectDeviceBasicInfos]
@AdminUserId INT=NULL
AS
BEGIN
      SELECT [Dev].[Id] AS DeviceId,
           [Dev].[Code],
         
		 [dev].[DeviceModelId],
           [DevModel].[Id] AS Model_Id,
           [DevModel].[Name] AS Model_Name,
      
           [DevModel].[GetLogMethodType] AS Model_GetLogMethodType,
           [DevModel].[Description] AS Model_Description,
		   [DevModel].[DefaultPortNumber] AS Model_DefaultPortNumber,

		   [DevModel].[BrandId] AS Model_Brand_Code,
		   [DevBrand].[Name] AS  Model_Brand_Name,
           [DevBrand].[Description] AS  Model_Brand_Description,
		   [DevBrand].[Code] AS Brand_Code,
           [DevBrand].[Name] AS Brand_Name,
           [DevBrand].[Description] AS Brand_Description,
           
		   [Dev].[Name],
           [dev].[Active],
           [dev].[IPAddress],
           [dev].[Port],
           [dev].[MacAddress],
           [dev].[RegisterDate],
           [dev].[HardwareVersion],
           [dev].[FirmwareVersion],
           [dev].[DeviceLockPassword],
           [DevModel].[ManufactureCode],
           [dev].[SSL],
           [dev].[TimeSync],
           [dev].[SerialNumber],
           [dev].[DeviceTypeId]
    FROM   [dbo].[Device] AS Dev
           INNER JOIN
           [dbo].[DeviceModel] AS DevModel
           ON Dev.[DeviceModelId] = DevModel.[Id]
           INNER JOIN
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
    WHERE  (isnull(@AdminUserId, 0) = 0
            OR AAG.UserId = @AdminUserId
            OR EXISTS (SELECT Id
                       FROM   [dbo].[USER]
                       WHERE  IsMasterAdmin = 1
                              AND ID = @AdminUserId));
END

