Alter PROCEDURE [dbo].[SelectDeviceGroupById]
@Id INT, @adminUserId INT = null,@pageNumber INT = NULL, @PageSize INT = Null
AS
BEGIN
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
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
    SELECT DISTINCT
		   [DevGroup].[Id] AS Data_Id,
           [DevGroup].[Name] AS Data_Name,
           [DevGroup].[Description] AS Data_Description,
           [Dev].[Id] AS Data_Devices_DeviceId,
		   [Dev].[Code] AS Data_Devices_Code,
           [Dev].[DeviceModelId] AS Data_Devices_ModelId,          
           [Dev].[Name] AS Data_Devices_Name,
           [Active] AS Data_Devices_Active,
           [IPAddress] AS Data_Devices_IPAddress,
           [Port] AS Data_Devices_Port,
           [MacAddress] AS Data_Devices_MacAddress,
           Dev.[RegisterDate] AS Data_Devices_RegisterDate,
           [HardwareVersion] AS Data_Devices_HardwareVersion,
           [FirmwareVersion] AS Data_Devices_FirmwareVersion,
           [DeviceLockPassword] AS Data_Devices_DeviceLockPassword,
           [SSL] AS Data_Devices_SSL,
           [TimeSync] AS Data_Devices_TimeSync,
           [SerialNumber] AS Data_Devices_SerialNumber,
		   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(1) OVER() AS [Count],
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
           [dbo].[AccessGroupDevice] AS agd
           ON agd.DeviceGroupId = DevGroup.Id
           LEFT OUTER JOIN
           [dbo].[AccessGroup] AS ag
           ON ag.Id = agd.AccessGroupId
           LEFT OUTER JOIN
           [dbo].[AdminAccessGroup] AS AAG
           ON ag.Id = AAG.AccessGroupId
           LEFT OUTER JOIN
           [dbo].[User] AS U
           ON AAG.UserId = U.Id
    WHERE  (isnull(@Id, 0) = 0
            OR DevGroup.Id = @Id)
			AND
            (isnull(@AdminUserId, 0) = 0
            OR U.Id = @AdminUserId
            OR EXISTS (SELECT Id
                       FROM   [dbo].[USER]
                       WHERE  IsMasterAdmin = 1
                              AND Id = @AdminUserId))
			ORDER BY [DevGroup].[Id]
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY	


 END
 ELSE
 BEGIN
   SELECT DISTINCT
		   [DevGroup].[Id] AS Data_Id,
           [DevGroup].[Name] AS Data_Name,
           [DevGroup].[Description] AS Data_Description,
           [Dev].[Id] AS Data_Devices_DeviceId,
		   [Dev].[Code] AS Data_Devices_Code,
           [Dev].[DeviceModelId] AS Data_Devices_ModelId,          
           [Dev].[Name] AS Data_Devices_Name,
           [Active] AS Data_Devices_Active,
           [IPAddress] AS Data_Devices_IPAddress,
           [Port] AS Data_Devices_Port,
           [MacAddress] AS Data_Devices_MacAddress,
           Dev.[RegisterDate] AS Data_Devices_RegisterDate,
           [HardwareVersion] AS Data_Devices_HardwareVersion,
           [FirmwareVersion] AS Data_Devices_FirmwareVersion,
           [DeviceLockPassword] AS Data_Devices_DeviceLockPassword,
           [SSL] AS Data_Devices_SSL,
           [TimeSync] AS Data_Devices_TimeSync,
           [SerialNumber] AS Data_Devices_SerialNumber,
		   1 AS [from],
		   1 AS PageNumber,
		   count(1) OVER() As PageSize,
		   count(1) OVER() AS [Count],
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
           [dbo].[AccessGroupDevice] AS agd
           ON agd.DeviceGroupId = DevGroup.Id
           LEFT OUTER JOIN
           [dbo].[AccessGroup] AS ag
           ON ag.Id = agd.AccessGroupId
           LEFT OUTER JOIN
           [dbo].[AdminAccessGroup] AS AAG
           ON ag.Id = AAG.AccessGroupId
           LEFT OUTER JOIN
           [dbo].[User] AS U
           ON AAG.UserId = U.Id
    WHERE  (isnull(@Id, 0) = 0
            OR DevGroup.Id = @Id)
			AND
            (isnull(@AdminUserId, 0) = 0
            OR U.Id = @AdminUserId
            OR EXISTS (SELECT Id
                       FROM   [dbo].[USER]
                       WHERE  IsMasterAdmin = 1
                              AND Id = @AdminUserId))
 END
 END