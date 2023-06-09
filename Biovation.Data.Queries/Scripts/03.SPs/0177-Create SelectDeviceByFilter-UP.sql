CREATE PROCEDURE [dbo].[SelectDevicesByFilter]
@AdminUserId INT=NULL, @GroupId INT=NULL,  @DeviceCode INT=NULL, @DeviceBrandId INT=NULL, @Name NVARCHAR (100)=NULL,@DeviceModelId INT=NULL,@DeviceTypeId INT=NULL,@PageNumber AS INT=0, @PageSize AS INT =0
AS
          DECLARE  @HasPaging   BIT;
		  DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
   
BEGIN


 SET @HasPaging = CASE
                                 WHEN @PageSize =0
                                      AND @PageNumber =0 THEN
                                     0
                                 ELSE
                                     1
                             END;
 IF @HasPaging = 1
     BEGIN
    SELECT 
	       [Dev].[Id] AS Data_DeviceId,
           [Dev].[Code] AS Data_Code,
           [dev].[DeviceModelId] AS Data_DeviceModelId,
           [DevModel].[Id] AS Data_Model_Id,
           [DevModel].[Name] AS Data_Model_Name,
           [DevModel].[GetLogMethodType] AS Data_Model_GetLogMethodType,
           [DevModel].[Description] AS Data_Model_Description,
           [DevModel].[DefaultPortNumber] AS Data_Model_DefaultPortNumber,
           [DevModel].[BrandId] AS Data_Model_Brand_Code,
           [DevBrand].[Name] AS Data_Model_Brand_Name,
           [DevBrand].[Description] AS Data_Model_Brand_Description,
           [DevBrand].[Code] AS Data_Brand_Code,
           [DevBrand].[Name] AS Data_Brand_Name,
           [DevBrand].[Description] AS Data_Brand_Description,
           [Dev].[Name]  AS Data_Name,
           [dev].[Active] AS Data_Active,
           [dev].[IPAddress] AS Data_IpAddress,
           [dev].[Port] AS Data_Port,
           [dev].[MacAddress] AS Data_MacAddress,
           [dev].[RegisterDate] AS Data_RegisterDate,
           [dev].[HardwareVersion] AS Data_HardwareVersion,
           [dev].[FirmwareVersion] AS Data_FirmwareVersion,
           [dev].[DeviceLockPassword]AS Data_DeviceLockPassword,
           [DevModel].[ManufactureCode] AS Data_ManufactureCode,
           [dev].[SSL] AS Data_SSL ,
           [dev].[TimeSync] AS Data_TimeSync,
           [dev].[SerialNumber] AS Data_SerialNumber,
           [dev].[DeviceTypeId] AS Data_DeviceTypeId,
		   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
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
                              AND ID = @AdminUserId))         
           AND (Dev.[Code] = @DeviceCode
                OR ISNULL(@DeviceCode, 0) = 0)
           AND (DGM.[GroupId] = @GroupId
                OR ISNULL(@GroupId, 0) = 0)
           AND ([dev].[DeviceModelId] = @DeviceModelId
                OR ISNULL(@DeviceModelId, 0) = 0)
           AND ([Dev].[Name] LIKE '%' + @Name + '%'
                OR ISNULL(@Name, '') = '')
		   AND ([Dev].[DeviceTypeId]= @DeviceTypeId
                OR ISNULL(@DeviceTypeId, 0) = 0)
           AND (DevBrand.[Code] = @DeviceBrandId
                OR ISNULL(@DeviceBrandId, 0) = 0)
		ORDER BY dev.Id
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY		
END
Else
BEGIN
    SELECT 
	       [Dev].[Id] AS Data_DeviceId,
           [Dev].[Code] AS Data_Code,
           [dev].[DeviceModelId] AS Data_DeviceModelId,
           [DevModel].[Id] AS Data_Model_Id,
           [DevModel].[Name] AS Data_Model_Name,
           [DevModel].[GetLogMethodType] AS Data_Model_GetLogMethodType,
           [DevModel].[Description] AS Data_Model_Description,
           [DevModel].[DefaultPortNumber] AS Data_Model_DefaultPortNumber,
           [DevModel].[BrandId] AS Data_Model_Brand_Code,
           [DevBrand].[Name] AS Data_Model_Brand_Name,
           [DevBrand].[Description] AS Data_Model_Brand_Description,
           [DevBrand].[Code] AS Data_Brand_Code,
           [DevBrand].[Name] AS Data_Brand_Name,
           [DevBrand].[Description] AS Data_Brand_Description,
           [Dev].[Name]  AS Data_Name,
           [dev].[Active] AS Data_Active,
           [dev].[IPAddress] AS Data_IpAddress,
           [dev].[Port] AS Data_Port,
           [dev].[MacAddress] AS Data_MacAddress,
           [dev].[RegisterDate] AS Data_RegisterDate,
           [dev].[HardwareVersion] AS Data_HardwareVersion,
           [dev].[FirmwareVersion] AS Data_FirmwareVersion,
           [dev].[DeviceLockPassword]AS Data_DeviceLockPassword,
           [DevModel].[ManufactureCode] AS Data_ManufactureCode,
           [dev].[SSL] AS Data_SSL ,
           [dev].[TimeSync] AS Data_TimeSync,
           [dev].[SerialNumber] AS Data_SerialNumber,
           [dev].[DeviceTypeId] AS Data_DeviceTypeId,
		   1 AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
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
                              AND ID = @AdminUserId))
           AND (Dev.[Code] = @DeviceCode
                OR ISNULL(@DeviceCode, 0) = 0)
           AND (DGM.[GroupId] = @GroupId
                OR ISNULL(@GroupId, 0) = 0)
           AND ([dev].[DeviceModelId] = @DeviceModelId
                OR ISNULL(@DeviceModelId, 0) = 0)
           AND ([Dev].[Name] LIKE '%' + @Name + '%'
                OR ISNULL(@Name, '') = '')
		   AND ([Dev].[DeviceTypeId]= @DeviceTypeId
                OR ISNULL(@DeviceTypeId, 0) = 0)
           AND (DevBrand.[Code] = @DeviceBrandId
                OR ISNULL(@DeviceBrandId, 0) = 0)	
END
END