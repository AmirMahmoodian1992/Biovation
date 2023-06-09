
CREATE PROCEDURE [dbo].[SelectDeviceBasicInfoById]
@Id INT, @AdminUserId INT=NULL
AS
DECLARE @Message AS NVARCHAR (200) = N' درخواست با موفقیت انجام گرفت', @Validate AS INT = 1,  @Code AS NVARCHAR (15) = N'200';
BEGIN
       SELECT [Dev].[Id] AS DeviceId,
           [Dev].[Code],          
		   [DevModel].[Id] AS Model_id,
           [DevModel].[Name] AS Model_Name,
           [DevModel].[BrandId] AS Model_Brand_code,
           [DevModel].[GetLogMethodType] AS Model_GetLogMethodType,
           [DevModel].[Description] AS Model_Description,
		   [DevModel].[DefaultPortNumber] AS Model_DefaultPortNumber,
		   [DevBrand].[code] AS Model_Brand_Code,
		   [DevBrand].[Name] AS Model_Brand_Name,
           [DevBrand].[Description] AS Model_Brand_Description,
           [DevBrand].[code] AS Brand_code,
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
           [Dev].[DeviceTypeId],
           @Message AS e_Message,
           @Validate AS e_Validate,
           @Code AS e_Code
    FROM   [dbo].[Device] AS Dev
           INNER JOIN
           [dbo].[DeviceModel] AS DevModel
           ON [Dev].[DeviceModelId] = DevModel.[Id]
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
           ON ag.id = AAG.AccessGroupId
    WHERE   (isnull(@AdminUserId, 0) = 0
            OR AAG.UserId = @AdminUserId
            OR EXISTS (SELECT Id
                       FROM   [dbo].[USER]
                       WHERE  IsMasterAdmin = 1
                              AND ID = @AdminUserId))
           and (Dev.[Id] = @Id
                          OR ISNULL(@Id, 0) = 0)
END
GO
