CREATE PROCEDURE [dbo].[SelectDeviceOfAccessGroup]
@Id INT, @pageNumber INT = NULL, @PageSize INT = Null
AS
BEGIN
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200'
DECLARE  @HasPaging  BIT;
	 SET @HasPaging = CASE
                                       WHEN @PageSize =0
                                      AND @PageNumber =0 THEN
                                     0                                   
                                 ELSE
                                     1
                             END;
 IF @HasPaging = 1
  BEGIN
    SELECT [Dev].[Id] AS Data_DeviceId,
				   [Dev].[Code] AS Data_Code,
				   [dev].[DeviceModelId] AS Data_DeviceModelId,
				   [DevModel].[Id] AS Data_Model_Id,
				   [DevModel].[Name] AS Data_Model_Name,
				   [DevModel].[BrandId] AS Data_Model_Brand_code,
				   [DevModel].[GetLogMethodType] AS Data_Model_GetLogMethodType,
				   [DevModel].[Description] AS Data_Model_Description,
				   [DevBrand].[code] AS Data_Brand_code,
				   [DevBrand].[Name] AS Data_Brand_Name,
				   [DevBrand].[Description] AS Data_Brand_Description,
				   [Dev].[Name] AS Data_Name,
				   [dev].[Active] AS Data_Active,
				   [dev].[IPAddress] AS Data_IPAddress,
				   [dev].[Port] AS Data_Port,
				   [dev].[MacAddress] AS Data_MacAddress,
				   [dev].[RegisterDate] AS Data_RegisterDate,
				   [dev].[HardwareVersion] AS Data_HardwareVersion,
				   [dev].[FirmwareVersion] AS Data_FirmwareVersion,
				   [dev].[DeviceLockPassword] AS Data_DeviceLockPassword,
				   [DevModel].[ManufactureCode] AS Data_ManufactureCode,
				   [dev].[SSL] AS Data_SSL,
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


    FROM   AccessGroup AS AG
           INNER JOIN
           AccessGroupDevice AS AGD
           ON AG.Id = AGD.AccessGroupId
           INNER JOIN
           DeviceGroup AS DG
           ON DG.Id = AGD.DeviceGroupId
           INNER JOIN
           DeviceGroupMember AS DGM
           ON DGM.GroupId = DG.Id
           INNER JOIN
           Device AS DEV
           ON DEV.Id = DGM.DeviceId
           INNER JOIN
           [dbo].[DeviceModel] AS DevModel
           ON Dev.[DeviceModelId] = DevModel.[Id]
           INNER JOIN
           [dbo].[lookup] AS DevBrand
           ON DevModel.[BrandId] = DevBrand.[code]
    WHERE  AG.Id = @ID
	ORDER BY AG.Id
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY;
END
Else
BEGIN
 SELECT [Dev].[Id] AS Data_DeviceId,
				   [Dev].[Code] AS Data_Code,
				   [dev].[DeviceModelId] AS Data_DeviceModelId,
				   [DevModel].[Id] AS Data_Model_Id,
				   [DevModel].[Name] AS Data_Model_Name,
				   [DevModel].[BrandId] AS Data_Model_Brand_code,
				   [DevModel].[GetLogMethodType] AS Data_Model_GetLogMethodType,
				   [DevModel].[Description] AS Data_Model_Description,
				   [DevBrand].[code] AS Data_Brand_code,
				   [DevBrand].[Name] AS Data_Brand_Name,
				   [DevBrand].[Description] AS Data_Brand_Description,
				   [Dev].[Name] AS Data_Name,
				   [dev].[Active] AS Data_Active,
				   [dev].[IPAddress] AS Data_IPAddress,
				   [dev].[Port] AS Data_Port,
				   [dev].[MacAddress] AS Data_MacAddress,
				   [dev].[RegisterDate] AS Data_RegisterDate,
				   [dev].[HardwareVersion] AS Data_HardwareVersion,
				   [dev].[FirmwareVersion] AS Data_FirmwareVersion,
				   [dev].[DeviceLockPassword] AS Data_DeviceLockPassword,
				   [DevModel].[ManufactureCode] AS Data_ManufactureCode,
				   [dev].[SSL] AS Data_SSL,
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



    FROM   AccessGroup AS AG
           INNER JOIN
           AccessGroupDevice AS AGD
           ON AG.Id = AGD.AccessGroupId
           INNER JOIN
           DeviceGroup AS DG
           ON DG.Id = AGD.DeviceGroupId
           INNER JOIN
           DeviceGroupMember AS DGM
           ON DGM.GroupId = DG.Id
           INNER JOIN
           Device AS DEV
           ON DEV.Id = DGM.DeviceId
           INNER JOIN
           [dbo].[DeviceModel] AS DevModel
           ON Dev.[DeviceModelId] = DevModel.[Id]
           INNER JOIN
           [dbo].[lookup] AS DevBrand
           ON DevModel.[BrandId] = DevBrand.[code]
    WHERE  AG.Id = @ID;
END
END