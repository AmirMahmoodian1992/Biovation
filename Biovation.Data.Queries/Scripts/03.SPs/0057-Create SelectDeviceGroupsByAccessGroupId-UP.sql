
CREATE PROCEDURE [dbo].[SelectDeviceGroupsByAccessGroupId]
@AccessGroupId INT,@PageNumber AS INT=0, @PageSize AS INT =0
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
    SELECT [DevGroup].[Id] AS Data_Id,
           [DevGroup].[Name] AS Data_Name,
           [DevGroup].[Description] AS Data_Description,
           [Dev].[Id] AS Data_Devices_DeviceId,
           [Dev].[Code] AS Data_Devices_Code,
           [Dev].[DeviceModelId] AS Data_Devices_ModelId,
           [DevModel].[BrandId] AS Data_Devices_BrandId,
           [Dev].[Name] AS Data_Devices_Name,
           [Active] AS Data_Devices_Active,
           [IPAddress] AS Data_Devices_IPAddress,
           [Port] AS Data_Devices_Port,
           [MacAddress] AS Data_Devices_MacAddress,
           [RegisterDate] AS Data_Devices_RegisterDate,
           [HardwareVersion] AS Data_Devices_HardwareVersion,
           [FirmwareVersion] AS Data_Devices_FirmwareVersion,
           [DeviceLockPassword] AS Data_Devices_DeviceLockPassword,
           [SSL] AS Data_Devices_SSL,
           [TimeSync] AS Data_Devices_TimeSync,
           [SerialNumber] AS Data_Devices_SerialNumber,
		   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
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
           LEFT OUTER JOIN
           [dbo].[AccessGroupDevice] AS AGD
           ON AGD.[DeviceGroupId] = DevGroup.[Id]
    WHERE  (isnull(@AccessGroupId, 0) = 0
            OR AGD.[AccessGroupId] = @AccessGroupId)
		   ORDER BY [DevGroup].[Id]
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY	
END
ELSE
BEGIN
    SELECT [DevGroup].[Id] AS Data_Id,
           [DevGroup].[Name] AS Data_Name,
           [DevGroup].[Description] AS Data_Description,
           [Dev].[Id] AS Data_Devices_DeviceId,
           [Dev].[Code] AS Data_Devices_Code,
           [Dev].[DeviceModelId] AS Data_Devices_ModelId,
           [DevModel].[BrandId] AS Data_Devices_BrandId,
           [Dev].[Name] AS Data_Devices_Name,
           [Active] AS Data_Devices_Active,
           [IPAddress] AS Data_Devices_IPAddress,
           [Port] AS Data_Devices_Port,
           [MacAddress] AS Data_Devices_MacAddress,
           [RegisterDate] AS Data_Devices_RegisterDate,
           [HardwareVersion] AS Data_Devices_HardwareVersion,
           [FirmwareVersion] AS Data_Devices_FirmwareVersion,
           [DeviceLockPassword] AS Data_Devices_DeviceLockPassword,
           [SSL] AS Data_Devices_SSL,
           [TimeSync] AS Data_Devices_TimeSync,
           [SerialNumber] AS Data_Devices_SerialNumber,
		   1 AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
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
           LEFT OUTER JOIN
           [dbo].[AccessGroupDevice] AS AGD
           ON AGD.[DeviceGroupId] = DevGroup.[Id]
    WHERE  (isnull(@AccessGroupId, 0) = 0
            OR AGD.[AccessGroupId] = @AccessGroupId);
END
END