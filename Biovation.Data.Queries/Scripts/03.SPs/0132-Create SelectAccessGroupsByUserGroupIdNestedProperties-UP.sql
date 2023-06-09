
CREATE PROCEDURE [dbo].[SelectAccessGroupsByUserGroupIdNestedProperties]
@UserGroupId int
AS
BEGIN
		SELECT [AG].[ID],
           [AG].[Name],
           [AG].[TimeZoneId],
           [AG].[Description],
           [AAG].UserId AS AdminUserId_Id,

           [USG].[Id] AS UserGroup_Id,
           [USG].[Name] AS UserGroup_Name,

		   [UGM].[UserId] AS UserGroup_Users_UserId,
		   [UGM].[UserType] AS UserGroup_Users_UserType,
		   [UGM].[UserTypeTitle] AS UserGroup_Users_UserTypeTitle,
		   [USR].[FirstName] AS UserGroup_Users_FirstName,
		   [USR].[SurName] AS UserGroup_Users_SurName,
		   [USR].[UserName] AS UserGroup_Users_UserName,
		   [USR].[StartDate] AS UserGroup_Users_StartDate,
		   [USR].[EndDate] AS UserGroup_Users_EndDate,
		   [USR].[RegisterDate] AS UserGroup_Users_RegisterDate,
		   [USR].[IsActive] AS UserGroup_Users_IsActive,
		   [USR].[IsAdmin] AS UserGroup_Users_IsAdmin,
		   [USR].[AdminLevel] AS UserGroup_Users_AdminLevel,
		   [USR].[Password] AS UserGroup_Users_Password,
		   [USR].[PasswordBytes] AS UserGroup_Users_PasswordBytes,
		   [USR].[AuthMode] AS UserGroup_Users_AuthMode,
		   [USR].[IsMasterAdmin] AS UserGroup_Users_IsMasterAdmin,
		   [USR].[EntityId] AS UserGroup_Users_EntityId,
		   [USR].[Email] AS UserGroup_Users_Email,
		   [USR].[TelNumber] AS UserGroup_Users_TelNumber,
		   [USR].[Type] AS UserGroup_Users_Type,
		   [USR].[RemainingCredit] AS UserGroup_Users_RemainingCredit,
		   [USR].[AllowedStockCount] AS UserGroup_Users_AllowedStockCount,

		   [DG].[Id] AS DeviceGroup_Id,
           [DG].[Name] AS DeviceGroup_Name,

		   [DGM].[DeviceId] AS DeviceGroup_Devices_DeviceId,
		   [Dev].[Code] AS DeviceGroup_Devices_Code,
		   [Dev].[Name] AS DeviceGroup_Devices_Name,
		   [Dev].[Active] AS DeviceGroup_Devices_Active,
		   [Dev].[IPAddress] AS DeviceGroup_Devices_IpAddress,
		   [Dev].[Port] AS DeviceGroup_Devices_Port,
		   [Dev].[MacAddress] AS DeviceGroup_Devices_MacAddress,
		   [Dev].[RegisterDate] AS DeviceGroup_Devices_RegisterDate,
		   [Dev].[HardwareVersion] AS DeviceGroup_Devices_HardwareVersion,
		   [Dev].[FirmwareVersion] AS DeviceGroup_Devices_FirmwareVersion,
		   [Dev].[DeviceLockPassword] AS DeviceGroup_Devices_DeviceLockPassword,
		   [Dev].[SSL] AS DeviceGroup_Devices_SSL,
		   [Dev].[SerialNumber] AS DeviceGroup_Devices_SerialNumber,
		   [Dev].[DeviceTypeId] AS DeviceGroup_Devices_DeviceTypeId,
		   [Dev].[RestaurantId] AS DeviceGroup_Devices_RestaurantId,

		   [DevModel].[Id] AS DeviceGroup_Devices_Model_Id,
           [DevModel].[Name] AS DeviceGroup_Devices_Model_Name,
           [DevModel].[BrandId] AS DeviceGroup_Devices_Model_BrandId,
           [DevModel].[GetLogMethodType] AS DeviceGroup_Devices_Model_GetLogMethodType,
           [DevModel].[Description] AS DeviceGroup_Devices_Model_Description,

           [DevBrand].[code] AS DeviceGroup_Devices_Brand_code,
           [DevBrand].[Name] AS DeviceGroup_Devices_Brand_Name,
           [DevBrand].[Description] AS DeviceGroup_Devices_Brand_Description

    FROM   [dbo].[AccessGroup] AS AG
           LEFT OUTER JOIN
           [dbo].[AccessGroupUser] AS AGU
           ON AGU.AccessGroupId = AG.ID
           LEFT OUTER JOIN
           [dbo].[UserGroup] AS USG
           ON AGU.UserGroupId = USG.ID
           LEFT OUTER JOIN
           [dbo].[AccessGroupDevice] AS AGD
           ON AGD.AccessGroupId = AG.Id
           LEFT OUTER JOIN
           [dbo].[DeviceGroup] AS DG
           ON AGD.DeviceGroupId = DG.ID
           LEFT OUTER JOIN
           [dbo].[AdminAccessGroup] AS AAG
           ON AG.Id = AAG.AccessGroupId
		   LEFT OUTER JOIN
		   [dbo].[UserGroupMember] AS UGM
		   ON UGM.GroupId = USG.Id
		   LEFT OUTER JOIN
		   [dbo].[DeviceGroupMember] AS DGM
		   ON DGM.GroupId = DG.ID
		   LEFT OUTER JOIN
		   [dbo].[Device] AS Dev
		   ON [DGM].[DeviceId] = [Dev].[Id]
		   LEFT OUTER JOIN
		   [dbo].[User] AS USR
		   ON [USR].[Code] = [UGM].[UserId]
		   INNER JOIN
           [dbo].[DeviceModel] AS DevModel
           ON [Dev].[DeviceModelId] = DevModel.[Id]
           INNER JOIN
           [dbo].[lookup] AS DevBrand
           ON DevModel.[BrandId] = DevBrand.[code]
				WHERE USG.Id = @UserGroupId
END
GO
