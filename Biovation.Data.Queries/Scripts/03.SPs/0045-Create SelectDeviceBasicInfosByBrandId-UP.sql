IF OBJECT_ID('SelectDeviceBasicInfosByBrandCode', 'P') IS NOT NULL
	DROP PROCEDURE [SelectDeviceBasicInfosByBrandCode]
GO


CREATE PROCEDURE [dbo].[SelectDeviceBasicInfosByBrandId]
@DeviceBrandId INT, @AdminUserId INT=NULL
AS
BEGIN
     SELECT [Dev].[Id] AS DeviceId,
           [Dev].[Code],
           [Dev].[DeviceModelId],
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
           [Dev].[Active],
           [Dev].[IPAddress],
           [Dev].[Port],
           [Dev].[MacAddress],
           [Dev].[RegisterDate],
           [Dev].[HardwareVersion],
           [Dev].[FirmwareVersion],
           [Dev].[DeviceLockPassword],
           [Dev].[SSL],
           [Dev].[TimeSync],
           [DevModel].[ManufactureCode],
           [Dev].[SerialNumber],
           [Dev].[DeviceTypeId]
    FROM   [dbo].[Device] AS Dev
           INNER JOIN
           [dbo].[DeviceModel] AS DevModel
           ON Dev.[DeviceModelId] = DevModel.[Id]
           INNER JOIN
           [dbo].[LookUp] AS DevBrand
           ON DevModel.[BrandId] = DevBrand.[code]
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
           [dbo].[User] AS U
           ON AAG.UserId = U.Id
    WHERE  (isnull(@AdminUserId, 0) = 0
            OR U.Code = @AdminUserId
            OR EXISTS (SELECT Id
                           FROM   [dbo].[USER]
                           WHERE  IsMasterAdmin = 1
                                  AND Code = @AdminUserId));
END
GO
